using IgorSay.Services;
using IgorSay.ViewModels;
using Plugin.Maui.Audio;

namespace IgorSay;

public partial class MainPage : ContentPage
{

  private readonly ContentView _gameView;
  private readonly ContentView _addPasswordView;
  private readonly ContentView _settingsView;
  public MainPage(IAudioManager audioManager, ITermService termService)
  {
    InitializeComponent();
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
}

