import { NextRequest, NextResponse } from "next/server";

interface TokenClaims {
  exp?: unknown;
  AccessLevel?: unknown;
}

function readTokenClaims(token: string | undefined): TokenClaims | null {
  if (!token) {
    return null;
  }

  try {
    const parts = token.split(".");
    if (parts.length !== 3 || !parts[1]) {
      return null;
    }

    const payload = parts[1].replace(/-/g, "+").replace(/_/g, "/");
    return JSON.parse(atob(payload)) as TokenClaims;
  } catch {
    return null;
  }
}

function hasAdminRole(claims: TokenClaims): boolean {
  const roleClaims = claims.AccessLevel;
  return Array.isArray(roleClaims)
    ? roleClaims.includes("Admin")
    : roleClaims === "Admin";
}

export function proxy(request: NextRequest) {
  const token = request.cookies.get("access_token")?.value;
  const claims = readTokenClaims(token);
  const authenticated =
    typeof claims?.exp === "number" &&
    Number.isFinite(claims.exp) &&
    claims.exp * 1000 > Date.now();
  const isSalonPage =
    request.nextUrl.pathname === "/salon" ||
    request.nextUrl.pathname.startsWith("/salon/");

  if (request.nextUrl.pathname === "/login") {
    if (authenticated) {
      return NextResponse.redirect(new URL("/personnel", request.url));
    }

    const response = NextResponse.next();
    if (token) {
      response.cookies.delete("access_token");
    }
    return response;
  }

  if (!authenticated) {
    const response = NextResponse.redirect(new URL("/login", request.url));
    if (token) {
      response.cookies.delete("access_token");
    }
    return response;
  }

  if (isSalonPage && !hasAdminRole(claims)) {
    return NextResponse.redirect(new URL("/403", request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: [
    "/((?!_next/static|_next/image|favicon.ico|.*\\.(?:svg|png|jpg|jpeg|gif|webp|ico|css|js|woff2?)$).*)",
  ],
};
