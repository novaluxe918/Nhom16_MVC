import axios from "axios";

const API = "http://localhost:5014/api/sanbong";

export const getAllSanBong = async () => {
    return await axios.get(API);
};