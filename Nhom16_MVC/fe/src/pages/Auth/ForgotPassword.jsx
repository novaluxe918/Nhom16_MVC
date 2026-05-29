import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Mail, Lock, ArrowLeft, KeyRound, Eye, EyeOff } from 'lucide-react';
import './Auth.css';

const API_BASE_URL = "https://localhost:7295/api/Auth";

export default function ForgotPassword() {
    const navigate = useNavigate();
    const [step, setStep] = useState(1); // Quy trình gồm 2 bước

    const [email, setEmail] = useState('');
    const [otp, setOtp] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmNewPassword, setConfirmNewPassword] = useState('');

    const [showNewPass, setShowNewPass] = useState(false);
    const [showConfirmNewPass, setShowConfirmPass] = useState(false);
    const [loading, setLoading] = useState(false);

    // BƯỚC 1: Gửi email yêu cầu cung cấp mã khôi phục OTP
    const handleSendEmail = async (e) => {
        e.preventDefault();
        setLoading(true);

        const payload = { email: email.trim() };

        try {
            const response = await fetch(`${API_BASE_URL}/forgot-password`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            const result = await response.json();

            if (response.ok && result.success) {
                alert("Hệ thống đã gửi mã xác thực khôi phục mật khẩu vào Email của bạn.");
                setStep(2); // Chuyển sang bước 2 đặt lại mật khẩu mới
            } else {
                alert(result.message || "Email không tồn tại trên hệ thống!");
            }
        } catch (error) {
            console.error("Lỗi kết nối API gửi mã khôi phục:", error);
            alert("Không thể kết nối đến máy chủ!");
        } finally {
            setLoading(false);
        }
    };

    // BƯỚC 2: Gửi OTP kèm mật khẩu mới về API /reset-password
    const handleResetPassword = async (e) => {
        e.preventDefault();

        if (newPassword !== confirmNewPassword) {
            alert("Mật khẩu mới không trùng khớp!");
            return;
        }

        setLoading(true);

        const payload = {
            email: email.trim(),
            otp: otp.trim(),
            newPassword: newPassword
        };

        try {
            const response = await fetch(`${API_BASE_URL}/reset-password`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            const result = await response.json();

            if (response.ok && result.success) {
                alert("Chúc mừng! Bạn đã cập nhật mật khẩu mới thành công.");
                navigate('/login');
            } else {
                alert(result.message || "Mã OTP khôi phục không đúng hoặc đã hết hạn.");
            }
        } catch (error) {
            console.error("Lỗi kết nối API đổi mật khẩu:", error);
            alert("Lỗi kết nối máy chủ đặt lại mật khẩu!");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-sidebar">
                <div className="auth-sidebar-overlay"></div>
            </div>

            <div className="auth-form-section">
                <div className="auth-form-card">
                    <button type="button" className="auth-back-link" onClick={() => step === 2 ? setStep(1) : navigate('/login')}>
                        <ArrowLeft size={16} /> {step === 2 ? "Quay lại bước trước" : "Quay lại đăng nhập"}
                    </button>

                    <h2 className="auth-title">Quên mật khẩu</h2>
                    <p className="auth-subtitle">
                        {step === 1
                            ? "Cung cấp email tài khoản để nhận mã khôi phục mật khẩu."
                            : "Nhập mã OTP từ hòm thư và thiết lập mật khẩu bảo mật mới."
                        }
                    </p>

                    {step === 1 ? (
                        <form onSubmit={handleSendEmail} className="auth-main-form">
                            <div className="form-layout-group">
                                <label>Địa chỉ Email tài khoản</label>
                                <div className="input-container-box">
                                    <span className="input-icon-left"><Mail size={18} /></span>
                                    <input type="email" placeholder="Nhập email của bạn..." value={email} onChange={(e) => setEmail(e.target.value)} required />
                                </div>
                            </div>
                            <button type="submit" className="auth-submit-btn" disabled={loading}>
                                {loading ? "Đang xử lý..." : "Gửi mã khôi phục"}
                            </button>
                        </form>
                    ) : (
                        <form onSubmit={handleResetPassword} className="auth-main-form">
                            <div className="form-layout-group">
                                <label>Mã khôi phục OTP</label>
                                <div className="input-container-box">
                                    <span className="input-icon-left"><KeyRound size={18} /></span>
                                    <input type="text" placeholder="Nhập mã OTP nhận được..." value={otp} onChange={(e) => setOtp(e.target.value)} required />
                                </div>
                            </div>

                            <div className="form-layout-group">
                                <label>Mật khẩu mới</label>
                                <div className="input-container-box">
                                    <span className="input-icon-left"><Lock size={18} /></span>
                                    <input type={showNewPass ? "text" : "password"} placeholder="Nhập mật khẩu mới..." value={newPassword} onChange={(e) => setNewPassword(e.target.value)} required />
                                    <button type="button" className="input-icon-eye-right" onClick={() => setShowNewPass(!showNewPass)}>
                                        {showNewPass ? <EyeOff size={18} /> : <Eye size={18} />}
                                    </button>
                                </div>
                            </div>

                            <div className="form-layout-group">
                                <label>Nhập lại mật khẩu mới</label>
                                <div className="input-container-box">
                                    <span className="input-icon-left"><Lock size={18} /></span>
                                    <input type={showConfirmNewPass ? "text" : "password"} placeholder="Nhập lại mật khẩu mới để đối chiếu..." value={confirmNewPassword} onChange={(e) => setConfirmNewPassword(e.target.value)} required />
                                    <button type="button" className="input-icon-eye-right" onClick={() => setShowConfirmPass(!showConfirmNewPass)}>
                                        {showConfirmNewPass ? <EyeOff size={18} /> : <Eye size={18} />}
                                    </button>
                                </div>
                            </div>

                            <button type="submit" className="auth-submit-btn" disabled={loading}>
                                {loading ? "Đang xử lý..." : "Cập nhật mật khẩu mới"}
                            </button>
                        </form>
                    )}
                </div>
            </div>
        </div>
    );
}