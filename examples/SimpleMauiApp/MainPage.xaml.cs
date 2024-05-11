using FeatBit.Sdk.Client.Model;

namespace SimpleMauiApp;

public partial class MainPage : ContentPage
{
    private bool _isLoggedIn;
    private static readonly FbUser AuthorizedUser = FbUser.Builder("authorized-id").Name("authorized").Build();

    public MainPage()
    {
        InitializeComponent();
    }

    private async void ToggleLogin(object? sender, EventArgs e)
    {
        if (_isLoggedIn)
        {
            await FeatBit.LogoutAsync();

            CurrentUserLabel.Text = "Anonymous";
            ToggleLoginButton.Text = "Login";

            _isLoggedIn = false;
        }
        else
        {
            await FeatBit.LoginAsync(AuthorizedUser);

            CurrentUserLabel.Text = "Authorized";
            ToggleLoginButton.Text = "Logout";

            _isLoggedIn = true;
        }
    }
}