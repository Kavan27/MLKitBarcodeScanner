using Android.Content;
using Microsoft.Maui.ApplicationModel;

namespace MLKitBarcodeScannerApp.Platforms.Android;

public class CameraXBarcodeScannerService : IBarcodeScanner
{
    internal static System.Threading.Tasks.TaskCompletionSource<string?>? ResultSource;

    public System.Threading.Tasks.Task<string?> ScanAsync()
    {
        var activity = Platform.CurrentActivity ?? throw new System.InvalidOperationException("No current activity");
        ResultSource = new System.Threading.Tasks.TaskCompletionSource<string?>();
        var intent = new Intent(activity, typeof(CameraXScannerActivity));
        activity.StartActivity(intent);
        return ResultSource.Task;
    }
}
