namespace IgorSay;

public partial class SettingsView : ContentView
{
  public SettingsView()
  {
    InitializeComponent();
  }
  private void OnSaveSettingsClicked(object sender, EventArgs e)
  {
    if (!string.IsNullOrWhiteSpace(AddressEntry.Text))
    {
      // Zapisz adres, np. do Preferences lub pliku
      Preferences.Set("ExternalAddress", AddressEntry.Text);
      AddressEntry.Text = string.Empty;
      DisplayAlert("Sukces", "Adres zapisany!", "OK");
    }
    else
    {
      DisplayAlert("B≥πd", "Wprowadü adres!", "OK");
    }
  }

  private async void DisplayAlert(string title, string message, string cancel)
  {
    await Application.Current.MainPage.DisplayAlert(title, message, cancel);
  }
}