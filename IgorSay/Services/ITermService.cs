using IgorSay.Models;

namespace IgorSay.Services;

public interface ITermService
{
  Task<Dictionary<string, string>> GetTermsAsync();
  Task GetByNameAsync(Term term);
  Task AddTermAsync(Term term);
}