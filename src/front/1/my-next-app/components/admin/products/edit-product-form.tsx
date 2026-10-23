"use client";

import { useActionState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import { ProductDto } from "@/app/DTOs/Products/product.dto";

import {
  updateProductAction,
  UpdateProductActionState,
} from "@/actions/products/product.update.actions";

type Props = {
  product: ProductDto;
};

export function ProductEditForm({ product }: Props) {
  const initialState: UpdateProductActionState = {
    success: false,
    errors: {},
    values: {
      name: product.name,
      price: String(product.price),
    },
  };

  const [state, formAction, isPending] = useActionState(
    updateProductAction,
    initialState,
  );

  return (
    <Card>
      <CardHeader>
        <CardTitle>اطلاعات محصول</CardTitle>
      </CardHeader>

      <CardContent>
        <form action={formAction} className="space-y-6">
          <input type="hidden" name="id" value={product.id} />

          <div className="space-y-2">
            <Label htmlFor="name">نام محصول</Label>

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
            <Label htmlFor="price">قیمت</Label>

            <Input
              id="price"
              name="price"
              type="number"
              placeholder="مثلاً 50000000"
              defaultValue={state.values?.price ?? ""}
            />

            {state.errors?.price && (
              <p className="text-sm text-destructive">
                {state.errors.price[0]}
              </p>
            )}
          </div>

          <Button type="submit" disabled={isPending} className="w-full">
            {isPending ? "در حال ذخیره..." : "ذخیره تغییرات"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
