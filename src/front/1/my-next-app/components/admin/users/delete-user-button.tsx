"use client";

import { useTransition } from "react";
import { Trash2 } from "lucide-react";
import { deleteUserAction } from "@/actions/users/user.delete.actions";

type Props = {
  userId: number;
};

export function DeleteUserButton({ userId }: Props) {
  const [isPending, startTransition] = useTransition();

  function handleDelete() {
    const confirmed = window.confirm("آیا از حذف این کاربر مطمئن هستید؟");

    if (!confirmed) return;

    startTransition(async () => {
      await deleteUserAction(userId);
    });
  }

  return (
    <button
      type="button"
      onClick={handleDelete}
      disabled={isPending}
      className="flex w-full items-center gap-2 px-2 py-1.5 text-sm text-destructive"
    >
      <Trash2 className="size-4" />
      {isPending ? "در حال حذف..." : "حذف"}
    </button>
  );
}
