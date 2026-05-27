-- ============================================================
-- ============================================================
-- ============================================================




-- 0. EXTENSION & ENUMS (Tạo trước tiên)
-- ============================================================
-- Bật extension btree_gist để dùng tính năng chặn trùng lặp khoảng thời gian (Vấn đề 2)
CREATE EXTENSION IF NOT EXISTS btree_gist;

-- Sử dụng ENUM thay cho VARCHAR giúp chặt chẽ dữ liệu và tối ưu index (Vấn đề 3)
CREATE TYPE vai_tro_enum   AS ENUM ('nguoiThue', 'chuSan', 'admin');
CREATE TYPE trang_thai_dat AS ENUM ('cho_xac_nhan', 'da_xac_nhan', 'da_huy', 'hoan_thanh');
CREATE TYPE trang_thai_nap AS ENUM ('cho_xu_ly', 'thanh_cong', 'that_bai');
CREATE TYPE trang_thai_rut AS ENUM ('cho_xu_ly', 'da_chuyen', 'that_bai');

-- ============================================================
-- 1. NGUOIDUNG (Phiên bản mới)
-- ============================================================
CREATE TABLE NGUOIDUNG (
    maNguoiDung        SERIAL          PRIMARY KEY,
    hoTen              VARCHAR(150)    NOT NULL,
    avatar             TEXT,                           
    email              VARCHAR(255)    UNIQUE NOT NULL,
    soDienThoai        VARCHAR(15)     UNIQUE,
    matKhau            TEXT            NOT NULL,       
    vaiTro             vai_tro_enum    NOT NULL DEFAULT 'nguoiThue', 
    soDuTaiKhoan       BIGINT          NOT NULL DEFAULT 0, 
    
    -- Các trường bổ sung phục vụ Xác thực và Quên Mật Khẩu
    isEmailVerified    BOOLEAN         DEFAULT FALSE,
    verificationToken  TEXT,
    tokenExpiry        TIMESTAMP,
    resetToken         VARCHAR(255)    DEFAULT NULL,
    resetTokenExpiry   TIMESTAMP       DEFAULT NULL,
    
    createdAt          TIMESTAMP       DEFAULT NOW()
);

CREATE INDEX IX_NguoiDung_Email  ON NGUOIDUNG(email);
CREATE INDEX IX_NguoiDung_VaiTro ON NGUOIDUNG(vaiTro);
-- ============================================================
-- 2. SANBONG
-- ============================================================
CREATE TABLE SANBONG (
    maSanBong   SERIAL        PRIMARY KEY,
    chuSan      INT           NOT NULL REFERENCES NGUOIDUNG(maNguoiDung) ON DELETE RESTRICT,
    tenSan      VARCHAR(255)  NOT NULL,
    moTa        TEXT,
    hinhAnh     TEXT,                         
    daDuyet     BOOLEAN       DEFAULT FALSE,  
    diaChi      TEXT,                         
    quan        VARCHAR(100),
    huyen       VARCHAR(100),
    xa          VARCHAR(100),
    thanhPho    VARCHAR(100)  DEFAULT 'Đà Nẵng',
    kinhDo      DECIMAL(11,8),               
    viDo        DECIMAL(10,8),               
    createdAt   TIMESTAMP     DEFAULT NOW(),
    updatedAt   TIMESTAMP     DEFAULT NOW()   -- Thêm updatedAt (Vấn đề 5)
);

CREATE INDEX IX_SanBong_ChuSan  ON SANBONG(chuSan);
CREATE INDEX IX_SanBong_DaDuyet ON SANBONG(daDuyet);

-- ============================================================
-- 3. LOAISAN & 4. LOAIHINHDAT
-- ============================================================
CREATE TABLE LOAISAN (
    maLoaiSan   SERIAL       PRIMARY KEY,
    tenLoaiSan  VARCHAR(100) NOT NULL    
);

CREATE TABLE LOAIHINHDAT (
    maLoaiDat   SERIAL       PRIMARY KEY,
    tenLoaiDat  VARCHAR(100) NOT NULL    
);

-- ============================================================
-- 5. SANBONGCHITIET
-- ============================================================
CREATE TABLE SANBONGCHITIET (
    maSanChiTiet      SERIAL        PRIMARY KEY,
    maSanBong         INT           NOT NULL REFERENCES SANBONG(maSanBong) ON DELETE CASCADE,
    maLoaiSan         INT           NOT NULL REFERENCES LOAISAN(maLoaiSan) ON DELETE RESTRICT,
    tenSanChiTiet     VARCHAR(255)  NOT NULL,   
    giaThueBuoiSang   BIGINT        NOT NULL DEFAULT 0,  
    giaThueBuoiToi    BIGINT        NOT NULL DEFAULT 0   
);

CREATE INDEX IX_SanChiTiet_SanBong  ON SANBONGCHITIET(maSanBong);
CREATE INDEX IX_SanChiTiet_LoaiSan  ON SANBONGCHITIET(maLoaiSan);

-- ============================================================
-- 6. DATSAN
-- ============================================================
CREATE TABLE DATSAN (
    maDatSan        SERIAL      PRIMARY KEY,
    nguoiThue       INT         NOT NULL REFERENCES NGUOIDUNG(maNguoiDung) ON DELETE RESTRICT,
    ngayDat         DATE        NOT NULL DEFAULT CURRENT_DATE,
    ngayThanhToan   TIMESTAMP,
    soTienThanhToan BIGINT      DEFAULT 0 -- Snapshot giá khi thanh toán (Vấn đề 4)
);

CREATE INDEX IX_DatSan_NguoiThue ON DATSAN(nguoiThue);
CREATE INDEX IX_DatSan_NgayDat   ON DATSAN(ngayDat);

-- ============================================================
-- 7. CHITIETDATSAN
-- ============================================================
CREATE TABLE CHITIETDATSAN (
    maChiTietDatSan SERIAL          PRIMARY KEY,
    maDatSan        INT             NOT NULL REFERENCES DATSAN(maDatSan) ON DELETE CASCADE,
    maSanChiTiet    INT             NOT NULL REFERENCES SANBONGCHITIET(maSanChiTiet) ON DELETE RESTRICT,
    maLoaiDat       INT             REFERENCES LOAIHINHDAT(maLoaiDat) ON DELETE SET NULL,
    gioBatDau       TIMESTAMP       NOT NULL, -- Đã đổi sang TIMESTAMP (Vấn đề 2)
    gioKetThuc      TIMESTAMP       NOT NULL, -- Đã đổi sang TIMESTAMP (Vấn đề 2)
    coVanDe         BOOLEAN         DEFAULT FALSE,         
    trangThaiDatSan trang_thai_dat  DEFAULT 'cho_xac_nhan', -- Đã đổi sang ENUM
    
    -- Chống trùng giờ ở cấp độ Database (Vấn đề 2)
    CONSTRAINT chk_khong_trung_gio EXCLUDE USING GIST (
        maSanChiTiet WITH =,
        tsrange(gioBatDau, gioKetThuc) WITH &&
    ) WHERE (trangThaiDatSan <> 'da_huy')
);

CREATE INDEX IX_ChiTietDatSan_DatSan     ON CHITIETDATSAN(maDatSan);
CREATE INDEX IX_ChiTietDatSan_SanChiTiet ON CHITIETDATSAN(maSanChiTiet);
CREATE INDEX IX_ChiTietDatSan_TrangThai  ON CHITIETDATSAN(trangThaiDatSan);

-- ============================================================
-- 8. DANHGIA
-- ============================================================
CREATE TABLE DANHGIA (
    maDanhGia       SERIAL      PRIMARY KEY,
    maSanBong       INT         NOT NULL REFERENCES SANBONG(maSanBong) ON DELETE CASCADE,
    nguoiThue       INT         NOT NULL REFERENCES NGUOIDUNG(maNguoiDung) ON DELETE CASCADE,
    diemSo          SMALLINT    NOT NULL CHECK (diemSo >= 1 AND diemSo <= 5),
    binhLuan        TEXT,
    thoiGianDanhGia TIMESTAMP   DEFAULT NOW()
);

CREATE INDEX IX_DanhGia_SanBong   ON DANHGIA(maSanBong);
CREATE INDEX IX_DanhGia_NguoiThue ON DANHGIA(nguoiThue);

-- ============================================================
-- 9. CHAT
-- ============================================================
CREATE TABLE CHAT (
    maTinNhan    SERIAL      PRIMARY KEY,
    nguoiGui     INT         NOT NULL REFERENCES NGUOIDUNG(maNguoiDung) ON DELETE CASCADE,
    nguoiNhan    INT         NOT NULL REFERENCES NGUOIDUNG(maNguoiDung) ON DELETE CASCADE,
    noiDung      TEXT        NOT NULL,
    thoiGianGui  TIMESTAMP   DEFAULT NOW(),
    daDoc        BOOLEAN     DEFAULT FALSE, -- Thêm trạng thái đã đọc (Vấn đề 5)

    CONSTRAINT chk_khac_nguoi CHECK (nguoiGui <> nguoiNhan)
);

CREATE INDEX IX_Chat_NguoiGui  ON CHAT(nguoiGui);
CREATE INDEX IX_Chat_NguoiNhan ON CHAT(nguoiNhan);
CREATE INDEX IX_Chat_DaDoc     ON CHAT(nguoiNhan, daDoc);

-- ============================================================
-- 10. NAPTIEN & 11. YEUCAURUTTIEN
-- ============================================================
CREATE TABLE NAPTIEN (
    maNapTien    SERIAL         PRIMARY KEY,
    nguoiNap     INT            NOT NULL REFERENCES NGUOIDUNG(maNguoiDung) ON DELETE CASCADE,
    soTien       BIGINT         NOT NULL CHECK (soTien > 0),
    thoiGianNap  TIMESTAMP      DEFAULT NOW(),
    maGiaoDich   VARCHAR(100)   UNIQUE,           
    phuongThuc   VARCHAR(50)    DEFAULT 'vnpay', -- vnpay / momo / banking (Vấn đề 5)
    trangThai    trang_thai_nap DEFAULT 'cho_xu_ly' -- Đã đổi sang ENUM
);

CREATE INDEX IX_NapTien_NguoiNap  ON NAPTIEN(nguoiNap);
CREATE INDEX IX_NapTien_TrangThai ON NAPTIEN(trangThai);

CREATE TABLE YEUCAURUTTIEN (
    maYeuCau      SERIAL         PRIMARY KEY,
    maNguoiDung   INT            NOT NULL REFERENCES NGUOIDUNG(maNguoiDung) ON DELETE CASCADE,
    soTien        BIGINT         NOT NULL CHECK (soTien > 0),
    thoiGianRut   TIMESTAMP      DEFAULT NOW(),
    maGiaoDich    VARCHAR(100)   UNIQUE,           
    moTa          TEXT,                          
    trangThai     trang_thai_rut DEFAULT 'cho_xu_ly', -- Đã đổi sang ENUM
    tenNganHang   VARCHAR(200)   NOT NULL,         
    soTaiKhoan    VARCHAR(50)    NOT NULL          
);

CREATE INDEX IX_RutTien_NguoiDung ON YEUCAURUTTIEN(maNguoiDung);
CREATE INDEX IX_RutTien_TrangThai ON YEUCAURUTTIEN(trangThai);

-- ============================================================
-- 12. MEDIA_SANBONG & 13. MEDIA_SANBONGCHITIET
-- ============================================================
CREATE TABLE MEDIA_SANBONG (
    maMedia    SERIAL       PRIMARY KEY,
    maSanBong  INT          NOT NULL REFERENCES SANBONG(maSanBong) ON DELETE CASCADE,
    loaiMedia  VARCHAR(20)  NOT NULL CHECK (loaiMedia IN ('hinh_anh', 'video')),
    ten        VARCHAR(255),                    
    link       TEXT         NOT NULL,           
    mediaId    VARCHAR(255)                     
);

CREATE TABLE MEDIA_SANBONGCHITIET (
    maMedia          SERIAL       PRIMARY KEY,
    maSanBongChiTiet INT          NOT NULL REFERENCES SANBONGCHITIET(maSanChiTiet) ON DELETE CASCADE,
    loaiMedia        VARCHAR(20)  NOT NULL CHECK (loaiMedia IN ('hinh_anh', 'video')),
    ten              VARCHAR(255),
    link             TEXT         NOT NULL,
    mediaId          VARCHAR(255)
);

-- ============================================================
-- CÁC TRIGGER & FUNCTION BẢO VỆ TOÀN VẸN DỮ LIỆU
-- ============================================================

-- Tự động cập nhật updatedAt cho SANBONG (Vấn đề 5)
CREATE OR REPLACE FUNCTION fn_update_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updatedAt = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_SanBong_UpdatedAt
BEFORE UPDATE ON SANBONG
FOR EACH ROW EXECUTE FUNCTION fn_update_timestamp();

-- Nạp tiền thành công cộng số dư (Không đổi)
CREATE OR REPLACE FUNCTION fn_cap_nhat_so_du_nap()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.trangThai = 'thanh_cong' AND OLD.trangThai <> 'thanh_cong' THEN
        UPDATE NGUOIDUNG
        SET soDuTaiKhoan = soDuTaiKhoan + NEW.soTien
        WHERE maNguoiDung = NEW.nguoiNap;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_NapTien_CapNhatSoDu
AFTER UPDATE ON NAPTIEN
FOR EACH ROW EXECUTE FUNCTION fn_cap_nhat_so_du_nap();

-- Rút tiền dùng Khóa Hàng (Pessimistic Locking) chống Race Condition (Vấn đề 1)
CREATE OR REPLACE FUNCTION fn_cap_nhat_so_du_rut()
RETURNS TRIGGER AS $$
DECLARE    
    v_sodu BIGINT;
BEGIN    
    IF NEW.trangThai = 'da_chuyen' AND OLD.trangThai <> 'da_chuyen' THEN        
        -- Khóa hàng trước khi đọc, transaction khác phải chờ (Tránh lỗ hổng double-spending)
        SELECT soDuTaiKhoan INTO v_sodu        
        FROM NGUOIDUNG        
        WHERE maNguoiDung = NEW.maNguoiDung        
        FOR UPDATE;        
        
        IF v_sodu < NEW.soTien THEN            
            RAISE EXCEPTION 'Số dư không đủ (hiện có: %, cần rút: %)', v_sodu, NEW.soTien;        
        END IF;        
        
        UPDATE NGUOIDUNG        
        SET soDuTaiKhoan = soDuTaiKhoan - NEW.soTien        
        WHERE maNguoiDung = NEW.maNguoiDung;    
    END IF;    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_RutTien_CapNhatSoDu
AFTER UPDATE ON YEUCAURUTTIEN
FOR EACH ROW EXECUTE FUNCTION fn_cap_nhat_so_du_rut();

-- Tự tính tổng tiền DATSAN khi chi tiết hoàn thành (Vấn đề 4)
CREATE OR REPLACE FUNCTION fn_tinh_tong_tien_datsan()
RETURNS TRIGGER AS $$
BEGIN    
    IF NEW.trangThaiDatSan = 'hoan_thanh' AND OLD.trangThaiDatSan <> 'hoan_thanh' THEN        
        UPDATE DATSAN        
        SET soTienThanhToan = (            
            SELECT SUM(                
                CASE                    
                    WHEN EXTRACT(HOUR FROM ct.gioBatDau) < 12 THEN sbc.giaThueBuoiSang                    
                    ELSE sbc.giaThueBuoiToi                
                END            
            )            
            FROM CHITIETDATSAN ct            
            JOIN SANBONGCHITIET sbc ON ct.maSanChiTiet = sbc.maSanChiTiet            
            WHERE ct.maDatSan = NEW.maDatSan        
        )        
        WHERE maDatSan = NEW.maDatSan;    
    END IF;    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_ChiTietDatSan_TinhTong
AFTER UPDATE ON CHITIETDATSAN
FOR EACH ROW EXECUTE FUNCTION fn_tinh_tong_tien_datsan();

-- Chặn đánh giá ảo: Chỉ người đã hoàn thành đặt sân mới được đánh giá (Vấn đề 6)
CREATE OR REPLACE FUNCTION fn_kiem_tra_quyen_danh_gia()
RETURNS TRIGGER AS $$
DECLARE    
    da_dat BOOLEAN;
BEGIN    
    SELECT EXISTS (        
        SELECT 1        
        FROM DATSAN d        
        JOIN CHITIETDATSAN ct    ON d.maDatSan = ct.maDatSan        
        JOIN SANBONGCHITIET sbc  ON ct.maSanChiTiet = sbc.maSanChiTiet        
        WHERE d.nguoiThue        = NEW.nguoiThue          
          AND sbc.maSanBong      = NEW.maSanBong          
          AND ct.trangThaiDatSan = 'hoan_thanh'    
    ) INTO da_dat;    

    IF NOT da_dat THEN        
        RAISE EXCEPTION 'Bạn cần đặt và hoàn thành ít nhất 1 lần tại sân này mới được đánh giá.';    
    END IF;    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_DanhGia_KiemTraQuyen
BEFORE INSERT ON DANHGIA
FOR EACH ROW EXECUTE FUNCTION fn_kiem_tra_quyen_danh_gia();

-- ============================================================
-- VIEW BÁO CÁO
-- ============================================================
CREATE VIEW SanBongRatingSummary AS
SELECT
    maSanBong,
    ROUND(AVG(diemSo::DECIMAL), 2) AS diemTrungBinh,
    COUNT(*)                        AS tongDanhGia,
    COUNT(*) FILTER (WHERE diemSo = 5) AS so_5_sao,
    COUNT(*) FILTER (WHERE diemSo = 4) AS so_4_sao,
    COUNT(*) FILTER (WHERE diemSo = 3) AS so_3_sao,
    COUNT(*) FILTER (WHERE diemSo = 2) AS so_2_sao,
    COUNT(*) FILTER (WHERE diemSo = 1) AS so_1_sao
FROM DANHGIA
GROUP BY maSanBong;


-- ============================================================
-- DỮ LIỆU MẪU (10 DÒNG MỖI BẢNG)
-- Chú ý: Cần chạy code Create Table trước khi chạy phần này
-- ============================================================

-- 1. NGUOIDUNG (ID từ 1 đến 10: 5 người thuê, 4 chủ sân, 1 admin)
INSERT INTO NGUOIDUNG (hoTen, email, soDienThoai, matKhau, vaiTro, soDuTaiKhoan, isEmailVerified) VALUES
('Nguyễn Văn An',   'an@gmail.com',   '0901000001', 'hash1', 'nguoiThue', 1000000, TRUE),
('Trần Thị Bình',   'binh@gmail.com', '0901000002', 'hash2', 'nguoiThue', 500000,  TRUE),
('Lê Văn Cường',    'cuong@gmail.com','0901000003', 'hash3', 'nguoiThue', 200000,  FALSE),
('Phạm Thị Dung',   'dung@gmail.com', '0901000004', 'hash4', 'nguoiThue', 300000,  TRUE),
('Hoàng Văn Em',    'em@gmail.com',   '0901000005', 'hash5', 'nguoiThue', 150000,  TRUE),
('Vũ Chủ Sân 1',    'chu1@gmail.com', '0902000001', 'hash6', 'chuSan',    5000000, TRUE),
('Đặng Chủ Sân 2',  'chu2@gmail.com', '0902000002', 'hash7', 'chuSan',    4500000, TRUE),
('Bùi Chủ Sân 3',   'chu3@gmail.com', '0902000003', 'hash8', 'chuSan',    6000000, TRUE),
('Đỗ Chủ Sân 4',    'chu4@gmail.com', '0902000004', 'hash9', 'chuSan',    3200000, TRUE),
('Admin Hệ Thống',  'admin@app.com',  '0999999999', 'hash0', 'admin',     0,       TRUE);

-- 2. LOAISAN (10 loại)
INSERT INTO LOAISAN (tenLoaiSan) VALUES 
('Sân 5 người'), ('Sân 7 người'), ('Sân 11 người'), ('Sân Futsal'), ('Sân Trong Nhà'),
('Sân Cỏ Tự Nhiên'), ('Sân Cỏ Nhân Tạo'), ('Sân 9 người'), ('Sân Thi Đấu'), ('Sân Tập Trẻ Em');

-- 3. LOAIHINHDAT (10 loại)
INSERT INTO LOAIHINHDAT (tenLoaiDat) VALUES 
('Đặt theo giờ'), ('Đặt theo buổi'), ('Đặt cả ngày'), ('Đặt cố định tháng'), ('Đặt giải đấu'),
('Đặt sự kiện'), ('Theo trận 90 phút'), ('Theo trận 120 phút'), ('Khung giờ vàng'), ('Khung giờ khuya');

-- 4. SANBONG (10 sân, thuộc sở hữu của các chủ sân ID 6,7,8,9)
INSERT INTO SANBONG (chuSan, tenSan, moTa, daDuyet, diaChi, quan, thanhPho) VALUES
(6, 'Sân Bóng Thanh Niên 1', 'Sân trung tâm', TRUE, '11 Lê Duẩn', 'Hải Châu', 'Đà Nẵng'),
(6, 'Sân Bóng Thanh Niên 2', 'Sân mới nâng cấp', TRUE, '12 Lê Duẩn', 'Hải Châu', 'Đà Nẵng'),
(7, 'Sân Bóng Mỹ Khê', 'Gió biển mát', TRUE, '45 Võ Nguyên Giáp', 'Sơn Trà', 'Đà Nẵng'),
(7, 'Sân Bóng Biển Đông', 'Mặt cỏ tốt', TRUE, '46 Võ Nguyên Giáp', 'Sơn Trà', 'Đà Nẵng'),
(8, 'Sân Bóng Bách Khoa', 'Dành cho sinh viên', TRUE, '54 Nguyễn Lương Bằng', 'Liên Chiểu', 'Đà Nẵng'),
(8, 'Sân Bóng Sư Phạm', 'Giá rẻ', TRUE, '55 Tôn Đức Thắng', 'Liên Chiểu', 'Đà Nẵng'),
(9, 'Sân Bóng Hòa Xuân 1', 'Khu liên hợp', TRUE, 'KĐT Hòa Xuân', 'Cẩm Lệ', 'Đà Nẵng'),
(9, 'Sân Bóng Hòa Xuân 2', 'Có khán đài', TRUE, 'KĐT Hòa Xuân', 'Cẩm Lệ', 'Đà Nẵng'),
(6, 'Sân Bóng VIP Hải Châu', 'VIP Dịch vụ cao cấp', FALSE, '100 Nguyễn Văn Linh', 'Hải Châu', 'Đà Nẵng'),
(7, 'Sân Bóng Sơn Trà Mới', 'Đang xây dựng', FALSE, 'Bán đảo Sơn Trà', 'Sơn Trà', 'Đà Nẵng');

-- 5. SANBONGCHITIET (10 cụm sân nhỏ thuộc 10 sân trên)
INSERT INTO SANBONGCHITIET (maSanBong, maLoaiSan, tenSanChiTiet, giaThueBuoiSang, giaThueBuoiToi) VALUES
(1, 1, 'Sân số 1 (5 người)', 150000, 200000),
(2, 2, 'Sân số 2 (7 người)', 200000, 250000),
(3, 1, 'Sân Biển 1', 180000, 220000),
(4, 3, 'Sân Lớn 11 người', 500000, 700000),
(5, 1, 'Sân BK 1', 120000, 150000),
(6, 4, 'Sân Futsal SP', 160000, 200000),
(7, 2, 'Sân HX A', 220000, 280000),
(8, 2, 'Sân HX B', 220000, 280000),
(9, 6, 'Sân VIP Cỏ Thật', 400000, 600000),
(10,1, 'Sân Mini ST', 140000, 180000);

-- 6. DATSAN (10 lượt đặt sân từ những người thuê ID 1->5)
INSERT INTO DATSAN (nguoiThue, ngayDat, ngayThanhToan) VALUES
(1, '2026-06-01', '2026-06-01 08:00:00'),
(2, '2026-06-02', '2026-06-02 09:00:00'),
(3, '2026-06-03', '2026-06-03 10:00:00'),
(4, '2026-06-04', '2026-06-04 11:00:00'),
(5, '2026-06-05', '2026-06-05 12:00:00'),
(1, '2026-06-06', '2026-06-06 13:00:00'),
(2, '2026-06-07', '2026-06-07 14:00:00'),
(3, '2026-06-08', '2026-06-08 15:00:00'),
(4, '2026-06-09', '2026-06-09 16:00:00'),
(5, '2026-06-10', '2026-06-10 17:00:00');

-- 7. CHITIETDATSAN (10 chi tiết tương ứng với 10 lượt đặt)
-- Lưu ý logic: Đặt các ngày khác nhau để KHÔNG BỊ TRÙNG GIỜ (tránh vướng Trigger EXCLUDE USING GIST)
-- Đều set 'hoan_thanh' để Lát nữa có thể Đánh giá được.
INSERT INTO CHITIETDATSAN (maDatSan, maSanChiTiet, maLoaiDat, gioBatDau, gioKetThuc, trangThaiDatSan) VALUES
(1, 1, 1, '2026-06-01 17:00:00', '2026-06-01 19:00:00', 'hoan_thanh'),
(2, 2, 1, '2026-06-02 18:00:00', '2026-06-02 20:00:00', 'hoan_thanh'),
(3, 3, 1, '2026-06-03 17:30:00', '2026-06-03 19:30:00', 'hoan_thanh'),
(4, 4, 1, '2026-06-04 16:00:00', '2026-06-04 18:00:00', 'hoan_thanh'),
(5, 5, 1, '2026-06-05 19:00:00', '2026-06-05 21:00:00', 'hoan_thanh'),
(6, 6, 1, '2026-06-06 18:30:00', '2026-06-06 20:30:00', 'hoan_thanh'),
(7, 7, 1, '2026-06-07 17:00:00', '2026-06-07 19:00:00', 'hoan_thanh'),
(8, 8, 1, '2026-06-08 18:00:00', '2026-06-08 20:00:00', 'hoan_thanh'),
(9, 9, 1, '2026-06-09 19:00:00', '2026-06-09 21:00:00', 'hoan_thanh'),
(10,10,1, '2026-06-10 20:00:00', '2026-06-10 22:00:00', 'hoan_thanh');

-- 8. DANHGIA (10 lượt đánh giá)
-- Logic bảo vệ: Trigger bắt buộc User phải "hoàn thành" đặt sân đó mới được đánh giá. 
-- Tôi đã map cẩn thận User_ID và SanBong_ID khớp với 10 lượt đặt ở bảng trên.
INSERT INTO DANHGIA (maSanBong, nguoiThue, diemSo, binhLuan) VALUES
(1, 1, 5, 'Mặt cỏ rất đẹp, đèn sáng'),
(2, 2, 4, 'Sân tốt nhưng chỗ để xe hơi hẹp'),
(3, 3, 5, 'Gió mát, đá rất thích'),
(4, 4, 5, 'Sân 11 chuẩn quốc tế'),
(5, 5, 3, 'Cỏ hơi mòn, cần thay thế'),
(6, 1, 4, 'Dịch vụ nước uống rẻ'),
(7, 2, 5, 'Sân sạch sẽ, chủ sân nhiệt tình'),
(8, 3, 4, 'Bóng hơi mềm, còn lại OK'),
(9, 4, 5, 'VIP đúng chuẩn VIP'),
(10,5, 4, 'Sân mới, đáng trải nghiệm');

-- 9. CHAT (10 đoạn chat giữa người thuê và chủ sân)
INSERT INTO CHAT (nguoiGui, nguoiNhan, noiDung, daDoc) VALUES
(1, 6, 'Anh ơi cho em hỏi sân chiều mai còn trống không?', TRUE),
(6, 1, 'Còn trống khung 17h nhé em', TRUE),
(2, 7, 'Giá thuê có bao gồm nước suối chưa ạ?', TRUE),
(7, 2, 'Giá chưa bao gồm nước em nhé', FALSE),
(3, 8, 'Em lỡ đặt nhầm ngày, anh đổi giúp em được không?', TRUE),
(8, 3, 'Ok anh đổi qua ngày 15 cho em rồi đó', TRUE),
(4, 9, 'Cho em xuất hóa đơn VAT nhé', FALSE),
(9, 4, 'Bên anh không xuất VAT, em thông cảm', FALSE),
(5, 6, 'Sân có cho mượn áo pitch không anh?', TRUE),
(6, 5, 'Miễn phí mượn áo pitch nhé', TRUE);

-- 10. NAPTIEN (10 giao dịch - CHỈ DÙNG vnpay theo yêu cầu)
INSERT INTO NAPTIEN (nguoiNap, soTien, maGiaoDich, phuongThuc, trangThai) VALUES
(1, 500000, 'VNP2026001', 'vnpay', 'thanh_cong'),
(2, 200000, 'VNP2026002', 'vnpay', 'thanh_cong'),
(3, 300000, 'VNP2026003', 'vnpay', 'thanh_cong'),
(4, 100000, 'VNP2026004', 'vnpay', 'cho_xu_ly'),
(5, 500000, 'VNP2026005', 'vnpay', 'that_bai'),
(1, 200000, 'VNP2026006', 'vnpay', 'thanh_cong'),
(2, 400000, 'VNP2026007', 'vnpay', 'cho_xu_ly'),
(3, 100000, 'VNP2026008', 'vnpay', 'thanh_cong'),
(4, 150000, 'VNP2026009', 'vnpay', 'thanh_cong'),
(5, 250000, 'VNP2026010', 'vnpay', 'that_bai');

-- 11. YEUCAURUTTIEN (10 giao dịch từ các chủ sân)
INSERT INTO YEUCAURUTTIEN (maNguoiDung, soTien, maGiaoDich, moTa, tenNganHang, soTaiKhoan, trangThai) VALUES
(6, 1000000, 'RUT001', 'Rút tiền tháng 5', 'MB Bank', '0902000001', 'da_chuyen'),
(7, 2000000, 'RUT002', 'Rút tiền kinh doanh', 'Vietcombank', '0123456789', 'cho_xu_ly'),
(8, 1500000, 'RUT003', 'Rút doanh thu', 'Techcombank', '190123456', 'da_chuyen'),
(9, 500000,  'RUT004', 'Rút tiền', 'ACB', '987654321', 'cho_xu_ly'),
(6, 2000000, 'RUT005', 'Rút tiếp', 'MB Bank', '0902000001', 'that_bai'),
(7, 1000000, 'RUT006', 'Chi tiêu cá nhân', 'Vietcombank', '0123456789', 'da_chuyen'),
(8, 3000000, 'RUT007', 'Doanh thu Lễ', 'Techcombank', '190123456', 'cho_xu_ly'),
(9, 800000,  'RUT008', 'Rút một phần', 'ACB', '987654321', 'da_chuyen'),
(6, 500000,  'RUT009', 'Rút lắt nhắt', 'MB Bank', '0902000001', 'cho_xu_ly'),
(7, 1500000, 'RUT010', 'Doanh thu bù lỗ', 'Vietcombank', '0123456789', 'cho_xu_ly');

-- 12. MEDIA_SANBONG (10 ảnh/video cho sân bóng chính)
INSERT INTO MEDIA_SANBONG (maSanBong, loaiMedia, ten, link, mediaId) VALUES
(1, 'hinh_anh', 'Ảnh toàn cảnh sân 1', 'https://link.com/sb1.jpg', 'img_sb1_01'),
(2, 'hinh_anh', 'Ảnh toàn cảnh sân 2', 'https://link.com/sb2.jpg', 'img_sb2_01'),
(3, 'video',    'Video flycam biển', 'https://link.com/sb3.mp4', 'vid_sb3_01'),
(4, 'hinh_anh', 'Ảnh mặt cỏ', 'https://link.com/sb4.jpg', 'img_sb4_01'),
(5, 'hinh_anh', 'Ảnh khán đài', 'https://link.com/sb5.jpg', 'img_sb5_01'),
(6, 'hinh_anh', 'Ảnh cột gôn', 'https://link.com/sb6.jpg', 'img_sb6_01'),
(7, 'video',    'Review sân', 'https://link.com/sb7.mp4', 'vid_sb7_01'),
(8, 'hinh_anh', 'Ảnh đèn LED', 'https://link.com/sb8.jpg', 'img_sb8_01'),
(9, 'hinh_anh', 'Ảnh phòng thay đồ', 'https://link.com/sb9.jpg', 'img_sb9_01'),
(10,'hinh_anh', 'Ảnh bãi xe', 'https://link.com/sb10.jpg', 'img_sb10_01');

-- 13. MEDIA_SANBONGCHITIET (10 ảnh/video cho chi tiết từng sân)
INSERT INTO MEDIA_SANBONGCHITIET (maSanBongChiTiet, loaiMedia, ten, link, mediaId) VALUES
(1, 'hinh_anh', 'Ảnh lưới sân 1', 'https://link.com/sbc1.jpg', 'img_sbc1_01'),
(2, 'hinh_anh', 'Ảnh vạch vôi sân 2', 'https://link.com/sbc2.jpg', 'img_sbc2_01'),
(3, 'hinh_anh', 'Góc nhìn từ biển', 'https://link.com/sbc3.jpg', 'img_sbc3_01'),
(4, 'video',    'Trận đấu mẫu', 'https://link.com/sbc4.mp4', 'vid_sbc4_01'),
(5, 'hinh_anh', 'Khán đài A', 'https://link.com/sbc5.jpg', 'img_sbc5_01'),
(6, 'hinh_anh', 'Lưới mơi', 'https://link.com/sbc6.jpg', 'img_sbc6_01'),
(7, 'hinh_anh', 'Cỏ nhân tạo loại 1', 'https://link.com/sbc7.jpg', 'img_sbc7_01'),
(8, 'hinh_anh', 'Ghế chờ', 'https://link.com/sbc8.jpg', 'img_sbc8_01'),
(9, 'hinh_anh', 'Bảng tỷ số VIP', 'https://link.com/sbc9.jpg', 'img_sbc9_01'),
(10,'hinh_anh', 'Lưới trẻ em', 'https://link.com/sbc10.jpg', 'img_sbc10_01');