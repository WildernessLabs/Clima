using WildernessLabs.Clima.App;
using Xamarin.Forms;

namespace MobileApp.Views
{
    public partial class MaplePage : ContentPage
    {
        public MaplePage(bool isClimaPro)
        {
            InitializeComponent();
            BindingContext = new MapleViewModel(isClimaPro);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await (BindingContext as MapleViewModel).LoadData();
        }
    }
}