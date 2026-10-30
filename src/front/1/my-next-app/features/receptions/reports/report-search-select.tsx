"use client";

import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Check, ChevronDown, LoaderCircle, Search, X } from "lucide-react";

import { Input } from "@/components/ui/input";
import { handleApiError } from "@/lib/api/handle-api-error";

import {
  ReceptionReportService,
  type ReceptionReportSelectKind,
} from "./reception-report.service";
import type { ReceptionReportSelectOption } from "./reception-report.types";

export function ReportSearchSelect({
  id,
  label,
  kind,
  selectedId,
  selectedLabel,
  onChange,
}: {
  id: string;
  label: string;
  kind: ReceptionReportSelectKind;
  selectedId: string;
  selectedLabel: string;
  onChange: (id: string, label: string) => void;
}) {
  const service = useMemo(() => new ReceptionReportService(), []);
  const requestController = useRef<AbortController | null>(null);
  const rootRef = useRef<HTMLDivElement>(null);
  const [open, setOpen] = useState(false);
  const [query, setQuery] = useState("");
  const [options, setOptions] = useState<ReceptionReportSelectOption[]>([]);
  const [loadedQuery, setLoadedQuery] = useState<string | null>(null);
  const [hasMore, setHasMore] = useState(false);
  const [nextPage, setNextPage] = useState(2);
  const [loading, setLoading] = useState(false);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadPage = useCallback(
    (search: string, pageNumber: number, signal: AbortSignal) => {
      switch (kind) {
        case "customer":
          return service.searchCustomers(search, pageNumber, signal);
        case "personnel":
          return service.searchPersonnel(search, pageNumber, signal);
        case "service":
          return service.searchServices(search, pageNumber, signal);
        case "material":
          return service.searchMaterials(search, pageNumber, signal);
      }
    },
    [kind, service],
  );

  useEffect(() => {
    if (!open) return;
    const controller = new AbortController();
    requestController.current = controller;
    const debounceId = window.setTimeout(() => {
      setLoading(true);
      setError(null);
      void loadPage(query.trim(), 1, controller.signal)
        .then((result) => {
          if (controller.signal.aborted) return;
          setOptions(result.list);
          setHasMore(result.hasMore);
          setNextPage(2);
          setLoadedQuery(query);
        })
        .catch((cause: unknown) => {
          if (!controller.signal.aborted) {
            setError(
              handleApiError(cause),
            );
            setOptions([]);
            setLoadedQuery(query);
          }
        })
        .finally(() => {
          if (!controller.signal.aborted) setLoading(false);
        });
    }, 300);

    return () => {
      window.clearTimeout(debounceId);
      controller.abort();
      if (requestController.current === controller) {
        requestController.current = null;
      }
    };
  }, [loadPage, open, query]);

  const showOptions = loadedQuery === query;

  async function loadMore() {
    const previous = requestController.current;
    if (!hasMore || loadingMore || !showOptions) return;
    previous?.abort();
    const controller = new AbortController();
    requestController.current = controller;
    setLoadingMore(true);
    setError(null);
    try {
      const result = await loadPage(query.trim(), nextPage, controller.signal);
      if (controller.signal.aborted) return;
      setOptions((current) => [...current, ...result.list]);
      setHasMore(result.hasMore);
      setNextPage((current) => current + 1);
    } catch (cause: unknown) {
      if (!controller.signal.aborted) {
        setError(handleApiError(cause));
      }
    } finally {
      if (!controller.signal.aborted) setLoadingMore(false);
    }
  }

  function close() {
    setOpen(false);
    setQuery("");
  }

  return (
    <div ref={rootRef} className="relative grid gap-1.5">
      <label htmlFor={id} className="text-sm font-medium leading-none">
        {label}
      </label>
      <div className="relative">
        <Search
          aria-hidden="true"
          className="pointer-events-none absolute start-3 top-1/2 z-10 size-4 -translate-y-1/2 text-slate-400"
        />
        <Input
          id={id}
          role="combobox"
          aria-autocomplete="list"
          aria-expanded={open}
          aria-controls={`${id}-options`}
          value={open ? query : selectedLabel}
          placeholder={`جستجوی ${label}...`}
          autoComplete="off"
          className="h-9 border-slate-200 bg-white pe-9 ps-9 text-sm"
          onFocus={() => {
            if (!open) {
              setQuery("");
              setOpen(true);
              setLoading(true);
            }
          }}
          onChange={(event) => {
            setQuery(event.target.value);
            setLoading(true);
            onChange("", "");
          }}
          onKeyDown={(event) => {
            if (event.key === "Escape") close();
          }}
          onBlur={(event) => {
            const nextTarget = event.relatedTarget;
            if (!nextTarget || !rootRef.current?.contains(nextTarget)) close();
          }}
        />
        {selectedId ? (
          <button
            type="button"
            aria-label={`پاک‌کردن ${label} انتخاب‌شده`}
            onMouseDown={(event) => event.preventDefault()}
            onClick={() => {
              onChange("", "");
              setQuery("");
              setOpen(true);
            }}
            className="absolute end-2 top-1/2 z-10 flex size-6 -translate-y-1/2 items-center justify-center rounded-full text-slate-400 hover:bg-slate-100 hover:text-slate-700 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-fuchsia-500"
          >
            <X className="size-3.5" />
          </button>
        ) : (
          <ChevronDown
            aria-hidden="true"
            className="pointer-events-none absolute end-3 top-1/2 size-4 -translate-y-1/2 text-slate-400"
          />
        )}
      </div>

      {open && (
        <div
          className="absolute inset-x-0 top-full z-40 mt-1 overflow-hidden rounded-xl border border-slate-200 bg-white shadow-lg"
          dir="rtl"
        >
          <div
            id={`${id}-options`}
            role="listbox"
            aria-label={`گزینه‌های ${label}`}
            className="max-h-60 overflow-y-auto p-1"
          >
            {loading && !showOptions ? (
              <p className="flex items-center justify-center gap-2 px-3 py-5 text-xs text-slate-500">
                <LoaderCircle className="size-4 animate-spin" />
                در حال جستجو در سرور...
              </p>
            ) : error && !showOptions ? (
              <p className="px-3 py-4 text-xs text-rose-700" role="alert">{error}</p>
            ) : showOptions && options.length === 0 ? (
              <p className="px-3 py-4 text-center text-xs text-slate-500">
                گزینه‌ای پیدا نشد.
              </p>
            ) : (
              showOptions &&
              options.map((option) => (
                <button
                  key={option.id}
                  type="button"
                  role="option"
                  aria-selected={String(option.id) === selectedId}
                  onMouseDown={(event) => event.preventDefault()}
                  onClick={() => {
                    onChange(String(option.id), option.label);
                    close();
                  }}
                  className="flex w-full items-center justify-between gap-3 rounded-lg px-3 py-2 text-start hover:bg-fuchsia-50 focus-visible:bg-fuchsia-50 focus-visible:outline-none"
                >
                  <span className="min-w-0">
                    <span className="block truncate text-xs font-medium text-slate-800">
                      {option.label}
                    </span>
                    <span className="mt-0.5 block truncate text-[10px] text-slate-500">
                      {option.description}
                    </span>
                  </span>
                  {String(option.id) === selectedId && (
                    <Check aria-hidden="true" className="size-4 shrink-0 text-fuchsia-700" />
                  )}
                </button>
              ))
            )}
          </div>
          {error && showOptions && (
            <p className="px-3 py-2 text-xs text-rose-700" role="alert">{error}</p>
          )}
          {showOptions && hasMore && (
            <button
              type="button"
              disabled={loadingMore}
              onMouseDown={(event) => event.preventDefault()}
              onClick={() => void loadMore()}
              className="flex w-full items-center justify-center gap-2 border-t border-slate-100 px-3 py-2 text-xs font-medium text-fuchsia-800 hover:bg-fuchsia-50 disabled:opacity-60"
            >
              {loadingMore && <LoaderCircle className="size-3.5 animate-spin" />}
              نمایش گزینه‌های بیشتر
            </button>
          )}
        </div>
      )}
    </div>
  );
}
