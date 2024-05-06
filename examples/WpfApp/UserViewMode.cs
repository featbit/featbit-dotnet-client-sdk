using System.ComponentModel;

namespace WpfApp;

public class UserViewMode : INotifyPropertyChanged
{
    private string _userLayout;

    public UserViewMode()
    {
        var evalDetail = FeatBit.Instance.StringVariationDetail("user-layout", "fallback");
        _userLayout = $"{evalDetail.Value} ({evalDetail.Reason})";

        FeatBit.Instance.FlagTracker.Subscribe(
            "user-layout",
            valueChangedEvent => UserLayout = valueChangedEvent.NewValue
        );
    }

    public string UserLayout
    {
        get { return _userLayout; }
        set
        {
            _userLayout = value;
            OnPropertyChanged("UserLayout");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}