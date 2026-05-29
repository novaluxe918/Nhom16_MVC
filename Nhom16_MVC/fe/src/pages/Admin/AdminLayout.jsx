import React from 'react';
import { LogOut } from 'lucide-react';
import './Admin.css';

// Sửa lại đường dẫn import đúng chuẩn vị trí cùng thư mục Admin
import DashboardOverview from "./DashboardOverview";
import UsersManagement from "./UsersManagement";
import StadiumsApproval from "./StadiumsApproval";

export default function AdminLayout({ children }) {
    return (
        <div className="main-content-clean">
            {/* Topbar phía trên khớp với Sidebar xanh đen có sẵn của hệ thống */}
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
                            window.location.href = '/login';
                        }}
                    >
                        <LogOut size={16} /> Đăng xuất
                    </button>
                </div>
            </div>

            {/* Khung nội dung hiển thị các trang con */}
            <div className="page-body-content">
                {children}
            </div>
        </div>
    );
}