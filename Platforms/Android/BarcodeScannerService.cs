using Android.Gms.Tasks;
using Xamarin.Google.MLKit.Vision.Barcode.Common;
using Xamarin.Google.MLKit.Vision.CodeScanner;
using Java.Lang;
using Task = Android.Gms.Tasks.Task;
using Android.Runtime;
using Android.Media;

namespace MLKitBarcodeScannerApp.Platforms.Android
{
    public class BarcodeScannerService : IBarcodeScanner
    {
        public async Task<string?> ScanAsync()
        {
            try
            {
                // Create scanner instance
                var scanner = GmsBarcodeScanning.GetClient(
                    Platform.AppContext,
                    new GmsBarcodeScannerOptions.Builder()
                        .SetBarcodeFormats(Barcode.FormatAllFormats)
                        .Build());

                // Perform the scan
                var barcode = await ScanBarcodeAsync(scanner);
                return barcode?.RawValue;
            }
            catch (System.Exception ex)
            {
                // Handle any errors
                return $"Scan failed: {ex.Message}";
            }
        }

        private Task<Barcode?> ScanBarcodeAsync(IGmsBarcodeScanner scanner)
        {
            var tcs = new TaskCompletionSource<Barcode?>();

            var listener = new BarcodeScanListener(tcs);
            scanner.StartScan().AddOnCompleteListener(listener);

            return tcs.Task;
        }

        private class BarcodeScanListener : Java.Lang.Object, IOnCompleteListener
        {
            private readonly TaskCompletionSource<Barcode?> _tcs;

            public BarcodeScanListener(TaskCompletionSource<Barcode?> tcs)
            {
                _tcs = tcs;
            }

            public void OnComplete(Task task)
            {
                if (task.IsSuccessful)
                {
                    try
                    {
                        using var tone = new ToneGenerator(Stream.Notification, 100);
                        tone.StartTone(Tone.PropBeep);
                    }
                    catch
                    {
                        // ignore tone playback failures
                    }
                    var barcode = task.Result.JavaCast<Barcode>();
                    _tcs.TrySetResult(barcode);
                }
                else if (task.IsCanceled)
                {
                    _tcs.TrySetResult(null);
                }
                else
                {
                    _tcs.TrySetException(task.Exception ??
                        new System.Exception("Barcode scan failed"));
                }
            }
        }
    }
}