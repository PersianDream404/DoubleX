"use client";

import { useActionState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import {
  updateUserAction,
  type UpdateUserActionState,
} from "@/actions/users/user.update.action";
import { UserListDto } from "@/app/DTOs/users/user-list.dto";

type UserEditFormProps = {
  user: UserListDto;
};

const initialState: UpdateUserActionState = {
  success: false,
};

export function UserEditForm({ user }: UserEditFormProps) {
  const [state, formAction, isPending] = useActionState(
    updateUserAction,
    initialState,
  );

  const currentRole = user.roles[0] ?? "Employer";

  return (
    <Card>
      <CardHeader>
        <CardTitle>اطلاعات کاربر</CardTitle>
      </CardHeader>

      <CardContent>
        <form action={formAction} className="space-y-6">
          <input type="hidden" name="id" value={user.id} />

          <div className="space-y-2">
            <Label htmlFor="username">نام کاربری</Label>

            <Input id="username" name="username" defaultValue={user.username} />
          </div>

          <div className="space-y-2">
            <Label htmlFor="password">رمز عبور جدید</Label>

            <Input
              id="password"
              name="password"
              type="password"
              placeholder="در صورت عدم تغییر خالی بگذارید"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="role">نقش</Label>

            <select
              id="role"
              name="role"
              defaultValue={currentRole}
              className="w-full rounded-md border bg-background px-3 py-2 text-sm"
            >
              <option value="Admin">مدیر</option>

              <option value="Employer">کارفرما</option>
            </select>
          </div>

          {state.error && (
            <p className="text-sm text-destructive">{state.error}</p>
          )}

          <Button type="submit" disabled={isPending}>
            {isPending ? "در حال ذخیره..." : "ذخیره تغییرات"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
