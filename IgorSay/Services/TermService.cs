using IgorSay.Models;

namespace IgorSay.Services;

public class TermService : ITermService
{
  private Dictionary<string, string> dictionary = new();
  public TermService()
  {
    if (dictionary is null || dictionary.Count == 0)
      GenerateDictionary();
  }
  public async Task<Dictionary<string, string>> GetTermsAsync()
  {
    return await Task.FromResult(dictionary);
  }

  public async Task GetByNameAsync(Term term)
  {
    // Implementacja w przyszłości dla bazy danych
    throw new NotImplementedException();
  }

  public async Task AddTermAsync(Term term)
  {
    if (term != null && !string.IsNullOrWhiteSpace(term.key) && !string.IsNullOrWhiteSpace(term.value))
    {
      dictionary[term.key] = term.value;
      await Task.CompletedTask; // Symulacja asynchroniczności
    }
    else
    {
      throw new ArgumentException("Termin lub wartość nie może być pusta.");
    }
  }

  private Dictionary<string, string> GenerateDictionary()
  {
    dictionary = new Dictionary<string, string>
    {
      ["Afektywność"] = "zdolność do doświadczania emocji",
      ["Anhedonia"] = "brak zdolności do odczuwania przyjemności",
      ["Apatia psychiczna"] = "obniżona motywacja i inicjatywa w działaniu",
      ["Autyzm spektrum"] = "zaburzenia rozwoju wpływające na komunikację: interakcje społeczne i zainteresowania",
      ["Awersja warunkowa"] = "wyuczona reakcja unikania w odpowiedzi na bodziec negatywny"
    };
    return dictionary;
  }
}