namespace IgorSay;

public partial class AddPasswordView : ContentView
{
  private readonly Dictionary<string, string> _passwords = new();
  public AddPasswordView()
  {
    InitializeComponent();
  }
  private void OnSaveClicked(object sender, EventArgs e)
  {
    if (!string.IsNullOrWhiteSpace(KeyEntry.Text) && !string.IsNullOrWhiteSpace(ValueEntry.Text))
    {
      _passwords[KeyEntry.Text] = ValueEntry.Text;
      KeyEntry.Text = string.Empty;
      ValueEntry.Text = string.Empty;
      // Opcjonalnie: Wy�wietl potwierdzenie lub zapisz do pliku/bazy danych
      DisplayAlert("Sukces", "Has�o dodane!", "OK");
    }
    else
    {
      DisplayAlert("B��d", "Wype�nij oba pola!", "OK");
    }
  }

  private async void DisplayAlert(string title, string message, string cancel)
  {
    await Application.Current.MainPage.DisplayAlert(title, message, cancel);
  }
}