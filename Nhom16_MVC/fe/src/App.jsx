import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Auth/Login';
import Register from './pages/Auth/Register';
import VerifyEmail from './pages/Auth/VerifyEmail';
import ForgotPassword from './pages/Auth/ForgotPassword';

// Component Trang Chính tạm thời sau khi đăng nhập thành công
function HomePage() {
    const token = localStorage.getItem('token');
    const userEmail = localStorage.getItem('userEmail');
    const userHoTen = localStorage.getItem('userHoTen');
    const userVaiTro = localStorage.getItem('userVaiTro');

    const handleLogout = () => {
        localStorage.clear();
        window.location.href = '/login';
    };

    return (
        <div style={{ padding: '40px', maxWidth: '900px', margin: '40px auto', fontFamily: 'Arial, sans-serif' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderBottom: '2px solid #e5e7eb', paddingBottom: '20px' }}>
                <h1 style={{ color: '#10b981', margin: 0 }}>⚽ SportSync Dashboard</h1>
                {token && (
                    <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
                        <span style={{ fontSize: '14px', color: '#4b5563' }}>
                            Xin chào, <strong>{userHoTen}</strong> ({userVaiTro === 'nguoiThue' ? 'Người thuê' : 'Chủ sân'})
                        </span>
                        <button onClick={handleLogout} style={{ padding: '8px 16px', backgroundColor: '#ef4444', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: '600' }}>
                            Đăng xuất
                        </button>
                    </div>
                )}
            </div>

            {token ? (
                <div style={{ marginTop: '30px', padding: '20px', backgroundColor: '#ecfdf5', borderRadius: '8px', border: '1px solid #10b981' }}>
                    <h3>🎉 Đăng nhập hệ thống thành công!</h3>
                    <p style={{ color: '#065f46', marginTop: '10px' }}>Email tài khoản: {userEmail}</p>
                </div>
            ) : (
                <div style={{ marginTop: '30px', textalign: 'center' }}>
                    <p style={{ fontSize: '16px', color: '#4b5563', marginBottom: '20px' }}>Bạn chưa đăng nhập vào hệ thống SportSync.</p>
                    <div style={{ display: 'flex', gap: '15px', justifyContent: 'center' }}>
                        <button onClick={() => window.location.href = '/login'} style={{ padding: '10px 20px', backgroundColor: '#10b981', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: '600' }}>Đăng nhập ngay</button>
                        <button onClick={() => window.location.href = '/register'} style={{ padding: '10px 20px', backgroundColor: '#ffffff', color: '#4b5563', border: '1px solid #d1d5db', borderRadius: '6px', cursor: 'pointer', fontWeight: '600' }}>Tạo tài khoản</button>
                    </div>
                </div>
            )}
        </div>
    );
}

export default function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/verify-email" element={<VerifyEmail />} />
                <Route path="/forgot-password" element={<ForgotPassword />} />
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Router>
    );
}