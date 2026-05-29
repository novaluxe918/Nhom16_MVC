// src/pages/ThemSanBong.jsx

import { useState, useEffect } from "react";
import axios from "axios";

const API = "http://localhost:5014/api/sanbong";

// dữ liệu giả lập từ database
// sau này có thể gọi API lấy danh sách địa chỉ
const quanHuyenData = {
    "Hải Châu": [
        "Hòa Cường Bắc",
        "Hòa Cường Nam",
        "Bình Hiên",
        "Bình Thuận",
    ],

    "Thanh Khê": [
        "An Khê",
        "Tam Thuận",
        "Xuân Hà",
        "Tân Chính",
    ],

    "Liên Chiểu": [
        "Hòa Minh",
        "Hòa Khánh Bắc",
        "Hòa Khánh Nam",
    ],
};

const ThemSanBong = () => {

    const [formData, setFormData] = useState({
        tensan: "",
        chusan: 6,
        mota: "",
        hinhanh: null,
        diachi: "",
        quan: "",
        huyen: "",
        xa: "",
        thanhpho: "Đà Nẵng",
        kinhdo: "",
        vido: "",
        giomocua: "",
        giodongcua: "",
    });

    const [preview, setPreview] = useState("");
    const [loading, setLoading] = useState(false);

    // onchange input
    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    };

    // upload ảnh
    const handleImageChange = (e) => {
        const file = e.target.files[0];

        if (file) {
            setFormData({
                ...formData,
                hinhanh: file,
            });

            setPreview(URL.createObjectURL(file));
        }
    };

    // đổi quận => reset huyện
    const handleQuanChange = (e) => {
        setFormData({
            ...formData,
            quan: e.target.value,
            huyen: "",
        });
    };

    // submit
    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            setLoading(true);

            const data = new FormData();

            data.append("tensan", formData.tensan);
            data.append("mota", formData.mota);
            data.append("diachi", formData.diachi);
            data.append("quan", formData.quan);
            data.append("huyen", formData.huyen);
            data.append("xa", formData.xa);
            data.append("thanhpho", formData.thanhpho);
            data.append("giomocua", formData.giomocua);
            data.append("giodongcua", formData.giodongcua);

            if (formData.kinhdo)
                data.append("kinhdo", formData.kinhdo);

            if (formData.vido)
                data.append("vido", formData.vido);

            if (formData.hinhanh)
                data.append("hinhanh", formData.hinhanh);

            
            await axios.post(`${API}?chusan=6`, data);

            alert("Gửi xét duyệt sân bóng thành công");

            setFormData({
                chusan: 6,
                tensan: "",
                mota: "",
                hinhanh: null,
                diachi: "",
                quan: "",
                huyen: "",
                xa: "",
                thanhpho: "Đà Nẵng",
                kinhdo: "",
                vido: "",
                giomocua: "",
                giodongcua: "",
            });

            setPreview("");

        } catch (error) {
            console.log(error);
            console.log(error.response?.data);
        } finally {
            setLoading(false);
        }

        console.log("CHUSAN =", formData.chusan);
    };

    return (
        <div className="min-h-screen bg-[#f5f7f5] p-10">

            <div className="max-w-5xl mx-auto bg-white rounded-3xl p-10 shadow-[0_12px_32px_rgba(0,107,10,0.06)]">

                {/* header */}
                <div className="mb-10">
                    <p className="uppercase tracking-widest text-green-700 font-bold text-sm">
                        Quản lý sân bóng
                    </p>

                    <h1 className="text-4xl font-black mt-2 text-gray-800">
                        Đăng ký sân bóng mới
                    </h1>
                </div>

                <form
                    onSubmit={handleSubmit}
                    className="space-y-6"
                >

                    {/* tên sân */}
                    <div>
                        <label className="block mb-2 font-bold">
                            Tên sân
                        </label>

                        <input
                            type="text"
                            name="tensan"
                            value={formData.tensan}
                            onChange={handleChange}
                            placeholder="Nhập tên sân"
                            className="w-full p-4 rounded-xl bg-gray-100 outline-none focus:ring-2 focus:ring-green-500"
                            required
                        />
                    </div>

                    {/* mô tả */}
                    <div>
                        <label className="block mb-2 font-bold">
                            Mô tả
                        </label>

                        <textarea
                            name="mota"
                            value={formData.mota}
                            onChange={handleChange}
                            rows="4"
                            placeholder="Mô tả sân bóng..."
                            className="w-full p-4 rounded-xl bg-gray-100 outline-none focus:ring-2 focus:ring-green-500"
                        />
                    </div>

                    {/* upload ảnh */}
                    <div>
                        <label className="block mb-2 font-bold">
                            Hình ảnh sân bóng
                        </label>

                        <input
                            type="file"
                            accept="image/*"
                            onChange={handleImageChange}
                            className="w-full p-4 rounded-xl bg-gray-100"
                        />

                        {preview && (
                            <img
                                src={preview}
                                alt="preview"
                                className="mt-4 w-full h-72 object-cover rounded-2xl"
                            />
                        )}
                    </div>

                    {/* địa chỉ */}
                    <div>
                        <label className="block mb-2 font-bold">
                            Địa chỉ
                        </label>

                        <input
                            type="text"
                            name="diachi"
                            value={formData.diachi}
                            onChange={handleChange}
                            placeholder="123 Nguyễn Văn Linh"
                            className="w-full p-4 rounded-xl bg-gray-100 outline-none focus:ring-2 focus:ring-green-500"
                        />
                    </div>

                    {/* quận huyện */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">

                        {/* quận */}
                        <div>
                            <label className="block mb-2 font-bold">
                                Quận
                            </label>

                            <select
                                name="quan"
                                value={formData.quan}
                                onChange={handleQuanChange}
                                className="w-full p-4 rounded-xl bg-gray-100 outline-none focus:ring-2 focus:ring-green-500"
                            >
                                <option value="">
                                    -- Chọn quận --
                                </option>

                                {Object.keys(quanHuyenData).map((quan) => (
                                    <option
                                        key={quan}
                                        value={quan}
                                    >
                                        {quan}
                                    </option>
                                ))}
                            </select>
                        </div>

                        {/* huyện/phường */}
                        <div>
                            <label className="block mb-2 font-bold">
                                Phường / Huyện
                            </label>

                            <select
                                name="huyen"
                                value={formData.huyen}
                                onChange={handleChange}
                                disabled={!formData.quan}
                                className="w-full p-4 rounded-xl bg-gray-100 outline-none focus:ring-2 focus:ring-green-500 disabled:opacity-50"
                            >
                                <option value="">
                                    -- Chọn --
                                </option>

                                {formData.quan &&
                                    quanHuyenData[formData.quan].map((item) => (
                                        <option
                                            key={item}
                                            value={item}
                                        >
                                            {item}
                                        </option>
                                    ))}
                            </select>
                        </div>

                    </div>

                    {/* xã + thành phố */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">

                        <div>
                            <label className="block mb-2 font-bold">
                                Xã
                            </label>

                            <input
                                type="text"
                                name="xa"
                                value={formData.xa}
                                onChange={handleChange}
                                className="w-full p-4 rounded-xl bg-gray-100 outline-none focus:ring-2 focus:ring-green-500"
                            />
                        </div>

                        <div>
                            <label className="block mb-2 font-bold">
                                Thành phố
                            </label>

                            <input
                                type="text"
                                name="thanhpho"
                                value={formData.thanhpho}
                                onChange={handleChange}
                                className="w-full p-4 rounded-xl bg-gray-100 outline-none focus:ring-2 focus:ring-green-500"
                            />
                        </div>

                    </div>

                    {/* tọa độ */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">

                        <div>
                            <label className="block mb-2 font-bold">
                                Kinh độ
                            </label>

                            <input
                                type="number"
                                step="0.000001"
                                name="kinhdo"
                                value={formData.kinhdo}
                                onChange={handleChange}
                                className="w-full p-4 rounded-xl bg-gray-100 outline-none focus:ring-2 focus:ring-green-500"
                            />
                        </div>

                        <div>
                            <label className="block mb-2 font-bold">
                                Vĩ độ
                            </label>

                            <input
                                type="number"
                                step="0.000001"
                                name="vido"
                                value={formData.vido}
                                onChange={handleChange}
                                className="w-full p-4 rounded-xl bg-gray-100 outline-none focus:ring-2 focus:ring-green-500"
                            />
                        </div>

                    </div>

                    {/* giờ mở cửa */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">

                        <div>
                            <label className="block mb-2 font-bold">
                                Giờ mở cửa
                            </label>

                            <input
                                type="time"
                                name="giomocua"
                                value={formData.giomocua}
                                onChange={handleChange}
                                className="w-full p-4 rounded-xl bg-gray-100 outline-none focus:ring-2 focus:ring-green-500"
                                required
                            />
                        </div>

                        <div>
                            <label className="block mb-2 font-bold">
                                Giờ đóng cửa
                            </label>

                            <input
                                type="time"
                                name="giodongcua"
                                value={formData.giodongcua}
                                onChange={handleChange}
                                className="w-full p-4 rounded-xl bg-gray-100 outline-none focus:ring-2 focus:ring-green-500"
                                required
                            />
                        </div>

                    </div>

                    {/* button */}
                    <button
                        type="submit"
                        disabled={loading}
                        className="w-full bg-gradient-to-r from-green-700 to-green-400 text-white py-4 rounded-xl font-bold hover:scale-[1.01] transition-all"
                    >
                        {loading
                            ? "Đang gửi xét duyệt..."
                            : "Gửi xét duyệt sân bóng"}
                    </button>

                </form>

            </div>

        </div>
    );
};

export default ThemSanBong;