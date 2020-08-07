using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Egoal.Report.Tickets
{
    public class QrCodeHelper
    {
        public string CreateQrCode(string appDir, string listNo, string content)
        {
            string _codeUrlDir = "";
            if (string.IsNullOrEmpty(_codeUrlDir))
            {
                _codeUrlDir = System.IO.Path.Combine(appDir, @"QrCodeImage");
                try
                {
                    if (!System.IO.Directory.Exists(_codeUrlDir))
                    {
                        System.IO.Directory.CreateDirectory(_codeUrlDir);
                    }
                }
                catch { }
            }

            string imgName = System.IO.Path.Combine(_codeUrlDir, string.Format("{0}.jpg", listNo));

            try
            {
                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                QrCode qrCode = new QrCode();
                qrEncoder.TryEncode(content, out qrCode);
                GraphicsRenderer gRenderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                //
                using (FileStream stream = new FileStream(imgName, FileMode.Create))
                {
                    gRenderer.WriteToStream(qrCode.Matrix, ImageFormat.Jpeg, stream);
                }
                if (File.Exists(imgName))
                {
                    return imgName;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
