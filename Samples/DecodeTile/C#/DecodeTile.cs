using BitMiracle.Jpeg2k;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DecodeTile
{
    class DecodeTile
    {
        static void Main(string[] args)
        {
            // The library won't work without a license. You can get free time limited license
            // at https://bitmiracle.com/jpeg2000/
            LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license");

            string fileName = @"Sample data/p1_04.j2k";
            string tileFileName = @"tile";

            using (var image = new J2kImage(fileName))
            {
                var options = new J2kDecodingOptions
                {
                    ForceRgbColorSpace = false
                };

                foreach (int i in new int[] { 5, 25, 45} )
                {
                    var imageData = image.DecodeTile(i, options);
                    string outputName = string.Format("{0}_{1}.tif", tileFileName, i);
                    imageData.Save(outputName, J2kOutputFormat.Tiff);

                    Process.Start(outputName);
                }
            }
        }
    }
}
