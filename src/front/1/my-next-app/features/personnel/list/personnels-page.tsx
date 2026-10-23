"use client";

import { useState } from "react";
import { MoreHorizontal, Plus, Search } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { handleApiError } from "@/lib/api/handle-api-error";
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

import { PersonnelDetailsDialog } from "./personnel-details-dialog";
import { PersonnelCreateDialog } from "./personnel-create-dialog";
import { PersonnelService } from "./personnel.service";
import { usePersonnels } from "./use-personnels";

export function PersonnelsPage() {
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const [createDialogOpen, setCreateDialogOpen] = useState(false);
  const [refreshKey, setRefreshKey] = useState(0);
  const [deleteTarget, setDeleteTarget] = useState<{
    id: number;
    fullName: string;
  } | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [selectedPersonnelId, setSelectedPersonnelId] = useState<number | null>(
    null,
  );
  const { data, error, loading } = usePersonnels(query, page, refreshKey);
  const totalPages = Math.max(1, data?.pagination.totalPages ?? 1);
  const personnels = data?.list ?? [];

  function requestDelete(id: number, fullName: string) {
    setSelectedPersonnelId(null);
    setDeleteError(null);
    setDeleteTarget({ id, fullName });
  }

  async function confirmDelete() {
    if (!deleteTarget || deleting) {
      return;
    }

    setDeleting(true);
    setDeleteError(null);

    try {
      await new PersonnelService().delete(deleteTarget.id);
      setDeleteTarget(null);
      setSelectedPersonnelId(null);
      setRefreshKey((current) => current + 1);
    } catch (error: unknown) {
      setDeleteError(handleApiError(error));
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
              <BreadcrumbItem>
                <BreadcrumbLink href="/staff">مدیریت پرسنل</BreadcrumbLink>
              </BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem>
                <BreadcrumbPage className="text-white">
                  فهرست پرسنل
                </BreadcrumbPage>
              </BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>

          <div className="mt-1 flex flex-row-reverse items-center justify-between gap-3">
            <Button
              className="h-8 shrink-0 bg-fuchsia-700 text-white hover:bg-fuchsia-800"
              onClick={() => setCreateDialogOpen(true)}
            >
              <Plus className="size-4" />
              پرسنل جدید
            </Button>

            <div className="relative w-full max-w-[340px]">
              <Search className="absolute start-2.5 top-1/2 size-4 -translate-y-1/2 text-fuchsia-800" />
              <Input
                aria-label="جستجو بر اساس نام پرسنل"
                value={query}
                onChange={(event) => {
                  setQuery(event.target.value);
                  setPage(1);
                }}
                placeholder="جستجو بر اساس نام پرسنل..."
                className="h-8 border-slate-300 bg-white pe-3 ps-8 text-xs"
              />
            </div>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-[114px] w-[92%] pb-8 sm:w-[84%] sm:max-w-[1200px]">
        <div className="flex min-h-[368px] flex-col rounded-xl border border-fuchsia-100 bg-background px-2 pb-2 pt-1 shadow-sm">
          <div className="overflow-hidden rounded-lg">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/60 hover:bg-muted/60">
                  <TableHead className="h-9 px-2 text-start">
                    نام و نام خانوادگی
                  </TableHead>
                  <TableHead className="h-9 px-2 text-start">کد ملی</TableHead>
                  <TableHead className="h-9 px-2 text-start">عنوان شغلی</TableHead>
                  <TableHead className="h-9 px-2 text-start">شماره کارت</TableHead>
                  <TableHead className="h-9 px-2 text-start">تاریخ تولد</TableHead>
                  <TableHead className="h-9 px-2 text-start">شماره همراه</TableHead>
                  <TableHead className="h-9 w-14 px-2" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {error ? (
                  <TableRow>
                    <TableCell
                      colSpan={7}
                      className="py-8 text-center text-sm text-destructive"
                      role="alert"
                    >
                      {error}
                    </TableCell>
                  </TableRow>
                ) : loading ? (
                  <TableRow>
                    <TableCell
                      colSpan={7}
                      className="py-8 text-center text-sm"
                      role="status"
                    >
                      در حال دریافت فهرست پرسنل...
                    </TableCell>
                  </TableRow>
                ) : personnels.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={7} className="py-8 text-center text-sm">
                      پرسنلی یافت نشد.
                    </TableCell>
                  </TableRow>
                ) : (
                  personnels.map((personnel) => (
                    <TableRow key={personnel.id} className="h-9">
                      <TableCell className="px-2 py-2 text-sm">
                        <button
                          type="button"
                          className="rounded-sm text-start hover:text-fuchsia-700 focus-visible:outline-2 focus-visible:outline-fuchsia-600"
                          onClick={() => setSelectedPersonnelId(personnel.id)}
                        >
                          {personnel.fullName}
                        </button>
                      </TableCell>
                      <TableCell className="px-2 py-2 text-sm">
                        {personnel.nationalCode}
                      </TableCell>
                      <TableCell className="px-2 py-2 text-sm">
                        {personnel.jobTitle}
                      </TableCell>
                      <TableCell className="px-2 py-2 text-sm">
                        {personnel.cardNumber}
                      </TableCell>
                      <TableCell className="px-2 py-2 text-sm">
                        {new Date(
                          `${personnel.birthDate}T00:00:00`,
                        ).toLocaleDateString("fa-IR")}
                      </TableCell>
                      <TableCell className="px-2 py-2 text-sm">
                        {personnel.mobilePhone}
                      </TableCell>
                      <TableCell className="px-2 py-2">
                        <DropdownMenu>
                          <DropdownMenuTrigger
                            render={
                              <Button
                                variant="ghost"
                                size="icon"
                                aria-label={`عملیات ${personnel.fullName}`}
                                className="size-7 text-fuchsia-700 hover:text-fuchsia-800"
                              />
                            }
                          >
                            <MoreHorizontal className="size-4" />
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="start">
                            <DropdownMenuItem
                              onClick={() =>
                                setSelectedPersonnelId(personnel.id)
                              }
                            >
                              مشاهده
                            </DropdownMenuItem>
                            <DropdownMenuItem>ویرایش</DropdownMenuItem>
                            <DropdownMenuItem
                              className="text-destructive focus:text-destructive"
                              onClick={() =>
                                requestDelete(personnel.id, personnel.fullName)
                              }
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
                    setPage((currentPage) => Math.max(1, currentPage - 1));
                  }}
                />
              </PaginationItem>

              <PaginationItem className="flex h-7 items-center gap-0 rounded-full border border-slate-300 bg-white px-1">
                {Array.from(
                  new Set([1, page, totalPages].filter((pageNumber) => pageNumber > 0)),
                )
                  .sort((first, second) => first - second)
                  .map((pageNumber, index, pages) => (
                    <span key={pageNumber} className="flex items-center">
                      {index > 0 && pageNumber - pages[index - 1] > 1 && (
                        <PaginationEllipsis className="size-6 text-muted-foreground" />
                      )}
                      <PaginationLink
                        href="#"
                        isActive={pageNumber === page}
                        size="icon-sm"
                        className={
                          pageNumber === page
                            ? "size-6 rounded-full border-0 bg-fuchsia-600 text-white hover:bg-fuchsia-700 hover:text-white"
                            : "size-6 rounded-full border-0 text-foreground hover:bg-fuchsia-50"
                        }
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
                  className="size-7 rounded-full border border-slate-300 bg-white p-0 text-slate-500 hover:bg-slate-50"
                  onClick={(event) => {
                    event.preventDefault();
                    setPage((currentPage) =>
                      Math.min(totalPages, currentPage + 1),
                    );
                  }}
                />
              </PaginationItem>
            </PaginationContent>
          </Pagination>
        </div>
      </main>
      <PersonnelDetailsDialog
        key={selectedPersonnelId ?? "closed"}
        personnelId={selectedPersonnelId}
        onOpenChange={(open) => {
          if (!open) {
            setSelectedPersonnelId(null);
          }
        }}
        onDeleteRequest={requestDelete}
      />
      <PersonnelCreateDialog
        open={createDialogOpen}
        onOpenChange={setCreateDialogOpen}
        onCreated={() => setRefreshKey((current) => current + 1)}
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
            <DialogTitle className="text-right">حذف پرسنل</DialogTitle>
            <DialogDescription className="text-right leading-6">
              آیا از حذف «{deleteTarget?.fullName}» مطمئن هستید؟ این عملیات
              قابل بازگشت نیست.
            </DialogDescription>
          </div>
          {deleteError && (
            <p className="text-sm text-destructive" role="alert">
              {deleteError}
            </p>
          )}
          <DialogFooter className="flex-row-reverse">
            <Button
              type="button"
              variant="destructive"
              disabled={deleting}
              onClick={() => void confirmDelete()}
            >
              {deleting ? "در حال حذف..." : "حذف"}
            </Button>
            <Button
              type="button"
              variant="outline"
              disabled={deleting}
              onClick={() => {
                setDeleteTarget(null);
                setDeleteError(null);
              }}
            >
              انصراف
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
