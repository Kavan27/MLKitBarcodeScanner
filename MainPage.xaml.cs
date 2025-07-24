using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using MLKitBarcodeScannerApp;

namespace MLKitBarcodeScannerApp
{
    public partial class MainPage : ContentPage
    {
        private readonly IBarcodeScanner _barcodeScanner;

        public MainPage(IBarcodeScanner barcodeScanner)
        {
            InitializeComponent();
            _barcodeScanner = barcodeScanner;
        }

        private async void OnScanButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var barcode = await _barcodeScanner.ScanAsync();

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Toast.Make(
                        barcode is null ? "Error has occurred during barcode scanning" : barcode,
                        ToastDuration.Long).Show();
                });
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Toast.Make($"Error: {ex.Message}", ToastDuration.Long).Show();
                });
            }
        }
    }
}