"use client";

import { useState } from "react";
import { MoreHorizontal, Plus, Search } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { Pagination, PaginationContent, PaginationEllipsis, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious } from "@/components/ui/pagination";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { handleApiError } from "@/lib/api/handle-api-error";

import { CustomerDetailsDialog } from "./customer-details-dialog";
import { CustomerFormDialog } from "./customer-form-dialog";
import { CustomerService } from "./customer.service";
import type { CustomerDto } from "./customer.types";
import { useCustomers } from "./use-customers";

function displayValue(value: string | null | undefined) {
  return value?.trim() || "—";
}

export function CustomersPage() {
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const [refreshKey, setRefreshKey] = useState(0);
  const [formOpen, setFormOpen] = useState(false);
  const [selectedCustomerId, setSelectedCustomerId] = useState<number | null>(null);
  const [startEditing, setStartEditing] = useState(false);
  const [deleteTarget, setDeleteTarget] = useState<CustomerDto | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [toggleTarget, setToggleTarget] = useState<CustomerDto | null>(null);
  const [toggleError, setToggleError] = useState<string | null>(null);
  const [toggling, setToggling] = useState(false);
  const { data, error, loading } = useCustomers(query, page, refreshKey);
  const customers = data?.list ?? [];
  const totalPages = Math.max(1, data?.pagination.totalPages ?? 1);

  function openCreate() {
    setFormOpen(true);
  }

  function openEdit(customer: CustomerDto) {
    setStartEditing(true);
    setSelectedCustomerId(customer.id);
  }

  async function confirmDelete() {
    if (!deleteTarget || deleting) return;
    setDeleting(true);
    setDeleteError(null);
    try {
      await new CustomerService().delete(deleteTarget.id);
      setDeleteTarget(null);
      setSelectedCustomerId(null);
      setRefreshKey((current) => current + 1);
    } catch (requestError: unknown) {
      setDeleteError(handleApiError(requestError));
    } finally {
      setDeleting(false);
    }
  }

  async function confirmToggle() {
    if (!toggleTarget || toggling) return;
    setToggling(true);
    setToggleError(null);
    try {
      await new CustomerService().toggle(toggleTarget.id);
      setToggleTarget(null);
      setSelectedCustomerId(null);
      setRefreshKey((current) => current + 1);
    } catch (requestError: unknown) {
      setToggleError(handleApiError(requestError));
    } finally {
      setToggling(false);
    }
  }

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[182px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] pt-2 sm:w-[84%] sm:max-w-[1200px]">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem><BreadcrumbLink href="/customers">مدیریت مشتریان</BreadcrumbLink></BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem><BreadcrumbPage className="text-white">فهرست مشتریان</BreadcrumbPage></BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <div className="mt-1 flex flex-row-reverse items-center justify-between gap-3">
            <Button onClick={openCreate} className="h-8 shrink-0 bg-fuchsia-700 text-white hover:bg-fuchsia-800">
              <Plus className="size-4" />
              مشتری جدید
            </Button>
            <div className="relative w-full max-w-[340px]">
              <Search className="absolute start-2.5 top-1/2 size-4 -translate-y-1/2 text-fuchsia-800" />
              <Input
                aria-label="جستجو بر اساس نام، کد ملی یا شماره همراه"
                value={query}
                onChange={(event) => {
                  setQuery(event.target.value);
                  setPage(1);
                }}
                placeholder="نام، کد ملی یا شماره همراه..."
                className="h-8 border-slate-300 bg-white pe-3 ps-8 text-xs"
              />
            </div>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-[114px] w-[92%] pb-8 sm:w-[84%] sm:max-w-[1200px]">
        <div className="flex min-h-[368px] flex-col rounded-xl border border-fuchsia-100 bg-background px-2 pb-2 pt-1 shadow-sm">
          <div className="overflow-x-auto rounded-lg">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/60 hover:bg-muted/60">
                  <TableHead className="h-9 px-2 text-start">نام و نام خانوادگی</TableHead>
                  <TableHead className="h-9 px-2 text-start">کد ملی</TableHead>
                  <TableHead className="h-9 px-2 text-start">شماره همراه</TableHead>
                  <TableHead className="h-9 px-2 text-start">شغل</TableHead>
                  <TableHead className="h-9 px-2 text-start">وضعیت</TableHead>
                  <TableHead className="h-9 w-14 px-2" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {error ? (
                  <TableRow><TableCell colSpan={6} role="alert" className="py-8 text-center text-sm text-destructive">{error}</TableCell></TableRow>
                ) : loading ? (
                  <TableRow><TableCell colSpan={6} className="py-8 text-center text-sm">در حال دریافت مشتریان...</TableCell></TableRow>
                ) : customers.length === 0 ? (
                  <TableRow><TableCell colSpan={6} className="py-8 text-center text-sm">مشتری‌ای با این مشخصات پیدا نشد.</TableCell></TableRow>
                ) : customers.map((customer) => (
                  <TableRow
                    key={customer.id}
                    className="h-10 cursor-pointer"
                    onClick={() => { setStartEditing(false); setSelectedCustomerId(customer.id); }}
                  >
                    <TableCell className="px-2 py-2 text-sm font-medium">{customer.fullName}</TableCell>
                    <TableCell className="px-2 py-2 text-sm" dir="ltr">{customer.nationalCode}</TableCell>
                    <TableCell className="px-2 py-2 text-sm" dir="ltr">{customer.mobilePhone}</TableCell>
                    <TableCell className="px-2 py-2 text-sm">{displayValue(customer.jobTitle)}</TableCell>
                    <TableCell className="px-2 py-2">
                      <Badge className={customer.status ? "bg-emerald-600 text-white hover:bg-emerald-600" : "bg-muted text-muted-foreground hover:bg-muted"}>
                        {customer.status ? "فعال" : "غیرفعال"}
                      </Badge>
                    </TableCell>
                    <TableCell className="px-2 py-2" onClick={(event) => event.stopPropagation()}>
                      <DropdownMenu>
                        <DropdownMenuTrigger render={<Button variant="ghost" size="icon" aria-label={`عملیات مشتری ${customer.fullName}`} className="size-7 text-fuchsia-700 hover:text-fuchsia-800" />}>
                          <MoreHorizontal className="size-4" />
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="start">
                          <DropdownMenuItem onClick={() => { setStartEditing(false); setSelectedCustomerId(customer.id); }}>مشاهده</DropdownMenuItem>
                          <DropdownMenuItem onClick={() => openEdit(customer)}>ویرایش</DropdownMenuItem>
                          <DropdownMenuItem onClick={() => { setToggleError(null); setToggleTarget(customer); }}>
                            {customer.status ? "غیرفعال کردن" : "فعال کردن"}
                          </DropdownMenuItem>
                          <DropdownMenuItem className="text-destructive" onClick={() => { setDeleteError(null); setDeleteTarget(customer); }}>حذف</DropdownMenuItem>
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>

          <Pagination className="mt-auto pt-5" dir="ltr">
            <PaginationContent className="gap-2">
              <PaginationItem>
                <PaginationPrevious href="#" text="" aria-label="صفحه قبلی" className="size-7 rounded-full border border-fuchsia-500 bg-white p-0 text-fuchsia-700 hover:bg-fuchsia-50" onClick={(event) => { event.preventDefault(); setPage((current) => Math.max(1, current - 1)); }} />
              </PaginationItem>
              <PaginationItem className="flex h-7 items-center gap-0 rounded-full border border-slate-300 bg-white px-1">
                {Array.from(new Set([1, page, totalPages])).sort((first, second) => first - second).map((pageNumber, index, pages) => (
                  <span key={pageNumber} className="flex items-center">
                    {index > 0 && pageNumber - pages[index - 1] > 1 && <PaginationEllipsis className="size-6 text-muted-foreground" />}
                    <PaginationLink href="#" isActive={pageNumber === page} size="icon-sm" className={pageNumber === page ? "size-6 rounded-full border-0 bg-fuchsia-600 text-white hover:bg-fuchsia-700" : "size-6 rounded-full border-0 text-slate-600"} onClick={(event) => { event.preventDefault(); setPage(pageNumber); }}>{pageNumber}</PaginationLink>
                  </span>
                ))}
              </PaginationItem>
              <PaginationItem>
                <PaginationNext href="#" text="" aria-label="صفحه بعدی" className="size-7 rounded-full border border-fuchsia-500 bg-white p-0 text-fuchsia-700 hover:bg-fuchsia-50" onClick={(event) => { event.preventDefault(); setPage((current) => Math.min(totalPages, current + 1)); }} />
              </PaginationItem>
            </PaginationContent>
          </Pagination>
        </div>
      </main>

      <CustomerFormDialog
        open={formOpen}
        customer={null}
        onOpenChange={(open) => {
          setFormOpen(open);
        }}
        onSaved={() => setRefreshKey((current) => current + 1)}
      />

      <CustomerDetailsDialog
        open={selectedCustomerId !== null}
        customerId={selectedCustomerId}
        startEditing={startEditing}
        onOpenChange={(open) => {
          if (!open) {
            setSelectedCustomerId(null);
            setStartEditing(false);
          }
        }}
        onSaved={() => setRefreshKey((current) => current + 1)}
        onDelete={(customer) => {
          setSelectedCustomerId(null);
          setStartEditing(false);
          setDeleteError(null);
          setDeleteTarget(customer);
        }}
      />

      <Dialog open={deleteTarget !== null} onOpenChange={(open) => !open && !deleting && setDeleteTarget(null)}>
        <DialogContent dir="rtl" className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>حذف مشتری</DialogTitle>
            <DialogDescription>
              آیا از حذف مشتری «{deleteTarget?.fullName}» مطمئن هستید؟ این عملیات قابل بازگشت نیست.
            </DialogDescription>
          </DialogHeader>
          {deleteError && <p role="alert" className="text-sm text-destructive">{deleteError}</p>}
          <DialogFooter>
            <Button variant="outline" onClick={() => setDeleteTarget(null)} disabled={deleting}>انصراف</Button>
            <Button variant="destructive" onClick={() => void confirmDelete()} disabled={deleting}>
              {deleting ? "در حال حذف..." : "حذف مشتری"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={toggleTarget !== null} onOpenChange={(open) => !open && !toggling && setToggleTarget(null)}>
        <DialogContent dir="rtl" className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>{toggleTarget?.status ? "غیرفعال کردن مشتری" : "فعال کردن مشتری"}</DialogTitle>
            <DialogDescription>
              وضعیت مشتری «{toggleTarget?.fullName}» تغییر کند؟
            </DialogDescription>
          </DialogHeader>
          {toggleError && <p role="alert" className="text-sm text-destructive">{toggleError}</p>}
          <DialogFooter>
            <Button variant="outline" onClick={() => setToggleTarget(null)} disabled={toggling}>انصراف</Button>
            <Button onClick={() => void confirmToggle()} disabled={toggling} className="bg-fuchsia-700 text-white hover:bg-fuchsia-800">
              {toggling ? "در حال تغییر..." : "تأیید"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
