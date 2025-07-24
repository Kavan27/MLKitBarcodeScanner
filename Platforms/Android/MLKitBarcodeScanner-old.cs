

using Xamarin.Google.MLKit.Vision.Barcode.Common;
using Xamarin.Google.MLKit.Vision.CodeScanner;
using Android.Gms.Tasks;
using Android.Runtime;
using Task = Android.Gms.Tasks.Task;

namespace MLKitBarcodeScanner.Platforms.Android;

public class MLKitBarcodeScanner : IDisposable
{
    private readonly IGmsBarcodeScanner barcodeScanner = GmsBarcodeScanning.GetClient(
        Platform.AppContext,
        new GmsBarcodeScannerOptions.Builder()
            .AllowManualInput()
            .EnableAutoZoom()
            .SetBarcodeFormats(Barcode.FormatAllFormats)
            .Build());

    public async Task<Barcode?> ScanAsync()
    {
        var taskCompletionSource = new TaskCompletionSource<Barcode?>();
        var barcodeResultListener = new OnBarcodeResultListener(taskCompletionSource);
        using var task = barcodeScanner.StartScan()
                    .AddOnCompleteListener(barcodeResultListener);
        return await taskCompletionSource.Task;
    }

    public void Dispose()
    {
        barcodeScanner.Dispose();
    }
}

public class OnBarcodeResultListener(TaskCompletionSource<Barcode?> taskCompletionSource) : Java.Lang.Object, IOnCompleteListener
{
    public void OnComplete(Task task)
    {
        if (task.IsSuccessful)
        {
            taskCompletionSource.SetResult(task.Result.JavaCast<Barcode>());
        }
        else if (task.IsCanceled)
        {
            taskCompletionSource.SetResult(null);
        }
        else
        {
            taskCompletionSource.SetException(task.Exception);
        }
    }
}