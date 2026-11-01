"use client";

import { useCallback, useEffect, useId, useRef, useState } from "react";
import { Check, ChevronDown, LoaderCircle, Search, X } from "lucide-react";

import { handleApiError } from "@/lib/api/handle-api-error";

export type ServerSearchOption<T> = {
  value: string;
  label: string;
  description?: string;
  data: T;
};

export type ServerSearchPage<T> = {
  options: ServerSearchOption<T>[];
  hasMore: boolean;
};

type ServerSearchSelectProps<T> = {
  value: string;
  onChange: (value: string, option: ServerSearchOption<T> | null) => void;
  loadOptions: (
    query: string,
    pageNumber: number,
    signal: AbortSignal,
  ) => Promise<ServerSearchPage<T>>;
  ariaLabel: string;
  placeholder?: string;
  emptyMessage?: string;
  disabled?: boolean;
};

export function ServerSearchSelect<T>({
  value,
  onChange,
  loadOptions,
  ariaLabel,
  placeholder = "برای جستجو تایپ کنید...",
  emptyMessage = "نتیجه‌ای پیدا نشد.",
  disabled = false,
}: ServerSearchSelectProps<T>) {
  const listboxId = useId();
  const rootRef = useRef<HTMLDivElement>(null);
  const activeRequestRef = useRef<AbortController | null>(null);
  const [open, setOpen] = useState(false);
  const [query, setQuery] = useState("");
  const [options, setOptions] = useState<ServerSearchOption<T>[]>([]);
  const [selectedOption, setSelectedOption] = useState<ServerSearchOption<T> | null>(null);
  const [pageNumber, setPageNumber] = useState(1);
  const [hasMore, setHasMore] = useState(false);
  const [loading, setLoading] = useState(false);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open || disabled) return;

    const controller = new AbortController();
    activeRequestRef.current?.abort();
    activeRequestRef.current = controller;
    const timeout = window.setTimeout(() => {
      setLoading(true);
      setLoadingMore(false);
      setError(null);
      setOptions([]);
      setPageNumber(1);
      setHasMore(false);
      loadOptions(query.trim(), 1, controller.signal)
        .then((result) => {
          if (!controller.signal.aborted) {
            setOptions(result.options);
            setHasMore(result.hasMore);
          }
        })
        .catch((requestError: unknown) => {
          if (!controller.signal.aborted) {
            setError(handleApiError(requestError));
          }
        })
        .finally(() => {
          if (!controller.signal.aborted) setLoading(false);
        });
    }, 250);

    return () => {
      window.clearTimeout(timeout);
      controller.abort();
      activeRequestRef.current?.abort();
      activeRequestRef.current = null;
    };
  }, [disabled, loadOptions, open, query]);

  useEffect(() => {
    function closeOnOutsideClick(event: PointerEvent) {
      if (!rootRef.current?.contains(event.target as Node)) setOpen(false);
    }

    document.addEventListener("pointerdown", closeOnOutsideClick);
    return () => document.removeEventListener("pointerdown", closeOnOutsideClick);
  }, []);

  const loadNextPage = useCallback(async () => {
    if (!hasMore || loading || loadingMore || error) return;
    const controller = new AbortController();
    activeRequestRef.current?.abort();
    activeRequestRef.current = controller;
    setLoadingMore(true);
    try {
      const nextPage = pageNumber + 1;
      const result = await loadOptions(query.trim(), nextPage, controller.signal);
      if (!controller.signal.aborted) {
        setOptions((current) => [...current, ...result.options]);
        setPageNumber(nextPage);
        setHasMore(result.hasMore);
      }
    } catch (requestError: unknown) {
      if (!controller.signal.aborted) setError(handleApiError(requestError));
    } finally {
      if (!controller.signal.aborted) setLoadingMore(false);
    }
  }, [error, hasMore, loadOptions, loading, loadingMore, pageNumber, query]);

  const selectedLabel = value
    ? selectedOption?.value === value
      ? selectedOption.label
      : ""
    : "";

  return (
    <div ref={rootRef} className="relative min-w-0">
      <div className="relative">
        <Search className="pointer-events-none absolute start-3 top-1/2 size-4 -translate-y-1/2 text-slate-400" />
        <input
          type="text"
          role="combobox"
          aria-label={ariaLabel}
          aria-expanded={open}
          aria-autocomplete="list"
          aria-controls={listboxId}
          value={open ? query : selectedLabel}
          onFocus={() => {
            setOpen(true);
            setQuery("");
          }}
          onChange={(event) => {
            setQuery(event.target.value);
            setOpen(true);
          }}
          onKeyDown={(event) => {
            if (event.key === "Escape") setOpen(false);
          }}
          placeholder={open ? placeholder : selectedLabel || placeholder}
          disabled={disabled}
          className="h-10 w-full rounded-lg border border-slate-200 bg-white pe-16 ps-9 text-sm text-slate-800 outline-none placeholder:text-slate-400 focus-visible:border-fuchsia-500 focus-visible:ring-3 focus-visible:ring-fuchsia-500/20 disabled:bg-slate-50 disabled:text-slate-400"
        />
        {value && !disabled && (
          <button
            type="button"
            aria-label={`پاک کردن ${ariaLabel}`}
            onClick={() => {
              setSelectedOption(null);
              setQuery("");
              onChange("", null);
            }}
            className="absolute end-8 top-1/2 grid size-7 -translate-y-1/2 place-items-center rounded-md text-slate-400 hover:bg-slate-100 hover:text-slate-700"
          >
            <X className="size-4" />
          </button>
        )}
        <ChevronDown
          aria-hidden="true"
          className={`pointer-events-none absolute end-3 top-1/2 size-4 -translate-y-1/2 text-slate-400 transition-transform ${open ? "rotate-180" : ""}`}
        />
      </div>
      {open && !disabled && (
        <div
          id={listboxId}
          role="listbox"
          aria-label={ariaLabel}
          className="absolute z-50 mt-1 max-h-64 w-full overflow-y-auto rounded-xl border border-slate-200 bg-white p-1.5 shadow-xl"
        >
          {loading && (
            <p className="flex items-center justify-center gap-2 px-3 py-4 text-xs text-slate-500">
              <LoaderCircle className="size-4 animate-spin" />
              در حال جستجو در سرور...
            </p>
          )}
          {!loading && error && (
            <p role="alert" className="px-3 py-3 text-xs leading-5 text-rose-700">
              {error}
            </p>
          )}
          {!loading && !error && options.length === 0 && (
            <p className="px-3 py-4 text-center text-xs text-slate-500">{emptyMessage}</p>
          )}
          {!loading &&
            !error &&
            options.map((option) => (
              <button
                type="button"
                role="option"
                aria-selected={value === option.value}
                key={option.value}
                onClick={() => {
                  setSelectedOption(option);
                  setQuery("");
                  setOpen(false);
                  onChange(option.value, option);
                }}
                className="flex w-full items-center justify-between gap-3 rounded-lg px-3 py-2 text-start text-sm text-slate-700 hover:bg-fuchsia-50 hover:text-fuchsia-800 focus-visible:bg-fuchsia-50 focus-visible:outline-none"
              >
                <span className="min-w-0">
                  <span className="block truncate">{option.label}</span>
                  {option.description && (
                    <span className="mt-0.5 block truncate text-xs text-slate-500">
                      {option.description}
                    </span>
                  )}
                </span>
                {value === option.value && <Check className="size-4 shrink-0 text-fuchsia-700" />}
              </button>
            ))}
          {!loading && !error && hasMore && (
            <button
              type="button"
              onClick={() => void loadNextPage()}
              disabled={loadingMore}
              className="flex w-full items-center justify-center gap-2 rounded-lg px-3 py-2 text-xs font-medium text-fuchsia-700 hover:bg-fuchsia-50 disabled:opacity-60"
            >
              {loadingMore && <LoaderCircle className="size-4 animate-spin" />}
              نمایش نتایج بیشتر
            </button>
          )}
        </div>
      )}
    </div>
  );
}
