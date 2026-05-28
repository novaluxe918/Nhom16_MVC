const AiBanner = () => {
    return (
        <section className="bg-[#f5f7f5] rounded-3xl mx-10 my-16 flex flex-col md:flex-row items-center justify-between p-12 shadow-sm border border-gray-100">
            <div className="max-w-lg mb-8 md:mb-0">
                <p className="text-xs font-bold text-[#006b0a] tracking-widest uppercase mb-2">Tính năng mới</p>
                <h2 className="text-4xl font-extrabold text-[#2c2f2e] mb-4 leading-tight">Theo dõi trận đấu trực tiếp qua Sportsync AI</h2>
                <p className="text-[#abaeac] font-medium mb-8 leading-relaxed">
                    Chúng tôi cung cấp hệ thống camera AI giúp bạn ghi lại những bàn thắng đẹp nhất và xem lại thông số trận đấu ngay trên ứng dụng.
                </p>
                <button className="bg-[#006b0a] text-white font-bold py-3 px-8 rounded-full hover:bg-[#59ee50] hover:text-[#006b0a] transition-all shadow-md">
                    Khám phá ngay
                </button>
            </div>
            <div className="w-full md:w-1/2 flex justify-end">
                {/* Placeholder cho ảnh Bóng rổ / AI */}
                <div className="w-80 h-80 bg-gray-800 rounded-2xl flex items-center justify-center shadow-2xl relative overflow-hidden">
                    <div className="absolute top-4 text-white/50 text-sm font-mono">AI TRACKING ACTIVE</div>
                    <div className="w-48 h-48 bg-orange-500 rounded-full shadow-inner border-4 border-orange-700 opacity-90"></div>
                </div>
            </div>
        </section>
    );
};

export default AiBanner;