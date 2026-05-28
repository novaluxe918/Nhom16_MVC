const Footer = () => {
    return (
        <footer className="bg-[#f5f7f5] border-t border-gray-200 pt-16 pb-8 px-10 mt-auto">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-12 mb-12">
                <div>
                    <h3 className="font-extrabold text-2xl text-[#006b0a] mb-4">Người Thuê Sân</h3>
                    <p className="text-[#abaeac] text-sm leading-relaxed max-w-xs">
                        Nền tảng kết nối đam mê bóng đá hàng đầu tại Đà Nẵng. Tìm sân, đặt lịch và tận hưởng trận đấu.
                    </p>
                </div>
                <div>
                    <h4 className="font-bold text-[#2c2f2e] mb-4">LIÊN KẾT</h4>
                    <ul className="text-[#abaeac] text-sm space-y-3 font-medium">
                        <li><a href="#" className="hover:text-[#006b0a]">Chính sách bảo mật</a></li>
                        <li><a href="#" className="hover:text-[#006b0a]">Điều khoản sử dụng</a></li>
                        <li><a href="#" className="hover:text-[#006b0a]">Liên hệ: 09xx.xxx.xxx</a></li>
                    </ul>
                </div>
                <div>
                    <h4 className="font-bold text-[#2c2f2e] mb-4">BẢN TIN</h4>
                    <p className="text-[#abaeac] text-sm mb-4">Đăng ký để nhận ưu đãi từ các sân bóng.</p>
                    <div className="flex">
                        <input
                            type="email"
                            placeholder="Email của bạn"
                            className="px-4 py-2 w-full rounded-l-lg border border-gray-300 outline-none focus:border-[#006b0a] text-sm"
                        />
                        <button className="bg-[#006b0a] text-white px-4 py-2 rounded-r-lg hover:bg-[#59ee50] hover:text-[#006b0a] transition-colors">
                            Gửi
                        </button>
                    </div>
                </div>
            </div>
            <div className="border-t border-gray-300 pt-8 flex justify-between items-center text-xs text-[#abaeac] font-medium">
                <p>© 2026 Sportsync Da Nang. Bảo lưu mọi quyền.</p>
                <p>Tiếng Việt | VND</p>
            </div>
        </footer>
    );
};

export default Footer;