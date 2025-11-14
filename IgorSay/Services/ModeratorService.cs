using IgorSay.Models;

namespace IgorSay.Services;

public class ModeratorService : IModeratorService
{
  private readonly Supabase.Client _client;
  public ModeratorService(Supabase.Client client)
  {
    _client = client;
  }


  public async Task<bool> LoginAsync(string login, string password)
  {
    try
    {
      var session = await _client.Auth.SignIn(login, password);
      return session?.User != null;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Błąd logowania: {ex.Message}");
      return false;
    }
  }

  public async Task<bool> IsModeratorAsync()
  {
    var user = _client.Auth.CurrentUser;
    if (user == null) return false;

    // user.Id → Guid
    // m.UserId → Guid
    var response = await _client
        .From<Moderator>()
        .Where(m => m.UserId == user.Id)  // Guid == Guid → OK!
        .Get();

    return response.Models.Any();
  }

  public async Task ApproveTermAsync(Term term)
  {
    if (term?.Id <= 0) return;
    if (!await IsModeratorAsync()) return;

    term.Approved = true;

    try
    {
      await _client
          .From<Term>()
          .Where(x => x.Id == term.Id)
          .Update(term);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Approve error: {ex.Message}");
    }
  }
}
