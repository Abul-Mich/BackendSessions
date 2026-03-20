using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DbApi.Models;

public partial class UniDbContext : DbContext
{
    public UniDbContext() { }

    public UniDbContext(DbContextOptions<UniDbContext> options)
        : base(options) { }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("courses_pkey");

            entity.ToTable("courses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Teacherid).HasColumnName("teacherid");
            entity.Property(e => e.Title).HasMaxLength(100).HasColumnName("title");

            entity
                .HasOne(d => d.Teacher)
                .WithMany(p => p.Courses)
                .HasForeignKey(d => d.Teacherid)
                .HasConstraintName("courses_teacherid_fkey");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("enrollments_pkey");

            entity.ToTable("enrollments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Courseid).HasColumnName("courseid");
            entity
                .Property(e => e.Enrollmentdate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("enrollmentdate");
            entity.Property(e => e.Studentid).HasColumnName("studentid");

            entity
                .HasOne(d => d.Course)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.Courseid)
                .HasConstraintName("enrollments_courseid_fkey");

            entity
                .HasOne(d => d.Student)
                .WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.Studentid)
                .HasConstraintName("enrollments_studentid_fkey");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("students_pkey");

            entity.ToTable("students");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Birthyear).HasColumnName("birthyear");
            entity.Property(e => e.Country).HasMaxLength(100).HasColumnName("country");
            entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
            entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("teachers_pkey");

            entity.ToTable("teachers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasMaxLength(100).HasColumnName("email");
            entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
