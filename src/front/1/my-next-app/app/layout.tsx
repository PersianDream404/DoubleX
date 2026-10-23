import type { Metadata } from "next";
import { Vazirmatn } from "next/font/google";

import "./globals.css";
import { Navbar } from "@/components/common/nav";

const vazir = Vazirmatn({ subsets: ["arabic", "latin"] });

export const metadata: Metadata = {
  title: "Beauty | مدیریت سالن",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="fa" dir="rtl">
      <body className={`${vazir.className} antialiased`}>
        <Navbar />
        {children}
      </body>
    </html>
  );
}
