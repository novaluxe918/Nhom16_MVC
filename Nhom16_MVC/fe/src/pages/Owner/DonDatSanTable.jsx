const DonDatSanTable = ({ data }) => {
    return (
        <div className="bg-white rounded-xl shadow overflow-hidden">
            <table className="w-full text-sm">
                
                <thead className="bg-gray-100">
                    <tr>
                        <th className="p-3 text-left">Mã đơn</th>
                        <th className="text-left">Khách hàng</th>
                        <th className="text-left">Sân con</th>
                        <th className="text-left">Ngày đặt</th>
                        <th className="text-left">Thời gian</th>
                        <th className="text-left">Trạng thái</th>
                    </tr>
                </thead>

                <tbody>
                    {data?.length > 0 ? (
                        data.map((item) => (
                            <tr key={item.maDatSan} className="border-t hover:bg-gray-50">

                                <td className="p-3 font-medium">
                                    {item.maDatSan}
                                </td>

                                <td>
                                    {item.tenNguoiDat}
                                </td>

                                <td>
                                    {item.tenSanCon}
                                </td>

                                <td>
                                    {new Date(item.ngayDat).toLocaleDateString("vi-VN")}
                                </td>

                                <td>
                                    {new Date(item.gioBatDau).toLocaleTimeString("vi-VN", {
                                        hour: "2-digit",
                                        minute: "2-digit"
                                    })}{" "}
                                    -{" "}
                                    {new Date(item.gioKetThuc).toLocaleTimeString("vi-VN", {
                                        hour: "2-digit",
                                        minute: "2-digit"
                                    })}
                                </td>

                                <td>
                                    <span
                                        className={`px-2 py-1 rounded text-xs font-medium
                                        ${item.trangThai === "hoan_thanh"
                                            ? "bg-green-100 text-green-700"
                                            : item.trangThai === "da_huy"
                                            ? "bg-red-100 text-red-700"
                                            : item.trangThai === "da_xac_nhan"
                                            ? "bg-blue-100 text-blue-700"
                                            : "bg-yellow-100 text-yellow-700"
                                        }`}
                                    >
                                        {item.trangThai}
                                    </span>
                                </td>

                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="6" className="text-center p-5 text-gray-500">
                                Không có dữ liệu
                            </td>
                        </tr>
                    )}
                </tbody>

            </table>
        </div>
    );
};

export default DonDatSanTable;