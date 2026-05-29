// src/components/Admin/AdminLayout.jsx
import React from 'react';
import { Link } from 'react-router-dom'; // Thêm import Link để sửa lỗi chuyển trang
import {
    LayoutDashboard,
    Users,
    ClipboardCheck,
    Wallet,
    Star,
    LogOut
} from 'lucide-react';
import './Admin.css';

export default function AdminLayout({ children, activeTab }) {
    return (
        <div className="admin-container" style={{ display: 'flex', minHeight: '100vh' }}>

            {/* === SIDEBAR (MENU BÊN TRÁI) === */}
            <div className="admin-sidebar" style={{
                width: '260px',
                background: '#1e293b', // Màu xanh đen (slate-800) khớp với hệ thống của bạn
                color: '#ffffff',
                display: 'flex',
                flexDirection: 'column',
                boxShadow: '2px 0 5px rgba(0,0,0,0.1)'
            }}>
                {/* Logo / Tên hệ thống */}
                <div style={{ padding: '24px 20px', borderBottom: '1px solid #334155', textAlign: 'center' }}>
                    <h3 style={{ color: '#22c55e', fontStyle: 'italic', letterSpacing: '1px' }}>SPORTSYNC</h3>
                    <span style={{ fontSize: '11px', color: '#94a3b8' }}>Hệ thống Quản trị v1.0</span>
                </div>

                {/* Danh sách Menu điều hướng */}
                <div className="sidebar-menu" style={{ flex: 1, padding: '20px 12px' }}>
                    <ul style={{ listStyle: 'none', padding: 0, margin: 0 }}>
                        <li style={{ marginBottom: '8px' }}>
                            {/* Đổi từ <a> href sang <Link> to */}
                            <Link to="/admin/dashboard" className={`menu-item ${activeTab === 'dashboard' ? 'active' : ''}`} style={menuItemStyle(activeTab === 'dashboard')}>
                                <LayoutDashboard size={18} /> Tổng quan (Dashboard)
                            </Link>
                        </li>
                        <li style={{ marginBottom: '8px' }}>
                            {/* Đổi từ <a> href sang <Link> to */}
                            <Link to="/admin/users" className={`menu-item ${activeTab === 'users' ? 'active' : ''}`} style={menuItemStyle(activeTab === 'users')}>
                                <Users size={18} /> Quản lý Người dùng
                            </Link>
                        </li>
                        <li style={{ marginBottom: '8px' }}>
                            {/* Đổi từ <a> href sang <Link> to */}
                            <Link to="/admin/stadiums" className={`menu-item ${activeTab === 'stadiums' ? 'active' : ''}`} style={menuItemStyle(activeTab === 'stadiums')}>
                                <ClipboardCheck size={18} /> Kiểm duyệt Sân bóng
                            </Link>
                        </li>
                        <li style={{ marginBottom: '8px' }}>
                            {/* Đổi từ <a> href sang <Link> to */}
                            <Link to="/admin/financial" className={`menu-item ${activeTab === 'financial' ? 'active' : ''}`} style={menuItemStyle(activeTab === 'financial')}>
                                <Wallet size={18} /> Quản lý Tài chính
                            </Link>
                        </li>
                        <li style={{ marginBottom: '8px' }}>
                            {/* Đổi từ <a> href sang <Link> to */}
                            <Link to="/admin/ratings" className={`menu-item ${activeTab === 'ratings' ? 'active' : ''}`} style={menuItemStyle(activeTab === 'ratings')}>
                                <Star size={18} /> Quản lý Đánh giá
                            </Link>
                        </li>
                    </ul>
                </div>
            </div>

            {/* === PHẦN NỘI DUNG CHÍNH (BÊN PHẢI) === */}
            <div className="main-content-clean" style={{ flex: 1, display: 'flex', flexDirection: 'column', background: '#f3f4f6' }}>

                {/* Topbar phía trên */}
                <div className="header-top">
                    <div>
                        <div style={{ color: '#166534', fontWeight: 'bold', marginBottom: '4px' }}>
                            Hệ thống Quản trị Sportsync
                        </div>
                    </div>
                    <div className="user-profile">
                        <div className="user-info">
                            <div className="name">Phan Công Phước</div>
                            <div className="email">admin@sportsync.vn</div>
                        </div>
                        <img src="https://i.pravatar.cc/150?img=11" alt="Admin Avatar" />
                        <button
                            className="btn btn-outline"
                            style={{ marginLeft: '16px', border: 'none', color: '#dc2626', background: 'transparent', cursor: 'pointer' }}
                            onClick={() => {
                                localStorage.clear();
                                window.location.href = '/'; // Đưa về đúng gốc trang Login của bạn trong App.jsx
                            }}
                        >
                            <LogOut size={16} /> Đăng xuất
                        </button>
                    </div>
                </div>

                {/* Khung nội dung hiển thị các trang con */}
                <div className="page-body-content" style={{ padding: '24px', flex: 1, overflowY: 'auto' }}>
                    {children}
                </div>
            </div>

        </div>
    );
}

// Hàm bổ trợ style nhanh cho các mục menu
const menuItemStyle = (isActive) => ({
    display: 'flex',
    alignItems: 'center',
    gap: '12px',
    padding: '12px 16px',
    borderRadius: '8px',
    color: isActive ? '#ffffff' : '#94a3b8',
    backgroundColor: isActive ? '#16a34a' : 'transparent', // Active lên màu xanh lá chủ đạo hệ thống
    textDecoration: 'none',
    fontWeight: isActive ? '600' : '500',
    fontSize: '14px',
    transition: 'all 0.2s'
});