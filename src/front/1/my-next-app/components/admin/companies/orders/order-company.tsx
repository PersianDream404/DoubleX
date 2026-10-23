import { CompanyOrderDto } from "@/app/DTOs/orders/order.dto";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { OrderActions } from "./order-customer-actions";
import { ProductSelectDto } from "@/app/DTOs/Products/product.dto";
import { AddOrderModal } from "./Add/order-add-customer-modal";
import { CompanyCustomerDto } from "@/app/DTOs/customers/customer.dto";

type CompanyOrdersProps = {
  companyId: number;
  orders: CompanyOrderDto[];
  products: ProductSelectDto[];
  customers: CompanyCustomerDto[];
};

export function CompanyOrders({
  companyId,
  orders,
  products,
  customers,
}: CompanyOrdersProps) {
  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <div>
          <CardTitle>سفارش‌ها</CardTitle>

          <p className="mt-1 text-sm text-muted-foreground">
            {orders.length} سفارش ثبت شده است.
          </p>
        </div>

        <AddOrderModal
          companyId={companyId}
          customers={customers}
          products={products}
        />
      </CardHeader>

      <CardContent>
        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="text-right">#</TableHead>

                <TableHead className="text-right">شماره سفارش</TableHead>
                <TableHead className="text-right">نام سفارش</TableHead>
                <TableHead className="text-right">نام مشتری</TableHead>

                <TableHead className="text-right">مبلغ</TableHead>

                <TableHead className="text-right">وضعیت</TableHead>

                <TableHead className="text-right">تاریخ ثبت</TableHead>

                <TableHead className="w-[80px]" />
              </TableRow>
            </TableHeader>

            <TableBody>
              {orders.map((order, index) => (
                <TableRow key={order.id}>
                  <TableCell>{index + 1}</TableCell>

                  <TableCell className="font-medium">#{order.id}</TableCell>
                  <TableCell className="font-medium">{order.name}</TableCell>
                  <TableCell className="font-medium">
                    {order.customerName}
                  </TableCell>

                  <TableCell>
                    {order.totalPrice?.toLocaleString("fa-IR")} تومان
                  </TableCell>

                  <TableCell>{order.status}</TableCell>

                  <TableCell>
                    {new Intl.DateTimeFormat("fa-IR").format(
                      new Date(order.createdAt),
                    )}
                  </TableCell>

                  <TableCell>
                    <OrderActions
                      order={order}
                      companyId={companyId}
                      orderId={order.id}
                    />
                  </TableCell>
                </TableRow>
              ))}

              {orders.length === 0 && (
                <TableRow>
                  <TableCell colSpan={6} className="h-24 text-center">
                    هنوز سفارشی برای این شرکت ثبت نشده است.
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
