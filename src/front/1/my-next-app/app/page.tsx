import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

export default function Home() {
  return (
    <main className="flex min-h-screen items-center justify-center">
      <Card className="w-[350px]">
        <CardHeader>
          <CardTitle>Next Shop</CardTitle>
        </CardHeader>

        <CardContent>
          <Button className="w-full">ورود به پنل</Button>
        </CardContent>
      </Card>
    </main>
  );
}
