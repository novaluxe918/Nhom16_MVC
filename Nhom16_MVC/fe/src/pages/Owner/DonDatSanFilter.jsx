const DonDatSanFilter = ({ filter, setFilter, listSanCon = [] }) => {
    return (
        <div className="grid grid-cols-3 gap-4 mb-6">

            {/* sân con dynamic */}
            <select
                className="border p-2 rounded"
                value={filter.sanConId || ""}
                onChange={(e) =>
                    setFilter({ ...filter, sanConId: e.target.value })
                }
            >
                <option value="">Tất cả sân con</option>

                {listSanCon.map((item) => (
                    <option key={item.masanchitiet} value={item.masanchitiet}>
                        {item.tensanchitiet}
                    </option>
                ))}
            </select>

            {/* ngày */}
            <input
                type="date"
                className="border p-2 rounded"
                value={filter.ngay || ""}
                onChange={(e) =>
                    setFilter({ ...filter, ngay: e.target.value })
                }
            />

            {/* trạng thái */}
            <select
                className="border p-2 rounded"
                value={filter.trangThai || ""}
                onChange={(e) =>
                    setFilter({ ...filter, trangThai: e.target.value })
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