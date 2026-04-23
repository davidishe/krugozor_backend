using Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Krugozor.Core.Models;
using Krugozor.Core.Models.Messages;

namespace Krugozor.Infrastructure.Database
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
      Database.EnsureCreated();
    }

    public DbSet<ProposalProfile> ProposalProfiles { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<RequestStatus> RequestStatuses { get; set; }
    public DbSet<ProposalProfileStatus> ProposalProfileStatuses { get; set; }
    public DbSet<Favour> Favours { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<Correspondent> Correspondents { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Chat> Chats { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

  }

}
