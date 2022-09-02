using Domain.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.DataAccess.Configurations
{
    internal class ExcelDbEntityTypeConfiguration : IEntityTypeConfiguration<ExcelDb>
    {
        public void Configure(EntityTypeBuilder<ExcelDb> builder)
        {
            builder.ToTable("ExcelDbs");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedNever();

        }
    }
}