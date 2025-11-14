using System.Security.Cryptography;
using System.Security.Principal;
using IgorSay.Services;
using IgorSay.ViewModels;
using Plugin.Maui.Audio;

namespace IgorSay;

public partial class MainPage : ContentPage
{
 
  private int _tapCount = 0;
  private readonly ContentView _gameView;
  private readonly ContentView _addPasswordView;
  private readonly ContentView _settingsView;
  private readonly IModeratorService _moderatorService;
  public MainPage(IAudioManager audioManager, ITermService termService, IModeratorService moderatorService)
  {
    InitializeComponent();
    _moderatorService = moderatorService;
    _gameView = new GameView(audioManager, termService);
    _addPasswordView = new AddPasswordView() { BindingContext = new AddPasswordViewModel(termService, SwitchToGameView) };

    MainContentView.Content = _gameView;
  }

  public void SwitchToGameView()
  {
    MainContentView.Content = _gameView;
    if (_gameView.BindingContext is GameViewModel gameVm)
    {
      gameVm.ReloadTerms();
    }
  }

  private void OnAddClicked(object sender, EventArgs e)
  {
    MainContentView.Content = _addPasswordView;
  }

  private async void OnSecretTapped(object sender, EventArgs e)
  {
    _tapCount++;

    if (_tapCount >= 7) // po 7 kliknięciach
    {
      _tapCount = 0; // reset licznika
      ModeratorLoginPanel.IsVisible = true;
      /*await Shell.Current.GoToAsync("ModerationPage");*/ // przejście do ukrytej strony
    }
  }

  private async void OnPasswordSubmit(object sender, EventArgs e)
  {
    var login = ModeratorEntry.Text?.Trim();
    var password = PasswordEntry.Text;

    if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
    {
      await DisplayAlert("Błąd", "Wypełnij pola", "OK");
      return;
    }

    var success = await _moderatorService.LoginAsync(login, password);
    if (success)
    {
      ModeratorLoginPanel.IsVisible = false;
      await Shell.Current.GoToAsync("ModerationPage");
    }
    else
    {
      await DisplayAlert("Błąd", "Zły login lub hasło", "OK");
    }

    PasswordEntry.Text = string.Empty;
  }

}

