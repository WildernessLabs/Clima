using WildernessLabs.Clima.App;
using Xamarin.Forms;

namespace WildernessLabs.Clima.Client.Views
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