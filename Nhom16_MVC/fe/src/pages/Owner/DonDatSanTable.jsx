const DonDatSanTable = ({ data }) => {
    return (
        <div className="bg-white rounded-xl shadow overflow-hidden">
            <table className="w-full text-sm">
                <thead className="bg-gray-100">
                    <tr>
                        <th className="p-3">Mã đơn</th>
                        <th>Khách hàng</th>
                        <th>Sân</th>
                        <th>Thời gian</th>
                        <th>Tiền</th>
                        <th>Trạng thái</th>
                    </tr>
                </thead>

                <tbody>
                    {data?.map((item) => (
                        <tr key={item.madatsan} className="border-t">
                            <td className="p-3">{item.madatsan}</td>

                            <td>
                                {item.tenkhachhang || item.nguoithueNavigation?.hoten}
                            </td>

                            <td>
                                {item.tensancon}
                            </td>

                            <td>
                                {item.ngaydat}
                            </td>

                            <td>
                                {item.sotienthanhtoan}
                            </td>

                            <td>
                                {item.trangthai}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default DonDatSanTable;