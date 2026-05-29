import React, { useState } from "react";
import { register as apiRegister, verifyEmail as apiVerifyEmail } from "../../services/authService";

export default function RegisterPage() {
  const [hoTen, setHoTen] = useState("");
  const [email, setEmail] = useState("");
  const [soDienThoai, setSoDienThoai] = useState("");
  const [matKhau, setMatKhau] = useState("");
  const [vaiTro, setVaiTro] = useState("nguoiThue"); // "nguoiThue" or "chuSan"
  const [loading, setLoading] = useState(false);
  const [step, setStep] = useState("form"); // form | otp | success
  const [otp, setOtp] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  async function handleRegister(e) {
    e.preventDefault();
    setError("");
    setMessage("");
    setLoading(true);

    try {
      const payload = {
        hoTen,
        email,
        soDienThoai,
        matKhau,
        vaiTro
      };

      const res = await apiRegister(payload);

      if (!res) throw new Error("No response from server");
      if (!res.success) {
        setError(res.message || "Registration error");
        setLoading(false);
        return;
      }

      // If email was sent -> show OTP verification form.
      if (res.emailSent === false) {
        // server created account but failed to send OTP
        setMessage("Registration created but email sending failed. Please request resend OTP.");
        setStep("otp"); // still allow OTP flow if user later requests resend
      } else {
        setMessage("OTP has been sent to your email. Please check and enter the code.");
        setStep("otp");
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : String(err));
    } finally {
      setLoading(false);
    }
  }

  async function handleVerifyOtp(e) {
    e.preventDefault();
    setError("");
    setMessage("");
    setLoading(true);

    try {
      const res = await apiVerifyEmail({ email, otp });

      if (!res) throw new Error("No response from server");
      if (!res.success) {
        setError(res.message || "OTP verification failed");
        setLoading(false);
        return;
      }

      setMessage("Email verified. You can now sign in.");
      setStep("success");
    } catch (err) {
      setError(err instanceof Error ? err.message : String(err));
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 p-6">
      <div className="w-full max-w-md bg-white rounded-lg shadow-md p-8">
        <h1 className="text-2xl font-semibold text-emerald-600 text-center mb-4">Đăng ký SportSync</h1>

        {step === "form" && (
          <form onSubmit={handleRegister} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700">Họ và tên</label>
              <input
                required
                value={hoTen}
                onChange={(e) => setHoTen(e.target.value)}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-2 focus:ring-emerald-300 focus:border-emerald-500"
                placeholder="Nguyễn Văn A"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">Email</label>
              <input
                required
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-2 focus:ring-emerald-300 focus:border-emerald-500"
                placeholder="you@example.com"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">Số điện thoại (tùy chọn)</label>
              <input
                value={soDienThoai}
                onChange={(e) => setSoDienThoai(e.target.value)}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-2 focus:ring-emerald-300 focus:border-emerald-500"
                placeholder="0912xxxxxx"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700">Mật khẩu</label>
              <input
                required
                type="password"
                value={matKhau}
                onChange={(e) => setMatKhau(e.target.value)}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-2 focus:ring-emerald-300 focus:border-emerald-500"
                placeholder="••••••••"
                minLength={6}
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Vai trò</label>
              <div className="flex gap-3">
                <button
                  type="button"
                  onClick={() => setVaiTro("nguoiThue")}
                  className={`flex-1 py-2 rounded-md border ${vaiTro === "nguoiThue" ? "bg-emerald-600 text-white border-emerald-600" : "bg-white text-gray-700 border-gray-200"} transition`}
                >
                  Người thuê
                </button>
                <button
                  type="button"
                  onClick={() => setVaiTro("chuSan")}
                  className={`flex-1 py-2 rounded-md border ${vaiTro === "chuSan" ? "bg-emerald-600 text-white border-emerald-600" : "bg-white text-gray-700 border-gray-200"} transition`}
                >
                  Chủ sân
                </button>
              </div>
            </div>

            <div>
              <button
                type="submit"
                disabled={loading}
                className="w-full py-2 rounded-md bg-emerald-600 hover:bg-emerald-700 text-white font-medium disabled:opacity-60"
              >
                {loading ? "Đang gửi..." : "Đăng ký"}
              </button>
            </div>

            {error && <p className="text-sm text-red-600">{error}</p>}
            {message && <p className="text-sm text-emerald-600">{message}</p>}
          </form>
        )}

        {step === "otp" && (
          <form onSubmit={handleVerifyOtp} className="space-y-4">
            <p className="text-sm text-gray-700">
              Chúng tôi đã gửi mã OTP đến <strong>{email}</strong>. Vui lòng nhập mã 6 chữ số.
            </p>

            <div>
              <label className="block text-sm font-medium text-gray-700">Mã OTP</label>
              <input
                required
                value={otp}
                onChange={(e) => setOtp(e.target.value)}
                className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:ring-2 focus:ring-emerald-300 focus:border-emerald-500"
                placeholder="123456"
                maxLength={6}
              />
            </div>

            <div className="flex gap-2">
              <button
                type="submit"
                disabled={loading}
                className="flex-1 py-2 rounded-md bg-emerald-600 hover:bg-emerald-700 text-white font-medium disabled:opacity-60"
              >
                {loading ? "Đang xác thực..." : "Xác thực OTP"}
              </button>
              <button
                type="button"
                onClick={() => setStep("form")}
                className="flex-1 py-2 rounded-md border border-gray-200 text-gray-700"
              >
                Quay lại
              </button>
            </div>

            {error && <p className="text-sm text-red-600">{error}</p>}
            {message && <p className="text-sm text-emerald-600">{message}</p>}
          </form>
        )}

        {step === "success" && (
          <div className="space-y-4">
            <p className="text-sm text-emerald-700">{message}</p>
            <a href="/login" className="block text-center py-2 bg-emerald-600 text-white rounded-md">Đăng nhập</a>
          </div>
        )}
      </div>
    </div>
  );
}   