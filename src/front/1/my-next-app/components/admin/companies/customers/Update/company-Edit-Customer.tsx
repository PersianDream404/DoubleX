"use client";

import { useActionState } from "react";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import {
  createCustomerAction,
  CustomerActionState,
} from "@/actions/companies/customers/company.customer.create.actions";
import { UpdateCustomerDto } from "@/app/DTOs/customers/customer.dto";
import { updateCustomerAction } from "@/actions/companies/customers/company.customer.update.actions";

type CustomerFormProps = {
  companyId: number;
  customerId: number;
  customer: UpdateCustomerDto;
};

export function EditCustomerForm({
  companyId,
  customerId,
  customer,
}: CustomerFormProps) {
  const initialState: CustomerActionState = {
    success: false,
    values: {
      name: customer.name,
      phone: customer.phone,
      email: customer.email,
    },
  };
  const [state, formAction, pending] = useActionState(
    updateCustomerAction,
    initialState,
  );

  return (
    <form action={formAction} className="space-y-4">
      <input type="hidden" name="companyId" value={companyId} />
      <input type="hidden" name="customerId" value={customerId} />
      <div className="space-y-2">
        <Label htmlFor="name">نام مشتری</Label>

        <Input id="name" name="name" defaultValue={state.values?.name} />

        {state.errors?.name && (
          <p className="text-sm text-destructive">{state.errors.name[0]}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="phone">تلفن</Label>

        <Input
          id="phone"
          name="phone"
          defaultValue={state.values?.phone ?? ""}
        />

        {state.errors?.phone && (
          <p className="text-sm text-destructive">{state.errors.phone[0]}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="email">ایمیل</Label>

        <Input
          id="email"
          name="email"
          type="email"
          defaultValue={state.values?.email ?? ""}
        />

        {state.errors?.email && (
          <p className="text-sm text-destructive">{state.errors.email[0]}</p>
        )}
      </div>

      <Button type="submit" disabled={pending} className="w-full">
        {pending ? "در حال ثبت..." : "ثبت مشتری"}
      </Button>
    </form>
  );
}
