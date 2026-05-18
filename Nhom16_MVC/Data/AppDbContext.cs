using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Nhom16_MVC.Models;

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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=ep-round-meadow-aozuay67-pooler.c-2.ap-southeast-1.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_LZkF4o6huAwt;SSL Mode=Require");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<chat>(entity =>
        {
            entity.HasKey(e => e.matinnhan).HasName("chat_pkey");

            entity.HasIndex(e => e.nguoigui, "ix_chat_nguoigui");

            entity.HasIndex(e => e.nguoinhan, "ix_chat_nguoinhan");

            entity.Property(e => e.thoigiangui)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.nguoiguiNavigation).WithMany(p => p.chatnguoiguiNavigation)
                .HasForeignKey(d => d.nguoigui)
                .HasConstraintName("chat_nguoigui_fkey");

            entity.HasOne(d => d.nguoinhanNavigation).WithMany(p => p.chatnguoinhanNavigation)
                .HasForeignKey(d => d.nguoinhan)
                .HasConstraintName("chat_nguoinhan_fkey");
        });

        modelBuilder.Entity<chitietdatsan>(entity =>
        {
            entity.HasKey(e => e.machitietdatsan).HasName("chitietdatsan_pkey");

            entity.HasIndex(e => e.madatsan, "ix_chitietdatsan_datsan");

            entity.HasIndex(e => e.masanchitiet, "ix_chitietdatsan_sanchitiet");

            entity.HasIndex(e => e.trangthaidatsan, "ix_chitietdatsan_trangthai");

            entity.Property(e => e.covande).HasDefaultValue(false);
            entity.Property(e => e.trangthaidatsan)
                .HasMaxLength(50)
                .HasDefaultValueSql("'cho_xac_nhan'::character varying");

            entity.HasOne(d => d.madatsanNavigation).WithMany(p => p.chitietdatsan)
                .HasForeignKey(d => d.madatsan)
                .HasConstraintName("chitietdatsan_madatsan_fkey");

            entity.HasOne(d => d.maloaidatNavigation).WithMany(p => p.chitietdatsan)
                .HasForeignKey(d => d.maloaidat)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("chitietdatsan_maloaidat_fkey");

            entity.HasOne(d => d.masanchitietNavigation).WithMany(p => p.chitietdatsan)
                .HasForeignKey(d => d.masanchitiet)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("chitietdatsan_masanchitiet_fkey");
        });

        modelBuilder.Entity<danhgia>(entity =>
        {
            entity.HasKey(e => e.madanhgia).HasName("danhgia_pkey");

            entity.HasIndex(e => e.nguoithue, "ix_danhgia_nguoithue");

            entity.HasIndex(e => e.masanbong, "ix_danhgia_sanbong");

            entity.Property(e => e.thoigiandanhgia)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.masanbongNavigation).WithMany(p => p.danhgia)
                .HasForeignKey(d => d.masanbong)
                .HasConstraintName("danhgia_masanbong_fkey");

            entity.HasOne(d => d.nguoithueNavigation).WithMany(p => p.danhgia)
                .HasForeignKey(d => d.nguoithue)
                .HasConstraintName("danhgia_nguoithue_fkey");
        });

        modelBuilder.Entity<datsan>(entity =>
        {
            entity.HasKey(e => e.madatsan).HasName("datsan_pkey");

            entity.HasIndex(e => e.ngaydat, "ix_datsan_ngaydat");

            entity.HasIndex(e => e.nguoithue, "ix_datsan_nguoithue");

            entity.Property(e => e.ngaydat).HasDefaultValueSql("CURRENT_DATE");
            entity.Property(e => e.ngaythanhtoan).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.nguoithueNavigation).WithMany(p => p.datsan)
                .HasForeignKey(d => d.nguoithue)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("datsan_nguoithue_fkey");
        });

        modelBuilder.Entity<loaihinhdat>(entity =>
        {
            entity.HasKey(e => e.maloaidat).HasName("loaihinhdat_pkey");

            entity.Property(e => e.tenloaidat).HasMaxLength(100);
        });

        modelBuilder.Entity<loaisan>(entity =>
        {
            entity.HasKey(e => e.maloaisan).HasName("loaisan_pkey");

            entity.Property(e => e.tenloaisan).HasMaxLength(100);
        });

        modelBuilder.Entity<media_sanbong>(entity =>
        {
            entity.HasKey(e => e.mamedia).HasName("media_sanbong_pkey");

            entity.HasIndex(e => e.masanbong, "ix_mediasanbong_sanbong");

            entity.Property(e => e.loaimedia).HasMaxLength(20);
            entity.Property(e => e.mediaid).HasMaxLength(255);
            entity.Property(e => e.ten).HasMaxLength(255);

            entity.HasOne(d => d.masanbongNavigation).WithMany(p => p.media_sanbong)
                .HasForeignKey(d => d.masanbong)
                .HasConstraintName("media_sanbong_masanbong_fkey");
        });

        modelBuilder.Entity<media_sanbongchitiet>(entity =>
        {
            entity.HasKey(e => e.mamedia).HasName("media_sanbongchitiet_pkey");

            entity.HasIndex(e => e.masanbongchitiet, "ix_mediachitiet_sanchitiet");

            entity.Property(e => e.loaimedia).HasMaxLength(20);
            entity.Property(e => e.mediaid).HasMaxLength(255);
            entity.Property(e => e.ten).HasMaxLength(255);

            entity.HasOne(d => d.masanbongchitietNavigation).WithMany(p => p.media_sanbongchitiet)
                .HasForeignKey(d => d.masanbongchitiet)
                .HasConstraintName("media_sanbongchitiet_masanbongchitiet_fkey");
        });

        modelBuilder.Entity<naptien>(entity =>
        {
            entity.HasKey(e => e.manaptien).HasName("naptien_pkey");

            entity.HasIndex(e => e.nguoinap, "ix_naptien_nguoinap");

            entity.HasIndex(e => e.trangthai, "ix_naptien_trangthai");

            entity.HasIndex(e => e.magiaodich, "naptien_magiaodich_key").IsUnique();

            entity.Property(e => e.magiaodich).HasMaxLength(100);
            entity.Property(e => e.thoigiannap)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.trangthai)
                .HasMaxLength(50)
                .HasDefaultValueSql("'cho_xu_ly'::character varying");

            entity.HasOne(d => d.nguoinapNavigation).WithMany(p => p.naptien)
                .HasForeignKey(d => d.nguoinap)
                .HasConstraintName("naptien_nguoinap_fkey");
        });

        modelBuilder.Entity<nguoidung>(entity =>
        {
            entity.HasKey(e => e.manguoidung).HasName("nguoidung_pkey");

            entity.HasIndex(e => e.email, "ix_nguoidung_email");

            entity.HasIndex(e => e.vaitro, "ix_nguoidung_vaitro");

            entity.HasIndex(e => e.email, "nguoidung_email_key").IsUnique();

            entity.HasIndex(e => e.sodienthoai, "nguoidung_sodienthoai_key").IsUnique();

            entity.Property(e => e.createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.hoten).HasMaxLength(150);
            entity.Property(e => e.isemailverified).HasDefaultValue(false);
            entity.Property(e => e.sodienthoai).HasMaxLength(15);
            entity.Property(e => e.sodutaikhoan).HasDefaultValue(0L);
            entity.Property(e => e.tokenexpiry).HasColumnType("timestamp without time zone");
            entity.Property(e => e.vaitro)
                .HasMaxLength(50)
                .HasDefaultValueSql("'nguoiThue'::character varying");
        });

        modelBuilder.Entity<sanbong>(entity =>
        {
            entity.HasKey(e => e.masanbong).HasName("sanbong_pkey");

            entity.HasIndex(e => e.chusan, "ix_sanbong_chusan");

            entity.HasIndex(e => e.daduyet, "ix_sanbong_daduyet");

            entity.Property(e => e.createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.daduyet).HasDefaultValue(false);
            entity.Property(e => e.huyen).HasMaxLength(100);
            entity.Property(e => e.kinhdo).HasPrecision(11, 8);
            entity.Property(e => e.quan).HasMaxLength(100);
            entity.Property(e => e.tensan).HasMaxLength(255);
            entity.Property(e => e.thanhpho)
                .HasMaxLength(100)
                .HasDefaultValueSql("'Đà Nẵng'::character varying");
            entity.Property(e => e.vido).HasPrecision(10, 8);
            entity.Property(e => e.xa).HasMaxLength(100);

            entity.HasOne(d => d.chusanNavigation).WithMany(p => p.sanbong)
                .HasForeignKey(d => d.chusan)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("sanbong_chusan_fkey");
        });

        modelBuilder.Entity<sanbongchitiet>(entity =>
        {
            entity.HasKey(e => e.masanchitiet).HasName("sanbongchitiet_pkey");

            entity.HasIndex(e => e.maloaisan, "ix_sanchitiet_loaisan");

            entity.HasIndex(e => e.masanbong, "ix_sanchitiet_sanbong");

            entity.Property(e => e.giathuebuoisang).HasDefaultValue(0L);
            entity.Property(e => e.giathuebuoitoi).HasDefaultValue(0L);
            entity.Property(e => e.tensanchitiet).HasMaxLength(255);

            entity.HasOne(d => d.maloaisanNavigation).WithMany(p => p.sanbongchitiet)
                .HasForeignKey(d => d.maloaisan)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("sanbongchitiet_maloaisan_fkey");

            entity.HasOne(d => d.masanbongNavigation).WithMany(p => p.sanbongchitiet)
                .HasForeignKey(d => d.masanbong)
                .HasConstraintName("sanbongchitiet_masanbong_fkey");
        });

        modelBuilder.Entity<sanbongratingsummary>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("sanbongratingsummary");
        });

        modelBuilder.Entity<yeucauruttien>(entity =>
        {
            entity.HasKey(e => e.mayeucau).HasName("yeucauruttien_pkey");

            entity.HasIndex(e => e.manguoidung, "ix_ruttien_nguoidung");

            entity.HasIndex(e => e.trangthai, "ix_ruttien_trangthai");

            entity.HasIndex(e => e.magiaodich, "yeucauruttien_magiaodich_key").IsUnique();

            entity.Property(e => e.magiaodich).HasMaxLength(100);
            entity.Property(e => e.sotaikhoan).HasMaxLength(50);
            entity.Property(e => e.tennganhang).HasMaxLength(200);
            entity.Property(e => e.thoigianrut)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.trangthai)
                .HasMaxLength(50)
                .HasDefaultValueSql("'cho_xu_ly'::character varying");

            entity.HasOne(d => d.manguoidungNavigation).WithMany(p => p.yeucauruttien)
                .HasForeignKey(d => d.manguoidung)
                .HasConstraintName("yeucauruttien_manguoidung_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
