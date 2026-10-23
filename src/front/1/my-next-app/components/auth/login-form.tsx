"use client";

import { useActionState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { loginAction, LoginActionState } from "@/actions/auths/login.action";

const initialState: LoginActionState = {
  success: false,
};

export function LoginForm() {
  const [state, formAction, isPending] = useActionState(
    loginAction,
    initialState,
  );

  return (
    <Card>
      <CardHeader>
        <CardTitle>ورود به حساب کاربری</CardTitle>
      </CardHeader>

      <CardContent>
        <form action={formAction} className="space-y-6">
          <div className="space-y-2">
            <Label htmlFor="username">نام کاربری</Label>

            <Input
              id="username"
              name="username"
              placeholder="نام کاربری"
              autoComplete="username"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="password">رمز عبور</Label>

            <Input
              id="password"
              name="password"
              type="password"
              placeholder="رمز عبور"
              autoComplete="current-password"
            />
          </div>

          {state.error && (
            <p className="text-sm text-destructive">{state.error}</p>
          )}

          <Button type="submit" className="w-full" disabled={isPending}>
            {isPending ? "در حال ورود..." : "ورود"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
