using MobileApp.ViewModels;
using Xamarin.Forms;

namespace MobileApp.Views
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