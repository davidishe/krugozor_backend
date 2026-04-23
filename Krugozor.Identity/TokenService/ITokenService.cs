using System.Threading.Tasks;
using Core.Identity;

namespace Krugozor.Identity
{
  public interface ITokenService
  {
    Task<string> CreateToken(AppUser user);
  }
}