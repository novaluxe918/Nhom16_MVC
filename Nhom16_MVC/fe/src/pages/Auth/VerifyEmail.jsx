import React, { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Mail, KeyRound, ArrowLeft } from 'lucide-react';
import './Auth.css';

const API_BASE_URL = "https://localhost:7295/api/Auth";

export default function VerifyEmail() {
    const navigate = useNavigate();
    const location = useLocation();

    // Lấy email tự động truyền sang từ trang Đăng ký (nếu có)
    const [email, setEmail] = useState(location.state?.email || '');
    const [otp, setOtp] = useState('');
    const [loading, setLoading] = useState(false);

    const handleVerifyOtp = async (e) => {
        e.preventDefault();
        setLoading(true);

        const payload = {
            email: email.trim(),
            otp: otp.trim()
        };

        try {
            const response = await fetch(`${API_BASE_URL}/verify-email`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            const result = await response.json();

            if (response.ok && result.success) {
                alert("Xác thực Email tài khoản thành công!");

                // Lưu token đăng nhập tự động từ dữ liệu trả về của API
                localStorage.setItem('token', result.data.token);
                localStorage.setItem('userEmail', result.data.email);
                localStorage.setItem('userHoTen', result.data.hoTen);
                localStorage.setItem('userVaiTro', result.data.vaiTro);

                // Điều hướng thẳng vào trang Dashboard chính
                window.location.href = '/';
            } else {
                alert(result.message || "Mã OTP không chính xác hoặc đã hết hạn!");
            }
        } catch (error) {
            console.error("Lỗi kết nối API xác thực:", error);
            alert("Lỗi kết nối máy chủ xác thực tài khoản!");
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
                    <button type="button" className="auth-back-link" onClick={() => navigate('/register')}>
                        <ArrowLeft size={16} /> Quay lại trang đăng ký
                    </button>

                    <h2 className="auth-title">Xác thực tài khoản</h2>
                    <p className="auth-subtitle">Nhập mã OTP vừa được gửi đến hòm thư điện tử của bạn</p>

                    <form onSubmit={handleVerifyOtp} className="auth-main-form">
                        <div className="form-layout-group">
                            <label>Địa chỉ Email cần xác thực</label>
                            <div className="input-container-box">
                                <span className="input-icon-left"><Mail size={18} /></span>
                                <input type="email" placeholder="Nhập email xác thực..." value={email} onChange={(e) => setEmail(e.target.value)} required />
                            </div>
                        </div>

                        <div className="form-layout-group">
                            <label>Mã xác thực OTP</label>
                            <div className="input-container-box">
                                <span className="input-icon-left"><KeyRound size={18} /></span>
                                <input type="text" placeholder="Nhập mã mã OTP 6 số..." value={otp} onChange={(e) => setOtp(e.target.value)} required maxLength={10} />
                            </div>
                        </div>

                        <button type="submit" className="auth-submit-btn" disabled={loading}>
                            {loading ? "Đang xác thực..." : "Xác nhận kích hoạt tài khoản"}
                        </button>
                    </form>
                </div>
            </div>
        </div>
    );
}