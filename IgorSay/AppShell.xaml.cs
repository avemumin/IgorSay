namespace IgorSay
{
  public partial class AppShell : Shell
  {
    public AppShell()
    {
      InitializeComponent();
      Routing.RegisterRoute("ModerationPage", typeof(IgorSay.Views.ModerationPage));
    }
  }
}
