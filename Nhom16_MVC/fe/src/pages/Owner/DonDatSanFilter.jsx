const DonDatSanFilter = ({ filter, setFilter, listSanCon = [] }) => {
    return (
        <div className="grid grid-cols-3 gap-4 mb-6">

            {/* sân con (map từ SANBONGCHITIET) */}
            <select
                className="border p-2 rounded"
                value={filter.maSanChiTiet || ""}
                onChange={(e) =>
                    setFilter({
                        ...filter,
                        maSanChiTiet: e.target.value || ""
                    })
                }
            >
                <option value="">Tất cả sân con</option>

                {listSanCon.map((item) => (
                    <option
                        key={item.masanchitiet}
                        value={item.masanchitiet}
                    >
                        {item.tensanchitiet}
                    </option>
                ))}
            </select>

            {/* từ ngày */}
            <input
                type="date"
                className="border p-2 rounded"
                value={filter.tuNgay || ""}
                onChange={(e) =>
                    setFilter({
                        ...filter,
                        tuNgay: e.target.value
                    })
                }
            />

            {/* đến ngày (QUAN TRỌNG - backend có DenNgay) */}
            <input
                type="date"
                className="border p-2 rounded"
                value={filter.denNgay || ""}
                onChange={(e) =>
                    setFilter({
                        ...filter,
                        denNgay: e.target.value
                    })
                }
            />

            {/* trạng thái (MAP ĐÚNG ENUM DATABASE) */}
            <select
                className="border p-2 rounded"
                value={filter.trangThai || ""}
                onChange={(e) =>
                    setFilter({
                        ...filter,
                        trangThai: e.target.value
                    })
                }
            >
                <option value="">Tất cả trạng thái</option>

                <option value="ChoXacNhan">Chờ xác nhận</option>
                <option value="DaXacNhan">Đã xác nhận</option>
                <option value="DaHuy">Đã hủy</option>
                <option value="HoanThanh">Hoàn thành</option>
            </select>

        </div>
    );
};

export default DonDatSanFilter;