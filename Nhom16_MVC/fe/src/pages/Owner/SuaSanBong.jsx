import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import {
    getSanBongById,
    updateSanBong,
} from "../../services/sanBongService";

const SuaSanBong = () => {
    const { id } = useParams();

    const [formData, setFormData] = useState({
        tensan: "",
        mota: "",
        hinhanh: null,
        diachi: "",
        quan: "",
        huyen: "",
        xa: "",
        thanhpho: "",
        kinhdo: "",
        vido: "",
        giomocua: "",
        giodongcua: "",
    });

    const [preview, setPreview] = useState("");
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const res = await getSanBongById(id);
                const data = res.data;

                setFormData({
                    tensan: data.tensan || "",
                    mota: data.mota || "",
                    diachi: data.diachi || "",
                    quan: data.quan || "",
                    huyen: data.huyen || "",
                    xa: data.xa || "",
                    thanhpho: data.thanhpho || "",
                    kinhdo: data.kinhdo || "",
                    vido: data.vido || "",
                    giomocua: data.giomocua || "",
                    giodongcua: data.giodongcua || "",
                    hinhanh: null,
                });

                setPreview(data.hinhanh || "");
            } catch (err) {
                console.log(err);
            }
        };

        fetchData();
    }, [id]);

    const handleChange = (e) => {
        const { name, value } = e.target;

        setFormData((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    // ================= IMAGE CHANGE =================
    const handleImageChange = (e) => {
        const file = e.target.files?.[0];
        if (!file) return;

        setFormData((prev) => ({
            ...prev,
            hinhanh: file,
        }));

        if (preview) URL.revokeObjectURL(preview);

        setPreview(URL.createObjectURL(file));
    };

  
    const handleSubmit = async (e) => {
        e.preventDefault();

        const data = new FormData();

        Object.entries(formData).forEach(([key, value]) => {
            if (value !== null && value !== "") {
                data.append(key, value);
            }
        });

        try {
            setLoading(true);

            await updateSanBong(id, data);

            alert("Cập nhật sân bóng thành công!");
        } catch (err) {
            console.log(err);
            alert("Cập nhật thất bại!");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-[#f5f7f5] p-10">
            <div className="max-w-5xl mx-auto bg-white rounded-3xl p-10 shadow">

                <h1 className="text-3xl font-black mb-8">
                    Cập nhật sân bóng
                </h1>

                <form onSubmit={handleSubmit} className="space-y-6">

                    <input
                        name="tensan"
                        value={formData.tensan}
                        onChange={handleChange}
                        placeholder="Tên sân"
                        className="w-full p-4 bg-gray-100 rounded-xl"
                    />

                    <textarea
                        name="mota"
                        value={formData.mota}
                        onChange={handleChange}
                        placeholder="Mô tả"
                        className="w-full p-4 bg-gray-100 rounded-xl"
                    />

                    <input
                        type="file"
                        onChange={handleImageChange}
                        className="w-full p-4 bg-gray-100 rounded-xl"
                    />

                    {preview && (
                        <img
                            src={preview}
                            alt="preview"
                            className="w-full h-64 object-cover rounded-xl"
                        />
                    )}

                    <div className="grid grid-cols-2 gap-4">

                        <input
                            name="diachi"
                            value={formData.diachi}
                            onChange={handleChange}
                            placeholder="Địa chỉ"
                            className="p-4 bg-gray-100 rounded-xl"
                        />

                        <input
                            name="quan"
                            value={formData.quan}
                            onChange={handleChange}
                            placeholder="Quận"
                            className="p-4 bg-gray-100 rounded-xl"
                        />

                        <input
                            name="huyen"
                            value={formData.huyen}
                            onChange={handleChange}
                            placeholder="Huyện"
                            className="p-4 bg-gray-100 rounded-xl"
                        />

                        <input
                            name="xa"
                            value={formData.xa}
                            onChange={handleChange}
                            placeholder="Xã"
                            className="p-4 bg-gray-100 rounded-xl"
                        />

                        <input
                            name="thanhpho"
                            value={formData.thanhpho}
                            onChange={handleChange}
                            placeholder="Thành phố"
                            className="p-4 bg-gray-100 rounded-xl"
                        />

                        <input
                            name="kinhdo"
                            value={formData.kinhdo}
                            onChange={handleChange}
                            placeholder="Kinh độ"
                            className="p-4 bg-gray-100 rounded-xl"
                        />

                        <input
                            name="vido"
                            value={formData.vido}
                            onChange={handleChange}
                            placeholder="Vĩ độ"
                            className="p-4 bg-gray-100 rounded-xl"
                        />

                        <input
                            type="time"
                            name="giomocua"
                            value={formData.giomocua}
                            onChange={handleChange}
                            className="p-4 bg-gray-100 rounded-xl"
                        />

                        <input
                            type="time"
                            name="giodongcua"
                            value={formData.giodongcua}
                            onChange={handleChange}
                            className="p-4 bg-gray-100 rounded-xl"
                        />
                    </div>

                    <button
                        type="submit"
                        disabled={loading}
                        className="w-full bg-green-600 text-white py-4 rounded-xl font-bold"
                    >
                        {loading ? "Đang cập nhật..." : "Cập nhật sân bóng"}
                    </button>

                </form>
            </div>
        </div>
    );
};

export default SuaSanBong;