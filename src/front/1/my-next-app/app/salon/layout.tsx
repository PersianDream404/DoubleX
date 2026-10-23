import Link from "next/link";

export default function AdminLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="fa" dir="rtl">
      <body>
        <header>
          <h2>Admin Panel</h2>

          <nav>
            <Link href="/">خانه</Link>
            {" | "}
            <Link href="/products">محصولات</Link>
            {" | "}
            <Link href="/users">کاربران</Link>
            {" | "}
            <Link href="/reports">گزارشات</Link>
          </nav>
        </header>

        <main>{children}</main>
      </body>
    </html>
  );
}
