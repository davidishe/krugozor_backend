using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Krugozor.Core.Models;

namespace Infrastructure.Data.Config
{
  public class ProfileConfiguration : IEntityTypeConfiguration<ProposalProfile>
  {
    public void Configure(EntityTypeBuilder<ProposalProfile> builder)
    {
      builder.Property(p => p.Id).IsRequired();
      builder.Property(p => p.StrapiCompanyId).IsRequired();
      builder.Property(p => p.CreatedAt).IsRequired();

    }
  }
}