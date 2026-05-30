import { Link } from 'react-router-dom';
import { MapPin } from 'lucide-react';

const FeaturedPitches = ({ sanBongs, isLoading, viewMode }) => {

    const SAN_ME_URL = "https://localhost:7295/images/SanMe/";
    const SAN_CON_URL = "https://localhost:7295/images/SanCon/";
    const resolveImageUrl = (imagePath, folderUrl) => {
        if (!imagePath) return "https://placehold.co/600x400/e2e8f0/a0aec0?text=No+Image";

        // 1. Trị bệnh "Link Fake" (Nếu C# trả về https://link.com/sb1.jpg)
        if (imagePath.includes("link.com")) {
            // Lệnh này cắt chuỗi, chỉ lấy phần đuôi cuối cùng (ra được chữ: sb1.jpg hoặc sbc1.jpg)
            const fileName = imagePath.split('/').pop();
            return `${folderUrl}${fileName}`;
        }

        // 2. Trị bệnh "Thiếu đuôi ảnh" (Nếu C# trả về img_sb1_01 từ cột mediaid)
        if (!imagePath.includes('.')) {
            // Tự động nhét thêm .jpg vào đằng sau
            return `${folderUrl}${imagePath}.jpg`;
        }

        // 3. Dành cho dữ liệu đã chuẩn (Nếu C# trả về img_sb1_01.jpg)
        return `${folderUrl}${imagePath}`;
    };
    return (
        <section className="px-10 mb-10">
            <div className="flex justify-between items-center mb-6 border-b pb-4">
                <div>
                    <p className="text-[11px] font-bold text-[#006b0a] tracking-[0.2em] uppercase mb-1">TÌM KIẾM TẠI ĐÀ NẴNG</p>
                    <h2 className="text-3xl font-extrabold text-[#2c2f2e]">
                        {viewMode === 'PARENT' ? 'Cụm Sân Bóng Phổ Biến' : 'Sân Bóng Còn Trống'}
                    </h2>
                </div>
            </div>

            {isLoading ? (
                <div className="text-center py-12 text-[#abaeac] font-semibold animate-pulse">Đang tải dữ liệu...</div>
            ) : sanBongs.length === 0 ? (
                <div className="text-center py-12 text-[#abaeac] font-semibold bg-white rounded-2xl border border-gray-100">Không tìm thấy kết quả phù hợp.</div>
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                    {sanBongs.map((san) => (

                        viewMode === 'PARENT' ? (
                            // ================= GIAO DIỆN THẺ SÂN MẸ =================
                            <div key={san.maSanBong} className="bg-white rounded-3xl shadow-sm hover:shadow-xl transition-all duration-300 border border-gray-100 overflow-hidden flex flex-col">
                                <div className="relative h-56 bg-gray-200 p-2">
                                    <img
                                        src={resolveImageUrl(san.albumMedia?.[0], SAN_ME_URL)}
                                        alt={san.tenSan}
                                        className="w-full h-full object-cover rounded-2xl"
                                        onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/600x400/e2e8f0/a0aec0?text=San+Me"; }}
                                    />
                                    <div className="absolute bottom-4 right-4 bg-[#006b0a] text-white text-[10px] font-bold px-3 py-1.5 rounded-full shadow-md">
                                        {san.soLuongSanCon} sân con
                                    </div>
                                </div>

                                <div className="p-6 flex-1 flex flex-col justify-between">
                                    <div>
                                        <h3 className="text-xl font-extrabold text-[#2c2f2e] mb-2">{san.tenSan}</h3>
                                        <div className="flex items-center text-sm font-medium text-[#abaeac] mb-4 gap-2">
                                            <MapPin size={16} className="text-[#006b0a]" /> {san.quan}, Đà Nẵng
                                        </div>
                                        {/* Các tag tiện ích giả lập UI */}
                                        <div className="flex gap-2 mb-6">
                                            <span className="bg-gray-100 text-gray-500 text-[10px] font-bold px-3 py-1 rounded">WIFI</span>
                                            <span className="bg-gray-100 text-gray-500 text-[10px] font-bold px-3 py-1 rounded">PARKING</span>
                                            <span className="bg-gray-100 text-gray-500 text-[10px] font-bold px-3 py-1 rounded">CANTEEN</span>
                                        </div>
                                    </div>
                                    <Link to={`/cum-san/${san.maSanBong}`} className="bg-[#006b0a] text-white text-center font-bold py-3 px-6 rounded-xl hover:bg-[#59ee50] hover:text-[#006b0a] transition-all">
                                        Xem chi tiết
                                    </Link>
                                </div>
                            </div>
                        ) : (
                            // ================= GIAO DIỆN THẺ SÂN CON (Code cũ giữ nguyên) =================
                            <div key={san.maSanChiTiet} className="bg-white rounded-3xl shadow-sm hover:shadow-xl transition-all duration-300 border border-gray-100 overflow-hidden flex flex-col">
                                <div className="relative h-56 bg-gray-200">
                                        <img
                                            src={resolveImageUrl(san.hinhAnh, SAN_CON_URL)}
                                            alt={san.tenSan}
                                            className="w-full h-full object-cover"
                                            onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/600x400/e2e8f0/a0aec0?text=San+Con"; }}
                                        />
                                    <div className="absolute top-4 left-4 flex gap-2">
                                        <span className="bg-[#006b0a] text-white text-xs font-bold px-4 py-1.5 rounded-full uppercase">{san.loaiSan}</span>
                                        <span className="bg-white text-[#2c2f2e] text-xs font-bold px-3 py-1.5 rounded-full flex items-center">⭐ {san.soSaoDanhGia}</span>
                                    </div>
                                    <div className="absolute top-14 left-4 bg-green-500 text-white text-[10px] font-bold px-3 py-1 rounded-full uppercase shadow-md">Còn chỗ</div>
                                </div>

                                <div className="p-6 flex-1 flex flex-col justify-between">
                                    <div>
                                        <p className="text-[10px] font-bold text-[#abaeac] uppercase tracking-widest mb-2">CỤM: {san.tenSan}</p>
                                        <h3 className="text-2xl font-extrabold text-[#2c2f2e] mb-3">{san.tenSanChiTiet}</h3>
                                    </div>
                                    <div className="flex justify-between items-end border-t border-gray-100 pt-5">
                                        <div>
                                            <p className="text-xl font-extrabold text-[#2c2f2e]">{san.giaThuebuoiSang?.toLocaleString()}đ</p>
                                        </div>
                                        <Link to={`/san-con/${san.maSanChiTiet}`} className="bg-[#006b0a] text-white font-bold py-2.5 px-6 rounded-xl hover:bg-[#59ee50] hover:text-[#006b0a] transition-all">
                                            Đặt sân ngay
                                        </Link>
                                    </div>
                                </div>
                            </div>
                        )

                    ))}
                </div>
            )}
        </section>
    );
};

export default FeaturedPitches;