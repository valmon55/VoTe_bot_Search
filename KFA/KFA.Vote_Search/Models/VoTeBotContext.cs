using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KFA.Vote_Search.Models;

public partial class VoTeBotContext : DbContext
{
    public VoTeBotContext()
    {
    }

    public VoTeBotContext(DbContextOptions<VoTeBotContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserMessage> UserMessages { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-ACP6245\\SQLEXPRESS;Initial Catalog=VoTe_Bot;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
