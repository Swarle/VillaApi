﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Models;

[Table("villa_status")]
public partial class VillaStatus
{
    [Column("status")]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [InverseProperty("Status")]
    public virtual ICollection<Villa> Villa { get; set; } = new List<Villa>();
}
