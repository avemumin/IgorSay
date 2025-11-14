using System.Windows.Input;

namespace IgorSay.ViewModels;

public class MainViewModel
{
  public ICommand SecretMenuCommand { get; }

  public MainViewModel()
  {
    SecretMenuCommand = new Command(OpenSecretMenu);
  }

  private async void OpenSecretMenu()
  {
    await Shell.Current.GoToAsync("ModerationPage");
  }
}