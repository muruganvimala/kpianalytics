using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BusinessIntelligence_API.Models
{
    public partial class KPIAnalyticsDevContext : DbContext
    {
        public KPIAnalyticsDevContext()
        {
        }

        public KPIAnalyticsDevContext(DbContextOptions<KPIAnalyticsDevContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BiOtherCost> BiOtherCosts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=dev-test-editgenie.ctrglcosg13x.ap-south-1.rds.amazonaws.com;User ID=kpidevapp;Password=Ytrew@18Dec23;Database=KPIAnalyticsDev;Trusted_Connection=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BiOtherCost>(entity =>
            {
                entity.ToTable("Bi_OtherCost");

                entity.Property(e => e.BudgetedAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Cgst)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CGST");

                entity.Property(e => e.CreatedTime)
                    .HasColumnType("datetime")
                    .HasColumnName("createdTime");

                entity.Property(e => e.Fxrate)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("FXRate");

                entity.Property(e => e.HsnSac)
                    .HasColumnType("numeric(8, 0)")
                    .HasColumnName("HSN_SAC");

                entity.Property(e => e.Igst)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("IGST");

                entity.Property(e => e.InvoiceDate).HasColumnType("date");

                entity.Property(e => e.InvoiceNo).HasMaxLength(16);

                entity.Property(e => e.PoDate)
                    .HasColumnType("date")
                    .HasColumnName("PO_Date");

                entity.Property(e => e.PoNo)
                    .HasMaxLength(16)
                    .HasColumnName("PO_No");

                entity.Property(e => e.Qty)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("QTY");

                entity.Property(e => e.Rate).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Rcm).HasColumnName("RCM");

                entity.Property(e => e.ServiceLine).HasMaxLength(50);

                entity.Property(e => e.Sgst)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SGST");

                entity.Property(e => e.TdsApplicable).HasColumnName("TDS_Applicable");

                entity.Property(e => e.TdsDeclaration).HasColumnName("TDS_Declaration");

                entity.Property(e => e.TdsRate)
                    .HasColumnType("decimal(5, 2)")
                    .HasColumnName("TDS_Rate");

                entity.Property(e => e.TdsSection)
                    .HasMaxLength(255)
                    .HasColumnName("TDS_Section");

                entity.Property(e => e.TdsValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TDS_Value");

                entity.Property(e => e.TotalInvoiceValueInr)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TotalInvoiceValue_INR");

                entity.Property(e => e.TypeOfExpense).HasMaxLength(50);

                entity.Property(e => e.UpdatedTime).HasColumnType("datetime");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ValueInr)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Value_INR");

                entity.Property(e => e.Variance).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Vat)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT");

                entity.Property(e => e.Vch).HasColumnName("vch#");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
