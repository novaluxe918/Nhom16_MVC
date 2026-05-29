import React, { useState, useEffect } from 'react';
import AdminLayout from './AdminLayout';
import { Search, Eye, RefreshCw } from 'lucide-react';
import StadiumDetailModal from './StadiumDetailModal';
import './Admin.css';

export default function StadiumsApproval() {
    const token = localStorage.getItem('token');
    const BASE_URL = "https://localhost:7295/api";

    const [stadiums, setStadiums] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState("cho_duyet");

    const [selectedStadium, setSelectedStadium] = useState(null);
    const [showModal, setShowModal] = useState(false);

    const fetchUnapprovedStadiums = async () => {
        setLoading(true);
        try {
            const res = await fetch(`${BASE_URL}/StadiumManagement/unapproved-stadiums`, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` }
            });
            if (res.ok) {
                const data = await res.json();
                setStadiums(data || []);
            }
        } catch (error) {
            console.error("Lỗi tải danh sách sân bóng chưa phê duyệt:", error);
        } finally { setLoading(false); }
    };

    useEffect(() => { fetchUnapprovedStadiums(); }, []);

    // Hàm xử lý Phê duyệt / Từ chối đồng bộ cấu trúc Object gửi lên API
    const handleProcessApproval = async (maSanBong, isApproved) => {
        let lyDoTuChoi = "";
        if (!isApproved) {
            lyDoTuChoi = prompt("Nhập lý do từ chối đăng ký cơ sở sân bóng này:");
            if (!lyDoTuChoi) return;
        } else {
            if (!window.confirm("Xác nhận phê duyệt cho sân bóng này đi vào hoạt động công khai?")) return;
        }

        try {
            const res = await fetch(`${BASE_URL}/StadiumManagement/process-approval`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
                body: JSON.stringify({
                    maSanBong: maSanBong,
                    isApproved: isApproved,
                    lyDoTuChoi: lyDoTuChoi
                })
            });
            const result = await res.json();
            alert(result.message || result.Message);
            setShowModal(false);
            fetchUnapprovedStadiums();
        } catch (error) {
            console.error("Lỗi phê duyệt sân bóng:", error);
        }
    };

    const filteredStadiums = stadiums.filter(s =>
        s.tenSan?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        s.chuSan?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <AdminLayout activeTab="pitch">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '24px' }}>
                <div>
                    <h1 className="page-title">Kiểm duyệt đăng ký sân bóng</h1>
                    <p className="page-subtitle">Xác thực tính pháp lý và thông tin các cơ sở đối tác mới.</p>
                </div>
            </div>

            <div style={{ display: 'flex', gap: '24px', borderBottom: '2px solid #e5e7eb', marginBottom: '24px' }}>
                <div onClick={() => setActiveTab("cho_duyet")} style={{ paddingBottom: '12px', borderBottom: activeTab === 'cho_duyet' ? '2px solid #16a34a' : 'none', color: activeTab === 'cho_duyet' ? '#111827' : '#6b7280', fontWeight: 600, cursor: 'pointer' }}>
                    Chờ xử lý từ API ({stadiums.length})
                </div>
            </div>

            <div className="table-container">
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px' }}>
                    <div style={{ position: 'relative' }}>
                        <Search size={18} style={{ position: 'absolute', left: '12px', top: '10px', color: '#9ca3af' }} />
                        <input type="text" placeholder="Tìm kiếm tên sân, chủ cơ sở..." value={searchTerm} onChange={(e) => setSearchTerm(e.target.value)} style={{ padding: '10px 10px 10px 36px', borderRadius: '8px', border: '1px solid #d1d5db', width: '320px' }} />
                    </div>
                    <button onClick={fetchUnapprovedStadiums} className="btn btn-outline"><RefreshCw size={18} /></button>
                </div>

                {loading ? <p>Đang tải danh sách sân bóng...</p> : (
                    <table>
                        <thead>
                            <tr>
                                <th>Hình Ảnh</th>
                                <th>Tên Sân Bóng</th>
                                <th>Chủ Sở Hữu</th>
                                <th>Địa Chỉ Cơ Sở</th>
                                <th style={{ textAlign: 'center' }}>Hành Động</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredStadiums.length === 0 ? (
                                <tr>
                                    <td colSpan="5" style={{ textAlign: 'center', color: '#6b7280', padding: '20px' }}>Không có sân nào đang xếp hàng chờ duyệt.</td>
                                </tr>
                            ) : (
                                filteredStadiums.map((s) => (
                                    <tr key={s.maSanBong}>
                                        <td>
                                            <img src={s.hinhAnhDaiDien || "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?q=80&w=200"}
                                                style={{ width: '70px', height: '45px', objectFit: 'cover', borderRadius: '6px' }} alt="Sân" />
                                        </td>
                                        <td><div style={{ fontWeight: 600 }}>{s.tenSan}</div></td>
                                        <td><span className="badge" style={{ background: '#f0fdf4', color: '#166534' }}>{s.chuSan}</span></td>
                                        <td style={{ color: '#4b5563' }}>{s.diaChi}</td>
                                        <td style={{ textAlign: 'center' }}>
                                            <button onClick={() => { setSelectedStadium(s); setShowModal(true); }} className="btn btn-primary" style={{ padding: '6px 14px', fontSize: '13px' }}>
                                                <Eye size={14} style={{ marginRight: '4px', inlineSize: 'auto' }} /> Xem Hồ Sơ
                                            </button>
                                        </td>
                                    </tr>
                                ))
                            )}
                        </tbody>
                    </table>
                )}
            </div>

            {showModal && selectedStadium && (
                <StadiumDetailModal
                    stadium={selectedStadium}
                    onApprove={(id) => handleProcessApproval(id, true)}
                    onReject={(id) => handleProcessApproval(id, false)}
                    onClose={() => setShowModal(false)}
                />
            )}
        </AdminLayout>
    );
}