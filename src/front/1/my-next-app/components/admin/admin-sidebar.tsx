import Link from "next/link";
import {
  LayoutDashboard,
  Package,
  Tags,
  ShoppingCart,
  Users,
  Settings,
} from "lucide-react";

import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@/components/ui/sidebar";

const items = [
  {
    title: "داشبورد",
    url: "/admin",
    icon: LayoutDashboard,
  },
  {
    title: "شرکت",
    url: "/admin/companies",
    icon: Package,
  },
  {
    title: "محصولات",
    url: "/admin/products",
    icon: Package,
  },
  {
    title: "دسته‌بندی‌ها",
    url: "/admin/categories",
    icon: Tags,
  },
  {
    title: "سفارشات",
    url: "/admin/orders",
    icon: ShoppingCart,
  },
  {
    title: "کاربران",
    url: "/admin/users",
    icon: Users,
  },
  {
    title: "گزارشات فاکتور",
    url: "/admin/reports/order",
    icon: Users,
  },
  {
    title: "تنظیمات",
    url: "/admin/settings",
    icon: Settings,
  },
];

export function AdminSidebar() {
  return (
    <Sidebar side="right">
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>My B2B App</SidebarGroupLabel>

          <SidebarGroupContent>
            <SidebarMenu>
              {items.map((item) => (
                <SidebarMenuItem key={item.url}>
                  <SidebarMenuButton>
                    <Link
                      href={item.url}
                      className="flex w-full items-center gap-2"
                    >
                      <item.icon className="size-4" />
                      <span>{item.title}</span>
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
    </Sidebar>
  );
}
