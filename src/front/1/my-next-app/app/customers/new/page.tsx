"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";

import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/components/ui/breadcrumb";
import { CustomerFormDialog } from "@/features/customers/list/customer-form-dialog";

export default function NewCustomerRoute() {
  const router = useRouter();

  return (
    <div className="min-h-[calc(100vh-4rem)] bg-white">
      <section className="h-[182px] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]">
        <div className="mx-auto w-[92%] pt-2 sm:w-[84%] sm:max-w-[1200px]">
          <Breadcrumb>
            <BreadcrumbList className="text-xs text-white/70">
              <BreadcrumbItem><BreadcrumbLink href="/customers">مدیریت مشتریان</BreadcrumbLink></BreadcrumbItem>
              <BreadcrumbSeparator />
              <BreadcrumbItem><BreadcrumbPage className="text-white">ثبت مشتری</BreadcrumbPage></BreadcrumbItem>
            </BreadcrumbList>
          </Breadcrumb>
          <Link href="/customers" className="mt-4 inline-flex text-sm text-white underline underline-offset-4">
            بازگشت به فهرست مشتریان
          </Link>
        </div>
      </section>
      <main className="mx-auto -mt-[114px] w-[92%] sm:w-[84%] sm:max-w-[1200px]">
        <div className="min-h-[300px] rounded-xl border border-fuchsia-100 bg-white p-6 shadow-sm">
          <h1 className="text-lg font-semibold text-slate-900">ثبت مشتری جدید</h1>
          <p className="mt-2 text-sm text-slate-500">اطلاعات مشتری را تکمیل و ذخیره کنید.</p>
        </div>
      </main>
      <CustomerFormDialog
        open
        customer={null}
        onOpenChange={(open) => {
          if (!open) router.replace("/customers");
        }}
        onSaved={() => router.replace("/customers")}
      />
    </div>
  );
}
