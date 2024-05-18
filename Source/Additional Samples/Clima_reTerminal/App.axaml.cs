using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Clima_reTerminal.ViewModels;
using Clima_reTerminal.Views;

namespace Clima_reTerminal
{
    public partial class App : Application
    {
        public App()
        {
            RegisterServices();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                    //WindowState = Avalonia.Controls.WindowState.FullScreen
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}