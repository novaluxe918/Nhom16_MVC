import React from 'react';
import { X, Check, Trash2 } from 'lucide-react';

export default function StadiumDetailModal({ stadium, onApprove, onReject, onClose }) {
    if (!stadium) return null;

    return (
        <div style={{ position: 'fixed', inset: 0, backgroundColor: 'rgba(0,0,0,0.6)', display: 'flex', justifyContent: 'center', alignItems: 'center', zIndex: 1100 }}>
            <div style={{ background: '#ffffff', borderRadius: '16px', width: '750px', maxHeight: '90vh', overflowY: 'auto', padding: '32px', position: 'relative' }}>
                <button onClick={onClose} style={{ position: 'absolute', top: '20px', right: '20px', background: 'transparent', border: 'none', cursor: 'pointer' }}>
                    <X size={22} color="#4b5563" />
                </button>

                <h2 style={{ fontSize: '22px', fontWeight: 700, marginBottom: '6px', color: '#111827' }}>Chi tiết hồ sơ đăng ký sân bóng</h2>
                <p style={{ color: '#6b7280', fontSize: '14px', marginBottom: '24px' }}>Mã định danh sân bóng hệ thống: #PITCH-{stadium.maSanBong}</p>

                <div style={{ background: '#f8fafc', padding: '20px', borderRadius: '12px', marginBottom: '24px' }}>
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px', marginBottom: '12px' }}>
                        <div><strong style={{ fontSize: '13px', color: '#4b5563' }}>TÊN CƠ SỞ:</strong> <div style={{ fontSize: '15px', fontWeight: 600, marginTop: '4px' }}>{stadium.tenSan}</div></div>
                        <div><strong style={{ fontSize: '13px', color: '#4b5563' }}>ĐỐI TÁC ĐĂNG KÝ (CHỦ SÂN):</strong> <div style={{ fontSize: '15px', fontWeight: 600, marginTop: '4px', color: '#16a34a' }}>{stadium.chuSan}</div></div>
                    </div>
                    <div><strong style={{ fontSize: '13px', color: '#4b5563' }}>ĐỊA CHỈ:</strong> <div style={{ fontSize: '14px', marginTop: '4px' }}>{stadium.diaChi}</div></div>
                    <div style={{ marginTop: '12px' }}><strong style={{ fontSize: '13px', color: '#4b5563' }}>MÔ TẢ CƠ SỞ VẬT CHẤT:</strong> <div style={{ fontSize: '14px', marginTop: '4px', color: '#4b5563', lineHeight: 1.5 }}>{stadium.moTa || 'Không có mô tả chi tiết từ đối tác.'}</div></div>
                </div>

                {/* Khối hiển thị mảng hình ảnh chi tiết lấy từ bảng media_sanbong */}
                <h4 style={{ fontSize: '14px', fontWeight: 600, marginBottom: '12px' }}>Bộ sưu tập hình ảnh chi tiết ({stadium.danhSachHinhAnhChiTiet?.length || 0} ảnh)</h4>
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: '12px', marginBottom: '32px' }}>
                    {stadium.danhSachHinhAnhChiTiet && stadium.danhSachHinhAnhChiTiet.length > 0 ? (
                        stadium.danhSachHinhAnhChiTiet.map((imgUrl, idx) => (
                            <img key={idx} src={imgUrl} style={{ width: '100%', height: '100px', objectFit: 'cover', borderRadius: '8px', border: '1px solid #e5e7eb' }} alt={`Chi tiết ${idx}`} />
                        ))
                    ) : (
                        <p style={{ color: '#9ca3af', fontSize: '13px', gridColumn: 'span 4' }}>Cơ sở này chưa đăng tải hình ảnh chi tiết lên bảng Media.</p>
                    )}
                </div>

                <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '12px' }}>
                    <button onClick={onClose} className="btn btn-outline">Thoát</button>
                    <button onClick={() => onReject(stadium.maSanBong)} className="btn" style={{ background: '#dc2626', color: 'white', display: 'flex', alignItems: 'center', gap: '6px' }}>
                        <Trash2 size={16} /> Từ chối & Gỡ sân
                    </button>
                    <button onClick={() => onApprove(stadium.maSanBong)} className="btn" style={{ background: '#16a34a', color: 'white', display: 'flex', alignItems: 'center', gap: '6px' }}>
                        <Check size={16} /> Phê duyệt cấp phép
                    </button>
                </div>
            </div>
        </div>
    );
}