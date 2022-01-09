using Xamarin.Forms;

namespace MobileApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void BtnHackKitClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new HackKitPage());
        }

        void BtnProKitClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ProKitPage());
        }
    }
}