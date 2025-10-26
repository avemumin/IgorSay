using IgorSay.Services;
using IgorSay.ViewModels;
using Plugin.Maui.Audio;

namespace IgorSay;

public partial class GameView : ContentView
{
  public GameView(IAudioManager audioManager, ITermService termService)
  {
    InitializeComponent();
    BindingContext = new GameViewModel(audioManager, termService, this);
  }

  public void SetBoarMoverTranslationX(double translationX)
  {
    BoarMover.TranslationX = translationX;
  }
  public async Task AnimateBoarAsync(CancellationToken token, uint animationSpeed)
  {
    try
    {
      await BoarMover.TranslateTo(0, 0, animationSpeed, Easing.SinOut);
      await Task.Delay(300, token);
      await BoarMover.TranslateTo(-300, 0, animationSpeed, Easing.SinIn);
    }
    catch (OperationCanceledException)
    {
      // Ignoruj anulowanie
    }
  }

  public async Task FinalizeBoarAnimationAsync()
  {
    BoarMover.TranslationX = 300;
    await BoarMover.TranslateTo(0, 0, 400, Easing.BounceOut);
  }

  public void SetBoarImage(string imageName)
  {
    BoarImage.Source = imageName;
  }
}