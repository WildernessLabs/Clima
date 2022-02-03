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
            Navigation.PushAsync(new BluetoothPage(true));
        }

        void BtnProMapleClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MaplePage(true));
        }

        void BtnHackKitBluetoothClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new BluetoothPage(false));
        }

        void BtnHackKitMapleClicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new MaplePage(false));
        }
    }
}