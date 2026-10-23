import { CompanyPaymentDto } from "@/app/DTOs/payments/payment.dto";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { PaymentActions } from "./payment-company-actions";
import { AddPaymentModal } from "./Add/payment-add-company-modal";
import { CompanyOrderDto, OrderSelectDto } from "@/app/DTOs/orders/order.dto";

type CompanyPaymentsProps = {
  companyId: number;
  payments: CompanyPaymentDto[];
  orders: CompanyOrderDto[];
};

export function CompanyPayments({
  companyId,
  payments,
  orders,
}: CompanyPaymentsProps) {
  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <div>
          <CardTitle>پرداخت‌ها</CardTitle>

          <p className="mt-1 text-sm text-muted-foreground">
            {payments.length} پرداخت ثبت شده است.
          </p>
        </div>
        <AddPaymentModal companyId={companyId} orders={orders} />
        {/* بعداً اینجا AddPaymentModal قرار می‌گیرد */}
      </CardHeader>

      <CardContent>
        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="text-right">#</TableHead>

                <TableHead className="text-right">شماره پرداخت</TableHead>

                <TableHead className="text-right">شماره سفارش</TableHead>

                <TableHead className="text-right">نام مشتری</TableHead>

                <TableHead className="text-right">مبلغ</TableHead>

                <TableHead className="text-right">روش پرداخت</TableHead>

                <TableHead className="text-right">وضعیت</TableHead>

                <TableHead className="text-right">تاریخ ثبت</TableHead>

                <TableHead className="w-[80px]" />
              </TableRow>
            </TableHeader>

            <TableBody>
              {payments.map((payment, index) => (
                <TableRow key={payment.id}>
                  <TableCell>{index + 1}</TableCell>

                  <TableCell className="font-medium">#{payment.id}</TableCell>

                  <TableCell className="font-medium">
                    #{payment.orderId}
                  </TableCell>

                  <TableCell>{payment.customerName}</TableCell>

                  <TableCell>
                    {payment.amount.toLocaleString("fa-IR")} تومان
                  </TableCell>

                  <TableCell>{payment.method}</TableCell>

                  <TableCell>{payment.status}</TableCell>

                  <TableCell>
                    {new Intl.DateTimeFormat("fa-IR").format(
                      new Date(payment.createdAt),
                    )}
                  </TableCell>

                  <TableCell>
                    <PaymentActions
                      payment={payment}
                      companyId={companyId}
                      orders={orders}
                    />
                  </TableCell>
                </TableRow>
              ))}

              {payments.length === 0 && (
                <TableRow>
                  <TableCell colSpan={9} className="h-24 text-center">
                    هنوز پرداختی برای این شرکت ثبت نشده است.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      </CardContent>
    </Card>
  );
}
