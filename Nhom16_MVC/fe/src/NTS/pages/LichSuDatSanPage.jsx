import  { useState, useEffect } from 'react';
import axios from 'axios';
import Header from '../components/Header';
import Footer from '../components/Footer';
import { Link, useNavigate } from 'react-router-dom';

//Hàm sử lý tg
const parseDateTime = (dateStr, timeStr) => {
    if (!dateStr || !timeStr) return new Date(0);
    const [day, month, year] = dateStr.split('/');
    const [hour, minute] = timeStr.split(':');
    return new Date(year, month - 1, day, hour, minute);
};

const LichSuDatSanPage = () => {
    const navigate = useNavigate();

    // 1. Quản lý State
    const [danhSachDon, setDanhSachDon] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [activeTab, setActiveTab] = useState('tat_ca'); 
    // 2. GỌI API LẤY LỊCH SỬ ĐẶT SÂN TỪ BACKEND
    useEffect(() => {
        const fetchLichSu = async () => {
            const token = localStorage.getItem('token');
            if (!token) {
                alert("Vui lòng đăng nhập để xem lịch sử đặt sân!");
                navigate('/login');
                return;
            }

            try {
                
                const response = await axios.get('https://localhost:7295/api/DatSan/lich-su', {
                    headers: { Authorization: `Bearer ${token}` }
                });

                if (response.data.success) {
                    const now = new Date(); // Lấy thời gian hiện tại

                    const mappedData = response.data.data.map(don => {
                        // 1. Tính toán thời gian bắt đầu đá
                        const bookingTime = parseDateTime(don.ngayDat, don.gioBatDau);

                        // 2. Phân loại trạng thái dựa vào đồng hồ
                        let mappedTrangThai = 'sap_toi';

                        if (don.trangThai === 'da_huy' || don.trangThai === 'Huy') {
                            mappedTrangThai = 'da_huy';
                        } else if (bookingTime <= now) {
                            // Nếu thời gian đá đã qua so với hiện tại -> Đẩy vào Hoàn thành
                            mappedTrangThai = 'hoan_thanh';
                        }

                        return {
                            id: don.maDatSan,
                            maSanChiTiet: don.maSanChiTiet || don.MaSanChiTiet,
                            trangThai: mappedTrangThai,
                            rawNgayDat: don.ngayDat,     // Giữ lại dữ liệu thô để dùng cho chức năng hủy
                            rawGioBatDau: don.gioBatDau, // Giữ lại dữ liệu thô
                            tenSan: don.tenSanChiTiet || 'Sân bóng',
                            diaChi: don.diaChi || 'Đà Nẵng',
                            ngayDa: don.ngayDat,
                            khungGio: `${don.gioBatDau} - ${don.gioKetThuc}`,
                            tongTien: don.soTienThanhToan,
                            daThanhToan: true,
                            hinhAnh: don.hinhAnhSan || '/placeholder.jpg',
                            hoanTien: mappedTrangThai === 'da_huy' ? '100%' : null
                        };
                    });

                    setDanhSachDon(mappedData);
                }
            } catch (error) {
                console.error("Lỗi lấy lịch sử:", error);
                alert("Không thể tải dữ liệu lịch sử đặt sân.");
            } finally {
                setIsLoading(false);
            }
        };

        fetchLichSu();
    }, [navigate]);

    // 3. Logic Lọc Đơn Hàng (Chạy trên RAM sau khi đã có data từ API)
    const filteredDanhSach = danhSachDon.filter(don => {
        if (activeTab === 'tat_ca') return true;
        return don.trangThai === activeTab;
    });

    const donSapToiList = filteredDanhSach.filter(d => d.trangThai === 'sap_toi');
    const donKhacList = filteredDanhSach.filter(d => d.trangThai !== 'sap_toi');

    // 4. GỌI API HỦY ĐƠN ĐẶT SÂN
    const handleHuyDon = async (don) => {
        // Kiểm tra luật 2 tiếng trước khi gọi API
        const bookingTime = parseDateTime(don.rawNgayDat, don.rawGioBatDau);
        const now = new Date();
        const diffInHours = (bookingTime - now) / (1000 * 60 * 60);

        if (diffInHours < 2) {
            alert("⏳ Đã quá hạn hủy sân! Theo chính sách, bạn chỉ được hủy và hoàn tiền trước 2 tiếng so với giờ bắt đầu.");
            return;
        }

        const xacNhan = window.confirm("Bạn có chắc chắn muốn hủy đơn đặt sân này? Tiền sẽ được hoàn tự động vào ví.");
        if (!xacNhan) return;

        const token = localStorage.getItem('token');
        try {
            const response = await axios.post(`https://localhost:7295/api/SanBongChiTiet/huy-lich/${don.id}`, {}, {
                headers: { Authorization: `Bearer ${token}` }
            });

            if (response.data.success) {
                alert("Hủy sân thành công! Tiền đã được hoàn lại vào ví.");
                setDanhSachDon(prev => prev.map(d =>
                    d.id === don.id ? { ...d, trangThai: 'da_huy', hoanTien: '100%' } : d
                ));
            } else {
                alert(`Lỗi: ${response.data.message}`);
            }
        } catch (error) {
            console.error("Lỗi hủy sân:", error);
            alert("Đã xảy ra sự cố khi hủy sân. Vui lòng thử lại.");
        }
    };

    if (isLoading) return <div className="...">Đang tải lịch sử...</div>;

    return (
        <div className="bg-surface text-on-surface min-h-screen flex flex-col font-body">
            <Header />

            <main className="pt-28 pb-20 px-4 md:px-8 max-w-7xl mx-auto w-full flex-grow">
                {/* Header Tiêu đề */}
                <div className="mb-10">
                    <h1 className="text-4xl md:text-5xl font-black text-on-surface tracking-tight mb-2">Lịch sử & Quản lý đặt sân</h1>
                    <p className="text-on-surface-variant max-w-2xl text-lg">Theo dõi và quản lý tất cả các lượt đặt sân của bạn tại các sân bóng.</p>
                </div>

                {/* Banner Chính sách hoàn tiền */}
                <div className="mb-8 flex items-start gap-4 p-6 bg-primary-container/20 rounded-xl border-l-4 border-primary">
                    <span className="material-symbols-outlined text-primary" style={{ fontVariationSettings: "'FILL' 1" }}>info</span>
                    <div>
                        <p className="font-bold text-on-primary-container">Chính sách hoàn tiền</p>
                        <p className="text-on-surface-variant text-sm mt-1 leading-relaxed">Bạn sẽ được hoàn tiền tự động vào ví nếu hủy trước 2 tiếng so với giờ đá. Tiền sẽ được cộng lại ngay lập tức.</p>
                    </div>
                </div>

                {/* Tab Lọc Trạng Thái */}
                <div className="bg-surface-container-low p-1.5 rounded-full inline-flex mb-8 overflow-x-auto max-w-full">
                    {[
                        { key: 'tat_ca', label: 'Tất cả' },
                        { key: 'sap_toi', label: 'Sắp tới' },
                        { key: 'hoan_thanh', label: 'Hoàn thành' },
                        { key: 'da_huy', label: 'Đã hủy' }
                    ].map(tab => (
                        <button
                            key={tab.key}
                            onClick={() => setActiveTab(tab.key)}
                            className={`px-6 py-2 rounded-full whitespace-nowrap transition-all font-bold ${activeTab === tab.key
                                    ? 'bg-surface-container-lowest shadow-sm text-primary'
                                    : 'text-on-surface-variant hover:bg-surface-container-high'
                                }`}
                        >
                            {tab.label}
                        </button>
                    ))}
                </div>

                {/* Grid Hiển Thị Dữ Liệu */}
                <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">

                    {/* Cột Trái: Đơn Sắp Tới */}
                    <div className="lg:col-span-8 space-y-6">
                        {donSapToiList.length === 0 && activeTab === 'sap_toi' && (
                            <div className="text-center py-12 bg-surface-container-lowest rounded-[2rem] border border-outline-variant/20">
                                <span className="material-symbols-outlined text-6xl text-outline-variant opacity-50 mb-4">event_busy</span>
                                <p className="text-lg font-bold text-on-surface-variant">Bạn không có lịch đá nào sắp tới.</p>
                            </div>
                        )}

                        {donSapToiList.map(don => (
                            <div key={don.id} className="bg-surface-container-lowest rounded-[2rem] overflow-hidden transition-all duration-300 hover:shadow-xl hover:shadow-primary/5 flex flex-col md:flex-row border border-outline-variant/10">
                                <div className="md:w-2/5 relative h-64 md:h-auto overflow-hidden">
                                    <img src={don.hinhAnh} alt="Sân" className="w-full h-full object-cover transition-transform duration-500 hover:scale-105" />
                                    <div className="absolute top-4 left-4 bg-primary-container text-on-primary-container px-4 py-1.5 rounded-full font-bold text-xs uppercase tracking-widest shadow-lg">Sắp tới</div>
                                </div>
                                <div className="md:w-3/5 p-6 md:p-8 flex flex-col justify-between">
                                    <div>
                                        <div className="flex justify-between items-start mb-4">
                                            <div>
                                                <h3 className="text-2xl md:text-3xl font-black text-on-surface leading-tight mb-1">{don.tenSan}</h3>
                                                <p className="text-on-surface-variant text-sm flex items-start gap-1 mt-2">
                                                    📍 {don.diaChi}
                                                </p>
                                            </div>
                                            <div className="text-right">
                                                <p className="text-[10px] md:text-xs font-bold uppercase tracking-wider text-on-surface-variant mb-1">Tổng cộng</p>
                                                <p className="text-xl md:text-2xl font-black text-primary">{don.tongTien.toLocaleString()}đ</p>
                                            </div>
                                        </div>
                                        <div className="grid grid-cols-2 gap-4 mt-8">
                                            <div className="bg-surface-container-low p-4 rounded-xl">
                                                <p className="text-[10px] font-bold uppercase tracking-tighter text-on-surface-variant mb-1">Ngày đá</p>
                                                <p className="font-bold text-base md:text-lg">{don.ngayDa}</p>
                                            </div>
                                            <div className="bg-surface-container-low p-4 rounded-xl">
                                                <p className="text-[10px] font-bold uppercase tracking-tighter text-on-surface-variant mb-1">Khung giờ</p>
                                                <p className="font-bold text-base md:text-lg">{don.khungGio}</p>
                                            </div>
                                        </div>
                                    </div>
                                    <div className="mt-8 pt-6 border-t border-outline-variant/15 flex items-center justify-between">
                                        <div className="flex items-center gap-2 text-primary font-bold text-sm">
                                            <span className="material-symbols-outlined">verified</span>
                                            Đã thanh toán
                                        </div>
                                        <button
                                            onClick={() => handleHuyDon(don)}
                                            className="px-6 py-2.5 rounded-full border-2 border-error text-error font-bold hover:bg-error hover:text-white transition-all active:scale-95 text-sm"
                                        >
                                            Hủy đặt sân
                                        </button>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>

                    {/* Cột Phải: Đơn Hoàn Thành / Đã Hủy */}
                    <div className="lg:col-span-4 space-y-4">
                        {donKhacList.map(don => (
                            <div key={don.id} className={`bg-surface-container-lowest p-6 rounded-[1.5rem] transition-all border border-outline-variant/10 ${don.trangThai === 'da_huy' ? 'opacity-70 grayscale hover:grayscale-0 hover:opacity-100' : 'border-b-4 border-b-primary/20 hover:border-b-primary'}`}>
                                <div className="flex justify-between items-start mb-4">
                                    <span className={`px-3 py-1 rounded-md text-[10px] font-black uppercase tracking-widest ${don.trangThai === 'da_huy' ? 'bg-error/10 text-error' : 'bg-surface-container-highest text-on-surface-variant'}`}>
                                        {don.trangThai === 'da_huy' ? 'Đã hủy' : 'Hoàn thành'}
                                    </span>
                                    <span className="text-sm font-bold text-on-surface-variant">{don.ngayDa}</span>
                                </div>
                                <h4 className="text-xl font-bold mb-1">{don.tenSan}</h4>
                                <p className="text-sm text-on-surface-variant mb-4 truncate">📍 {don.diaChi}</p>
                                <div className="flex justify-between items-end pt-4 border-t border-outline-variant/10">
                                    <div className="text-sm">
                                        <p className="text-on-surface-variant font-medium">{don.khungGio}</p>
                                        <p className="font-black text-lg mt-1">{don.tongTien.toLocaleString()}đ</p>
                                    </div>
                                    {don.trangThai === 'da_huy' ? (
                                        <span className="text-xs italic text-error font-medium">Hoàn tiền {don.hoanTien}</span>
                                    ) : (
                                        <Link
                                            to={`/danh-gia/${don.maSanChiTiet}`}
                                            className="px-4 py-2 rounded-full bg-primary text-white font-bold hover:bg-primary-dim transition-all shadow-md active:scale-95 text-xs flex items-center gap-1"
                                        >
                                            <span className="material-symbols-outlined text-[14px]">rate_review</span>
                                            Đánh giá
                                        </Link>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>

                </div>
            </main>
            <Footer />
        </div>
    );
};

export default LichSuDatSanPage;