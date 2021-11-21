using WildernessLabs.Clima.Client.ViewModels;
using Xamarin.Forms;

namespace WildernessLabs.Clima.Client.Views
{
    public partial class ProKitPage : ContentPage
    {
        public ProKitPage()
        {
            InitializeComponent();
            BindingContext = new ProKitViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as ProKitViewModel).CmdSearchForDevices.Execute(null);
        }
    }
}