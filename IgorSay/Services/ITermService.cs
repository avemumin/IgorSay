using IgorSay.Models;

namespace IgorSay.Services;

public interface ITermService
{
  Task<Dictionary<string, string>> GetTermsAsync();
  Task<Term?> GetByNameAsync(string key);
  Task AddTermAsync(Term term);
  Dictionary<string, string> GetCachedTerms();
  Task ReloadTermsAsync();
}