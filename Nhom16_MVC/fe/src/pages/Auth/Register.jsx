import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { User, Mail, Phone, Lock, ArrowLeft, Users, Building2, Eye, EyeOff } from 'lucide-react';
import './Auth.css';

const API_BASE_URL = "https://localhost:7295/api/Auth";

export default function Register() {
    const navigate = useNavigate();

    const [vaiTro, setVaiTro] = useState('nguoiThue');
    const [hoTen, setHoTen] = useState('');
    const [email, setEmail] = useState('');
    const [soDienThoai, setSoDienThoai] = useState('');
    const [matKhau, setMatKhau] = useState('');
    const [nhapLaiMatKhau, setNhapLaiMatKhau] = useState('');

    const [showPass, setShowPass] = useState(false);
    const [showConfirmPass, setShowConfirmPass] = useState(false);
    const [loading, setLoading] = useState(false);

    const handleRegister = async (e) => {
        e.preventDefault();

        if (matKhau !== nhapLaiMatKhau) {
            alert("Mật khẩu nhập lại không trùng khớp!");
            return;
        }

        setLoading(true);

        const payload = {
            hoTen: hoTen.trim(),
            email: email.trim(),
            soDienThoai: soDienThoai.trim(),
            matKhau: matKhau,
            vaiTro: vaiTro
        };

        try {
            const response = await fetch(`${API_BASE_URL}/register`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            const result = await response.json();

            if (response.ok && result.success) {
                alert("Đăng ký tài khoản thành công! Hệ thống đã gửi mã OTP xác thực qua Email của bạn.");
                // Chuyển hướng sang trang nhập mã OTP kèm theo trạng thái email vừa đăng ký
                navigate('/verify-email', { state: { email: email.trim() } });
            } else {
                alert(result.message || "Đăng ký thất bại, vui lòng thử lại.");
            }
        } catch (error) {
            console.error("Lỗi kết nối API:", error);
            alert("Không thể kết nối đến máy chủ API!");
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
                    <button type="button" className="auth-back-link" onClick={() => navigate('/login')}>
                        <ArrowLeft size={16} /> Quay lại đăng nhập
                    </button>

                    <h2 className="auth-title">Đăng ký tài khoản</h2>
                    <p className="auth-subtitle">Tham gia hệ thống quản lý và đặt sân bóng SportSync</p>

                    <form onSubmit={handleRegister} className="auth-main-form">
                        <div className="form-layout-group">
                            <label>Bạn tham gia hệ thống với vai trò gì?</label>
                            <div className="role-selection-wrapper">
                                <button type="button" className={`role-option-btn ${vaiTro === 'nguoiThue' ? 'active' : ''}`} onClick={() => setVaiTro('nguoiThue')}>
                                    <Users size={18} />
                                    <span>Người thuê sân</span>
                                </button>
                                <button type="button" className={`role-option-btn ${vaiTro === 'chuSan' ? 'active' : ''}`} onClick={() => setVaiTro('chuSan')}>
                                    <Building2 size={18} />
                                    <span>Chủ sân bóng</span>
                                </button>
                            </div>
                        </div>

                        <div className="form-layout-group">
                            <label>Họ và tên</label>
                            <div className="input-container-box">
                                <span className="input-icon-left"><User size={18} /></span>
                                <input type="text" placeholder="Nhập họ và tên đầy đủ..." value={hoTen} onChange={(e) => setHoTen(e.target.value)} required />
                            </div>
                        </div>

                        <div className="form-layout-group">
                            <label>Địa chỉ Email</label>
                            <div className="input-container-box">
                                <span className="input-icon-left"><Mail size={18} /></span>
                                <input type="email" placeholder="Ví dụ: nva@gmail.com..." value={email} onChange={(e) => setEmail(e.target.value)} required />
                            </div>
                        </div>

                        <div className="form-layout-group">
                            <label>Số điện thoại</label>
                            <div className="input-container-box">
                                <span className="input-icon-left"><Phone size={18} /></span>
                                <input type="tel" placeholder="Nhập số điện thoại di động..." value={soDienThoai} onChange={(e) => setSoDienThoai(e.target.value)} required />
                            </div>
                        </div>

                        <div className="form-layout-group">
                            <label>Mật khẩu</label>
                            <div className="input-container-box">
                                <span className="input-icon-left"><Lock size={18} /></span>
                                <input type={showPass ? "text" : "password"} placeholder="Tạo mật khẩu an toàn..." value={matKhau} onChange={(e) => setMatKhau(e.target.value)} required />
                                <button type="button" className="input-icon-eye-right" onClick={() => setShowPass(!showPass)}>
                                    {showPass ? <EyeOff size={18} /> : <Eye size={18} />}
                                </button>
                            </div>
                        </div>

                        <div className="form-layout-group">
                            <label>Nhập lại mật khẩu</label>
                            <div className="input-container-box">
                                <span className="input-icon-left"><Lock size={18} /></span>
                                <input type={showConfirmPass ? "text" : "password"} placeholder="Xác nhận lại mật khẩu..." value={nhapLaiMatKhau} onChange={(e) => setNhapLaiMatKhau(e.target.value)} required />
                                <button type="button" className="input-icon-eye-right" onClick={() => setShowConfirmPass(!showConfirmPass)}>
                                    {showConfirmPass ? <EyeOff size={18} /> : <Eye size={18} />}
                                </button>
                            </div>
                        </div>

                        <button type="submit" className="auth-submit-btn" disabled={loading}>
                            {loading ? "Đang xử lý đăng ký..." : "Tạo tài khoản"}
                        </button>
                    </form>
                </div>
            </div>
        </div>
    );
}