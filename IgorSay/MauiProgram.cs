using IgorSay.Models;
using IgorSay.Services;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using Supabase;

namespace IgorSay
{
  public static class MauiProgram
  {
    public static MauiApp CreateMauiApp()
    {

      var builder = MauiApp.CreateBuilder();
      builder
          .UseMauiApp<App>()
          .AddAudio()
          .ConfigureFonts(fonts =>
          {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
          });
      //    var con = builder.Services.AddSingleton<AppSettings>();
      var settings = new AppSettings();
      builder.Services.AddSingleton(settings);
      builder.Services.AddSingleton<IAudioManager, AudioManager>();
      builder.Services.AddSingleton<ITermService, TermService>();

      builder.Services.AddScoped<Supabase.Client>(provider =>
      {
        var config = provider.GetRequiredService<AppSettings>();
        return new Supabase.Client(
          config.ApiUrl,
          config.SupabaseKey,
          new SupabaseOptions
          {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
          });
      });
#if DEBUG
      builder.Logging.AddDebug();
#endif


      return builder.Build();
    }
  }
}
