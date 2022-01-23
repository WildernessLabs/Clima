using WildernessLabs.Clima.App;
using Xamarin.Forms;

namespace MobileApp.Views
{
    public partial class HackKitPage : ContentPage
    {
        public HackKitPage()
        {
            InitializeComponent();
            BindingContext = new HackKitViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as HackKitViewModel).LoadData();
        }
    }
}