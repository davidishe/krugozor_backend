using System.Threading.Tasks;

namespace Krugozor.Identity.Services
{
  public interface IRoleManagerService
  {

    Task<bool> ChangeUserRoles(string[] roles, string userId);

  }
}