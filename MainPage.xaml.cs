namespace Uygulamam
{
    public partial class MainPage : ContentPage
    {
        public const double MyFontSize = 28;
        public MainPage()
        {
            InitializeComponent();
            /*
             * AdjustLayoutForDevice(); // bu kısım mobile ve desktopa göre layout boyutu
            */
            //SetDynamicSizes();//dinamik güncelleme
        }
        /*
        private void AdjustLayoutForDevice()
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
            {
                // Masaüstü cihazlar için düzenlemeler
                LoginLabel.HeightRequest = 30; // Örnek yüksekliği düşürme
                
                // Diğer öğeleri de benzer şekilde ayarlayabilirsiniz
            }
            else
            {
                // Mobil cihazlar için düzenlemeler
                LoginLabel.HeightRequest = 100; // Örnek yüksekliği artırma
            }
        }
        */
        /*
        private void SetDynamicSizes()
        {
            // Get device width and height
            var deviceWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            var deviceHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;

            // Set dynamic sizes
            spaceshipImage.HeightRequest = deviceHeight * 0.20; // 25% of device height
            loginLabel.FontSize = deviceWidth * 0.02; // 5% of device width
        }*/
        private async void OnCounterClicked(object sender, EventArgs e)
        {
            string username = Username.Text;
            string password = Password.Text;
            if (username == "Azad" && password=="123")
            {
                await Navigation.PushAsync(new NewPage2());
            }
        }
    }
    public class GlobalFontSizeExtension : IMarkupExtension
    {
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return MainPage.MyFontSize;
        }
    }

}