using System.Windows.Input;
using IgorSay.Models;
using IgorSay.Services;

namespace IgorSay.ViewModels;

public class AddPasswordViewModel : BaseViewModel
{
  private string _key;
  private string _value;
  private readonly ITermService _termService;
  private readonly Action _switchToGameView;
  
  public string Key
  {
    get => _key;
    set => SetProperty(ref _key, value);
  }

  public string Value
  {
    get => _value;
    set => SetProperty(ref _value, value);
  }

  public ICommand SaveCommand { get; }
  public ICommand PlayCommand { get; }
  public AddPasswordViewModel(ITermService termService,Action switchToGameView)
  {
    _switchToGameView = switchToGameView;
    _termService = termService;
    SaveCommand = new Command(async () => await OnSaveAsync(), () => !string.IsNullOrWhiteSpace(Key) && !string.IsNullOrWhiteSpace(Value));
    PlayCommand = new Command(() => _switchToGameView?.Invoke());
    
  }
 
  private async Task OnSaveAsync()
  {
    System.Diagnostics.Debug.WriteLine($"[OnSaveAsync] Start: {Key} → {Value}");
    try
    {
      await _termService.AddTermAsync(new Term(Key, Value));
      System.Diagnostics.Debug.WriteLine("[OnSaveAsync] Dodano termin");
      Key = string.Empty;
      Value = string.Empty;
      await Application.Current.MainPage.DisplayAlert("Sukces", "Hasło dodane!", "OK");
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"[OnSaveAsync] Błąd: {ex.Message}");
      await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się dodać hasła: {ex.Message}", "OK");
    }
  }
}
