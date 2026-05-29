import React, { useState, useEffect } from 'react';
import AdminLayout from './AdminLayout';
import { Users, ClipboardCheck, Wallet, ArrowUpRight, RefreshCw } from 'lucide-react';
import './Admin.css';

export default function DashboardOverview() {
    const token = localStorage.getItem('token');
    const BASE_URL = "https://localhost:7295/api";

    const [pendingStadiums, setPendingStadiums] = useState([]);
    const [loading, setLoading] = useState(true);

    const fetchDashboardData = async () => {
        setLoading(true);
        try {
            const res = await fetch(`${BASE_URL}/StadiumManagement/unapproved-stadiums`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });
            // Backend trả về thẳng List<StadiumApprovalViewDto> dưới dạng Array
            if (res.ok) {
                const data = await res.json();
                setPendingStadiums(data || []);
            }
        } catch (error) {
            console.error("Lỗi tải dữ liệu kiểm duyệt sân bóng:", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchDashboardData();
    }, []);

    return (
        <AdminLayout activeTab="dashboard">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <h1 className="page-title">Dashboard Overview</h1>
                    <p className="page-subtitle">Theo dõi hiệu suất hệ thống và quản lý các yêu cầu phê duyệt mới.</p>
                </div>
                <button onClick={fetchDashboardData} className="btn btn-outline"><RefreshCw size={16} /> Làm mới</button>
            </div>

            <div className="dashboard-grid" style={{ marginTop: '32px' }}>
                <div className="stat-card">
                    <div className="stat-icon" style={{ background: '#dcfce7', color: '#16a34a' }}><Users size={24} /></div>
                    <div className="stat-label">Tổng người dùng hệ thống</div>
                    <div className="stat-value">1,245</div>
                    <div className="stat-trend"><ArrowUpRight size={14} /> Hệ thống chạy ổn định</div>
                </div>

                <div className="stat-card">
                    <div className="stat-icon" style={{ background: '#fef3c7', color: '#d97706' }}><ClipboardCheck size={24} /></div>
                    <div className="stat-label">Sân bóng chờ duyệt</div>
                    <div className="stat-value" style={{ color: '#d97706' }}>{pendingStadiums.length}</div>
                    <div className="stat-trend" style={{ color: '#d97706' }}>🕒 Cần xử lý ngay</div>
                </div>

                <div className="stat-card">
                    <div className="stat-icon" style={{ background: '#e0e7ff', color: '#4f46e5' }}><Wallet size={24} /></div>
                    <div className="stat-label">Doanh thu ước tính</div>
                    <div className="stat-value">Đối soát ví</div>
                    <div className="stat-trend" style={{ color: '#4f46e5' }}>📊 Hệ thống kết nối DB</div>
                </div>
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '24px', marginTop: '24px' }}>
                <div className="card">
                    <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px' }}>
                        <h3 style={{ fontSize: '18px' }}>Yêu cầu duyệt sân bóng gần đây</h3>
                        <span onClick={() => window.location.href = '/admin/stadiums'} style={{ color: '#16a34a', fontSize: '14px', cursor: 'pointer', fontWeight: '500' }}>Xem tất cả</span>
                    </div>
                    {loading ? <p style={{ color: '#6b7280' }}>Đang tải dữ liệu...</p> : (
                        <table>
                            <thead>
                                <tr>
                                    <th>Tên sân bóng</th>
                                    <th>Địa chỉ</th>
                                    <th>Chủ sân</th>
                                    <th>Hành động</th>
                                </tr>
                            </thead>
                            <tbody>
                                {pendingStadiums.length === 0 ? (
                                    <tr>
                                        <td colSpan="4" style={{ textAlign: 'center', color: '#6b7280', padding: '20px' }}>Không có sân nào đang chờ duyệt.</td>
                                    </tr>
                                ) : (
                                    pendingStadiums.slice(0, 5).map((pitch) => (
                                        <tr key={pitch.maSanBong}>
                                            <td><div style={{ fontWeight: 600 }}>{pitch.tenSan}</div></td>
                                            <td style={{ color: '#4b5563' }}>{pitch.diaChi}</td>
                                            <td><span className="badge badge-success">{pitch.chuSan}</span></td>
                                            <td>
                                                <button onClick={() => window.location.href = '/admin/stadiums'} className="btn btn-primary" style={{ padding: '6px 12px', borderRadius: '20px', fontSize: '12px' }}>
                                                    Kiểm duyệt
                                                </button>
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    )}
                </div>

                <div>
                    <div className="card" style={{ background: '#166534', color: 'white', marginBottom: '24px' }}>
                        <h3 style={{ marginBottom: '12px' }}>Hệ thống SportSync</h3>
                        <p style={{ fontSize: '14px', opacity: 0.9, marginBottom: '20px' }}>Dữ liệu đồng bộ trực tiếp từ Database PostgreSQL của dự án.</p>
                    </div>
                </div>
            </div>
        </AdminLayout>
    );
}