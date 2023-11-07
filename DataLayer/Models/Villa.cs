﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

[Table("villa")]
public partial class Villa
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("describe")]
    [StringLength(200)]
    public string Describe { get; set; } = null!;

    [Column("image_url")]
    public string ImageUrl { get; set; } = null!;

    [Column("image_local_path")]
    public string? ImageLocalPath { get; set; }
    
    [Column("villa_number")]
    public int VillaNumber { get; set; }

    [Column("status_id")]
    public Guid StatusId { get; set; }

    [Column("price", TypeName = "money")]
    public decimal Price { get; set; }

    [ForeignKey("StatusId")]
    [InverseProperty("Villa")]
    public virtual VillaStatus Status { get; set; } = null!;

    [InverseProperty("Villa")]
    public virtual VillaDetails VillaDetails { get; set; } = null!;

    [InverseProperty("Villa")]
    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
}