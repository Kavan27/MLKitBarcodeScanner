# MLKit Barcode Scanner for Android

This repository contains a sample .NET MAUI application that focuses on Android barcode scanning using **Google ML Kit**. The goal is to provide a minimal app that opens directly to the camera, scans barcodes or QR codes, and displays the scanned value.

## Features

- **Button to launch camera launch** – When the app starts, there is a single "Scan Barcode" button clicking it opens the camera so you can begin scanning right away.
- **ML Kit barcode detection** – Uses `Xamarin.Google.MLKit.Vision.CodeScanner` to detect barcodes and QR codes.
- **Success dialog** – When a code is detected the app beeps, then the app shows a success message containing the scanned string/ID.
- **No result handling** – If nothing is detected the camera simply stays open with no additional UI.

## Folder Structure

```
Platforms/Android/       Android-specific code including manifest and barcode scanner implementation
MainPage.xaml            UI where the scan result is displayed
MauiProgram.cs           Registers platform services
```

Key files for barcode scanning are located under `Platforms/Android/`:

- `BarcodeScannerService.cs` – Wraps Google ML Kit's `GmsBarcodeScanner` to perform scans.
- `AndroidManifest.xml` – Declares camera permission and ML Kit dependency (`com.google.mlkit.vision.DEPENDENCIES`).

## Building and Running

1. Install the [.NET SDK](https://dotnet.microsoft.com/) with MAUI workload and Android tooling.
2. Open the solution `MLKitBarcodeScannerApp.sln` in Visual Studio or run from the command line with `dotnet build`.
3. Set the Android project as the startup target.
4. Deploy to an emulator or device. The app will request camera permission on first launch.

After launch, the camera view opens immediately. Once a barcode is scanned, a success toast displays the contents. If no code is present, the camera view remains open with no further action.

## Requirements

- Android 21 or later (as specified in `MLKitBarcodeScannerApp.csproj`).
- Camera permission must be granted by the user.

## Notes

This sample only targets Android even though the project includes folders for other platforms. The code uses the CommunityToolkit.Maui package for displaying toast notifications.

