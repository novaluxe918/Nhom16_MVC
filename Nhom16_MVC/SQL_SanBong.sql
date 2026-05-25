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
-- 1. NGUOIDUNG
-- ============================================================
CREATE TABLE NGUOIDUNG (
    maNguoiDung   SERIAL          PRIMARY KEY,
    hoTen         VARCHAR(150)    NOT NULL,
    avatar        TEXT,                           
    email         VARCHAR(255)    UNIQUE NOT NULL,
    soDienThoai   VARCHAR(15)     UNIQUE,
    matKhau       TEXT            NOT NULL,       
    vaiTro        vai_tro_enum    NOT NULL DEFAULT 'nguoiThue', -- Đã đổi sang ENUM
    soDuTaiKhoan  BIGINT          NOT NULL DEFAULT 0, 
    createdAt     TIMESTAMP       DEFAULT NOW()
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
    updatedAt   TIMESTAMP     DEFAULT NOW()   
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
-- DỮ LIỆU MẪU ĐÃ CẬP NHẬT THEO CẤU TRÚC MỚI
-- ============================================================

INSERT INTO NGUOIDUNG (hoTen, email, soDienThoai, matKhau, vaiTro, soDuTaiKhoan) VALUES
('Nguyễn Văn An',   'an@gmail.com',   '0901111111', 'hash_matkhau_1', 'nguoiThue', 500000),
('Trần Thị Bình',   'binh@gmail.com', '0902222222', 'hash_matkhau_2', 'chuSan',    2000000),
('Lê Văn Cường',    'cuong@gmail.com','0903333333', 'hash_matkhau_3', 'nguoiThue', 300000),
('Admin Hệ Thống',  'admin@app.com',  '0900000000', 'hash_matkhau_4', 'admin',     0);

INSERT INTO LOAISAN (tenLoaiSan) VALUES ('Sân 5 người'), ('Sân 7 người'), ('Sân 11 người');
INSERT INTO LOAIHINHDAT (tenLoaiDat) VALUES ('Đặt theo giờ'), ('Đặt theo buổi'), ('Đặt cả ngày');

INSERT INTO SANBONG (chuSan, tenSan, moTa, daDuyet, diaChi, quan, thanhPho, kinhDo, viDo) VALUES
(2, 'Sân Bóng Thanh Niên',  'Sân bóng đạt chuẩn...', TRUE,  '123 Lê Duẩn, Hải Châu',    'Hải Châu',    'Đà Nẵng', 108.2208, 16.0678),
(2, 'Sân Bóng Mỹ Khê',     'View biển cực đẹp...',   TRUE,  '456 Võ Nguyên Giáp, Sơn Trà','Sơn Trà',   'Đà Nẵng', 108.2470, 16.0600),
(2, 'Sân Bóng Mini Sport',  'Sân mới khai trương...', FALSE, '789 Nguyễn Văn Linh',        'Thanh Khê', 'Đà Nẵng', 108.1920, 16.0800);

INSERT INTO SANBONGCHITIET (maSanBong, maLoaiSan, tenSanChiTiet, giaThueBuoiSang, giaThueBuoiToi) VALUES
(1, 1, 'Sân A - 5 người', 150000, 200000),
(1, 2, 'Sân B - 7 người', 200000, 250000),
(2, 1, 'Sân số 1',        180000, 230000),
(2, 1, 'Sân số 2',        180000, 230000),
(3, 1, 'Sân Mini 1',      120000, 160000);

INSERT INTO DATSAN (nguoiThue, ngayDat, ngayThanhToan) VALUES
(1, '2025-06-10', '2025-06-10 08:30:00'),
(3, '2025-06-11', NULL);

-- Lưu ý: Dữ liệu mẫu giờ truyền hẳn Timestamp thay vì chỉ Giờ
INSERT INTO CHITIETDATSAN (maDatSan, maSanChiTiet, maLoaiDat, gioBatDau, gioKetThuc, trangThaiDatSan) VALUES
(1, 1, 1, '2025-06-10 18:00:00', '2025-06-10 20:00:00', 'hoan_thanh'),
(2, 3, 1, '2025-06-11 17:00:00', '2025-06-11 19:00:00', 'cho_xac_nhan');

-- Test Đánh Giá: Nguyễn Văn An được phép đánh giá Sân 1 vì CÓ chi tiết trạng thái 'hoan_thanh'
INSERT INTO DANHGIA (maSanBong, nguoiThue, diemSo, binhLuan) VALUES
(1, 1, 5, 'Sân rất đẹp, mặt cỏ tốt, nhân viên thân thiện!');

INSERT INTO NAPTIEN (nguoiNap, soTien, maGiaoDich, phuongThuc, trangThai) VALUES
(1, 500000, 'VNP20250610001', 'vnpay', 'thanh_cong'),
(3, 300000, 'VNP20250610002', 'momo',  'thanh_cong');