"use client";

import { useEffect, useMemo, useState } from "react";
import { MoreHorizontal, Plus, Search } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
} from "@/components/ui/pagination";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { SalonService } from "@/features/salons/list/salon.service";
import type { GetSelectListSalonResponse } from "@/features/salons/list/salon.types";
import { handleApiError } from "@/lib/api/handle-api-error";

import { ServiceTariffDetailsDialog } from "./service-tariff-details-dialog";
import { ServiceTariffFormDialog } from "./service-tariff-form-dialog";
import { ServiceTariffService } from "./service-tariff.service";
import { formatApiDate } from "./service-tariff-date";
import { useServiceTariffs } from "./use-service-tariffs";

const PAGE_SIZE = 10;

export function ServiceTariffsPage() {
  const [salons, setSalons] = useState<GetSelectListSalonResponse[]>([]);
  const [salonId, setSalonId] = useState(0);
  const [salonsLoading, setSalonsLoading] = useState(true);
  const [salonsError, setSalonsError] = useState<string | null>(null);
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const [refreshKey, setRefreshKey] = useState(0);
  const [formOpen, setFormOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [detailsId, setDetailsId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<{ id: number; name: string } | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    const controller = new AbortController();
    Promise.resolve()
      .then(async () => {
        const api = new SalonService();
        const first = await api.getSelectList(
          { PageNumber: 1, PageSize: 100 },
          { signal: controller.signal },
        );
        if (!first.data) throw new Error(first.message ?? "فهرست سالن‌ها دریافت نشد.");
        const items = [...first.data.list];
        for (let currentPage = 2; currentPage <= first.data.pagination.totalPages; currentPage += 1) {
          const response = await api.getSelectList(
            { PageNumber: currentPage, PageSize: 100 },
            { signal: controller.signal },
          );
          if (!response.data) throw new Error(response.message ?? "فهرست سالن‌ها کامل دریافت نشد.");
          items.push(...response.data.list);
        }
        if (controller.signal.aborted) return;
        const activeSalons = items.filter((item) => item.status);
        setSalons(activeSalons);
        if (activeSalons.length === 1) setSalonId(activeSalons[0].id);
        if (activeSalons.length === 0) setSalonsError("سالن فعالی برای مدیریت تعرفه‌ها وجود ندارد.");
      })
      .catch((reason: unknown) => {
        if (!controller.signal.aborted) setSalonsError(handleApiError(reason));
      })
      .finally(() => {
        if (!controller.signal.aborted) setSalonsLoading(false);
      });
    return () => controller.abort();
  }, []);

  const { data, error, loading } = useServiceTariffs(salonId, refreshKey);
  const tariffs = useMemo(() => {
    const normalized = query.trim().toLocaleLowerCase();
    return (data ?? []).filter((tariff) => !normalized || tariff.name.toLocaleLowerCase().includes(normalized));
  }, [data, query]);
  const totalPages = Math.max(1, Math.ceil(tariffs.length / PAGE_SIZE));
  const visibleTariffs = tariffs.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

  function openCreate() {
    setEditingId(null);
    setFormOpen(true);
  }

  function openEdit(id: number) {
    setDetailsId(null);
    setEditingId(id);
    setFormOpen(true);
  }

  async function confirmDelete() {
    if (!deleteTarget || deleting) return;
    setDeleting(true);
    setDeleteError(null);
    try {
      const response = await new ServiceTariffService().delete(deleteTarget.id, salonId);
      if (response.statusCode >= 400) throw new Error(response.message ?? "حذف تعرفه انجام نشد.");
      setDeleteTarget(null);
      setDetailsId(null);
      setRefreshKey((current) => current + 1);
    } catch (reason: unknown) {
      setDeleteError(handleApiError(reason));
    } finally {
      setDeleting(false);
    }
  }

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[182px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] pt-2 sm:w-[84%] sm:max-w-[1200px]">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem><BreadcrumbLink href="/services">مدیریت خدمات</BreadcrumbLink></BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem><BreadcrumbPage className="text-white">تعرفه خدمات</BreadcrumbPage></BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <div className="mt-1 flex flex-row-reverse items-center justify-between gap-3">
            <Button
              className="h-8 shrink-0 bg-fuchsia-700 text-white hover:bg-fuchsia-800"
              onClick={openCreate}
              disabled={!salonId}
            >
              <Plus className="size-4" />
              تعرفه جدید
            </Button>
            <div className="relative w-full max-w-[340px]">
              <Search className="absolute start-2.5 top-1/2 size-4 -translate-y-1/2 text-fuchsia-800" />
              <Input
                aria-label="جستجو بر اساس نام تعرفه"
                value={query}
                onChange={(event) => {
                  setQuery(event.target.value);
                  setPage(1);
                }}
                placeholder="جستجو بر اساس نام تعرفه..."
                className="h-8 border-slate-300 bg-white pe-3 ps-8 text-xs"
              />
            </div>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-[114px] w-[92%] pb-8 sm:w-[84%] sm:max-w-[1200px]">
        <div className="flex min-h-[368px] flex-col rounded-xl border border-fuchsia-100 bg-background px-2 pb-2 pt-1 shadow-sm">
          <div className="flex flex-wrap items-center justify-between gap-2 border-b border-slate-100 px-2 py-2">
            <label className="flex items-center gap-2 text-xs text-slate-600">
              سالن
              <select
                aria-label="انتخاب سالن برای تعرفه‌ها"
                value={salonId || ""}
                disabled={salonsLoading || salons.length === 0}
                onChange={(event) => {
                  setSalonId(Number(event.target.value));
                  setPage(1);
                }}
                className="h-8 min-w-40 rounded-md border border-slate-200 bg-white px-2 text-sm"
              >
                <option value="" disabled>{salonsLoading ? "در حال دریافت..." : "انتخاب سالن"}</option>
                {salons.map((salon) => <option key={salon.id} value={salon.id}>{salon.name}</option>)}
              </select>
            </label>
            {salonId > 0 && <span className="text-xs text-slate-500">{tariffs.length.toLocaleString("fa-IR")} تعرفه</span>}
          </div>
          <div className="overflow-x-auto rounded-lg">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/60 hover:bg-muted/60">
                  <TableHead className="h-9 min-w-40 px-2 text-start">نام تعرفه</TableHead>
                  <TableHead className="h-9 min-w-28 px-2 text-start">شروع اعتبار</TableHead>
                  <TableHead className="h-9 min-w-28 px-2 text-start">پایان اعتبار</TableHead>
                  <TableHead className="h-9 min-w-28 px-2 text-start">تعداد خدمات</TableHead>
                  <TableHead className="h-9 min-w-24 px-2 text-start">وضعیت اعتبار</TableHead>
                  <TableHead className="h-9 w-14 px-2" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {salonsError ? (
                  <TableRow><TableCell colSpan={6} role="alert" className="py-8 text-center text-sm text-destructive">{salonsError}</TableCell></TableRow>
                ) : salonsLoading ? (
                  <TableRow><TableCell colSpan={6} role="status" className="py-8 text-center text-sm">در حال دریافت سالن‌ها...</TableCell></TableRow>
                ) : !salonId ? (
                  <TableRow><TableCell colSpan={6} className="py-8 text-center text-sm">برای مشاهده تعرفه‌ها، سالن را انتخاب کنید.</TableCell></TableRow>
                ) : error ? (
                  <TableRow><TableCell colSpan={6} role="alert" className="py-8 text-center text-sm text-destructive">{error}</TableCell></TableRow>
                ) : loading ? (
                  <TableRow><TableCell colSpan={6} role="status" className="py-8 text-center text-sm">در حال دریافت تعرفه‌ها...</TableCell></TableRow>
                ) : visibleTariffs.length === 0 ? (
                  <TableRow><TableCell colSpan={6} className="py-8 text-center text-sm">تعرفه‌ای یافت نشد.</TableCell></TableRow>
                ) : (
                  visibleTariffs.map((tariff) => (
                    <TableRow key={tariff.id} className="h-9">
                      <TableCell className="px-2 py-2 text-sm">
                        <button
                          type="button"
                          className="rounded-sm text-start hover:text-fuchsia-700 focus-visible:outline-2 focus-visible:outline-fuchsia-600"
                          onClick={() => setDetailsId(tariff.id)}
                        >
                          {tariff.name}
                        </button>
                      </TableCell>
                      <TableCell className="px-2 py-2 text-sm">{formatApiDate(tariff.validFrom)}</TableCell>
                      <TableCell className="px-2 py-2 text-sm">{formatApiDate(tariff.validTo)}</TableCell>
                      <TableCell className="px-2 py-2 text-sm">{tariff.services.length.toLocaleString("fa-IR")}</TableCell>
                      <TableCell className="px-2 py-2">
                        <Badge className="bg-fuchsia-100 text-fuchsia-800 hover:bg-fuchsia-100">
                          {tariff.validTo ? "دارای پایان اعتبار" : "بدون محدودیت"}
                        </Badge>
                      </TableCell>
                      <TableCell className="px-2 py-2">
                        <DropdownMenu>
                          <DropdownMenuTrigger
                            render={<Button variant="ghost" size="icon" aria-label={`عملیات تعرفه ${tariff.name}`} className="size-7 text-fuchsia-700 hover:text-fuchsia-800" />}
                          >
                            <MoreHorizontal className="size-4" />
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="start">
                            <DropdownMenuItem onClick={() => setDetailsId(tariff.id)}>مشاهده</DropdownMenuItem>
                            <DropdownMenuItem onClick={() => openEdit(tariff.id)}>ویرایش</DropdownMenuItem>
                            <DropdownMenuItem
                              className="text-destructive focus:text-destructive"
                              onClick={() => {
                                setDeleteError(null);
                                setDeleteTarget({ id: tariff.id, name: tariff.name });
                              }}
                            >
                              حذف
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </div>
          <Pagination className="mt-auto pt-5" dir="ltr">
            <PaginationContent className="gap-2">
              <PaginationItem>
                <PaginationPrevious
                  href="#"
                  text=""
                  aria-label="صفحه قبلی"
                  className="size-7 rounded-full border border-fuchsia-500 bg-white p-0 text-fuchsia-700 hover:bg-fuchsia-50"
                  onClick={(event) => {
                    event.preventDefault();
                    setPage((current) => Math.max(1, current - 1));
                  }}
                />
              </PaginationItem>
              <PaginationItem className="flex h-7 items-center gap-0 rounded-full border border-slate-300 bg-white px-1">
                {Array.from(new Set([1, page, totalPages])).sort((a, b) => a - b).map((pageNumber, index, pages) => (
                  <span key={pageNumber} className="flex items-center">
                    {index > 0 && pageNumber - pages[index - 1] > 1 && <PaginationEllipsis className="size-6 text-muted-foreground" />}
                    <PaginationLink
                      href="#"
                      isActive={pageNumber === page}
                      size="icon-sm"
                      className={pageNumber === page ? "size-6 rounded-full border-0 bg-fuchsia-600 text-white hover:bg-fuchsia-700 hover:text-white" : "size-6 rounded-full border-0 text-foreground hover:bg-fuchsia-50"}
                      onClick={(event) => {
                        event.preventDefault();
                        setPage(pageNumber);
                      }}
                    >
                      {pageNumber.toLocaleString("fa-IR")}
                    </PaginationLink>
                  </span>
                ))}
              </PaginationItem>
              <PaginationItem>
                <PaginationNext
                  href="#"
                  text=""
                  aria-label="صفحه بعدی"
                  className="size-7 rounded-full border border-slate-300 bg-white p-0 text-slate-600 hover:bg-slate-50"
                  onClick={(event) => {
                    event.preventDefault();
                    setPage((current) => Math.min(totalPages, current + 1));
                  }}
                />
              </PaginationItem>
            </PaginationContent>
          </Pagination>
        </div>
      </main>

      <ServiceTariffFormDialog
        open={formOpen}
        onOpenChange={setFormOpen}
        onSaved={() => setRefreshKey((current) => current + 1)}
        tariffId={editingId}
        salonId={salonId}
      />
      <ServiceTariffDetailsDialog
        open={detailsId !== null}
        onOpenChange={(open) => !open && setDetailsId(null)}
        tariffId={detailsId}
        salonId={salonId}
        onChanged={() => setRefreshKey((current) => current + 1)}
        onEdit={openEdit}
      />
      <Dialog
        open={deleteTarget !== null}
        onOpenChange={(open) => {
          if (!open && !deleting) {
            setDeleteTarget(null);
            setDeleteError(null);
          }
        }}
      >
        <DialogContent dir="rtl" className="gap-4 sm:max-w-md">
          <div className="space-y-2">
            <DialogTitle className="text-right">حذف تعرفه</DialogTitle>
            <DialogDescription className="text-right leading-6">
              تعرفه «{deleteTarget?.name}» از فهرست فعال حذف شود؟ خدمات متصل به آن نیز غیرفعال می‌شوند.
            </DialogDescription>
          </div>
          {deleteError && <p role="alert" className="text-sm text-destructive">{deleteError}</p>}
          <DialogFooter className="flex-row-reverse">
            <Button type="button" variant="destructive" disabled={deleting} onClick={() => void confirmDelete()}>
              {deleting ? "در حال حذف..." : "حذف تعرفه"}
            </Button>
            <Button type="button" variant="outline" disabled={deleting} onClick={() => setDeleteTarget(null)}>انصراف</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
