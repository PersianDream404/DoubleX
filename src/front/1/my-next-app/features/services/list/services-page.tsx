"use client";

import { useEffect, useMemo, useState } from "react";
import { MoreHorizontal, Plus, Search } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
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
import { handleApiError } from "@/lib/api/handle-api-error";
import { useUserContext } from "@/contexts/user-context";

import { ServiceCatalogService } from "./service.service";
import { ServiceDetailsDialog } from "./service-details-dialog";
import { ServiceFormDialog } from "./service-form-dialog";
import { useServices } from "./use-services";

const PAGE_SIZE = 10;

type CategoryState = {
  salonId: number;
  names: Map<number, string>;
  error: string | null;
};

export function ServicesPage() {
  const { salonId, loading: userLoading, error: userError } = useUserContext();
  const [categoryState, setCategoryState] = useState<CategoryState>({
    salonId: 0,
    names: new Map(),
    error: null,
  });
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const [refreshKey, setRefreshKey] = useState(0);
  const [formOpen, setFormOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [detailsId, setDetailsId] = useState<number | null>(null);
  const [deactivateTarget, setDeactivateTarget] = useState<{
    id: number;
    name: string;
  } | null>(null);
  const [deactivateError, setDeactivateError] = useState<string | null>(null);
  const [deactivating, setDeactivating] = useState(false);

  useEffect(() => {
    if (!salonId) return;
    const controller = new AbortController();
    new ServiceCatalogService()
      .getCategories({ signal: controller.signal })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "فهرست دسته‌بندی‌ها دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setCategoryState({
            salonId,
            names: new Map(response.data.map((category) => [category.id, category.name])),
            error: null,
          });
        }
      })
      .catch((reason: unknown) => {
        if (!controller.signal.aborted) {
          setCategoryState({ salonId, names: new Map(), error: handleApiError(reason) });
        }
      });
    return () => controller.abort();
  }, [salonId]);

  const { data: services, error, loading } = useServices(salonId ?? 0, refreshKey);
  const filteredServices = useMemo(() => {
    const normalized = query.trim().toLocaleLowerCase();
    return (services ?? []).filter((service) =>
      !normalized ||
      service.name.toLocaleLowerCase().includes(normalized) ||
      service.code.toLocaleLowerCase().includes(normalized),
    );
  }, [query, services]);
  const totalPages = Math.max(1, Math.ceil(filteredServices.length / PAGE_SIZE));
  const visibleServices = filteredServices.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);
  const categoryNames = categoryState.salonId === salonId ? categoryState.names : new Map<number, string>();
  const categoriesError = categoryState.salonId === salonId ? categoryState.error : null;
  async function confirmDeactivate() {
    if (!deactivateTarget || salonId === null || deactivating) return;
    setDeactivating(true);
    setDeactivateError(null);
    try {
      await new ServiceCatalogService().deactivate(deactivateTarget.id);
      setDeactivateTarget(null);
      setRefreshKey((current) => current + 1);
    } catch (reason: unknown) {
      setDeactivateError(handleApiError(reason));
    } finally {
      setDeactivating(false);
    }
  }

  function openCreate() {
    setEditingId(null);
    setFormOpen(true);
  }

  function openEdit(id: number) {
    setDetailsId(null);
    setEditingId(id);
    setFormOpen(true);
  }

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[182px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] pt-2 sm:w-[84%] sm:max-w-[1200px]">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem><BreadcrumbLink href="/services">مدیریت خدمات</BreadcrumbLink></BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem><BreadcrumbPage className="text-white">فهرست خدمات</BreadcrumbPage></BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <div className="mt-1 flex flex-row-reverse items-center justify-between gap-3">
            <Button
              className="h-8 shrink-0 bg-fuchsia-700 text-white hover:bg-fuchsia-800"
              onClick={openCreate}
              disabled={!salonId}
            >
              <Plus className="size-4" />
              خدمت جدید
            </Button>
            <div className="relative w-full max-w-[340px]">
              <Search className="absolute start-2.5 top-1/2 size-4 -translate-y-1/2 text-fuchsia-800" />
              <Input
                aria-label="جستجو بر اساس نام یا کد خدمت"
                value={query}
                onChange={(event) => {
                  setQuery(event.target.value);
                  setPage(1);
                }}
                placeholder="جستجو بر اساس نام یا کد خدمت..."
                className="h-8 border-slate-300 bg-white pe-3 ps-8 text-xs"
              />
            </div>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-[114px] w-[92%] pb-8 sm:w-[84%] sm:max-w-[1200px]">
        <div className="flex min-h-[400px] flex-col rounded-xl border border-fuchsia-100 bg-background px-2 pb-2 pt-2 shadow-sm">
          <div className="flex flex-wrap items-center justify-between gap-2 border-b border-slate-100 px-2 pb-2">
            <label className="flex items-center gap-2 text-xs text-slate-600">
              سالن
              <span className="text-sm text-slate-700">
                {userLoading ? "در حال دریافت..." : salonId ? salonId.toLocaleString("fa-IR") : "در دسترس نیست"}
              </span>
            </label>
            {salonId !== null && salonId > 0 && <span className="text-xs text-slate-500">{filteredServices.length.toLocaleString("fa-IR")} خدمت</span>}
          </div>
          {categoriesError && <p role="alert" className="px-2 pt-2 text-xs text-destructive">{categoriesError}</p>}
          <div className="overflow-x-auto rounded-lg">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/60 hover:bg-muted/60">
                  <TableHead className="h-9 min-w-40 px-2 text-start">نام خدمت</TableHead>
                  <TableHead className="h-9 min-w-24 px-2 text-start">کد</TableHead>
                  <TableHead className="h-9 min-w-32 px-2 text-start">دسته‌بندی</TableHead>
                  <TableHead className="h-9 min-w-28 px-2 text-start">مدت زمان</TableHead>
                  <TableHead className="h-9 min-w-24 px-2 text-start">وضعیت</TableHead>
                  <TableHead className="h-9 w-14 px-2" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {userError ? (
                  <TableRow><TableCell colSpan={6} role="alert" className="py-8 text-center text-sm text-destructive">{userError}</TableCell></TableRow>
                ) : userLoading ? (
                  <TableRow><TableCell colSpan={6} role="status" className="py-8 text-center text-sm">در حال دریافت سالن‌ها...</TableCell></TableRow>
                ) : !salonId ? (
                  <TableRow><TableCell colSpan={6} className="py-8 text-center text-sm">شناسه سالن در توکن ورود شما موجود نیست.</TableCell></TableRow>
                ) : error ? (
                  <TableRow><TableCell colSpan={6} role="alert" className="py-8 text-center text-sm text-destructive">{error}</TableCell></TableRow>
                ) : loading ? (
                  <TableRow><TableCell colSpan={6} role="status" className="py-8 text-center text-sm">در حال دریافت خدمات...</TableCell></TableRow>
                ) : visibleServices.length === 0 ? (
                  <TableRow><TableCell colSpan={6} className="py-8 text-center text-sm">خدمتی یافت نشد.</TableCell></TableRow>
                ) : (
                  visibleServices.map((service) => (
                    <TableRow key={service.id} className="h-10">
                      <TableCell className="px-2 py-2 text-sm">
                        <button
                          type="button"
                          className="rounded-sm text-start hover:text-fuchsia-700 focus-visible:outline-2 focus-visible:outline-fuchsia-600"
                          onClick={() => setDetailsId(service.id)}
                        >
                          {service.name}
                        </button>
                      </TableCell>
                      <TableCell dir="ltr" className="px-2 py-2 text-start text-sm">{service.code}</TableCell>
                      <TableCell className="px-2 py-2 text-sm">{categoryNames.get(service.categoryId) ?? service.categoryId.toLocaleString("fa-IR")}</TableCell>
                      <TableCell className="px-2 py-2 text-sm">{service.durationMinutes.toLocaleString("fa-IR")} دقیقه</TableCell>
                      <TableCell className="px-2 py-2">
                        <Badge className={service.isActive ? "bg-emerald-100 text-emerald-700 hover:bg-emerald-100" : "bg-slate-100 text-slate-600 hover:bg-slate-100"}>
                          {service.isActive ? "فعال" : "غیرفعال"}
                        </Badge>
                      </TableCell>
                      <TableCell className="px-2 py-2">
                        <DropdownMenu>
                          <DropdownMenuTrigger
                            render={<Button variant="ghost" size="icon" aria-label={`عملیات ${service.name}`} className="size-7 text-fuchsia-700 hover:text-fuchsia-800" />}
                          >
                            <MoreHorizontal className="size-4" />
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="start">
                            <DropdownMenuItem onClick={() => setDetailsId(service.id)}>مشاهده</DropdownMenuItem>
                            <DropdownMenuItem onClick={() => openEdit(service.id)}>ویرایش</DropdownMenuItem>
                            {service.isActive ? (
                              <DropdownMenuItem
                                className="text-destructive focus:text-destructive"
                                onClick={() => {
                                  setDeactivateError(null);
                                  setDeactivateTarget({ id: service.id, name: service.name });
                                }}
                              >
                                غیرفعال کردن
                              </DropdownMenuItem>
                            ) : (
                              <DropdownMenuItem onClick={() => openEdit(service.id)}>فعال‌سازی از طریق ویرایش</DropdownMenuItem>
                            )}
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

      <ServiceFormDialog
        open={formOpen}
        onOpenChange={setFormOpen}
        onSaved={() => setRefreshKey((current) => current + 1)}
        serviceId={editingId}
        salonId={salonId ?? 0}
      />
      <ServiceDetailsDialog
        open={detailsId !== null}
        onOpenChange={(open) => !open && setDetailsId(null)}
        serviceId={detailsId}
        salonId={salonId ?? 0}
        onChanged={() => setRefreshKey((current) => current + 1)}
        onEdit={openEdit}
      />
      <Dialog
        open={deactivateTarget !== null}
        onOpenChange={(open) => {
          if (!open && !deactivating) {
            setDeactivateTarget(null);
            setDeactivateError(null);
          }
        }}
      >
        <DialogContent dir="rtl" className="gap-4 sm:max-w-md">
          <div className="space-y-2">
            <DialogTitle className="text-right">غیرفعال‌سازی خدمت</DialogTitle>
            <DialogDescription className="text-right leading-6">
              خدمت «{deactivateTarget?.name}» غیرفعال شود؟ این عملیات به‌جای حذف، وضعیت خدمت را تغییر می‌دهد.
            </DialogDescription>
          </div>
          {deactivateError && <p role="alert" className="text-sm text-destructive">{deactivateError}</p>}
          <DialogFooter className="flex-row-reverse">
            <Button type="button" variant="destructive" disabled={deactivating} onClick={() => void confirmDeactivate()}>
              {deactivating ? "در حال غیرفعال‌سازی..." : "غیرفعال کردن"}
            </Button>
            <Button type="button" variant="outline" disabled={deactivating} onClick={() => setDeactivateTarget(null)}>انصراف</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
