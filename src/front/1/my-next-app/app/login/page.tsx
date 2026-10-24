import { LoginForm } from "@/components/auth/login-form";

export default function LoginPage() {
  return (
    <main className="relative isolate flex min-h-svh items-center justify-center overflow-hidden bg-slate-50 px-4 py-12 sm:px-6">
      <div
        aria-hidden="true"
        className="absolute inset-x-0 top-0 h-[48%] bg-[linear-gradient(110deg,#10000f_0%,#210020_55%,#10000f_100%)]"
      />
      <div
        aria-hidden="true"
        className="absolute -start-24 top-20 size-64 rounded-full bg-fuchsia-800/20 blur-3xl"
      />
      <div
        aria-hidden="true"
        className="absolute -end-28 bottom-8 size-72 rounded-full bg-fuchsia-200/60 blur-3xl"
      />
      <div className="relative w-full max-w-[440px]">
        <LoginForm />
      </div>
    </main>
  );
}
