using Xamarin.Forms;

namespace WildernessLabs.Clima.Client.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void BtnHackKitClicked(object sender, System.EventArgs e)
        {

        }

        void BtnProKitClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ProKitPage());
        }
    }
}