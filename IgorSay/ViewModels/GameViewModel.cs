using System.Text;
using System.Windows.Input;
using IgorSay.Services;
using Plugin.Maui.Audio;

namespace IgorSay.ViewModels;

public class GameViewModel : BaseViewModel
{
  private readonly IAudioManager _audioManager;
  private readonly ITermService _termService;
  private readonly GameView _view; // Referencja do widoku dla animacji
  private IAudioPlayer? _currentAudioPlayer;
  private readonly Random _random = new();
  private bool _isAnimating;
  private CancellationTokenSource? _cts;
  private string _termText;
  private string _explanationText;
  private string _drawButtonText;
  private bool _drawButtonEnabled;
  private Dictionary<string, string>? _termsDictionary;

  public string TermText
  {
    get => _termText;
    set => SetProperty(ref _termText, value);
  }

  public string ExplanationText
  {
    get => _explanationText;
    set => SetProperty(ref _explanationText, value);
  }

  public string DrawButtonText
  {
    get => _drawButtonText;
    set => SetProperty(ref _drawButtonText, value);
  }

  public bool DrawButtonEnabled
  {
    get => _drawButtonEnabled;
    set => SetProperty(ref _drawButtonEnabled, value);
  }

  public ICommand DrawCommand { get; }

  public GameViewModel(IAudioManager audioManager, ITermService termService, GameView view)
  {
    _audioManager = audioManager;
    _termService = termService;
    _view = view;
    DrawButtonText = "Losuj";
    DrawButtonEnabled = true;
    TermText = "Witaj!";
    ExplanationText = "Kliknij przycisk, aby wylosować hasło.";
    DrawCommand = new Command(OnDrawClicked, () => DrawButtonEnabled);

    // Inicjalizuj słownik asynchronicznie
    //  InitializeTermsAsync().GetAwaiter().GetResult();
    MainThread.BeginInvokeOnMainThread(async () => await InitializeTermsAsync());
  }
  public void ReloadTerms()
  {
    _termsDictionary = _termService.GetCachedTerms();
  }
  private async Task InitializeTermsAsync()
  {
    try
    {
      var cached = _termService.GetCachedTerms();
      if (cached.Count > 0)
      {
        _termsDictionary = cached;
      }
      else
      {
        _termsDictionary = await _termService.GetTermsAsync();
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Błąd podczas inicjalizacji słownika: {ex.Message}");
      TermText = "Błąd inicjalizacji";
      ExplanationText = "Nie udało się załadować haseł.";
    }
  }

  private async void OnDrawClicked()
  {
    try
    {
      _view.SetBoarImage("boar3.png");
      // Odtwarzanie dźwięku w osobnym tasku
      string soundOfKnur = _isAnimating ? "shootgun.wav" : "boar.wav";
      _ = Task.Run(async () =>
      {
        try
        {
          if (_currentAudioPlayer != null && _currentAudioPlayer.IsPlaying)
          {
            _currentAudioPlayer.Stop();
            _currentAudioPlayer.Dispose();
          }
          _currentAudioPlayer = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(soundOfKnur));
          _currentAudioPlayer.Play();
        }
        catch (Exception ex)
        {
          await MainThread.InvokeOnMainThreadAsync(async () =>
              await Application.Current.MainPage.DisplayAlert("Błąd", $"Nie udało się odtworzyć dźwięku: {ex.Message}", "OK"));
        }
      });

      if (_termsDictionary == null || _termsDictionary.Count == 0)
      {
        TermText = "Brak haseł";
        ExplanationText = "Słownik jest pusty.";
        return;
      }

      if (!_isAnimating)
      {
        try
        {
          _isAnimating = true;
          DrawButtonText = "Szczelaj!";
          ExplanationText = "";
          TermText = "🔥💥 Losowanie...💥🔥";
          _cts = new CancellationTokenSource();
          DrawButtonEnabled = true;
          await StartAnimationLoopAsync(_cts.Token);
        }
        catch (Exception ex)
        {
          _isAnimating = false;
          DrawButtonText = "Losuj!";
          DrawButtonEnabled = true;
          System.Diagnostics.Debug.WriteLine($"Błąd w OnDrawClicked: {ex.Message}");
        }
      }
      else
      {
        try
        {
          _cts?.Cancel();
        }
        catch (Exception ex)
        {
          _isAnimating = false;
          DrawButtonText = "Losuj!";
          DrawButtonEnabled = true;
          System.Diagnostics.Debug.WriteLine($"Błąd w OnDrawClicked (cancel): {ex.Message}");
        }
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Błąd ogólny w OnDrawClicked: {ex.Message}");
    }
  }

  private async Task StartAnimationLoopAsync(CancellationToken token)
  {
    try
    {
      List<string> keys = new List<string>(_termsDictionary!.Keys);
      uint animationSpeed = 300;

      await _view.AnimateBoarAsync(token, animationSpeed);

      while (true)
      {
        token.ThrowIfCancellationRequested();

        _view.SetBoarMoverTranslationX(300); // Użyj nowej metody
        TermText = _isAnimating ? string.Empty : keys[_random.Next(keys.Count)];
        await _view.AnimateBoarAsync(token, animationSpeed);
        await Task.Delay(50 + _random.Next(20, 100), token);
        await Task.Delay(100, token);
      }
    }
    catch (OperationCanceledException)
    {
      _view.SetBoarImage("trup.png");
      DrawButtonEnabled = false;

      List<string> keys = new List<string>(_termsDictionary!.Keys);
      string randomTerm = keys[_random.Next(keys.Count)];
      string explanation = _termsDictionary[randomTerm];

      TermText = randomTerm;
      await _view.FinalizeBoarAnimationAsync();
      await AnimateTyping(explanation);

      _isAnimating = false;
      DrawButtonText = "Losuj!";
      DrawButtonEnabled = true;
    }
    catch (Exception ex)
    {
      _isAnimating = false;
      DrawButtonText = "Losuj!";
      DrawButtonEnabled = true;
      TermText = "Błąd animacji";
      ExplanationText = "Coś poszło nie tak.";
      System.Diagnostics.Debug.WriteLine($"Błąd w StartAnimationLoopAsync: {ex.Message}");
    }
    finally
    {
      _cts?.Dispose();
      _cts = null;
    }
  }

  private async Task AnimateTyping(string text)
  {
    try
    {
      var stringBuilder = new StringBuilder();
      int typingDelay = 25;

      foreach (char c in text)
      {
        stringBuilder.Append(c);
        ExplanationText = stringBuilder.ToString();
        await Task.Delay(typingDelay);
      }
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Błąd w AnimateTyping: {ex.Message}");
    }
  }
}