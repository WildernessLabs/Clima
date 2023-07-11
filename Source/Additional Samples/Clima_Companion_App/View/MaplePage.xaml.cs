using MobileClima.ViewModel;

namespace MobileClima.View
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