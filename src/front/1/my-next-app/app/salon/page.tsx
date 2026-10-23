"use client";

import { useMemo, useState } from "react";
import { MoreHorizontal, Plus, Search } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from "@/components/ui/breadcrumb";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
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

type Customer = {
  id: number;
  name: string;
  phone: string;
  nationalId: string;
  nextAppointment: string;
  vip: boolean;
};

// داده‌ی نمونه — بعداً با fetch از API جایگزین کن
const DATA: Customer[] = Array.from({ length: 53 }, (_, i) => ({
  id: i + 1,
  name: i === 0 ? "سارا زمانی" : "ناخن کار",
  phone: "۰۹۱۲۳۴۵۶۷۸۹",
  nationalId: "۰۰۱۲۳۴۵۶۷۸",
  nextAppointment: "۱۴۰۴/۰۹/۱۲",
  vip: i === 0,
}));

const PAGE_SIZE = 10;

export default function CustomersPage() {
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);

  const filtered = useMemo(
    () => DATA.filter((c) => c.name.includes(query.trim())),
    [query],
  );
  const totalPages = Math.max(1, Math.ceil(filtered.length / PAGE_SIZE));
  const rows = filtered.slice((page - 1) * PAGE_SIZE, page * PAGE_SIZE);

  return (
    <div className="min-h-screen bg-muted/30">
      {/* هدر تیره بالای صفحه */}
      <div className="bg-black px-6 pb-24 pt-4">
        <Breadcrumb>
          <BreadcrumbList className="text-white/70">
            <BreadcrumbItem>
              <BreadcrumbLink href="/customers">مدیریت مشتریان</BreadcrumbLink>
            </BreadcrumbItem>
            <BreadcrumbSeparator />
            <BreadcrumbItem>
              <BreadcrumbPage className="text-white">
                ثبت مشتریان
              </BreadcrumbPage>
            </BreadcrumbItem>
          </BreadcrumbList>
        </Breadcrumb>
      </div>

      {/* کارت اصلی که روی هدر می‌افتد */}
      <main className="-mt-16 px-6">
        <div className="rounded-xl bg-background p-4 shadow-sm">
          <div className="mb-4 flex items-center justify-between gap-4">
            <Button className="bg-fuchsia-700 text-white hover:bg-fuchsia-800">
              <Plus className="size-4" />
              مشتری جدید
            </Button>

            <div className="relative w-full max-w-sm">
              <Search className="absolute start-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                value={query}
                onChange={(e) => {
                  setQuery(e.target.value);
                  setPage(1);
                }}
                placeholder="جستجو بر اساس نام مشتری و عنوان شغلی..."
                className="ps-9"
              />
            </div>
          </div>

          <div className="rounded-lg border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="text-start">
                    نام و نام خانوادگی
                  </TableHead>
                  <TableHead className="text-start">تلفن همراه</TableHead>
                  <TableHead className="text-start">کدملی</TableHead>
                  <TableHead className="text-start">نوبت بعدی</TableHead>
                  <TableHead className="text-start">وضعیت</TableHead>
                  <TableHead className="w-12" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {rows.map((c) => (
                  <TableRow key={c.id}>
                    <TableCell>{c.name}</TableCell>
                    <TableCell>{c.phone}</TableCell>
                    <TableCell>{c.nationalId}</TableCell>
                    <TableCell>{c.nextAppointment}</TableCell>
                    <TableCell>
                      {c.vip && (
                        <Badge className="bg-fuchsia-700 text-white hover:bg-fuchsia-700">
                          VIP
                        </Badge>
                      )}
                    </TableCell>
                    <TableCell>
                      <DropdownMenu>
                        <DropdownMenuTrigger
                          render={
                            <Button
                              variant="ghost"
                              size="icon"
                              className="size-8"
                            />
                          }
                        >
                          <MoreHorizontal className="size-4" />
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="start">
                          <DropdownMenuItem>مشاهده</DropdownMenuItem>
                          <DropdownMenuItem>ویرایش</DropdownMenuItem>
                          <DropdownMenuItem className="text-destructive">
                            حذف
                          </DropdownMenuItem>
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>

          {/* صفحه‌بندی */}
          <Pagination className="mt-4">
            <PaginationContent>
              <PaginationItem>
                <PaginationPrevious
                  href="#"
                  text="قبلی"
                  onClick={(e) => {
                    e.preventDefault();
                    setPage((p) => Math.max(1, p - 1));
                  }}
                />
              </PaginationItem>

              {Array.from({ length: totalPages }, (_, i) => i + 1)
                .filter(
                  (p) => p === 1 || p === totalPages || Math.abs(p - page) <= 1,
                )
                .map((p, idx, arr) => (
                  <PaginationItem key={p}>
                    {idx > 0 && p - arr[idx - 1] > 1 && <PaginationEllipsis />}
                    <PaginationLink
                      href="#"
                      isActive={p === page}
                      onClick={(e) => {
                        e.preventDefault();
                        setPage(p);
                      }}
                    >
                      {p.toLocaleString("fa-IR")}
                    </PaginationLink>
                  </PaginationItem>
                ))}

              <PaginationItem>
                <PaginationNext
                  href="#"
                  text="بعدی"
                  onClick={(e) => {
                    e.preventDefault();
                    setPage((p) => Math.min(totalPages, p + 1));
                  }}
                />
              </PaginationItem>
            </PaginationContent>
          </Pagination>
        </div>
      </main>
    </div>
  );
}
