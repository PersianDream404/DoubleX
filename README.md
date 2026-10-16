
# TowX Project 🛍️

این پروژه یک سیستم مدیریت فروشگاه (TowX) است که با استفاده از معماری **Modular Monolith** و اصول **Clean Architecture** طراحی شده است تا مقیاس‌پذیری و نگهداری کد در درازمدت تضمین شود.

## 🏗️ معماری سیستم (Architecture)

پروژه بر پایه تفکیک دغغه‌ها (Separation of Concerns) بنا شده و از ساختار زیر پیروی می‌کند:

### ۱. Building Blocks (زیرساخت‌های مشترک)
این بخش شامل ابزارهایی است که توسط تمامی ماژول‌ها استفاده می‌شوند تا از تکرار کد جلوگیری شود:
- **Framework**: پیاده‌سازی الگوی CQRS، مدیریت Busها برای Command و Query، و Decoratorهایی برای Logging و Validation.
- **Infrastructure**: مدیریت دیتابیس (DbContext)، پیاده‌سازی Repositoryهای عمومی و سرویس‌های زیرساختی مانند PaymentService.
- **SharedKernel**: شامل موجودات پایه (Base Entity, Value Object)، اینترفیس‌های مشترک، ثابت‌ها (Constants) و مدل‌های پاسخ API.
- **Shared.Contract**: DTOهای مشترک برای تبادل داده بین لایه‌ها و ماژول‌ها.

### ۲. Modular Structure (ساختار ماژولار)
هر قابلیت بیزنسی در یک ماژول مجزا قرار دارد که هر کدام دارای لایه‌های خود هستند:
- **Domain**: شامل موجودات (Entities)، قوانین بیزنسی و Aggregate Rootها.
- **Application**: شامل Use Caseها، Commandها، Queryها و منطق برنامه.
- **Persistence**: پیاده‌سازی ذخیره‌سازی داده‌ها و مدیریت دیتابیس مخصوص هر ماژول.
- **Presentation**: لایه API و کنترلرها برای تعامل با کاربر.

**ماژول‌های شناسایی شده:**
- 🆔 **Identity**: مدیریت کاربران و دسترسی‌ها.
- 📦 **Product**: مدیریت محصولات.
- 🛒 **Order**: مدیریت سفارشات.
- 💳 **Payment**: مدیریت پرداخت‌ها.
- 📁 **FileStore**: مدیریت فایل‌ها و تصاویر.

## 🛠️ تکنولوژی‌ها و الگوهای مورد استفاده

- **Runtime**: .NET 10.0
- **Patterns**: 
  - **CQRS**: تفکیک عملیات خواندن (Query) و نوشتن (Command).
  - **Unit of Work**: برای مدیریت تراکنش‌های دیتابیس.
  - **Repository Pattern**: برای انتزاع لایه داده‌ها.
  - **Decorator Pattern**: برای افزودن قابلیت‌هایی مثل Logging بدون تغییر در کد اصلی.
- **Libraries**:
  - **MediatR**: برای پیاده‌سازی In-process messaging.
  - **FluentValidation**: برای اعتبارسنجی داده‌های ورودی.
  - **Entity Framework Core**: به عنوان ORM.
  - **Mapster**: برای تبدیل DTOها به Entityها.

## 📂 ساختار پوشه‌ها

```text
src/
├── BuildingBlocks/
│   ├── Framework/        # ابزارهای پایه (Bus, Decorators)
│   ├── Infrastructure/  # پیاده‌سازی‌های زیرساختی (DB, Services)
│   ├── SharedKernel/     # هسته مشترک (Base Classes, Interfaces)
│   └── Shared.Contract/  # قراردادهای مشترک (DTOs)
├── Modules/
│   ├── Identity/        # ماژول مدیریت کاربران
│   ├── Order/            # ماژول سفارشات
│   ├── Product/          # ماژول محصولات
│   └── ...               # سایر ماژول‌ها
└── Web/                  # نقطه ورود برنامه و تنظیمات Host
```

## 🚀 شروع به کار

۱. کلون کردن پروژه.
۲. تنظیم رشته اتصال به دیتابیس در `appsettings.json`.
۳. اجرای Migrationها برای ایجاد جداول دیتابیس.
۴. اجرای پروژه از طریق Visual Studio یا دستور `dotnet run`.
