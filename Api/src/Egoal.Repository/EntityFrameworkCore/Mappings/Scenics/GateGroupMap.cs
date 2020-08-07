﻿using Egoal.Scenics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Egoal.EntityFrameworkCore.Mappings.Scenics
{
    public class GateGroupMap : IEntityTypeConfiguration<GateGroup>
    {
        public void Configure(EntityTypeBuilder<GateGroup> entity)
        {
            entity.ToTable("VM_GateGroup");

            entity.HasIndex(e => e.GroundId);

            entity.Property(e => e.Id)
                .HasColumnName("ID")
                .ValueGeneratedNever();

            entity.Property(e => e.GroundId)
                .HasColumnName("GroundID");

            entity.Property(e => e.Name)
                .HasMaxLength(50);

            entity.Property(e => e.Pcid)
                .HasColumnName("PCID");

            entity.Property(e => e.SortCode)
                .HasMaxLength(50);

            entity.Property(e => e.SpPort)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(e => e.TcpGateway)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.TcpIp)
                .HasColumnName("TcpIP")
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.TcpMac)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.TcpMask)
                .HasMaxLength(50)
                .IsUnicode(false);
        }
    }
}
