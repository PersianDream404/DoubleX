"use client";

import { useActionState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { createUserAction } from "@/actions/users/user.create.action";

export type CreateUserActionState = {
  success: boolean;
  error?: string;
};

export function CreateUserForm() {
  const initialState: CreateUserActionState = {
    success: false,
  };
  const [state, formAction, isPending] = useActionState(
    createUserAction,
    initialState,
  );

  return (
    <Card>
      <CardHeader>
        <CardTitle>اطلاعات کاربر</CardTitle>
      </CardHeader>

      <CardContent>
        <form action={formAction} className="space-y-6">
          <div className="space-y-2">
            <Label htmlFor="username">نام کاربری</Label>

            <Input id="username" name="username" placeholder="نام کاربری" />
          </div>

          <div className="space-y-2">
            <Label htmlFor="password">رمز عبور</Label>

            <Input
              id="password"
              name="password"
              type="password"
              placeholder="رمز عبور"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="role">نقش</Label>

            <select
              id="role"
              name="role"
              defaultValue="Admin"
              className="w-full rounded-md border bg-background px-3 py-2 text-sm"
            >
              <option value="Admin">مدیر</option>
              {/* 
              <option value="Employer">کارفرما</option>

              <option value="JobSeeker">کارجو</option> */}
            </select>
          </div>

          {state.error && (
            <p className="text-sm text-destructive">{state.error}</p>
          )}

          <Button type="submit" disabled={isPending}>
            {isPending ? "در حال ایجاد..." : "ایجاد کاربر"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
