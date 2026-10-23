import Link from "next/link";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import { Badge } from "@/components/ui/badge";
import { CompanyDto } from "@/app/DTOs/companies/company.dto";

type CompanyInfoProps = {
  company: CompanyDto;
};

export function CompanyInfo({ company }: CompanyInfoProps) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>اطلاعات شرکت</CardTitle>
      </CardHeader>

      <CardContent>
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          <div>
            <p className="text-sm text-muted-foreground">نام شرکت</p>

            <Link
              href={`/admin/companies/${company.id}`}
              className="font-medium hover:underline"
            >
              {company.name}
            </Link>
          </div>

          <div>
            <p className="text-sm text-muted-foreground">تلفن</p>

            <p className="font-medium">{company.phone || "-"}</p>
          </div>

          <div>
            <p className="text-sm text-muted-foreground">ایمیل</p>

            <p className="font-medium">{company.email || "-"}</p>
          </div>

          <div>
            <p className="text-sm text-muted-foreground">وضعیت</p>

            <Badge variant={company.status ? "default" : "secondary"}>
              {company.status ? "فعال" : "غیرفعال"}
            </Badge>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
