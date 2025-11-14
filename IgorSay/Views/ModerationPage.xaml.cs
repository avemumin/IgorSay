using IgorSay.ViewModels;

namespace IgorSay.Views;

public partial class ModerationPage : ContentPage
{
   

  public ModerationPage(Supabase.Client client)
  {
    InitializeComponent();

    
    BindingContext = new ModerationViewModel(client);
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    await (BindingContext as ModerationViewModel)?.LoadPendingTermsAsync();
  }
}
