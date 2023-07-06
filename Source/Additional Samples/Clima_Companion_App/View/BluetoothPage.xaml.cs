using MobileClima.ViewModel;

namespace MobileClima.View
{
    public partial class BluetoothPage : ContentPage
    {
        public BluetoothPage(bool isClimaPro)
        {
            InitializeComponent();
            BindingContext = new BluetoothViewModel(isClimaPro);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as BluetoothViewModel).CmdSearchForDevices.Execute(null);
        }
    }
}