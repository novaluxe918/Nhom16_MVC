import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import axios from 'axios';
import Header from '../components/Header'; 
import Footer from '../components/Footer'; 

const CumSanPage = () => {
    const { id } = useParams(); // Lấy ID sân mẹ từ URL
    const [sanMe, setSanMe] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    const SAN_ME_URL = "https://localhost:7295/images/SanMe/";
    const SAN_CON_URL = "https://localhost:7295/images/SanCon/";

    const resolveImageUrl = (imagePath, folderUrl) => {
        if (!imagePath) return "https://placehold.co/600x400/e2e8f0/a0aec0?text=No+Image";
        if (imagePath.includes("link.com")) return `${folderUrl}${imagePath.split('/').pop()}`;
        if (!imagePath.includes('.')) return `${folderUrl}${imagePath}.jpg`;
        return `${folderUrl}${imagePath}`;
    };

    useEffect(() => {
        const fetchChiTietSanMe = async () => {
            try {
                // Gọi API lấy chi tiết cụm sân
                const response = await axios.get(`https://localhost:7295/api/SanBong/chi-tiet-san-me/${id}`);
                if (response.data.success) {
                    setSanMe(response.data.data);
                }
            } catch (error) {
                console.error("Lỗi khi tải chi tiết sân mẹ:", error);
            } finally {
                setIsLoading(false);
            }
        };

        fetchChiTietSanMe();
    }, [id]);

    if (isLoading) return <div className="min-h-screen flex items-center justify-center font-bold text-primary">Đang tải dữ liệu cụm sân...</div>;
    if (!sanMe) return <div className="min-h-screen flex items-center justify-center font-bold text-error">Không tìm thấy thông tin sân.</div>;

    return (
        <div className="bg-surface text-on-surface min-h-screen font-body selection:bg-primary-container flex flex-col">
            <Header />

            <main className="pt-12 pb-20 px-4 md:px-8 max-w-7xl mx-auto flex-grow w-full">
                {/* ================= HERO SECTION (THÔNG TIN CHUNG) ================= */}
                <section className="grid grid-cols-1 lg:grid-cols-12 gap-8 mb-16 items-center">
                    <div className="lg:col-span-7 space-y-6">
                        {/* Breadcrumb */}
                        <nav className="flex items-center gap-2 text-sm uppercase tracking-wider text-on-surface-variant mb-4 font-bold">
                            <span>Đà Nẵng</span>
                            <span>&gt;</span>
                            <span>{sanMe.quan}</span>
                            <span>&gt;</span>
                            <span className="text-primary">{sanMe.tenSan}</span>
                        </nav>

                        <h2 className="font-display text-5xl md:text-7xl font-black text-on-surface tracking-tight leading-[1.1]">
                            Sân bóng <br />
                            <span className="text-primary">{sanMe.tenSan}</span>
                        </h2>

                        <div className="flex flex-col sm:flex-row gap-6 py-2">
                            <div className="flex items-center gap-3">
                                <div className="w-10 h-10 rounded-full bg-surface-container-high flex items-center justify-center text-primary font-bold text-xl">
                                    📍
                                </div>
                                <div>
                                    <p className="text-xs uppercase tracking-wide text-on-surface-variant font-bold">Khu Vực</p>
                                    <p className="font-body text-lg font-medium">{sanMe.diaChi}, {sanMe.quan}</p>
                                </div>
                            </div>
                            <div className="flex items-center gap-3">
                                <div className="w-10 h-10 rounded-full bg-surface-container-high flex items-center justify-center text-primary font-bold text-xl">
                                    ⏱️
                                </div>
                                <div>
                                    <p className="text-xs uppercase tracking-wide text-on-surface-variant font-bold">Giờ Hoạt Động</p>
                                    <p className="font-body text-lg font-medium">{sanMe.gioHoatDong}</p>
                                </div>
                            </div>
                        </div>

                        <p className="font-body text-lg text-on-surface-variant leading-relaxed max-w-2xl">
                            {sanMe.moTa || "Chưa có mô tả cho cụm sân này."}
                        </p>

                        <div className="flex flex-wrap gap-3">
                            <span className="px-4 py-2 bg-surface-container-high rounded-sm text-xs font-bold uppercase tracking-wider">Wifi Miễn Phí</span>
                            <span className="px-4 py-2 bg-surface-container-high rounded-sm text-xs font-bold uppercase tracking-wider">Bãi Đỗ Ô Tô</span>
                            <span className="px-4 py-2 bg-surface-container-high rounded-sm text-xs font-bold uppercase tracking-wider">Căng Tin</span>
                        </div>
                    </div>

                    <div className="lg:col-span-5 h-[500px] rounded-xl overflow-hidden shadow-2xl relative group">
                        {/* Render ảnh đầu tiên trong Album, nếu không có thì lấy ảnh mặc định */}
                        <img
                            className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110"
                            src={resolveImageUrl(sanMe.albumMedia?.[0], SAN_ME_URL)}
                            alt={sanMe.tenSan}
                            onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/800x500?text=San+Me"; }}
                        />
                        <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent opacity-60"></div>
                        <div className="absolute bottom-8 left-8 text-white">
                            <p className="text-xs uppercase tracking-widest mb-1 font-bold">Cơ sở vật chất</p>
                            <p className="text-2xl font-bold">Tiêu Chuẩn Quốc Tế</p>
                        </div>
                    </div>
                </section>

                {/* ================= CHILD PITCH LIST (DANH SÁCH SÂN CON) ================= */}
                <section className="mt-24">
                    <div className="flex justify-between items-end mb-12">
                        <div>
                            <p className="text-xs uppercase tracking-widest text-primary font-bold mb-2">Hệ thống sân</p>
                            <h3 className="font-display text-4xl font-bold text-on-surface">Danh Sách Sân Con</h3>
                        </div>
                    </div>

                    <div className="grid grid-cols-1 gap-8">
                        {sanMe.danhSachSanCon && sanMe.danhSachSanCon.length > 0 ? (
                            sanMe.danhSachSanCon.map((sanCon) => (
                                <div key={sanCon.maSanChiTiet} className="bg-surface-container-lowest rounded-xl overflow-hidden flex flex-col md:flex-row group transition-all hover:shadow-xl hover:-translate-y-1">

                                    {/* Ảnh Sân Con */}
                                    <div className="md:w-1/3 h-64 md:h-auto overflow-hidden relative">
                                        <img
                                            className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                                            src={resolveImageUrl(sanCon.anhDaiDien, SAN_CON_URL)}
                                            alt={sanCon.tenSanChiTiet}
                                            onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/400x300?text=San+Con"; }}
                                        />
                                        <div className="absolute top-4 left-4 bg-primary text-white px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wider shadow-md">
                                            {sanCon.loaiSan}
                                        </div>
                                    </div>

                                    {/* Chi tiết Sân Con */}
                                    <div className="p-8 flex-1 flex flex-col justify-between">
                                        <div>
                                            <div className="flex justify-between items-start mb-4">
                                                <div>
                                                    <h4 className="font-display text-2xl font-bold mb-1">{sanCon.tenSanChiTiet}</h4>
                                                </div>
                                                <div className="text-right">
                                                    <span className="bg-green-100 text-primary px-3 py-1 rounded-full text-xs font-bold">Sẵn sàng</span>
                                                </div>
                                            </div>

                                            <div className="grid grid-cols-2 gap-4 mb-8">
                                                <div className="bg-surface p-4 rounded-lg border border-outline-variant/20">
                                                    <p className="text-[10px] uppercase tracking-wider text-on-surface-variant font-bold mb-1">Giá sáng (05:00 - 16:00)</p>
                                                    <p className="text-xl font-black text-on-surface">{sanCon.giaThueBuoiSang?.toLocaleString()}đ<span className="text-sm font-normal text-on-surface-variant">/trận</span></p>
                                                </div>
                                                <div className="bg-surface p-4 rounded-lg border border-primary/20">
                                                    <p className="text-[10px] uppercase tracking-wider text-on-surface-variant font-bold mb-1">Giá tối (16:00 - 22:00)</p>
                                                    <p className="text-xl font-black text-primary">{sanCon.giaThueBuoiToi?.toLocaleString()}đ<span className="text-sm font-normal text-on-surface-variant">/trận</span></p>
                                                </div>
                                            </div>
                                        </div>

                                        <div className="flex items-center justify-end pt-4 border-t border-outline-variant/15">
                                            {/* Chuyển hướng đến trang Đặt lịch (Chi tiết Sân con) */}
                                            <Link to={`/san-con/${sanCon.maSanChiTiet}`} className="bg-gradient-to-r from-primary to-primary-container px-8 py-3 rounded-full text-white font-bold hover:shadow-lg active:scale-95 transition-all">
                                                Xem lịch trống
                                            </Link>
                                        </div>
                                    </div>

                                </div>
                            ))
                        ) : (
                            <div className="text-center py-10 text-on-surface-variant font-bold bg-white rounded-xl border">
                                Cụm sân này hiện chưa có sân con nào được thiết lập.
                            </div>
                        )}
                    </div>
                </section>
            </main>

            <Footer />
        </div>
    );
};

export default CumSanPage;