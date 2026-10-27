"use client";

import { useEffect, useMemo, useState } from "react";
import {
  ArrowDownToLine,
  ArrowUpFromLine,
  CircleDollarSign,
  LoaderCircle,
  MoreHorizontal,
  Plus,
  RefreshCw,
  Search,
} from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Pagination, PaginationContent, PaginationEllipsis, PaginationItem, PaginationLink, PaginationNext, PaginationPrevious } from "@/components/ui/pagination";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { handleApiError } from "@/lib/api/handle-api-error";

import {
  CashMovementSchema,
  CashRegisterFormSchema,
  ClosingBalanceSchema,
  OpeningBalanceSchema,
} from "../cash-register.schema";
import { CashRegisterService } from "../cash-register.service";
import type {
  CashRegisterBalance,
  CashRegisterOption,
  CashRegisterSession,
  CashRegisterTransaction,
} from "../cash-register.types";
import { useCashRegisters } from "./use-cash-registers";

type DialogAction = "create" | "edit" | "open" | "close" | "cash-in" | "cash-out";

const EMPTY_FIELDS = { name: "", code: "", description: "" };

function amountLabel(value: number) {
  return `${new Intl.NumberFormat("fa-IR").format(value)} تومان`;
}

function dateLabel(value: string) {
  return new Intl.DateTimeFormat("fa-IR", {
    dateStyle: "short",
    timeStyle: "short",
  }).format(new Date(value));
}

function transactionLabel(type: CashRegisterTransaction["type"]) {
  switch (type) {
    case 1:
      return "موجودی افتتاح";
    case 2:
      return "پرداخت پذیرش";
    case 3:
      return "واریز دستی";
    case 4:
      return "برداشت دستی";
  }
}

export function CashRegistersPage() {
  const service = useMemo(() => new CashRegisterService(), []);
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const [refreshKey, setRefreshKey] = useState(0);
  const { data, error, loading } = useCashRegisters(query, page, refreshKey);
  const [selectedId, setSelectedId] = useState<number | null>(null);
  const [session, setSession] = useState<CashRegisterSession | null>(null);
  const [balance, setBalance] = useState<CashRegisterBalance | null>(null);
  const [transactions, setTransactions] = useState<CashRegisterTransaction[]>([]);
  const [detailsLoading, setDetailsLoading] = useState(false);
  const [detailsError, setDetailsError] = useState<string | null>(null);
  const [dialogAction, setDialogAction] = useState<DialogAction | null>(null);
  const [dialogRegisterId, setDialogRegisterId] = useState<number | null>(null);
  const [formValues, setFormValues] = useState(EMPTY_FIELDS);
  const [amount, setAmount] = useState("");
  const [description, setDescription] = useState("");
  const [formError, setFormError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [togglePendingId, setTogglePendingId] = useState<number | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);
  const registers = data?.list ?? [];
  const selectedRegister = registers.find((register) => register.id === selectedId) ?? null;
  const totalPages = Math.max(1, data?.pagination.totalPages ?? 1);

  useEffect(() => {
    if (!selectedRegister) {
      return;
    }

    const controller = new AbortController();
    Promise.resolve()
      .then(async () => {
        setDetailsLoading(true);
        setDetailsError(null);
        setSession(null);
        setBalance(null);
        setTransactions([]);
        const [transactionResponse, sessionResponse, balanceResponse] = await Promise.all([
          service.getTransactions(
            selectedRegister.id,
            { pageNumber: 1, pageSize: 50, sessionId: selectedRegister.activeSessionId ?? undefined },
            { signal: controller.signal },
          ),
          selectedRegister.activeSessionId !== null
            ? service.getActiveSession(selectedRegister.id, { signal: controller.signal })
            : Promise.resolve(null),
          selectedRegister.activeSessionId !== null
            ? service.getBalance(selectedRegister.id, { signal: controller.signal })
            : Promise.resolve(null),
        ]);
        if (!transactionResponse.data) {
          throw new Error(transactionResponse.message ?? "گردش‌های صندوق دریافت نشد.");
        }
        if (sessionResponse && !sessionResponse.data) {
          throw new Error(sessionResponse.message ?? "اطلاعات جلسهٔ باز دریافت نشد.");
        }
        if (balanceResponse && !balanceResponse.data) {
          throw new Error(balanceResponse.message ?? "موجودی صندوق دریافت نشد.");
        }
        if (!controller.signal.aborted) {
          setTransactions(transactionResponse.data.list);
          setSession(sessionResponse?.data ?? null);
          setBalance(balanceResponse?.data ?? null);
        }
      })
      .catch((requestError: unknown) => {
        if (!controller.signal.aborted) {
          setDetailsError(handleApiError(requestError));
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) {
          setDetailsLoading(false);
        }
      });

    return () => controller.abort();
  }, [selectedRegister, refreshKey, service]);

  function openDialog(action: DialogAction, register?: CashRegisterOption) {
    setDialogAction(action);
    setDialogRegisterId(register?.id ?? selectedRegister?.id ?? null);
    setFormError(null);
    setActionError(null);
    setAmount(
      action === "close" && balance
        ? String(balance.expectedBalance)
        : "",
    );
    setDescription("");
    setFormValues(
      register
        ? {
            name: register.name,
            code: register.code,
            description: register.description ?? "",
          }
        : EMPTY_FIELDS,
    );
  }

  async function submitAction(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!dialogAction || submitting) return;
    setSubmitting(true);
    setFormError(null);

    try {
      if (dialogAction === "create" || dialogAction === "edit") {
        const parsed = CashRegisterFormSchema.safeParse(formValues);
        if (!parsed.success) {
          throw new Error(parsed.error.issues[0]?.message ?? "اطلاعات صندوق معتبر نیست.");
        }
        const request = {
          ...parsed.data,
          description: parsed.data.description.trim() || null,
        };
        if (dialogAction === "create") {
          const response = await service.create(request);
          if (!response.data) throw new Error(response.message ?? "صندوق ایجاد نشد.");
          setSelectedId(response.data.id);
        } else if (dialogRegisterId !== null) {
          await service.update(dialogRegisterId, request);
          setSelectedId(dialogRegisterId);
        }
      } else if (dialogAction === "open" && selectedRegister) {
        const parsed = OpeningBalanceSchema.safeParse({ openingBalance: amount });
        if (!parsed.success) {
          throw new Error(parsed.error.issues[0]?.message ?? "موجودی اولیه معتبر نیست.");
        }
        await service.openSession(selectedRegister.id, parsed.data.openingBalance);
      } else if (dialogAction === "close" && selectedRegister && session) {
        const parsed = ClosingBalanceSchema.safeParse({ actualClosingBalance: amount });
        if (!parsed.success) {
          throw new Error(parsed.error.issues[0]?.message ?? "موجودی شمارش‌شده معتبر نیست.");
        }
        await service.closeSession(
          selectedRegister.id,
          session.id,
          parsed.data.actualClosingBalance,
        );
      } else if (
        (dialogAction === "cash-in" || dialogAction === "cash-out") &&
        selectedRegister
      ) {
        const parsed = CashMovementSchema.safeParse({ amount, description });
        if (!parsed.success) {
          throw new Error(parsed.error.issues[0]?.message ?? "اطلاعات گردش معتبر نیست.");
        }
        await service.recordMovement(selectedRegister.id, dialogAction, {
          amount: parsed.data.amount,
          description: parsed.data.description,
          idempotencyKey: crypto.randomUUID(),
        });
      }

      setDialogAction(null);
      setRefreshKey((current) => current + 1);
    } catch (requestError: unknown) {
      setFormError(handleApiError(requestError));
    } finally {
      setSubmitting(false);
    }
  }

  async function toggleRegister(register: CashRegisterOption) {
    setTogglePendingId(register.id);
    try {
      await service.toggle(register.id);
      setRefreshKey((current) => current + 1);
    } catch (requestError: unknown) {
      setActionError(handleApiError(requestError));
    } finally {
      setTogglePendingId(null);
    }
  }

  const dialogTitle = {
    create: "ایجاد صندوق",
    edit: "ویرایش صندوق",
    open: "افتتاح جلسهٔ صندوق",
    close: "بستن جلسهٔ صندوق",
    "cash-in": "ثبت واریز به صندوق",
    "cash-out": "ثبت برداشت از صندوق",
  } satisfies Record<DialogAction, string>;

  return (
    <div className="min-h-screen bg-white">
      <section className="h-[182px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[84%] max-w-[1200px] pt-2">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem><BreadcrumbLink href="/cash-registers">مدیریت صندوق</BreadcrumbLink></BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem><BreadcrumbPage className="text-white">فهرست صندوق‌ها</BreadcrumbPage></BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <div className="mt-1 flex flex-row-reverse items-center justify-between gap-3">
            <div className="flex shrink-0 gap-2">
              <Button
                variant="outline"
                onClick={() => setRefreshKey((current) => current + 1)}
                className="h-8 border-fuchsia-300 bg-white text-fuchsia-800 hover:bg-fuchsia-50"
              >
                <RefreshCw className="size-4" />
                بروزرسانی
              </Button>
              <Button
                onClick={() => openDialog("create")}
                className="h-8 bg-fuchsia-700 text-white hover:bg-fuchsia-800"
              >
                <Plus className="size-4" />
                صندوق جدید
              </Button>
            </div>
            <div className="relative w-full max-w-[340px]">
              <Search className="absolute start-2.5 top-1/2 size-4 -translate-y-1/2 text-fuchsia-800" />
              <Input
                value={query}
                onChange={(event) => {
                  setQuery(event.target.value);
                  setPage(1);
                }}
                placeholder="جستجو بر اساس نام یا کد صندوق..."
                className="h-8 border-slate-300 bg-white pe-3 ps-8 text-xs"
                aria-label="جستجوی صندوق"
              />
            </div>
          </div>
        </div>
      </section>

      <main className="mx-auto -mt-[114px] w-[84%] max-w-[1200px] space-y-4 pb-8">
        {actionError && <p role="alert" className="rounded-lg border border-rose-200 bg-rose-50 p-3 text-sm text-rose-700">{actionError}</p>}
        <section className="flex min-h-[300px] flex-col rounded-xl border border-fuchsia-100 bg-background px-2 pb-2 pt-1 shadow-sm">
          <div className="overflow-x-auto rounded-lg">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/60 hover:bg-muted/60">
                  <TableHead className="h-9 px-2 text-start">نام صندوق</TableHead>
                  <TableHead className="h-9 px-2 text-start">کد</TableHead>
                  <TableHead className="h-9 px-2 text-start">وضعیت</TableHead>
                  <TableHead className="h-9 px-2 text-start">جلسه</TableHead>
                  <TableHead className="h-9 w-14 px-2" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {error ? (
                  <TableRow><TableCell colSpan={5} role="alert" className="py-8 text-center text-sm text-destructive">{error}</TableCell></TableRow>
                ) : loading ? (
                  <TableRow><TableCell colSpan={5} className="py-8 text-center text-sm">در حال دریافت صندوق‌ها...</TableCell></TableRow>
                ) : registers.length === 0 ? (
                  <TableRow><TableCell colSpan={5} className="py-8 text-center text-sm">صندوقی یافت نشد.</TableCell></TableRow>
                ) : registers.map((register) => (
                  <TableRow
                    key={register.id}
                    className={`h-10 cursor-pointer ${selectedId === register.id ? "bg-fuchsia-50" : ""}`}
                    onClick={() => setSelectedId(register.id)}
                  >
                    <TableCell className="px-2 py-2 text-sm font-medium">{register.name}</TableCell>
                    <TableCell className="px-2 py-2 text-sm" dir="ltr">{register.code}</TableCell>
                    <TableCell className="px-2 py-2">
                      <Badge className={register.isActive ? "bg-emerald-600 text-white hover:bg-emerald-600" : "bg-muted text-muted-foreground hover:bg-muted"}>
                        {register.isActive ? "فعال" : "غیرفعال"}
                      </Badge>
                    </TableCell>
                    <TableCell className="px-2 py-2 text-sm">
                      {register.activeSessionId ? "باز" : "بسته"}
                    </TableCell>
                    <TableCell className="px-2 py-2" onClick={(event) => event.stopPropagation()}>
                      <DropdownMenu>
                        <DropdownMenuTrigger
                          render={<Button variant="ghost" size="icon" aria-label={`عملیات صندوق ${register.name}`} className="size-7 text-fuchsia-700 hover:text-fuchsia-800" />}
                        >
                          <MoreHorizontal className="size-4" />
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="start">
                          <DropdownMenuItem onClick={() => setSelectedId(register.id)}>مشاهده</DropdownMenuItem>
                          <DropdownMenuItem onClick={() => openDialog("edit", register)}>ویرایش</DropdownMenuItem>
                          <DropdownMenuItem
                            disabled={togglePendingId === register.id}
                            onClick={() => void toggleRegister(register)}
                          >
                            {register.isActive ? "غیرفعال کردن" : "فعال کردن"}
                          </DropdownMenuItem>
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
                {Array.from(new Set([1, page, totalPages])).sort((a, b) => a - b).map((pageNumber, index, pages) => (
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
        </section>

        {selectedRegister && (
          <section className="rounded-xl border border-fuchsia-100 bg-white p-4 shadow-sm">
            <div className="flex flex-wrap items-start justify-between gap-3">
              <div>
                <h2 className="text-base font-semibold text-slate-900">{selectedRegister.name}</h2>
                <p className="mt-1 text-xs text-slate-500">کد صندوق: {selectedRegister.code}</p>
              </div>
              <div className="flex flex-wrap gap-2">
                {!selectedRegister.activeSessionId ? (
                  <Button size="sm" onClick={() => openDialog("open")} disabled={!selectedRegister.isActive} className="bg-fuchsia-700 text-white hover:bg-fuchsia-800">
                    افتتاح صندوق
                  </Button>
                ) : (
                  <>
                    <Button size="sm" variant="outline" onClick={() => openDialog("cash-in")}><ArrowDownToLine className="size-4" />واریز</Button>
                    <Button size="sm" variant="outline" onClick={() => openDialog("cash-out")}><ArrowUpFromLine className="size-4" />برداشت</Button>
                    <Button size="sm" variant="outline" onClick={() => openDialog("close")}>بستن جلسه</Button>
                  </>
                )}
              </div>
            </div>

            {detailsError && <p role="alert" className="mt-3 text-sm text-destructive">{detailsError}</p>}
            {detailsLoading ? (
              <div className="flex items-center gap-2 py-8 text-sm text-slate-500"><LoaderCircle className="size-4 animate-spin" />در حال دریافت اطلاعات صندوق...</div>
            ) : (
              <>
                <div className="mt-4 grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
                  <div className="rounded-lg border border-slate-100 p-3">
                    <p className="text-xs text-slate-500">وضعیت جلسه</p>
                    <p className="mt-1 text-sm font-medium">{session ? "باز" : "جلسهٔ بازی وجود ندارد"}</p>
                    {session && selectedRegister.isActive && (
                      <p className="mt-1 text-xs text-slate-500">برای غیرفعال‌سازی صندوق، ابتدا جلسه را ببندید.</p>
                    )}
                  </div>
                  {session && (
                    <>
                      <div className="rounded-lg border border-slate-100 p-3">
                        <p className="text-xs text-slate-500">زمان افتتاح</p>
                        <p className="mt-1 text-sm font-medium">{dateLabel(session.openedAt)}</p>
                      </div>
                      <div className="rounded-lg border border-slate-100 p-3">
                        <p className="text-xs text-slate-500">موجودی آغازین</p>
                        <p className="mt-1 text-sm font-medium">{amountLabel(session.openingBalance)}</p>
                      </div>
                      <div className="rounded-lg border border-fuchsia-100 bg-fuchsia-50 p-3">
                        <p className="text-xs text-fuchsia-800">موجودی مورد انتظار</p>
                        <p className="mt-1 flex items-center gap-1 text-sm font-semibold text-fuchsia-900">
                          <CircleDollarSign className="size-4" />
                          {balance ? amountLabel(balance.expectedBalance) : "—"}
                        </p>
                      </div>
                    </>
                  )}
                </div>
                <div className="mt-5">
                  <h3 className="mb-2 text-sm font-semibold text-slate-800">گردش‌های اخیر {session ? "جلسه" : "صندوق"}</h3>
                  <p className="mb-2 text-xs text-slate-500">حداکثر ۵۰ گردش اخیر نمایش داده می‌شود.</p>
                  <div className="overflow-x-auto rounded-lg border border-slate-100">
                    <Table>
                      <TableHeader>
                        <TableRow className="bg-muted/60 hover:bg-muted/60">
                          <TableHead className="h-9 text-start">نوع گردش</TableHead>
                          <TableHead className="h-9 text-start">مبلغ</TableHead>
                          <TableHead className="h-9 text-start">زمان</TableHead>
                          <TableHead className="h-9 text-start">شرح</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {transactions.length === 0 ? (
                          <TableRow><TableCell colSpan={4} className="py-6 text-center text-sm text-slate-500">گردشی برای نمایش وجود ندارد.</TableCell></TableRow>
                        ) : transactions.map((transaction) => (
                          <TableRow key={transaction.id}>
                            <TableCell className="text-sm">{transactionLabel(transaction.type)}</TableCell>
                            <TableCell className="text-sm">{amountLabel(transaction.amount)}</TableCell>
                            <TableCell className="text-sm">{dateLabel(transaction.occurredAt)}</TableCell>
                            <TableCell className="max-w-64 truncate text-sm">{transaction.description || "—"}</TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </div>
                </div>
              </>
            )}
          </section>
        )}
      </main>

      <Dialog open={dialogAction !== null} onOpenChange={(open) => { if (!open && !submitting) setDialogAction(null); }}>
        <DialogContent dir="rtl" className="max-h-[90vh] overflow-y-auto sm:max-w-lg">
          <DialogHeader>
            <DialogTitle>{dialogAction ? dialogTitle[dialogAction] : ""}</DialogTitle>
          </DialogHeader>
          <form onSubmit={(event) => void submitAction(event)} className="space-y-4">
            {(dialogAction === "create" || dialogAction === "edit") && (
              <>
                <div className="space-y-2">
                  <Label htmlFor="cash-register-name">نام صندوق</Label>
                  <Input id="cash-register-name" value={formValues.name} maxLength={150} onChange={(event) => setFormValues((current) => ({ ...current, name: event.target.value }))} />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="cash-register-code">کد صندوق</Label>
                  <Input id="cash-register-code" value={formValues.code} maxLength={50} dir="ltr" onChange={(event) => setFormValues((current) => ({ ...current, code: event.target.value }))} />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="cash-register-description">توضیحات (اختیاری)</Label>
                  <Input id="cash-register-description" value={formValues.description} maxLength={1000} onChange={(event) => setFormValues((current) => ({ ...current, description: event.target.value }))} />
                </div>
              </>
            )}
            {(dialogAction === "open" || dialogAction === "close") && (
              <div className="space-y-2">
                <Label htmlFor="cash-register-amount">
                  {dialogAction === "open" ? "موجودی اولیه" : "موجودی شمارش‌شده هنگام بستن"}
                </Label>
                <Input id="cash-register-amount" type="number" min="0" step="0.01" value={amount} onChange={(event) => setAmount(event.target.value)} />
                {dialogAction === "close" && balance && (
                  <p className="text-xs text-slate-500">موجودی مورد انتظار: {amountLabel(balance.expectedBalance)}</p>
                )}
              </div>
            )}
            {(dialogAction === "cash-in" || dialogAction === "cash-out") && (
              <>
                <div className="space-y-2">
                  <Label htmlFor="cash-register-amount">مبلغ</Label>
                  <Input id="cash-register-amount" type="number" min="0.01" step="0.01" value={amount} onChange={(event) => setAmount(event.target.value)} />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="cash-register-movement-description">شرح گردش</Label>
                  <Input id="cash-register-movement-description" value={description} maxLength={1000} onChange={(event) => setDescription(event.target.value)} />
                </div>
              </>
            )}
            {formError && <p role="alert" className="text-sm text-destructive">{formError}</p>}
            <div className="flex justify-end gap-2">
              <Button type="button" variant="outline" onClick={() => setDialogAction(null)} disabled={submitting}>انصراف</Button>
              <Button type="submit" disabled={submitting} className="bg-fuchsia-700 text-white hover:bg-fuchsia-800">
                {submitting && <LoaderCircle className="size-4 animate-spin" />}
                {submitting ? "در حال ثبت..." : "ثبت"}
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
}
