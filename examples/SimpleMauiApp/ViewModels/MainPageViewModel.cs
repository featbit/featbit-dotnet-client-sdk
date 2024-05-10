using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SimpleMauiApp.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    private int _delta;
    private int _counter;

    private string _welcomeMessage;
    private string _deltaText;
    private string _counterText;

    public string WelcomeMessage
    {
        get => _welcomeMessage;
        private set
        {
            _welcomeMessage = value;
            OnPropertyChanged();
        }
    }

    public string DeltaText
    {
        get => _deltaText;
        private set
        {
            _deltaText = value;
            OnPropertyChanged();
        }
    }

    public string CounterText
    {
        get => _counterText;
        private set
        {
            _counterText = value;
            OnPropertyChanged();
        }
    }

    public MainPageViewModel()
    {
        IncrementCommand = new Command(() =>
        {
            _counter += _delta;
            CounterText = GetCounterText(_counter);
        });

        var fbClient = FeatBit.Instance;

        _delta = fbClient.IntVariation("counter-delta", defaultValue: 1);
        _deltaText = GetDeltaText(_delta);

        _counter = 0;
        _counterText = GetCounterText(_counter);

        var version = fbClient.StringVariation("welcome-message-version", defaultValue: "v0");
        _welcomeMessage = GetWelcomeMessage(version);

        fbClient.FlagTracker.Subscribe("counter-delta", changedEvent =>
        {
            if (!int.TryParse(changedEvent.NewValue, out var delta))
            {
                return;
            }

            _delta = delta;
            DeltaText = GetDeltaText(_delta);
        });

        fbClient.FlagTracker.Subscribe(
            "welcome-message-version",
            changedEvent => WelcomeMessage = GetWelcomeMessage(changedEvent.NewValue)
        );
    }

    public ICommand IncrementCommand { private set; get; }

    private static string GetWelcomeMessage(string version) => $"[{version}] Welcome to .NET Multi-platform App UI";

    private static string GetDeltaText(int delta) => $"Delta: {delta}";

    private static string GetCounterText(int count) =>
        count switch
        {
            0 => "Click me!",
            1 => "Clicked 1 time.",
            _ => $"Clicked {count} times."
        };

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}