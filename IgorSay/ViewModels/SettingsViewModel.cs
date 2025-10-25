using System.Windows.Input;

namespace IgorSay.ViewModels;

public class SettingsViewModel : BaseViewModel
{
  private string _externalAddress;

  public string ExternalAddress
  {
    get => _externalAddress;
    set => SetProperty(ref _externalAddress, value);
  }

  public ICommand SaveCommand { get; }

  public SettingsViewModel()
  {
    _externalAddress = Preferences.Get("ExternalAddress", string.Empty);
    SaveCommand = new Command(OnSave, () => !string.IsNullOrWhiteSpace(ExternalAddress));
  }

  private void OnSave()
  {
    Preferences.Set("ExternalAddress", ExternalAddress);
    ExternalAddress = string.Empty;
  }
}
