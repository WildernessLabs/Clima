using Xamarin.Forms;

namespace MobileApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void BtnProBluetoothClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new BluetoothPage(true) 
            { 
                Title = "Clima.Pro"
            });
        }

        void BtnProMapleClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MaplePage(true)
            {
                Title = "Clima.Pro"
            });
        }

        void BtnHackKitBluetoothClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new BluetoothPage(false)
            {
                Title = "Clima.HackKit"
            });
        }

        void BtnHackKitMapleClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MaplePage(false)
            {
                Title = "Clima.HackKit"
            });
        }
    }
}