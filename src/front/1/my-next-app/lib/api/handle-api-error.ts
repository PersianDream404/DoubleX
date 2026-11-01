import { ApiError } from "./api-client";

export function handleApiError(error: unknown): string {
  if (!(error instanceof ApiError)) {
    return error instanceof Error ? error.message : "خطای غیرمنتظره‌ای رخ داد.";
  }

  const message = error.errors?.filter(Boolean).join("، ") || error.message;

  switch (error.statusCode) {
    case 400:
    case 409:
    case 422:
      return message || "اطلاعات ارسالی معتبر نیست.";

    case 401:
      return "نشست شما منقضی شده است. لطفاً دوباره وارد شوید.";

    case 403:
      return "شما اجازه دسترسی به این بخش را ندارید.";

    case 404:
      return "مورد درخواستی پیدا نشد.";

    case 408:
      return "زمان انتظار برای انجام درخواست به پایان رسید.";

    case 429:
      return "تعداد درخواست‌های شما بیش از حد مجاز است.";

    case 500:
    case 501:
    case 502:
    case 503:
    case 504:
      return "خطایی در سرور رخ داده است. لطفاً بعداً تلاش کنید.";

    case 0:
      return "سرور در دسترس نیست. اتصال اینترنت را بررسی کنید.";

    default:
      return message || "خطای غیرمنتظره‌ای رخ داد.";
  }
}
