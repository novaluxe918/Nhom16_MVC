import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import Header from '../components/Header';
import Footer from '../components/Footer';

const ChiTietSanPage = () => {
    const { id } = useParams();
    const navigate = useNavigate();

    const SAN_CON_URL = "https://localhost:7295/images/SanCon/";
    const resolveImageUrl = (imagePath, folderUrl) => {
        if (!imagePath) return "https://placehold.co/600x400/e2e8f0/a0aec0?text=No+Image";
        if (imagePath.includes("link.com")) return `${folderUrl}${imagePath.split('/').pop()}`;
        if (!imagePath.includes('.')) return `${folderUrl}${imagePath}.jpg`;
        return `${folderUrl}${imagePath}`;
    };

    // 1. Quản lý State
    const [sanCon, setSanCon] = useState(null);
    const [danhGia, setDanhGia] = useState([]);
    const [lichTrong, setLichTrong] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isBooking, setIsBooking] = useState(false);

    // 🔥 Thay thế hàm cũ bằng hàm đã dọn sạch biến thừa này:
    const getLocalISOString = (date) => {
        const pad = (num) => String(num).padStart(2, '0');

        return date.getFullYear() +
            '-' + pad(date.getMonth() + 1) +
            '-' + pad(date.getDate());
    };

    // 2. Xử lý tạo danh sách 7 ngày chuẩn theo giờ máy tính người dùng
    const generateInitialDates = () => {
        const dates = [];
        const today = new Date();
        for (let i = 0; i < 7; i++) {
            const d = new Date(today);
            d.setDate(today.getDate() + i);

            // Sử dụng hàm local thay vì .toISOString() để tránh lệch ngày
            const localDateStr = getLocalISOString(d);
            const dayName = i === 0 ? 'HÔM NAY' : `T${d.getDay() === 0 ? 'CN' : d.getDay() + 1}`;

            dates.push({
                iso: localDateStr,
                dayStr: dayName,
                dateStr: d.getDate()
            });
        }
        return dates;
    };

    const [datesList] = useState(generateInitialDates);
    const [selectedDate, setSelectedDate] = useState(() => generateInitialDates()[0].iso);
    const [selectedSlots, setSelectedSlots] = useState([]);

    // 3. Lấy thông tin cơ bản & Đánh giá khi vào trang
    useEffect(() => {
        const fetchData = async () => {
            try {
                const [resThongTin, resDanhGia] = await Promise.all([
                    axios.get(`https://localhost:7295/api/SanBongChiTiet/${id}`),
                    axios.get(`https://localhost:7295/api/DanhGia/san-con/${id}?page=1`)
                ]);

                if (resThongTin.data.success) setSanCon(resThongTin.data.data);
                if (resDanhGia.data.success) setDanhGia(resDanhGia.data.data.listBinhLuan);
            } catch (error) {
                console.error("Lỗi lấy dữ liệu sân con:", error);
            } finally {
                setIsLoading(false);
            }
        };
        fetchData();
    }, [id]);

    // 4. Lấy lịch trống mỗi khi đổi Ngày
    useEffect(() => {
        const fetchLichTrong = async () => {
            if (!selectedDate) return;
            try {
                const response = await axios.get(`https://localhost:7295/api/SanBongChiTiet/${id}/lich-trong?ngay=${selectedDate}`);
                if (response.data.success) {
                    setLichTrong(response.data.data.slots);
                    setSelectedSlots([]);
                }
            } catch (error) {
                console.error("Lỗi lấy lịch trống:", error);
                setLichTrong([]);
            }
        };
        fetchLichTrong();
    }, [id, selectedDate]);

    // 5. Logic click chọn Slot
    const handleToggleSlot = (slot) => {
        if (!slot.conTrong) return;

        const isExist = selectedSlots.find(s => s.gioBatDau === slot.gioBatDau);
        if (isExist) {
            setSelectedSlots(selectedSlots.filter(s => s.gioBatDau !== slot.gioBatDau));
        } else {
            setSelectedSlots([...selectedSlots, slot]);
        }
    };

    // Hàm xử lý đổi ngày từ ô Calendar trực quan
    const handleCustomDateChange = (e) => {
        const dateVal = e.target.value;
        if (!dateVal) return;

        const todayStr = getLocalISOString(new Date());
        if (dateVal < todayStr) {
            alert("Không thể chọn đặt sân cho ngày trong quá khứ!");
            return;
        }
        setSelectedDate(dateVal);
    };

    // 6. Tính tổng tiền tự động
    const calculateTotal = () => {
        if (!sanCon) return 0;
        return selectedSlots.reduce((total, slot) => {
            const hour = parseInt(slot.gioBatDau.split(':')[0]);
            const price = (hour >= 5 && hour < 18) ? sanCon.giaBuoiSang : sanCon.giaBuoiToi;
            return total + price;
        }, 0);
    };

    // 7. Gọi API Đặt Sân
    const handleBooking = async () => {
        if (selectedSlots.length === 0) {
            alert("Vui lòng chọn ít nhất một khung giờ để đặt sân!");
            return;
        }

        const payload = {
            danhSachSlotDat: selectedSlots.map(slot => ({
                ngay: selectedDate,
                gioBatDau: slot.gioBatDau,
                gioKetThuc: slot.gioKetThuc
            }))
        };

        const token = localStorage.getItem('token');
        if (!token) {
            alert("Vui lòng đăng nhập để đặt sân!");
            navigate('/login');
            return;
        }

        setIsBooking(true);
        try {
            const response = await axios.post(`https://localhost:7295/api/SanBongChiTiet/${id}/dat-lich`, payload, {
                headers: { Authorization: `Bearer ${token}` }
            });

            if (response.data.success) {
                alert("🎉 Đặt sân thành công! Tiền đã được trừ vào ví của bạn.");
                navigate('/profile');
            }
        } catch (error) {
            console.error("Lỗi đặt sân:", error);
            if (error.response && error.response.data && error.response.data.message) {
                alert("❌ Lỗi: " + error.response.data.message);
            } else {
                alert("Đã xảy ra sự cố ngoài ý muốn. Vui lòng thử lại sau.");
            }
        } finally {
            setIsBooking(false);
        }
    };

    if (isLoading) return <div className="min-h-screen flex items-center justify-center font-bold text-primary">Đang tải dữ liệu...</div>;
    if (!sanCon) return <div className="min-h-screen flex items-center justify-center font-bold text-error">Không tìm thấy thông tin sân.</div>;

    // Kiểm tra xem ngày đang chọn có nằm ngoài danh sách 7 ngày lướt nhanh không
    const isCustomDateSelected = !datesList.some(d => d.iso === selectedDate);

    return (
        <div className="bg-surface text-on-surface selection:bg-primary-container min-h-screen flex flex-col font-body">
            <Header />

            <main className="pt-12 pb-20 max-w-7xl mx-auto px-4 md:px-8 w-full flex-grow">

                {/* ================= HERO & GALLERY ================= */}
                <section className="grid grid-cols-1 lg:grid-cols-12 gap-6 mb-12">
                    <div className="lg:col-span-8 group relative overflow-hidden rounded-xl h-[400px] md:h-[500px]">
                        <img
                            src={resolveImageUrl(sanCon.albumMediaSanCon?.[0], SAN_CON_URL)}
                            alt={sanCon.tenSanChiTiet}
                            className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
                            onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/800x500?text=San+Con+1"; }}
                        />
                        <div className="absolute top-6 left-6 flex gap-2">
                            <span className="bg-white/90 backdrop-blur text-primary px-3 py-1 rounded-full text-xs font-bold flex items-center gap-1 shadow-lg">
                                ⭐ {sanCon.diemTrungBinh} ({sanCon.tongSoBinhLuan} reviews)
                            </span>
                        </div>
                    </div>
                    <div className="lg:col-span-4 grid grid-cols-2 lg:grid-cols-1 gap-4">
                        <div className="rounded-xl overflow-hidden h-[192px] md:h-full lg:h-[242px]">
                            <img
                                src={resolveImageUrl(sanCon.albumMediaSanCon?.[1], SAN_CON_URL)}
                                className="w-full h-full object-cover"
                                alt="Ảnh sân 2"
                                onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/400x300?text=San+Con+2"; }}
                            />
                        </div>
                        <div className="rounded-xl overflow-hidden h-[192px] md:h-full lg:h-[242px] relative">
                            <img
                                src={resolveImageUrl(sanCon.albumMediaSanCon?.[2], SAN_CON_URL)}
                                className="w-full h-full object-cover"
                                alt="Ảnh sân 3"
                                onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/400x300?text=San+Con+3"; }}
                            />
                        </div>
                    </div>
                </section>

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                    {/* ================= LEFT COLUMN ================= */}
                    <div className="lg:col-span-2 space-y-12">

                        <div className="space-y-4">
                            <nav aria-label="Breadcrumb" className="flex mb-2 text-sm font-medium text-on-surface-variant gap-1">
                                Thuộc cụm sân: <span className="text-primary font-bold">{sanCon.tenSanMenge}</span>
                            </nav>
                            <h1 className="text-4xl md:text-5xl font-black tracking-tight">{sanCon.tenSanChiTiet}</h1>
                            <p className="flex items-center gap-2 text-on-surface-variant font-medium">📍 {sanCon.diaChiMenge}</p>
                        </div>

                        {/* Price Card */}
                        <div className="bg-surface-container-lowest rounded-3xl p-8 shadow-sm border border-outline-variant/10 grid grid-cols-1 md:grid-cols-2 gap-12">
                            <div className="space-y-8">
                                <div>
                                    <span className="text-xs uppercase tracking-wider font-bold text-outline">Quy mô sân</span>
                                    <div className="flex items-end gap-2 mt-2">
                                        <span className="text-body-lg text-on-surface-variant pb-1 font-bold">{sanCon.loaiSan}</span>
                                    </div>
                                </div>
                                <div>
                                    <span className="text-xs uppercase tracking-wider font-bold text-outline">Giờ hoạt động</span>
                                    <p className="text-xl font-bold mt-2">{sanCon.gioHoatDong}</p>
                                </div>
                            </div>
                            <div className="bg-surface-container-low rounded-2xl p-6 space-y-6">
                                <div className="flex justify-between items-center">
                                    <span className="font-bold">Giá ban ngày</span>
                                    <span className="text-2xl font-black text-primary">{sanCon.giaBuoiSang?.toLocaleString()}đ<small className="text-xs font-normal ml-1">/giờ</small></span>
                                </div>
                                <div className="w-full h-px bg-outline-variant/15"></div>
                                <div className="flex justify-between items-center">
                                    <span className="font-bold">Giá đèn đêm</span>
                                    <span className="text-2xl font-black text-primary">{sanCon.giaBuoiToi?.toLocaleString()}đ<small className="text-xs font-normal ml-1">/giờ</small></span>
                                </div>
                                <p className="text-[10px] text-on-surface-variant text-center">* Giá đèn đêm áp dụng từ 18:00</p>
                            </div>
                        </div>

                        {/* Booking Calendar */}
                        <section className="space-y-8">
                            <div className="flex flex-col md:flex-row md:items-end justify-between gap-4">
                                <div>
                                    <h3 className="text-2xl font-bold">Lịch đặt sân</h3>
                                    <p className="text-on-surface-variant">Chọn ngày và khung giờ thi đấu</p>
                                </div>
                                <div className="flex items-center gap-6 text-sm font-medium">
                                    <div className="flex items-center gap-2"><div className="w-4 h-4 rounded bg-primary-container"></div> Trống</div>
                                    <div className="flex items-center gap-2"><div className="w-4 h-4 rounded bg-surface-container-highest"></div> Đã đặt / Quá hạn</div>
                                    <div className="flex items-center gap-2"><div className="w-4 h-4 rounded bg-primary border-2 border-white"></div> Đang chọn</div>
                                </div>
                            </div>

                            {/* Date Scroll - ĐÃ NÂNG CẤP GIAO DIỆN CHỌN LỊCH TRỰC QUAN */}
                            <div className="flex gap-4 overflow-x-auto pb-2 items-center">

                                {/* 📅 NÚT CHỌN LỊCH TỰ DO: Hiển thị nổi bật, click vào ô nào cũng bung lịch */}
                                <div className={`flex-shrink-0 relative w-28 py-3 rounded-2xl flex flex-col items-center justify-center transition-all border cursor-pointer shadow-sm
                                    ${isCustomDateSelected
                                        ? 'bg-primary text-white border-primary shadow-lg'
                                        : 'bg-white border-outline-variant/40 text-on-surface hover:bg-surface-container-high'}`}
                                >
                                    <span className="text-[10px] uppercase font-bold opacity-80 mb-0.5">Chọn lịch</span>
                                    <div className="flex items-center gap-1">
                                        <span className="text-xl">📅</span>
                                        <span className="text-xs font-bold">{isCustomDateSelected ? selectedDate.split('-')[2] + '/' + selectedDate.split('-')[1] : 'Tùy chọn'}</span>
                                    </div>
                                    <input
                                        type="date"
                                        min={getLocalISOString(new Date())}
                                        onChange={handleCustomDateChange}
                                        className="absolute inset-0 opacity-0 cursor-pointer w-full h-full"
                                    />
                                </div>

                                {datesList.map((d, index) => (
                                    <button
                                        key={index}
                                        type="button"
                                        onClick={() => setSelectedDate(d.iso)}
                                        className={`flex-shrink-0 w-20 py-4 rounded-2xl flex flex-col items-center justify-center transition-all border ${selectedDate === d.iso ? 'bg-primary text-white shadow-lg border-primary font-bold' : 'bg-surface-container-lowest text-on-surface border-transparent hover:bg-surface-container-high'}`}
                                    >
                                        <span className={`text-xs uppercase font-bold ${selectedDate === d.iso ? 'opacity-80' : 'text-on-surface-variant'}`}>{d.dayStr}</span>
                                        <span className="text-2xl font-black">{d.dateStr}</span>
                                    </button>
                                ))}
                            </div>

                            {/* Time Slots Grid */}
                            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                                {lichTrong.length === 0 ? (
                                    <div className="col-span-4 text-center py-6 text-on-surface-variant italic">Không có dữ liệu lịch trống cho ngày này.</div>
                                ) : (
                                    lichTrong.map((slot, index) => {
                                        const isSelected = selectedSlots.some(s => s.gioBatDau === slot.gioBatDau);
                                        return (
                                            <button
                                                key={index}
                                                type="button"
                                                onClick={() => handleToggleSlot(slot)}
                                                disabled={!slot.conTrong}
                                                className={`p-4 rounded-2xl text-center font-bold transition-all
                                                    ${!slot.conTrong ? 'bg-surface-container-highest text-on-surface-variant cursor-not-allowed opacity-40' :
                                                        isSelected ? 'bg-primary text-white shadow-md ring-4 ring-primary-container scale-[1.02]' :
                                                            'bg-primary-container text-on-primary-container hover:scale-[1.02] active:scale-95'
                                                    }
                                                `}
                                            >
                                                {slot.gioBatDau} - {slot.gioKetThuc}
                                            </button>
                                        );
                                    })
                                )}
                            </div>
                        </section>

                        {/* Reviews */}
                        <section className="pt-12 border-t border-outline-variant/15 space-y-10">
                            <h3 className="text-3xl font-black text-on-surface mb-6">Đánh giá từ người dùng</h3>
                            <div className="space-y-4">
                                {danhGia.length === 0 ? (
                                    <p className="text-on-surface-variant italic">Chưa có đánh giá nào cho sân này.</p>
                                ) : (
                                    danhGia.map((dg) => (
                                        <div key={dg.maDanhGia} className="bg-surface-container-lowest rounded-2xl p-6 shadow-sm border border-outline-variant/10 space-y-4">
                                            <div className="flex justify-between items-start">
                                                <div className="flex gap-4 items-center">
                                                    <div className="w-12 h-12 rounded-full bg-primary-container flex items-center justify-center text-primary font-bold text-xl uppercase">
                                                        {dg.tenNguoiDung.charAt(0)}
                                                    </div>
                                                    <div>
                                                        <h5 className="font-bold">{dg.tenNguoiDung}</h5>
                                                        <div className="flex items-center gap-2">
                                                            <span className="text-xs text-on-surface-variant">{dg.thoiGian}</span>
                                                            <span className="text-xs font-bold text-primary">⭐ {dg.diemSo}.0</span>
                                                        </div>
                                                    </div>
                                                </div>
                                                <span className="bg-primary/10 text-primary px-2 py-1 rounded-md text-[10px] font-bold">ĐÃ THUÊ SÂN</span>
                                            </div>
                                            <p className="text-on-surface-variant leading-relaxed text-sm">{dg.binhLuan}</p>
                                        </div>
                                    ))
                                )}
                            </div>
                        </section>
                    </div>

                    {/* ================= RIGHT COLUMN ================= */}
                    <div className="lg:col-span-1">
                        <div className="sticky top-28 space-y-4">
                            <div className="bg-surface-container-lowest rounded-3xl p-6 shadow-xl border border-outline-variant/10">
                                <h4 className="text-xl font-bold mb-6">Tóm tắt đơn đặt</h4>

                                <div className="space-y-4">
                                    <div className="flex items-center justify-between mb-2">
                                        <span className="text-xs font-bold text-outline uppercase tracking-wider">Khung giờ đã chọn ({selectedSlots.length})</span>
                                    </div>

                                    <div className="max-h-60 overflow-y-auto space-y-3 hide-scrollbar">
                                        {selectedSlots.length === 0 ? (
                                            <p className="text-sm text-on-surface-variant italic text-center py-4">Chưa có khung giờ nào được chọn.</p>
                                        ) : (
                                            selectedSlots.map((slot, idx) => {
                                                const hour = parseInt(slot.gioBatDau.split(':')[0]);
                                                const price = (hour >= 5 && hour < 18) ? sanCon.giaBuoiSang : sanCon.giaBuoiToi;
                                                return (
                                                    <div key={idx} className="bg-surface-container-low rounded-2xl p-4 border border-outline-variant/10 relative group">
                                                        <button
                                                            onClick={() => handleToggleSlot(slot)}
                                                            className="absolute -top-2 -right-2 w-6 h-6 bg-error text-white rounded-full flex items-center justify-center shadow-lg font-bold"
                                                        >X</button>
                                                        <div className="flex flex-col gap-1">
                                                            <div className="flex justify-between items-start">
                                                                <span className="font-black text-sm">{sanCon.tenSanChiTiet}</span>
                                                                <span className="text-primary font-black text-sm">{(price / 1000)}k</span>
                                                            </div>
                                                            <div className="text-xs text-on-surface-variant">📅 {selectedDate} | ⏱️ {slot.gioBatDau} - {slot.gioKetThuc}</div>
                                                        </div>
                                                    </div>
                                                )
                                            })
                                        )}
                                    </div>

                                    <div className="w-full h-px bg-outline-variant/15 my-4"></div>

                                    <div className="flex justify-between items-center px-1">
                                        <span className="text-lg font-bold">Tổng cộng</span>
                                        <span className="text-2xl font-black text-primary">{calculateTotal().toLocaleString()}đ</span>
                                    </div>

                                    <div className="space-y-3 pt-4">
                                        <button
                                            onClick={handleBooking}
                                            disabled={isBooking || selectedSlots.length === 0}
                                            className={`w-full py-4 rounded-2xl font-black text-lg tracking-tight transition-all uppercase 
                                                ${selectedSlots.length === 0 ? 'bg-surface-container-highest text-on-surface-variant cursor-not-allowed' :
                                                    isBooking ? 'bg-primary-dim text-white opacity-80 cursor-wait' : 'bg-primary text-white shadow-lg hover:-translate-y-1 active:scale-95'}`}
                                        >
                                            {isBooking ? 'ĐANG XỬ LÝ...' : 'TIẾN HÀNH THANH TOÁN'}
                                        </button>

                                        {/* Nút danh sách chờ hiển thị tĩnh theo mong muốn */}
                                        <button
                                            type="button"
                                            onClick={() => alert("Tính năng danh sách chờ đang được phát triển phục vụ cho việc đặt nhiều sân cùng lúc!")}
                                            className="w-full py-4 border-2 border-primary text-primary bg-white hover:bg-primary/5 transition-all font-black text-lg rounded-2xl flex items-center justify-center gap-2 active:scale-95"
                                        >
                                            ⏳ Thêm vào danh sách chờ
                                        </button>
                                    </div>
                                    <p className="text-center text-[10px] text-on-surface-variant mt-4 leading-relaxed">
                                        Hệ thống sẽ tự động trừ tiền trong Ví Điện Tử của bạn.
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
            </main>
            <Footer />
        </div>
    );
};

export default ChiTietSanPage;