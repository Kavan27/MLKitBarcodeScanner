using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLKitBarcodeScannerApp
{
    public interface IBarcodeScanner
    {
        Task<string?> ScanAsync();
    }
}
