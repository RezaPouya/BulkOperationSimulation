using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Operator.Models.Configurations
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