"use client";

import { useTransition } from "react";
import { LogOut } from "lucide-react";

import { Button } from "@/components/ui/button";
import { logoutAction } from "@/actions/auths/logout.action";

export function LogoutButton() {
  const [isPending, startTransition] = useTransition();

  function handleLogout() {
    window.localStorage.removeItem("access_token");
    window.localStorage.removeItem("salon_id");
    window.dispatchEvent(new Event("auth-token-changed"));
    startTransition(async () => {
      await logoutAction();
    });
  }

  return (
    <Button
      type="button"
      variant="ghost"
      onClick={handleLogout}
      disabled={isPending}
      className="h-9 gap-2 rounded-lg px-3 text-white hover:bg-white/10 hover:text-white focus-visible:ring-fuchsia-400"
    >
      <LogOut className="size-4" aria-hidden="true" />
      {isPending ? "در حال خروج..." : "خروج"}
    </Button>
  );
}
