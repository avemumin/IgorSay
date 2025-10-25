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
    _settingsView = new SettingsView { BindingContext = new SettingsViewModel() };

    MainContentView.Content = _gameView;
  }

  public void SwitchToGameView()
  {
    MainContentView.Content = _gameView;
  }

  private void OnAddClicked(object sender, EventArgs e)
  {
    MainContentView.Content = _addPasswordView;
  }
}

