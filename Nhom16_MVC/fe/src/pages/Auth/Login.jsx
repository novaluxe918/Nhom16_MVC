import  { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Mail, Lock, Eye, EyeOff } from 'lucide-react';
import './Auth.css';

const API_BASE_URL = "https://localhost:7295/api";

export default function Login() {
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [matKhau, setMatKhau] = useState('');
    const [showPass, setShowPass] = useState(false);
    const [loading, setLoading] = useState(false);

    const handleLogin = async (e) => {
        e.preventDefault();
        setLoading(true);

        const payload = {
            email: email.trim(),
            matKhau: matKhau
        };

        try {
            const response = await fetch(`${API_BASE_URL}/Auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            if (!response.ok && response.status === 404) {
                throw new Error("Không tìm thấy đường dẫn API endpoint trên Server (404).");
            }

            const result = await response.json();
            console.log("Full login response:", result);

            if (response.ok && result.success) {
                localStorage.setItem('token', result.token);
                localStorage.setItem('userId', result.userId);
                localStorage.setItem('userEmail', email.trim());
                localStorage.setItem('userHoTen', result.hoTen);
                localStorage.setItem('userVaiTro', result.vaiTro);

                alert("Đăng nhập thành công!");

                // CẬP NHẬT LUỒNG ĐIỀU HƯỚNG THÔNG MINH:
                if (result.vaiTro === 'admin') {
                    navigate('/admin'); 
                } else if (result.vaiTro === 'chuSan') {
                    navigate('/quan-ly-san');
                } else {
                    navigate('/'); 
                }
            } else {
                alert(result.message || "Tài khoản hoặc mật khẩu không chính xác.");
            }
        } catch (error) {
            console.error("Lỗi đăng nhập:", error);
            alert(error.message || "Không thể kết nối đến máy chủ.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-container">
            {/* Nửa bên trái: Banner phong cách thể thao */}
            <div className="auth-sidebar">
                <div className="auth-sidebar-overlay"></div>
            </div>

            {/* Nửa bên phải: Khối form đăng nhập */}
            <div className="auth-form-section">
                <div className="auth-form-card">
                    <h2 className="auth-title">Chào mừng trở lại</h2>
                    <p className="auth-subtitle">Vui lòng đăng nhập hệ thống SportSync để tiếp tục quản lý và đặt sân.</p>

                    <form onSubmit={handleLogin} className="auth-main-form">
                        <div className="form-layout-group">
                            <label>Địa chỉ Email</label>
                            <div className="input-container-box">
                                <span className="input-icon-left"><Mail size={18} /></span>
                                <input
                                    type="email"
                                    placeholder="Nhập email của bạn..."
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                    required
                                    disabled={loading}
                                />
                            </div>
                        </div>

                        <div className="form-layout-group">
                            <label>Mật khẩu</label>
                            <div className="input-container-box">
                                <span className="input-icon-left"><Lock size={18} /></span>
                                <input
                                    type={showPass ? "text" : "password"}
                                    placeholder="Nhập mật khẩu..."
                                    value={matKhau}
                                    onChange={(e) => setMatKhau(e.target.value)}
                                    required
                                    disabled={loading}
                                />
                                <button type="button" className="input-icon-eye-right" onClick={() => setShowPass(!showPass)}>
                                    {showPass ? <EyeOff size={18} /> : <Eye size={18} />}
                                </button>
                            </div>
                        </div>

                        <div className="form-flex-actions">
                            <label className="remember-checkbox-label">
                                <input type="checkbox" />
                                <span>Ghi nhớ đăng nhập</span>
                            </label>
                            <button type="button" className="auth-redirect-link" onClick={() => navigate('/forgot-password')}>
                                Quên mật khẩu?
                            </button>
                        </div>

                        <button type="submit" className="auth-submit-btn" disabled={loading}>
                            {loading ? "Đang xử lý đăng nhập..." : "Đăng nhập"}
                        </button>
                    </form>

                    <p className="auth-redirect-text">
                        Bạn chưa có tài khoản?
                        <button type="button" className="auth-redirect-link" onClick={() => navigate('/register')}>Đăng ký ngay</button>
                    </p>
                </div>
            </div>
        </div>
    );
}