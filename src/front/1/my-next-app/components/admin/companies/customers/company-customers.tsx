import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

import { CompanyCustomerDto } from "@/app/DTOs/customers/customer.dto";

import { AddCustomerModal } from "./Add/company-add-customer-modal";
import { MoreHorizontal, Pencil } from "lucide-react";
import Link from "next/link";
import { DeleteCompanyButton } from "../delete-company-button";
import { DeleteCustomerButton } from "./delete-company-customer-button";
import { EditCustomerModal } from "./Update/company-Edit-customer-modal";
import { CustomerActions } from "./company-customer-actions";

type CompanyCustomersProps = {
  companyId: number;
  customers: CompanyCustomerDto[];
};

export function CompanyCustomers({
  companyId,
  customers,
}: CompanyCustomersProps) {
  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <div>
          <CardTitle>مشتریان</CardTitle>

          <p className="mt-1 text-sm text-muted-foreground">
            {customers.length} مشتری ثبت شده است.
          </p>
        </div>

        <AddCustomerModal companyId={companyId} />
      </CardHeader>

      <CardContent>
        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="text-right">#</TableHead>

                <TableHead className="text-right">نام</TableHead>

                <TableHead className="text-right">تلفن</TableHead>

                <TableHead className="text-right">ایمیل</TableHead>
                <TableHead className="w-[80px]" />
              </TableRow>
            </TableHeader>

            <TableBody>
              {customers.map((customer, index) => (
                <TableRow key={customer.id}>
                  <TableCell>{index + 1}</TableCell>

                  <TableCell className="font-medium">{customer.name}</TableCell>

                  <TableCell>{customer.phone || "-"}</TableCell>

                  <TableCell>{customer.email || "-"}</TableCell>

                  {/* Actions */}
                  <TableCell>
                    <CustomerActions
                      customer={customer}
                      companyId={companyId}
                    />
                  </TableCell>
                </TableRow>
              ))}

              {customers.length === 0 && (
                <TableRow>
                  <TableCell colSpan={4} className="h-24 text-center">
                    هنوز مشتری‌ای برای این شرکت ثبت نشده است.
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
