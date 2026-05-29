import React from 'react';
import { X } from 'lucide-react';

export default function UserDetailModal({ user, onClose }) {
    if (!user) return null;
    const isLocked = user.trangThai === 'bi_khoa';

    return (
        <div style={{ position: 'fixed', inset: 0, backgroundColor: 'rgba(0,0,0,0.5)', display: 'flex', justifyContent: 'center', alignItems: 'center', zIndex: 1200 }}>
            <div style={{ background: '#ffffff', borderRadius: '16px', width: '550px', padding: '28px', position: 'relative' }}>
                <button onClick={onClose} style={{ position: 'absolute', top: '20px', right: '20px', background: 'transparent', border: 'none', cursor: 'pointer' }}>
                    <X size={20} color="#6b7280" />
                </button>

                <div style={{ display: 'flex', alignItems: 'center', gap: '16px', marginBottom: '24px' }}>
                    <div style={{ width: '56px', height: '56px', borderRadius: '50%', background: '#e2e8f0', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '20px', fontWeight: 700, color: '#475569' }}>
                        {user.hoTen?.charAt(0).toUpperCase()}
                    </div>
                    <div>
                        <h2 style={{ fontSize: '18px', fontWeight: 700, margin: 0 }}>{user.hoTen}</h2>
                        <div style={{ display: 'flex', gap: '6px', marginTop: '4px' }}>
                            <span className="badge" style={{ background: '#f1f5f9', color: '#334155', fontSize: '11px' }}>{user.vaiTro}</span>
                            <span className="badge" style={{ background: isLocked ? '#fee2e2' : '#dcfce7', color: isLocked ? '#dc2626' : '#16a34a', fontSize: '11px' }}>
                                {isLocked ? 'BỊ KHÓA' : 'HOẠT ĐỘNG'}
                            </span>
                        </div>
                    </div>
                </div>

                <div style={{ background: '#f8fafc', padding: '16px', borderRadius: '8px', marginBottom: '20px' }}>
                    <div style={{ marginBottom: '12px' }}><span style={{ fontSize: '12px', color: '#6b7280' }}>EMAIL TÀI KHOẢN:</span><div style={{ fontWeight: 500 }}>{user.email}</div></div>
                    <div style={{ marginBottom: '12px' }}><span style={{ fontSize: '12px', color: '#6b7280' }}>SỐ ĐIỆN THOẠI:</span><div style={{ fontWeight: 500 }}>{user.soDienThoai || 'Chưa cập nhật'}</div></div>
                    <div><span style={{ fontSize: '12px', color: '#6b7280' }}>SỐ DƯ VÍ HIỆN TẠI:</span><div style={{ fontWeight: 600, color: '#16a34a' }}>{user.soDuTaiKhoan?.toLocaleString()}đ</div></div>
                </div>

                <div style={{ display: 'flex', justifyContent: 'flex-end' }}>
                    <button onClick={onClose} className="btn btn-outline" style={{ padding: '8px 20px' }}>Đóng lại</button>
                </div>
            </div>
        </div>
    );
}