import  { useState } from 'react';
import axios from 'axios';
import { Star, Camera, X } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

const DanhGiaSanPage = () => {
    const navigate = useNavigate();

    // State quản lý form
    const [rating, setRating] = useState(0);
    const [hoverRating, setHoverRating] = useState(0);
    const [comment, setComment] = useState('');
    const [selectedImages, setSelectedImages] = useState([]);
    const [imagePreviews, setImagePreviews] = useState([]);
    const [isSubmitting, setIsSubmitting] = useState(false);

    // Mock thông tin đơn hàng đang được đánh giá
    const bookingInfo = {
        tenSan: "Sân bóng Tuyên Sơn",
        ngayDa: "24/05/2026",
        khungGio: "18:00 - 19:30"
    };

    const handleImageUpload = (e) => {
        const files = Array.from(e.target.files);
        if (files.length + selectedImages.length > 3) {
            alert("Bạn chỉ được tải lên tối đa 3 ảnh.");
            return;
        }

        const newPreviews = files.map(file => URL.createObjectURL(file));
        setSelectedImages([...selectedImages, ...files]);
        setImagePreviews([...imagePreviews, ...newPreviews]);
    };

    const removeImage = (indexToRemove) => {
        setSelectedImages(selectedImages.filter((_, idx) => idx !== indexToRemove));
        setImagePreviews(imagePreviews.filter((_, idx) => idx !== indexToRemove));
    };

    const handleSubmit = async () => {
        if (rating === 0) {
            alert("Vui lòng chọn số sao để đánh giá!");
            return;
        }

        setIsSubmitting(true);
        try {
            // Sử dụng FormData để đẩy file vật lý lên Server C#
            const formData = new FormData();
            formData.append("DiemSo", rating);
            formData.append("BinhLuan", comment);
            // Giả lập truyền mã chi tiết đặt sân (cần lấy từ params trên URL thực tế)
            formData.append("MaChiTietDatSan", 1);

            selectedImages.forEach(image => {
                formData.append("HinhAnh", image); // Tên field "HinhAnh" phải khớp với IFormFile bên C#
            });

            const token = localStorage.getItem('token');
            const response = await axios.post('https://localhost:7295/api/DanhGia/gui-danh-gia', formData, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'multipart/form-data'
                }
            });

            if (response.data.success) {
                alert("Cảm ơn bạn đã đánh giá!");
                navigate('/lich-su-dat-san'); // Quay về trang lịch sử
            }
        } catch (error) {
            console.error("Lỗi gửi đánh giá:", error);
            alert("Có lỗi xảy ra khi gửi đánh giá.");
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <div className="bg-[#f5f7f5] min-h-screen flex items-center justify-center font-body text-[#2c2f2e] py-12 px-4">
            <div className="max-w-2xl w-full">

                {/* Header Title */}
                <div className="mb-8">
                    <p className="text-xs font-bold text-[#006b0a] uppercase tracking-widest mb-1">Trải nghiệm của bạn</p>
                    <h1 className="text-2xl md:text-3xl font-black">Người Thuê Sân - Đánh giá sân bóng</h1>
                </div>

                {/* Card Thông tin đơn đặt */}
                <div className="bg-white p-6 rounded-3xl shadow-sm border border-gray-100 flex flex-col md:flex-row items-center md:items-start gap-6 mb-6">
                    <img src="https://images.unsplash.com/photo-1543326727-cf6c39e8f84c?auto=format&fit=crop&w=150&q=80" alt="Avatar User" className="w-24 h-24 rounded-2xl object-cover shadow-inner" />
                    <div>
                        <p className="text-xs font-bold text-gray-400 uppercase tracking-widest mb-1 mt-2">Lịch sử đặt sân</p>
                        <h2 className="text-xl font-black mb-3">{bookingInfo.tenSan}</h2>
                        <div className="flex flex-wrap gap-3">
                            <span className="flex items-center gap-1.5 bg-gray-100 px-3 py-1.5 rounded-lg text-sm font-medium text-gray-600">
                                📅 {bookingInfo.ngayDa}
                            </span>
                            <span className="flex items-center gap-1.5 bg-gray-100 px-3 py-1.5 rounded-lg text-sm font-medium text-gray-600">
                                ⏱️ {bookingInfo.khungGio}
                            </span>
                        </div>
                    </div>
                </div>

                {/* Card Form Đánh giá */}
                <div className="bg-white p-6 md:p-10 rounded-3xl shadow-sm border border-gray-100 space-y-8">

                    {/* Chọn Sao */}
                    <div className="text-center">
                        <p className="font-bold mb-4">Chất lượng tổng thể</p>
                        <div className="flex justify-center gap-2 mb-2">
                            {[1, 2, 3, 4, 5].map((star) => (
                                <button
                                    key={star}
                                    type="button"
                                    onClick={() => setRating(star)}
                                    onMouseEnter={() => setHoverRating(star)}
                                    onMouseLeave={() => setHoverRating(0)}
                                    className="transition-transform hover:scale-110 focus:outline-none"
                                >
                                    <Star
                                        size={40}
                                        className={`transition-colors duration-200 ${(hoverRating || rating) >= star
                                                ? 'fill-yellow-400 text-yellow-400'
                                                : 'text-gray-300'
                                            }`}
                                    />
                                </button>
                            ))}
                        </div>
                        <p className="text-sm text-gray-500 italic">Vui lòng chọn số sao để đánh giá</p>
                    </div>

                    {/* Nhập Comment */}
                    <div>
                        <label className="block text-xs font-bold text-gray-400 uppercase tracking-widest mb-3">Nhận xét chi tiết</label>
                        <textarea
                            rows="4"
                            value={comment}
                            onChange={(e) => setComment(e.target.value)}
                            placeholder="Hãy chia sẻ cảm nhận của bạn về chất lượng mặt sân, ánh sáng và dịch vụ..."
                            className="w-full bg-gray-50 border border-gray-200 rounded-2xl p-4 text-sm focus:ring-2 focus:ring-[#59ee50] focus:border-[#006b0a] outline-none transition resize-none"
                        ></textarea>
                    </div>

                    {/* Upload Ảnh */}
                    <div>
                        <label className="block text-xs font-bold text-gray-400 uppercase tracking-widest mb-3">Tải ảnh thực tế (Tối đa 3)</label>
                        <div className="flex flex-wrap gap-4">
                            {/* Nút thêm ảnh */}
                            {imagePreviews.length < 3 && (
                                <label className="w-24 h-24 flex flex-col items-center justify-center border-2 border-dashed border-gray-300 rounded-2xl cursor-pointer hover:bg-gray-50 hover:border-[#006b0a] transition text-gray-400 hover:text-[#006b0a]">
                                    <Camera size={24} className="mb-1" />
                                    <span className="text-[10px] font-bold">Thêm ảnh</span>
                                    <input
                                        type="file"
                                        multiple
                                        accept="image/jpeg, image/png, image/jpg"
                                        className="hidden"
                                        onChange={handleImageUpload}
                                    />
                                </label>
                            )}

                            {/* Hiển thị ảnh Preview */}
                            {imagePreviews.map((previewUrl, idx) => (
                                <div key={idx} className="relative w-24 h-24">
                                    <img src={previewUrl} alt={`Preview ${idx}`} className="w-full h-full object-cover rounded-2xl shadow-sm border border-gray-100" />
                                    <button
                                        onClick={() => removeImage(idx)}
                                        className="absolute -top-2 -right-2 bg-red-500 text-white p-1 rounded-full hover:bg-red-600 shadow-md"
                                    >
                                        <X size={12} strokeWidth={3} />
                                    </button>
                                </div>
                            ))}
                        </div>
                    </div>

                    {/* Buttons */}
                    <div className="flex gap-4 pt-4 border-t border-gray-100">
                        <button
                            onClick={handleSubmit}
                            disabled={isSubmitting}
                            className={`flex-1 py-3.5 rounded-xl font-bold transition-all shadow-lg ${isSubmitting
                                    ? 'bg-gray-400 cursor-wait'
                                    : 'bg-[#006b0a] hover:bg-[#005d07] shadow-green-600/30 hover:-translate-y-1'
                                } text-white`}
                        >
                            {isSubmitting ? 'ĐANG GỬI...' : 'GỬI ĐÁNH GIÁ'}
                        </button>
                        <button
                            onClick={() => navigate('/lich-su-dat-san')}
                            className="w-32 py-3.5 bg-gray-200 text-gray-700 font-bold rounded-xl hover:bg-gray-300 transition"
                        >
                            Hủy
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default DanhGiaSanPage;