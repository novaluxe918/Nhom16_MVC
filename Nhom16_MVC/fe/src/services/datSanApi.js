import axios from "axios";

const API_URL = "http://localhost:5014/api/chu-san";

/**
 * Lấy lịch đặt sân theo chủ sân + filter
 */
export const getLichDat = async (chuSanId, filter) => {
    return await axios.get(`${API_URL}/lich-dat-san`, {
        params: {
            chuSanId: chuSanId,
            tuNgay: filter.tuNgay || null,
            denNgay: filter.denNgay || null,
            trangThai: filter.trangThai || null,
            maSanChiTiet: filter.maSanChiTiet || null
        }
    });
};