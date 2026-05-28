const API_BASE = import.meta.env.VITE_API_BASE_URL?.replace(/\/$/, "") ?? "";

async function requestJson(path, body) {
  const res = await fetch(`${API_BASE}${path}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    credentials: "include"
  });

  // try parse JSON, handle non-JSON gracefully
  const text = await res.text();
  try {
    return JSON.parse(text);
  } catch {
    return { success: false, message: "Invalid JSON response from server", raw: text };
  }
}

export async function register(payload) {
  // payload should contain: hoTen, email, soDienThoai, matKhau, vaiTro
  return requestJson("/api/auth/register", payload);
}

export async function verifyEmail(payload) {
  // payload should contain: email, otp
  return requestJson("/api/auth/verify-email", payload);
}

export async function login(payload) {
  // payload should contain: email, matKhau
  return requestJson("/api/auth/login", payload);
}

export default { register, verifyEmail, login };