using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IgorSay.Models;
using IgorSay.Services;

namespace IgorSay.ViewModels;

public class AddPasswordViewModel : BaseViewModel
{
  private string _key;
  private string _value;
  private readonly ITermService _termService;

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

  public AddPasswordViewModel(ITermService termService)
  {
    _termService = termService;
    SaveCommand = new Command(async () => await OnSaveAsync(), () => !string.IsNullOrWhiteSpace(Key) && !string.IsNullOrWhiteSpace(Value));
  }

  private async Task OnSaveAsync()
  {
    try
    {
      await _termService.AddTermAsync(new Term(Key, Value));
      Key = string.Empty;
      Value = string.Empty;
      await Application.Current.MainPage.DisplayAlert("Sukces", "Hasło dodane!", "OK");
    }
    catch (Exception ex)
    {
      await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się dodać hasła: {ex.Message}", "OK");
    }
  }
}
