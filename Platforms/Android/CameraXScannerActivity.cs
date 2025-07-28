using Android.App;
using Android.OS;
using AndroidX.Camera.Core;
using AndroidX.Camera.Lifecycle;
using AndroidX.Camera.View;
using AndroidX.Core.Content;
using Google.MLKit.Vision.Barcode;
using Google.MLKit.Vision.Barcode.Common;
using Google.MLKit.Vision.Common;
using Microsoft.Maui.ApplicationModel;

namespace MLKitBarcodeScannerApp.Platforms.Android;

[Activity(Label = "BarcodeScanner", Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
public class CameraXScannerActivity : AndroidX.AppCompat.App.AppCompatActivity
{
    PreviewView? previewView;
    IBarcodeScanner? barcodeScanner;
    ProcessCameraProvider? cameraProvider;
    AndroidX.Camera.Core.Camera? camera;
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        previewView = new PreviewView(this);
        SetContentView(previewView);

        barcodeScanner = BarcodeScanning.GetClient(new BarcodeScannerOptions.Builder()
            .SetBarcodeFormats(Barcode.FormatAllFormats)
            .Build());

        var cameraProviderFuture = ProcessCameraProvider.GetInstance(this);
        cameraProviderFuture.AddListener(new Runnable(() =>
        {
            cameraProvider = cameraProviderFuture.Get();
            BindCameraUseCases();
        }), ContextCompat.GetMainExecutor(this));
    }

    void BindCameraUseCases()
    {
        if (cameraProvider == null || previewView == null || barcodeScanner == null)
            return;

        var preview = new Preview.Builder().Build();
        preview.SetSurfaceProvider(previewView.SurfaceProvider);

        var analysis = new ImageAnalysis.Builder()
            .SetBackpressureStrategy(ImageAnalysis.StrategyKeepOnlyLatest)
            .Build();
        analysis.SetAnalyzer(ContextCompat.GetMainExecutor(this), new BarcodeAnalyzer(this, barcodeScanner));

        var selector = CameraSelector.DefaultBackCamera;

        cameraProvider.UnbindAll();
        camera = cameraProvider.BindToLifecycle(this, selector, preview, analysis);
        camera?.CameraControl?.SetLinearZoom(0.4f);
    }

    protected override void OnDestroy()
    {
        barcodeScanner?.Close();
        base.OnDestroy();
    }

    class BarcodeAnalyzer : Java.Lang.Object, ImageAnalysis.IAnalyzer
    {
        readonly CameraXScannerActivity activity;
        readonly IBarcodeScanner scanner;

        public BarcodeAnalyzer(CameraXScannerActivity activity, IBarcodeScanner scanner)
        {
            this.activity = activity;
            this.scanner = scanner;
        }

        public void Analyze(IImageProxy image)
        {
            var mediaImage = image.Image;
            if (mediaImage != null)
            {
                var input = InputImage.FromMediaImage(mediaImage, image.ImageInfo.RotationDegrees);
                scanner.Process(input)
                    .AddOnSuccessListener(new OnSuccessListener(results =>
                    {
                        if (results.Count > 0)
                        {
                            var value = results[0].RawValue;
                            CameraXBarcodeScannerService.ResultSource?.TrySetResult(value);
                            activity.Finish();
                        }
                    }))
                    .AddOnFailureListener(new OnFailureListener(ex =>
                    {
                        CameraXBarcodeScannerService.ResultSource?.TrySetException(ex);
                        activity.Finish();
                    }));
            }
            image.Close();
        }
    }

    class OnSuccessListener(System.Action<System.Collections.Generic.IList<Barcode>> handler) : Java.Lang.Object, Android.Gms.Tasks.IOnSuccessListener
    {
        readonly System.Action<System.Collections.Generic.IList<Barcode>> _handler = handler;
        public void OnSuccess(Java.Lang.Object? result)
        {
            _handler.Invoke(result.JavaCast<Java.Util.IList>()?.ToArray<Barcode>() ?? new System.Collections.Generic.List<Barcode>());
        }
    }

    class OnFailureListener(System.Action<System.Exception> handler) : Java.Lang.Object, Android.Gms.Tasks.IOnFailureListener
    {
        readonly System.Action<System.Exception> _handler = handler;
        public void OnFailure(Java.Lang.Exception? e)
        {
            _handler.Invoke(new System.Exception(e?.Message));
        }
    }
}
