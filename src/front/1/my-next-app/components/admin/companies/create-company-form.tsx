"use client";

import { useActionState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { createCompanyAction } from "@/actions/companies/company.create.actions";

const initialState = {
  success: false,
  errors: {},
};

export function CreateCompanyForm() {
  const [state, formAction, isPending] = useActionState(
    createCompanyAction,
    initialState,
  );

  return (
    <Card>
      <CardHeader>
        <CardTitle>اطلاعات شرکت</CardTitle>
      </CardHeader>

      <CardContent>
        <form action={formAction} className="space-y-6">
          <div className="space-y-2">
            <Label htmlFor="name">نام شرکت</Label>

            <Input
              id="name"
              name="name"
              placeholder="مثلاً لپ‌تاپ Lenovo"
              defaultValue={state.values?.name ?? ""}
            />

            {state.errors?.name && (
              <p className="text-sm text-destructive">{state.errors.name[0]}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="name">ایمیل </Label>

            <Input
              id="email"
              name="email"
              placeholder="مثلاً  abolfaz@gmail.com"
              defaultValue={state.values?.email ?? ""}
            />

            {state.errors?.email && (
              <p className="text-sm text-destructive">{state.errors.email[0]}</p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="name">تلفن </Label>

            <Input
              id="phone"
              name="phone"
              placeholder="مثلاً  093899222"
              defaultValue={state.values?.phone ?? ""}
            />

            {state.errors?.phone && (
              <p className="text-sm text-destructive">{state.errors.phone[0]}</p>
            )}
          </div>

          <Button type="submit" disabled={isPending} className="w-full">
            {isPending ? "در حال ثبت..." : "ثبت شرکت"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
