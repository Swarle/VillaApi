﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataLayer.Models;

namespace DataLayer.Context;

public partial class ApplicationContext : DbContext
{


    public ApplicationContext(DbContextOptions<ApplicationContext> options) :
        base(options)
    {
    }

    public virtual DbSet<OrderStatus> OrderStatus { get; set; }

    public virtual DbSet<Orders> Orders { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    public virtual DbSet<Villa> Villa { get; set; }

    public virtual DbSet<VillaDetails> VillaDetails { get; set; }

    public virtual DbSet<VillaStatus> VillaStatus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Orders>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_order_status");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_users_role");
        });

        modelBuilder.Entity<Villa>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Order).WithMany(p => p.Villa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_villa_orders");

            entity.HasOne(d => d.Status).WithMany(p => p.Villa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_villa_villa_status");

            entity.HasOne(d => d.VillaDetails).WithOne(p => p.Villa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_villa_villa_details");
        });

        modelBuilder.Entity<VillaDetails>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<VillaStatus>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}