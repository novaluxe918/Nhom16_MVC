import { useNavigate } from "react-router-dom";

const SanBongCard = ({ item }) => {
    const navigate = useNavigate();
    return (
        <div className="bg-white rounded-2xl overflow-hidden shadow flex flex-col md:flex-row">
            <div className="w-full md:w-52 h-52">
                <img
                    src={
                        item.hinhanh
                            ? `http://localhost:5014/uploads/${item.hinhanh}`
                            : "https://images.unsplash.com/photo-1518604666860-9ed391f76460?q=80&w=1200&auto=format&fit=crop"
                    }
                    alt={item.tensan}
                    className="w-full h-full object-cover"
                />
            </div>

            <div className="flex-1 p-8 flex flex-col justify-between">
                <div>
                    <div className="flex justify-between">
                        <div>
                            <h4 className="text-3xl font-black">
                                {item.tensan}
                            </h4>

                            <div className="flex items-center gap-2 mt-2">
                                <span
                                    className={`w-3 h-3 rounded-full ${item.daduyet
                                        ? "bg-green-500"
                                        : "bg-red-500"
                                        }`}
                                ></span>

                                <span
                                    className={`font-bold ${item.daduyet
                                        ? "text-green-600"
                                        : "text-red-500"
                                        }`}
                                >
                                    {item.daduyet
                                        ? "Đã duyệt"
                                        : "Chưa duyệt"}
                                </span>
                            </div>
                        </div>

                        <div className="text-right">
                            <p className="text-sm text-gray-500 uppercase">
                                Thành phố
                            </p>

                            <p className="font-bold text-green-700">
                                {item.thanhpho}
                            </p>
                        </div>
                    </div>

                    <div className="mt-5 space-y-2">
                        <p>
                            <span className="font-bold">
                                Địa chỉ:
                            </span>{" "}
                            {item.diachi}
                        </p>

                        <p>
                            <span className="font-bold">
                                Quận/Huyện:
                            </span>{" "}
                            {item.quan} - {item.huyen}
                        </p>

                        <p>
                            <span className="font-bold">
                                Giờ mở cửa:
                            </span>{" "}
                            {item.giomocua}
                        </p>

                        <p>
                            <span className="font-bold">
                                Giờ đóng cửa:
                            </span>{" "}
                            {item.giodongcua}
                        </p>
                    </div>
                </div>

                <div className="flex gap-3 mt-8">
                    <button onClick={() => navigate("/edit")} className="flex-1 bg-gray-200 hover:bg-gray-300 py-3 rounded-xl font-bold">
                        Chỉnh sửa
                    </button>

                    <button className="flex-1 bg-green-600 hover:bg-green-700 text-white py-3 rounded-xl font-bold">
                        Chi tiết
                    </button>
                </div>
            </div>
        </div>
    );
};

export default SanBongCard;