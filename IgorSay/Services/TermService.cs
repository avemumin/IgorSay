using IgorSay.Models;
namespace IgorSay.Services;

public class TermService : ITermService
{
  private Dictionary<string, string> _termsCache = new();
  private readonly Supabase.Client _client;
  public TermService(Supabase.Client client)
  {
    _client = client;
    Task.Run(async () => await _client.InitializeAsync());
  }
  public async Task<Dictionary<string, string>> GetTermsAsync()
  {
    var result = await _client.From<Term>()
      .Where(t => t.Approved == true)
      .Get();
    _termsCache = result.Models.ToDictionary(t => t.Key, t => t.Value);
    return _termsCache;
    //result.Models.ToDictionary(t => t.Key, t => t.Value);
  }

  public async Task<Term?> GetByNameAsync(string key)
  {
    try
    {
      var result = await _client
          .From<Term>()
          .Filter("key", Postgrest.Constants.Operator.Equals, key)
          .Get();

      return result.Models.FirstOrDefault();
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[GetByNameAsync] Błąd: {ex.Message}");
      throw;
    }
  }

  public async Task AddTermAsync(Term term)
  {
    try
    {
      var existing = await GetByNameAsync(term.Key);
      System.Diagnostics.Debug.WriteLine($"[AddTermAsync] Istniejący: {(existing != null ? "tak" : "nie")}");
      if (existing != null)
        throw new InvalidOperationException("Termin już istnieje.");

      await _client.From<Term>().Insert(term);
      _termsCache[term.Key] = term.Value;
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[AddTermAsync] Błąd: {ex.Message}");
      throw;
    }
  }
  public Dictionary<string, string> GetCachedTerms() => _termsCache;
  public async Task ReloadTermsAsync()
  {
    var result = await _client.From<Term>().Get();
    _termsCache = result.Models.ToDictionary(t => t.Key, t => t.Value);
  }

}