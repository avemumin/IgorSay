using System.Collections.ObjectModel;
using System.Windows.Input;
using IgorSay.Models;

namespace IgorSay.ViewModels;

public class ModerationViewModel : BaseViewModel
{
  private ObservableCollection<Term> _pendingTerms = new();
  public ObservableCollection<Term> PendingTerms
  {
    get => _pendingTerms;
    set => SetProperty(ref _pendingTerms, value);
  }

  public ICommand ApproveCommand { get; }
  public ICommand RejectCommand { get; }

  private readonly Supabase.Client _client;
  public ModerationViewModel(Supabase.Client client)
  {
    _client = client;
    ApproveCommand = new Command<Term>(Approve);
    RejectCommand = new Command<Term>(Reject);

    // LoadPendingTerms();

  }

  public async Task LoadPendingTermsAsync()
  {
    var response = await _client
      .From<Term>()
      .Where(t => t.Approved == false)
      .Get();

    MainThread.BeginInvokeOnMainThread(() =>
    {
      PendingTerms = new ObservableCollection<Term>(response.Models);
    });
  }


  private async void Approve(Term term)
  {
    var response = await _client
        .From<Term>()
        .Where(t => t.Id == term.Id)
        .Set(t => t.Approved, true)
        .Update();

    if (response.Models.Any())
    {
        PendingTerms.Remove(term);
    }
  }

  private async void Reject(Term term)
  {
    await _client.From<Term>().Delete(term);
    PendingTerms.Remove(term);
  }
}