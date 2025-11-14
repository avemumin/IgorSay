using IgorSay.Models;

namespace IgorSay.Services;

public interface IModeratorService
{
  Task<bool> LoginAsync(string login, string password);
  Task<bool> IsModeratorAsync();
  Task ApproveTermAsync(Term term);
}
