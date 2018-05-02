using BitMiracle.Jpeg2k;
using System.Diagnostics;

namespace DecodeJpeg2000
{
    class DecodeJpeg2000
    {
        static void Main(string[] args)
        {
            // The library won't work without a license. You can get free time limited license
            // at https://bitmiracle.com/jpeg2000/
            LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license");

            string fileName = @"Sample data/a2_colr.j2c";
            string bitmapFileName = @"output.bmp";
            string tiffFileName = @"output.tiff";

            using (var image = new J2kImage(fileName))
            {
                // there some decoding options that you can setup before you decode the image
                var options = new J2kDecodingOptions
                {
                    UpsampleComponents = true
                };

                var imageData = image.Decode(options);

                // decoded image data can be saved as BMP or TIFF file
                imageData.Save(bitmapFileName, J2kOutputFormat.Bmp);
                imageData.Save(tiffFileName, J2kOutputFormat.Tiff);
            }

            Process.Start(bitmapFileName);
            Process.Start(tiffFileName);
        }
    }
}
