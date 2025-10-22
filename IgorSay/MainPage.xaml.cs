using System.Text;
using Plugin.Maui.Audio;

namespace IgorSay;

public partial class MainPage : ContentPage
{
  private Dictionary<string, string>? termsDictionary;
  private IAudioPlayer? _currentAudioPlayer;
  private Random random = new Random();
  private bool isAnimating = false;
  private CancellationTokenSource? cts;
  private readonly IAudioManager _audioManager;


  public MainPage(IAudioManager audioManager)
  {
    InitializeComponent();
    LoadTerms();
    DrawButton.Text = "Losuj";
    _audioManager = audioManager;
  }

  private async void OnDrawClicked(object sender, EventArgs e)
  {
    try
    {
      // Odtwarzanie dźwięku w osobnym tasku
      string soundOfKnur = isAnimating ? "shootgun.wav" : "boar.wav";
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
              await DisplayAlert("Błąd", $"Nie udało się odtworzyć dźwięku: {ex.Message}", "OK"));
        }
      });

      if (termsDictionary == null || termsDictionary.Count == 0)
      {
        TermLabel.Text = "Brak haseł";
        ExplanationLabel.Text = "Słownik jest pusty.";
        return;
      }

      if (!isAnimating)
      {
        try
        {
          isAnimating = true;
          DrawButton.Text = "Szczelaj!";
          ExplanationLabel.Text = "";
          TermLabel.Text = "🔥💥 Losowanie...💥🔥";
          cts = new CancellationTokenSource();
          DrawButton.IsEnabled = true;
          await StartAnimationLoopAsync(cts.Token);
        }
        catch (Exception ex)
        {
          isAnimating = false;
          DrawButton.Text = "Losuj!";
          DrawButton.IsEnabled = true;
        }
      }
      else
      {
        try
        {
          cts?.Cancel();
        }
        catch (Exception ex)
        {
          isAnimating = false;
          DrawButton.Text = "Losuj!";
          DrawButton.IsEnabled = true;
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
      List<string> keys = new List<string>(termsDictionary!.Keys);
      uint animationSpeed = 300;

      await BoarMover.TranslateTo(0, 0, animationSpeed, Easing.SinOut);
      await Task.Delay(300, token);
      await BoarMover.TranslateTo(-300, 0, animationSpeed, Easing.SinIn);

      while (true)
      {
        token.ThrowIfCancellationRequested();

        BoarMover.TranslationX = 300;
        TermLabel.Text = isAnimating ? string.Empty : keys[random.Next(keys.Count)];
        
        await BoarMover.TranslateTo(0, 0, animationSpeed, Easing.SinOut);
        await Task.Delay(50 + random.Next(20, 100), token);
        await BoarMover.TranslateTo(-300, 0, animationSpeed, Easing.SinIn);
        await Task.Delay(100, token);
      }
    }
    catch (OperationCanceledException)
    {

      DrawButton.IsEnabled = false;

      List<string> keys = new List<string>(termsDictionary!.Keys);
      string randomTerm = keys[random.Next(keys.Count)];
      string explanation = termsDictionary[randomTerm];


      BoarMover.TranslationX = 300;
      TermLabel.Text = randomTerm;
      await BoarMover.TranslateTo(0, 0, 400, Easing.BounceOut);

      await AnimateTyping(explanation);


      isAnimating = false;
      DrawButton.Text = "Losuj!";
      DrawButton.IsEnabled = true;


    }
    catch (Exception ex)
    {

      isAnimating = false;
      DrawButton.Text = "Losuj!";
      DrawButton.IsEnabled = true;
      TermLabel.Text = "Błąd animacji";
      ExplanationLabel.Text = "Coś poszło nie tak.";
      System.Diagnostics.Debug.WriteLine($"Błąd w AnimateTyping: {ex.Message}");
    }
    finally
    {
      cts?.Dispose();
      cts = null;
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
        ExplanationLabel.Text = stringBuilder.ToString();
        await Task.Delay(typingDelay);
      }

    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Błąd w AnimateTyping: {ex.Message}");
    }
  }

  private void LoadTerms()
  {
    termsDictionary = new Dictionary<string, string>
    {
      ["Afektywność"] = "zdolność do doświadczania emocji",
      ["Anhedonia"] = "brak zdolności do odczuwania przyjemności",
      ["Apatia psychiczna"] = "obniżona motywacja i inicjatywa w działaniu",
      ["Autyzm spektrum"] = "zaburzenia rozwoju wpływające na komunikację: interakcje społeczne i zainteresowania",
      ["Awersja warunkowa"] = "wyuczona reakcja unikania w odpowiedzi na bodziec negatywny",
      ["Behavioralizm (behawioryzm)"] = "nurt psychologiczny skupiający się na obserwowalnych zachowaniach: a nie stanach wewnętrznych",
      ["Cognitywizm"] = "podejście w psychologii koncentrujące się na procesach poznawczych: takich jak myślenie: pamięć: uwaga",
      ["Dysocjacja"] = "oddzielenie pewnych procesów psychicznych od świadomości",
      ["Dysonans poznawczy"] = "napięcie psychiczne wynikające z posiadania sprzecznych przekonań lub zachowań",
      ["Egzaltacja"] = "stan nadmiernego pobudzenia emocjonalnego lub intelektualnego",
      ["Fenomenologia"] = "badanie doświadczeń subiektywnych jednostki",
      ["Flow"] = "stan całkowitego wczucia się w wykonywaną czynność: związany z optymalną motywacją",
      ["Heurystyki poznawcze"] = "uproszczone strategie myślenia i podejmowania decyzji",
      ["Inteligencja emocjonalna"] = "zdolność rozpoznawania: rozumienia i regulowania emocji własnych i innych",
      ["Katastrofizacja"] = "myślenie: które przewiduje najgorszy możliwy scenariusz",
      ["Kognitywna dezintegracja"] = "zaburzenie spójności myślenia i percepcji w schizofrenii",
      ["Labilność emocjonalna"] = "częste i szybkie zmiany nastroju",
      ["Neuroplastyczność"] = "zdolność mózgu do reorganizacji i tworzenia nowych połączeń nerwowych",
      ["Neurotyzm"] = "cecha osobowości związana z podatnością na stres i negatywne emocje",
      ["Obsessje"] = "natrętne myśli: które osoba próbuje ignorować lub tłumić",
      ["Paraliż decyzyjny"] = "niemożność podjęcia decyzji wskutek nadmiaru opcji",
      ["Placebo"] = "substancja lub interwencja wywołująca efekt dzięki oczekiwaniom pacjenta",
      ["Prokrastynacja"] = "odwlekanie wykonania zadania pomimo świadomości negatywnych konsekwencji",
      ["Psychopatia"] = "zaburzenie osobowości charakteryzujące się brakiem empatii: impulsywnością i manipulacją",
      ["Stroop efekt"] = "zjawisko opóźnienia w nazwaniu koloru napisanego słowa: gdy słowo oznacza inny kolor",
      ["Metafizyka"] = "dział filozofii zajmujący się tym: co wykracza poza doświadczenie zmysłowe: np. istota bytu: czas: przestrzeń",
      ["Ontologia"] = "nauka o bycie; bada: co istnieje i w jaki sposób istnieje",
      ["Nihilizm"] = "pogląd odrzucający obiektywne wartości: sens życia i moralność",
      ["Egzystencjalizm"] = "nurt filozoficzny koncentrujący się na jednostce: jej wolności i odpowiedzialności w świecie bez wrodzonego sensu",
      ["Stoicyzm"] = "filozofia ucząca panowania nad emocjami i życia zgodnie z rozumem oraz naturą",
      ["Pragmatyzm"] = "kierunek filozoficzny: według którego wartość idei mierzy się ich praktycznym skutkiem",
      ["Transcendencja"] = "to: co wykracza poza doświadczenie i granice zmysłów; często odnoszone do Boga lub absolutu",
      ["Abstrakcja"] = "ukrywanie szczegółów implementacji i eksponowanie tylko istotnych cech obiektu",
      ["Algorytm"] = "skończony zestaw kroków prowadzących do rozwiązania problemu",
      ["API (Application Programming Interface)"] = "zestaw funkcji i procedur pozwalających na komunikację między programami",
      ["Asynchroniczność"] = "wykonywanie operacji niezależnie od głównego wątku programu",
      ["Binarne drzewo"] = "struktura danych: w której każdy węzeł ma maksymalnie dwóch potomków",
      ["Big O Notation"] = "sposób opisu złożoności obliczeniowej algorytmu",
      ["Blokada (Lock)"] = "mechanizm synchronizacji wątków w programowaniu współbieżnym",
      ["Chmura obliczeniowa (Cloud Computing)"] = "udostępnianie zasobów komputerowych przez internet",
      ["Closure"] = "funkcja: która zachowuje dostęp do zmiennych z kontekstu: w którym została stworzona",
      ["Commit"] = "zapisanie zmian w systemie kontroli wersji: np. Git",
      ["Concurrency (współbieżność)"] = "możliwość wykonywania wielu zadań w tym samym czasie",
      ["Containerization"] = "pakowanie aplikacji i jej zależności w izolowane środowisko (np. Docker)",
      ["Dekorator (Decorator)"] = "wzorzec lub funkcja zmieniająca zachowanie innej funkcji lub klasy",
      ["Dependency Injection"] = "wzorzec: w którym obiekty otrzymują swoje zależności z zewnątrz",
      ["DSL (Domain Specific Language)"] = "język programowania dedykowany określonej dziedzinie",
      ["Encapsulation (enkapsulacja)"] = "ukrywanie danych wewnątrz klasy i udostępnianie ich tylko poprzez metody",
      ["Event Loop"] = "mechanizm obsługujący asynchroniczne zdarzenia",
      ["Garbage Collector"] = "mechanizm automatycznego zwalniania pamięci w niektórych językach programowania",
      ["Generics (szablony)"] = "mechanizm umożliwiający pisanie funkcji i klas działających z różnymi typami danych",
      ["GraphQL"] = "język zapytań do API pozwalający precyzyjnie określić: jakie dane chcemy otrzymać",
      ["Heap (sterta)"] = "obszar pamięci do dynamicznej alokacji obiektów",
      ["Immutable Object"] = "obiekt: którego stan nie może być zmieniony po utworzeniu",
      ["Inheritance (dziedziczenie)"] = "mechanizm pozwalający klasie na odziedziczenie właściwości i metod innej klasy",
      ["Interface"] = "kontrakt: który klasa musi implementować",
      ["IoC (Inversion of Control)"] = "wzorzec: w którym kontrola przepływu programu jest odwrócona",
      ["JSON Web Token (JWT)"] = "standard bezpiecznego przesyłania informacji w formacie JSON",
      ["Lambda"] = "anonimowa funkcja: często wykorzystywana w funkcjach wyższego rzędu",
      ["Lazy Loading"] = "technika opóźnionego ładowania zasobów do momentu ich użycia",
      ["Linked List (lista wiązana)"] = "struktura danych: w której elementy są powiązane wskaźnikami",
      ["Microservices"] = "architektura: w której aplikacja składa się z niezależnych: małych usług",
      ["Monads"] = "abstrakcje w programowaniu funkcyjnym pozwalające na sekwencyjne łączenie operacji",
      ["Multithreading (wielowątkowość)"] = "wykonywanie wielu wątków w ramach jednego procesu",
      ["Mutex"] = "mechanizm zapewniający wzajemne wykluczanie w dostępie do zasobów współdzielonych",
      ["Namespace"] = "przestrzeń nazw zapobiegająca konfliktom nazw w kodzie",
      ["Observer Pattern"] = "wzorzec: w którym obiekty obserwują zmiany stanu innego obiektu",
      ["Polymorphism (polimorfizm)"] = "możliwość traktowania obiektów różnych klas w ten sam sposób",
      ["Queue (kolejka)"] = "struktura danych FIFO (First In: First Out)",
      ["Recursion (rekurencja)"] = "funkcja wywołująca samą siebie",
      ["Refactoring"] = "poprawa struktury kodu bez zmiany jego zachowania",
      ["Regex (wyrażenia regularne)"] = "sposób definiowania wzorców do wyszukiwania w tekstach",
      ["REST API"] = "architektura komunikacji między aplikacjami poprzez protokół HTTP",
      ["Singleton"] = "wzorzec: który pozwala na istnienie tylko jednej instancji klasy",
      ["Stack (stos)"] = "struktura danych LIFO (Last In: First Out)",
      ["Streaming"] = "przetwarzanie danych w strumieniu zamiast w całości",
      ["Epistemologia"] = "teoria wiedzy",
      ["Aksjologia"] = "teoria wartości",
      ["Determinizm"] = "pogląd, że wszystko jest z góry określone",
      ["Hedonizm"] = "cel życia w przyjemności",
      ["Algebra"] = "struktury i równania",
      ["Analiza matematyczna"] = "badanie funkcji i granic",
      ["Rachunek różniczkowy"] = "analiza zmian funkcji",
      ["Rachunek całkowy"] = "obliczanie pól, objętości",
      ["Kombinatoryka"] = "liczenie sposobów łączenia elementów",
      ["Logarytm"] = "funkcja odwrotna do potęgowania",
      ["Macierz"] = "tablica liczb",
      ["Wektor"] = "obiekt z kierunkiem i długością",
      ["Równanie różniczkowe"] = "zależność między funkcją a jej pochodną",
      ["Fraktal"] = "obiekt o samopodobnej strukturze",
      ["Topologia"] = "badanie własności przestrzeni przy deformacjach",
      ["Hipoteza"] = "przypuszczenie wymagające dowodu",
      ["Twierdzenie"] = "stwierdzenie udowodnione logicznie",
      ["Permutacja"] = "różne ustawienia elementów",
      ["Kombinacja"] = "wybór elementów bez kolejności",
      ["Determinant"] = "liczba opisująca macierz",
      ["Entropia"] = "miara nieuporządkowania",
      ["Kwantyzacja"] = "podział energii na dyskretne jednostki",
      ["Relatywizm"] = "zależność obserwacji od układu odniesienia",
      ["Rezonans"] = "wzmocnienie drgań",
      ["Dyfuzja"] = "samoistne rozprzestrzenianie się cząsteczek",
      ["Dywersyfikacja"] = "rozproszenie inwestycji dla bezpieczeństwa",
      ["Amortyzacja"] = "stopniowe rozliczanie kosztów zakupu",
      ["Empatia"] = "zdolność wczuwania się w innych",
      ["Etymologia"] = "pochodzenie wyrazów",
      ["Bies"] = "demon w wierzeniach ludowych",
      ["Pyra"] = "ziemniak (regionalizm)",
      ["zwierzyna gruba"] = "dzik, jeleń, łoś",
      ["Zwierzyna drobna"] = "zając, bażant, kuropatwa",
      ["Zwierzyna płowa"] = "jelenie, sarny, daniele",
      ["Zwierzyna drapieżna"] = "lis, wilk, kuna",
      ["Zwierzyna ptasia"] = "kaczki, gęsi, bażanty, kuropatwy",
      ["Obwód łowiecki"] = "jednostka organizacyjna łowiectwa, wydzielony teren przypisany kołu łowieckiemu.",
      ["Kołek graniczny"] = "oznaczenie granicy obwodu łowieckiego.",
      ["Droga zwierzyny"] = "regularnie uczęszczany szlak migracyjny zwierząt.",
      ["Trop / podchód"] = "ślady pozostawione przez zwierzynę; podchód to także technika zbliżania się do niej.",
      ["Gody / rykowisko"] = "okres rozrodczy zwierzyny, np. rykowisko jeleni.",
      ["Dymorfizm płciowy"] = "różnice w wyglądzie samca i samicy danego gatunku.",
      ["Sezon lęgowy"] = "czas, w którym zwierzyna rozmnaża się i wychowuje młode.",
      ["Migracje zwierzyny"] = "przemieszczanie się zwierząt w poszukiwaniu pożywienia lub schronienia.",
      ["Dzik"] = "samiec: Odyniec, samica: Locha, Młode: Warchlak",
      ["Jeleń szlachetny"] = "samiec: Byk, samica: Łania, Młode: Cielę",
      ["Jeleń sika"] = "samiec: Byk, samica: Łania, Młode: Cielę",
      ["Łoś"] = "samiec: Byk, samica: Łania, Młode: Cielę",
      ["Sarna"] = "samiec: Kozioł, samica: Koza, Młode: Koźlę",
      ["Zając szarak"] = "samiec: Zając, samica: Zającica, Młode: Zające",
      ["Bóbr"] = "samiec: Bóbr, samica: Bobrzyca, Młode: Młode bobry",
      ["Blaze a trail"] = "przetrzyj szlaki to metafora odnosząca się do pionierskiego działania, dokonywania czegoś po raz pierwszy, np. tworzenia nowej trasy, odkrywania czegoś lub inicjowania nowego trendu",
      ["Aberracja "] = "to odchylenie od normy lub zasady",
      ["To co naturalne, nie może być złe"] = "chęć podążania człowieka za swoją naturą nie może być zła",
      ["Aproksymacja"] = "ujęcie czegoś w sposób niezupełnie ścisły; przybliżenie",
      ["Konundrum"] = "łamigłówka, skomplikowana zagadka",
      ["Inkorporować"] = "włączenie do jakiejś całości, zwykle przyłączenie jakiegoś terytorium do innego",
      ["Lakoniczny"] = "zwięźle wyrażony",
      ["Lapidarny"] = "krótki, zwięzły, ale jednocześnie odzwierciedlający kwintesencję czyichś myśli",
      ["Ekscentryk"] = "osoba wyróżniająca się poglądami, dziwacznym zachowaniem lub stylem bycia",
      ["Heurystyka"] = "metoda znajdowania rozwiązań bez gwarancji ich optymalności lub poprawności",
      ["Obstrukcja"] = "bierny opór przy użyciu dozwolonych prawem metod",
      ["Atawizm"] = "ponowne pojawienie się cech lub zachowań charakterystycznych dla odległych przodków",
      ["Eskapizm"] = "ucieczka od problemów życia codziennego w świat iluzji i fantazji",
      ["Kolektyw"] = "grupa osób związanych wspólnym działaniem, zespół",
      ["Koincydencja"] = "jednoczesne wystąpienie czegoś, zbieżność zjawisk lub zdarzeń, ich współwystępowanie",
      ["Konformizm"] = "postawa kogoś, kto bezkrytycznie godzi się z obowiązującymi normami, wartościami i poglądami, podporządkowując się im w oczekiwaniu korzyści lub z braku krytycyzmu.",
      ["Infantylny"] = "coś, co jest prymitywne, banalne, pozbawione głębszych treści.",
      ["Demagogia"] = "to głoszenie treści odwołujących się do emocji i oczekiwań ich odbiorców, mające na celu osiągnięcie własnych korzyści, uzyskanie powszechnego uznania i pozyskanie zwolenników."
    };

  }

}
