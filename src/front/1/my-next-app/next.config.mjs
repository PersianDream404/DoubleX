
/** @type {import('next').NextConfig} */
const apiBaseUrl = (
  process.env.API_SERVER_BASE_URL ||
  process.env.NEXT_PUBLIC_API_BASE_URL ||
  "http://xx2.runasp.net"
).replace(/\/+$/, "");

const nextConfig = {
  output: "standalone",
  async rewrites() {
    return [
      {
        source: "/api/backend/:path*",
        destination: `${apiBaseUrl}/:path*`,
      },
    ];
  },
};

export default nextConfig;
