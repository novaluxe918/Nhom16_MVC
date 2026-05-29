import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate, Link, Outlet, useNavigate } from 'react-router-dom';
import { LayoutDashboard, Users, MapPin, Wallet, Star, LogOut } from 'lucide-react';

import Login from './pages/Auth/Login';
import Register from './pages/Auth/Register';
import VerifyEmail from './pages/Auth/VerifyEmail';
import ForgotPassword from './pages/Auth/ForgotPassword';

import DashboardOverview from './pages/Admin/DashboardOverview';
import UsersManagement from './pages/Admin/UsersManagement';
import StadiumsApproval from './pages/Admin/StadiumsApproval';
import FinancialManagement from './pages/Admin/FinancialManagement';
import RatingsManagement from './pages/Admin/RatingsManagement';

/**
 * 🔒 Component Bảo vệ Tuyến đường Admin
 */
function AdminProtectedRoute({ children }) {
    const token = localStorage.getItem('token');
    const userVaiTro = localStorage.getItem('userVaiTro');

    if (!token || userVaiTro !== 'admin') {
        return <Navigate to="/login" replace />;
    }
    return children;
}

/**
 * 🏢 Bộ khung Layout Admin dùng chung (Có Sidebar cố định bên trái)
 */
function AdminLayout() {
    const navigate = useNavigate();
    const adminName = localStorage.getItem('userHoTen') || 'Phan Công Phước';

    const handleLogout = () => {
        localStorage.clear();
        navigate('/login');
    };

    return (
        <div style={{ display: 'flex', minHeight: '100vh', backgroundColor: '#f8fafc' }}>
            {/* 🧭 SIDEBAR BÊN TRÁI - Nơi chứa các nút để BẤM chuyển trang */}
            <div style={{ width: '260px', backgroundColor: '#1e293b', color: 'white', padding: '24px 16px', display: 'flex', flexDirection: 'column', gap: '8px' }}>
                <div style={{ padding: '0 8px 16px 8px', borderBottom: '1px solid #334155', marginBottom: '16px' }}>
                    <h3 style={{ color: '#38bdf8', margin: '0 0 4px 0' }}>SportSync Admin</h3>
                    <p style={{ fontSize: '12px', color: '#94a3b8', margin: 0 }}>Chào, {adminName}</p>
                </div>

                {/* Các nút bấm sử dụng thẻ Link của react-router-dom để không bị load lại trang */}
                <Link to="/admin/dashboard" style={sidebarLinkStyle}><LayoutDashboard size={18} /> Overview</Link>
                <Link to="/admin/users" style={sidebarLinkStyle}><Users size={18} /> Quản lý tài khoản</Link>
                <Link to="/admin/stadiums" style={sidebarLinkStyle}><MapPin size={18} /> Duyệt sân bóng</Link>
                <Link to="/admin/financial" style={sidebarLinkStyle}><Wallet size={18} /> Quản lý tài chính</Link>
                <Link to="/admin/ratings" style={sidebarLinkStyle}><Star size={18} /> Kiểm duyệt đánh giá</Link>

                <button onClick={handleLogout} style={{ marginTop: 'auto', display: 'flex', alignItems: 'center', gap: '10px', padding: '12px', backgroundColor: '#ef4444', color: 'white', border: 'none', borderRadius: '8px', cursor: 'pointer', fontWeight: '600' }}>
                    <LogOut size={18} /> Đăng xuất
                </button>
            </div>

            {/* 📂 NỘI DUNG PHẢI - Nơi hiển thị các phân hệ tương ứng khi bấm Sidebar */}
            <div style={{ flex: 1, padding: '30px', overflowY: 'auto' }}>
                <Outlet />
            </div>
        </div>
    );
}

// Style phụ trợ cho Sidebar Link
const sidebarLinkStyle = {
    display: 'flex',
    alignItems: 'center',
    gap: '12px',
    padding: '12px',
    color: '#cbd5e1',
    textDecoration: 'none',
    borderRadius: '8px',
    fontSize: '14px',
    fontWeight: '500',
    transition: 'all 0.2s'
};

/**
 * 🏠 Component Giao diện trang chủ Client tạm thời
 */
function HomePage() {
    const navigate = useNavigate();
    const token = localStorage.getItem('token');
    const userEmail = localStorage.getItem('userEmail');
    const userHoTen = localStorage.getItem('userHoTen');
    const userVaiTro = localStorage.getItem('userVaiTro');

    if (token && userVaiTro === 'admin') {
        return <Navigate to="/admin/dashboard" replace />;
    }

    return (
        <div style={{ padding: '40px', maxWidth: '900px', margin: '40px auto', fontFamily: 'Arial, sans-serif' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderBottom: '2px solid #e5e7eb', paddingBottom: '20px' }}>
                <h1 style={{ color: '#10b981', margin: 0 }}>⚽ SportSync Client Hub</h1>
                {token && (
                    <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
                        <span style={{ fontSize: '14px', color: '#4b5563' }}>
                            Xin chào, <strong>{userHoTen}</strong> ({userVaiTro === 'nguoiThue' ? 'Người thuê' : 'Chủ sân'})
                        </span>
                        <button onClick={() => { localStorage.clear(); navigate('/login'); }} style={{ padding: '8px 16px', backgroundColor: '#ef4444', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: '600' }}>
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
                <div style={{ marginTop: '30px', textAlign: 'center' }}>
                    <p style={{ fontSize: '16px', color: '#4b5563', marginBottom: '20px' }}>Bạn chưa đăng nhập vào hệ thống SportSync.</p>
                    <div style={{ display: 'flex', gap: '15px', justifyContent: 'center' }}>
                        <button onClick={() => navigate('/login')} style={{ padding: '10px 20px', backgroundColor: '#10b981', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: '600' }}>Đăng nhập ngay</button>
                        <button onClick={() => navigate('/register')} style={{ padding: '10px 20px', backgroundColor: '#ffffff', color: '#4b5563', border: '1px solid #d1d5db', borderRadius: '6px', cursor: 'pointer', fontWeight: '600' }}>Tạo tài khoản</button>
                    </div>
                </div>
            )}
        </div>
    );
}

/**
 * 🚀 COMPONENT APP CHÍNH CỦA HỆ THỐNG
 */
export default function App() {
    return (
        <Router>
            <Routes>
                {/* 🌏 Các tuyến đường công khai công cộng */}
                <Route path="/" element={<HomePage />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/verify-email" element={<VerifyEmail />} />
                <Route path="/forgot-password" element={<ForgotPassword />} />

                {/* 🔒 Nhóm các tuyến đường Admin được lồng gọn gàng vào Layout chung */}
                <Route
                    element={
                        <AdminProtectedRoute>
                            <AdminLayout />
                        </AdminProtectedRoute>
                    }
                >
                    <Route path="/admin/dashboard" element={<DashboardOverview />} />
                    <Route path="/admin/users" element={<UsersManagement />} />
                    <Route path="/admin/stadiums" element={<StadiumsApproval />} />
                    <Route path="/admin/financial" element={<FinancialManagement />} />
                    <Route path="/admin/ratings" element={<RatingsManagement />} />
                </Route>

                {/* 🔀 Điều hướng đường dẫn lỗi */}
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Router>
    );
}