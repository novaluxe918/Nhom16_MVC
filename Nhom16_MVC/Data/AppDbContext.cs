using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Models.Entities;
using Nhom16_MVC.Models.Enums;

namespace Nhom16_MVC.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<chat> chat { get; set; }
    public virtual DbSet<chitietdatsan> chitietdatsan { get; set; }
    public virtual DbSet<danhgia> danhgia { get; set; }
    public virtual DbSet<datsan> datsan { get; set; }
    public virtual DbSet<loaihinhdat> loaihinhdat { get; set; }
    public virtual DbSet<loaisan> loaisan { get; set; }
    public virtual DbSet<media_sanbong> media_sanbong { get; set; }
    public virtual DbSet<media_sanbongchitiet> media_sanbongchitiet { get; set; }
    public virtual DbSet<naptien> naptien { get; set; }
    public virtual DbSet<nguoidung> nguoidung { get; set; }
    public virtual DbSet<sanbong> sanbong { get; set; }
    public virtual DbSet<sanbongchitiet> sanbongchitiet { get; set; }
    public virtual DbSet<sanbongratingsummary> sanbongratingsummary { get; set; }
    public virtual DbSet<yeucauruttien> yeucauruttien { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // KHÔNG cấu hình gì ở đây vì đã dùng NpgsqlDataSource trong Program.cs
        // Việc MapEnum<VaiTroEnum> đã được thực hiện trong Program.cs
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CHỈ đăng ký các enum KHÁC, KHÔNG đăng ký VaiTroEnum ở đây
        // vì VaiTroEnum đã được MapEnum trong NpgsqlDataSourceBuilder ở Program.cs
        // Đăng ký lại ở đây sẽ gây xung đột và lỗi type mismatch
        modelBuilder.HasPostgresEnum<TrangThaiDatEnum>("trang_thai_dat")
            .HasPostgresEnum<TrangThaiNapEnum>("trang_thai_nap")
            .HasPostgresEnum<TrangThaiRutEnum>("trang_thai_rut");

        modelBuilder.Entity<chat>(entity =>
        {
            entity.HasKey(e => e.matinnhan).HasName("chat_pkey");

            entity.Property(e => e.matinnhan).HasColumnName("matinnhan");
            entity.Property(e => e.nguoigui).HasColumnName("nguoigui");
            entity.Property(e => e.nguoinhan).HasColumnName("nguoinhan");
            entity.Property(e => e.noidung).HasColumnName("noidung").HasColumnType("character varying");
            entity.Property(e => e.thoigiangui).HasColumnName("thoigiangui").HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");
            entity.Property(e => e.daDoc).HasColumnName("daDoc");

            entity.HasOne(d => d.nguoiguiNavigation).WithMany(p => p.chatnguoiguiNavigation)
                .HasForeignKey(d => d.nguoigui)
                .HasConstraintName("chat_manguoirgui_fkey");

            entity.HasOne(d => d.nguoinhanNavigation).WithMany(p => p.chatnguoinhanNavigation)
                .HasForeignKey(d => d.nguoinhan)
                .HasConstraintName("chat_manguoinhan_fkey");
        });

        modelBuilder.Entity<chitietdatsan>(entity =>
        {
            entity.HasKey(e => e.machitietdatsan).HasName("chitietdatsan_pkey");

            entity.Property(e => e.machitietdatsan).HasColumnName("machitietdatsan");
            entity.Property(e => e.madatsan).HasColumnName("madatsan");
            entity.Property(e => e.masanchitiet).HasColumnName("masanchitiet");
            entity.Property(e => e.maloaidat).HasColumnName("maloaidat");
            entity.Property(e => e.giobatdau).HasColumnName("giobatdau");
            entity.Property(e => e.gioketthuc).HasColumnName("gioketthuc");
            entity.Property(e => e.covande).HasColumnName("covande");
            entity.Property(e => e.trangthaidatsan).HasColumnName("trangthaidatsan");

            entity.HasOne(d => d.madatsanNavigation).WithMany(p => p.chitietdatsan)
                .HasForeignKey(d => d.madatsan)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("chitietdatsan_madatsan_fkey");

            entity.HasOne(d => d.maloaidatNavigation).WithMany(p => p.chitietdatsan)
                .HasForeignKey(d => d.maloaidat)
                .HasConstraintName("chitietdatsan_maloaidat_fkey");

            entity.HasOne(d => d.masanchitietNavigation).WithMany(p => p.chitietdatsan)
                .HasForeignKey(d => d.masanchitiet)
                .HasConstraintName("chitietdatsan_masanbongchitiet_fkey");
        });

        modelBuilder.Entity<danhgia>(entity =>
        {
            entity.HasKey(e => e.madanhgia).HasName("danhgia_pkey");

            entity.Property(e => e.madanhgia).HasColumnName("madanhgia");
            entity.Property(e => e.masanbong).HasColumnName("masanbong");
            entity.Property(e => e.nguoithue).HasColumnName("nguoithue");
            entity.Property(e => e.diemso).HasColumnName("diemso");
            entity.Property(e => e.binhluan).HasColumnName("binhluan").HasColumnType("character varying");
            entity.Property(e => e.thoigiandanhgia).HasColumnName("thoigiandanhgia").HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            entity.HasOne(d => d.masanbongNavigation).WithMany(p => p.danhgia)
                .HasForeignKey(d => d.masanbong)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("danhgia_masanbong_fkey");

            entity.HasOne(d => d.nguoithueNavigation).WithMany(p => p.danhgia)
                .HasForeignKey(d => d.nguoithue)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("danhgia_manguoidung_fkey");
        });

        modelBuilder.Entity<datsan>(entity =>
        {
            entity.HasKey(e => e.madatsan).HasName("datsan_pkey");

            entity.Property(e => e.madatsan).HasColumnName("madatsan");
            entity.Property(e => e.nguoithue).HasColumnName("nguoithue");
            entity.Property(e => e.ngaydat).HasColumnName("ngaydat");
            entity.Property(e => e.ngaythanhtoan).HasColumnName("ngaythanhtoan");
            entity.Property(e => e.sotienthanhtoan).HasColumnName("sotienthanhtoan");

            entity.HasOne(d => d.nguoithueNavigation).WithMany(p => p.datsan)
                .HasForeignKey(d => d.nguoithue)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("datsan_manguoidung_fkey");
        });

        modelBuilder.Entity<loaihinhdat>(entity =>
        {
            entity.HasKey(e => e.maloaidat).HasName("loaihinhdat_pkey");
            entity.Property(e => e.maloaidat).HasColumnName("maloaidat");
            entity.Property(e => e.tenloaidat).HasColumnName("tenloaidat").HasMaxLength(100);
        });

        modelBuilder.Entity<loaisan>(entity =>
        {
            entity.HasKey(e => e.maloaisan).HasName("loaisan_pkey");
            entity.Property(e => e.maloaisan).HasColumnName("maloaisan");
            entity.Property(e => e.tenloaisan).HasColumnName("tenloaisan").HasMaxLength(50);
        });

        modelBuilder.Entity<media_sanbong>(entity =>
        {
            entity.HasKey(e => e.mamedia).HasName("media_sanbong_pkey");

            entity.Property(e => e.mamedia).HasColumnName("mamedia");
            entity.Property(e => e.masanbong).HasColumnName("masanbong");
            entity.Property(e => e.loaimedia).HasColumnName("loaimedia").HasMaxLength(50);
            entity.Property(e => e.ten).HasColumnName("ten").HasMaxLength(255);
            entity.Property(e => e.link).HasColumnName("link");
            entity.Property(e => e.mediaid).HasColumnName("mediaid").HasMaxLength(100);

            entity.HasOne(d => d.masanbongNavigation).WithMany(p => p.media_sanbong)
                .HasForeignKey(d => d.masanbong)
                .HasConstraintName("media_sanbong_masanbong_fkey");
        });

        modelBuilder.Entity<media_sanbongchitiet>(entity =>
        {
            entity.HasKey(e => e.mamedia).HasName("media_sanbongchitiet_pkey");

            entity.Property(e => e.mamedia).HasColumnName("mamedia");
            entity.Property(e => e.masanbongchitiet).HasColumnName("masanbongchitiet");
            entity.Property(e => e.loaimedia).HasColumnName("loaimedia").HasMaxLength(50);
            entity.Property(e => e.ten).HasColumnName("ten").HasMaxLength(255);
            entity.Property(e => e.link).HasColumnName("link");
            entity.Property(e => e.mediaid).HasColumnName("mediaid").HasMaxLength(100);

            entity.HasOne(d => d.masanbongchitietNavigation).WithMany(p => p.media_sanbongchitiet)
                .HasForeignKey(d => d.masanbongchitiet)
                .HasConstraintName("media_sanbongchitiet_masanbongchitiet_fkey");
        });

        modelBuilder.Entity<naptien>(entity =>
        {
            entity.HasKey(e => e.manaptien).HasName("naptien_pkey");

            entity.Property(e => e.manaptien).HasColumnName("manaptien");
            entity.Property(e => e.nguoinap).HasColumnName("nguoinap");
            entity.Property(e => e.sotien).HasColumnName("sotien");
            entity.Property(e => e.thoigiannap).HasColumnName("thoigiannap").HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");
            entity.Property(e => e.magiaodich).HasColumnName("magiaodich").HasMaxLength(100);
            entity.Property(e => e.phuongthuc).HasColumnName("phuongthuc");
            entity.Property(e => e.trangthai).HasColumnName("trangthai");

            entity.HasOne(d => d.nguoinapNavigation).WithMany(p => p.naptien)
                .HasForeignKey(d => d.nguoinap)
                .HasConstraintName("naptien_manguoidung_fkey");
        });

        modelBuilder.Entity<nguoidung>(entity =>
        {
            entity.HasKey(e => e.manguoidung).HasName("nguoidung_pkey");

            entity.Property(e => e.manguoidung).HasColumnName("manguoidung");
            entity.Property(e => e.hoten).HasColumnName("hoten").HasMaxLength(150);
            entity.Property(e => e.avatar).HasColumnName("avatar");
            entity.Property(e => e.email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.sodienthoai).HasColumnName("sodienthoai").HasMaxLength(15);
            entity.Property(e => e.matkhau).HasColumnName("matkhau");
            entity.Property(e => e.sodutaikhoan).HasColumnName("sodutaikhoan");
            entity.Property(e => e.createdat).HasColumnName("createdat").HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");
            entity.Property(e => e.isemailverified).HasColumnName("isemailverified");
            entity.Property(e => e.verificationtoken).HasColumnName("verificationtoken");
            entity.Property(e => e.tokenexpiry).HasColumnName("tokenexpiry").HasColumnType("timestamp without time zone");
            entity.Property(e => e.trangthai).HasColumnName("trangthai").HasMaxLength(50).HasDefaultValue("hoat_dong");

            // VaiTroEnum đã được MapEnum trong NpgsqlDataSourceBuilder ở Program.cs
            // Chỉ cần khai báo tên cột, Npgsql tự xử lý mapping
            entity.Property(e => e.vaitro)
                .HasColumnName("vaitro");
        });

        modelBuilder.Entity<sanbong>(entity =>
        {
            entity.HasKey(e => e.masanbong).HasName("sanbong_pkey");

            entity.Property(e => e.masanbong).HasColumnName("masanbong");
            entity.Property(e => e.tensan).HasColumnName("tensan").HasMaxLength(150);
            entity.Property(e => e.diachi).HasColumnName("diachi").HasMaxLength(255);
            entity.Property(e => e.chusan).HasColumnName("chusan");
            entity.Property(e => e.createdat).HasColumnName("createdat").HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");

            entity.HasOne(d => d.chusanNavigation).WithMany(p => p.sanbong)
                .HasForeignKey(d => d.chusan)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("sanbong_machusan_fkey");
        });

        modelBuilder.Entity<sanbongchitiet>(entity =>
        {
            entity.HasKey(e => e.masanchitiet).HasName("sanbongchitiet_pkey");

            entity.Property(e => e.masanchitiet).HasColumnName("masanchitiet");
            entity.Property(e => e.masanbong).HasColumnName("masanbong");
            entity.Property(e => e.maloaisan).HasColumnName("maloaisan");
            entity.Property(e => e.tensanchitiet).HasColumnName("tensanchitiet").HasMaxLength(100);
            entity.Property(e => e.giathuebuoisang).HasColumnName("giathuebuoisang");
            entity.Property(e => e.giathuebuoitoi).HasColumnName("giathuebuoitoi");

            entity.HasOne(d => d.maloaisanNavigation).WithMany(p => p.sanbongchitiet)
                .HasForeignKey(d => d.maloaisan)
                .HasConstraintName("sanbongchitiet_maloaisan_fkey");

            entity.HasOne(d => d.masanbongNavigation).WithMany(p => p.sanbongchitiet)
                .HasForeignKey(d => d.masanbong)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("sanbongchitiet_masanbong_fkey");
        });

        modelBuilder.Entity<sanbongratingsummary>(entity =>
        {
            entity.HasNoKey().ToView("sanbongratingsummary");
        });

        modelBuilder.Entity<yeucauruttien>(entity =>
        {
            entity.HasKey(e => e.mayeucau).HasName("yeucauruttien_pkey");

            entity.Property(e => e.mayeucau).HasColumnName("mayeucau");
            entity.Property(e => e.manguoidung).HasColumnName("manguoidung");
            entity.Property(e => e.sotien).HasColumnName("sotien");
            entity.Property(e => e.tennganhang).HasColumnName("tennganhang").HasMaxLength(200);
            entity.Property(e => e.sotaikhoan).HasColumnName("sotaikhoan").HasMaxLength(50);
            entity.Property(e => e.thoigianrut).HasColumnName("thoigianrut").HasColumnType("timestamp without time zone").HasDefaultValueSql("now()");
            entity.Property(e => e.magiaodich).HasColumnName("magiaodich").HasMaxLength(100);
            entity.Property(e => e.trangthai).HasColumnName("trangthai");

            entity.HasOne(d => d.manguoidungNavigation).WithMany(p => p.yeucauruttien)
                .HasForeignKey(d => d.manguoidung)
                .HasConstraintName("yeucauruttien_manguoidung_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}