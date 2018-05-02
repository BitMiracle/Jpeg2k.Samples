using BitMiracle.Jpeg2k;
using System;

namespace CreateJpeg2000
{
    class CreateJpeg2000
    {
        static void Main(string[] args)
        {
            // The library won't work without a license. You can get free time limited license
            // at https://bitmiracle.com/jpeg2000/
            LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license");

            string fileName = @"Sample data/lena.tif";
            var imageData = J2kImageData.FromImage(fileName);
            Console.WriteLine("Image size is {0}x{1}", imageData.Width, imageData.Height);

            // For a lossless encoding with default options you would not need options.
            string j2kFileName = @"lena.j2k";
            imageData.Encode(j2kFileName);

            // You can produce tiled images too. 
            var tiledOptions = new J2kEncodingOptions
            {
                Codec = J2kCodec.J2k,
                ProduceTiledImage = true,
                TileWidth = 128,
                TileHeight = 256,
            };

            string tiledFileName = @"lena_tiled.j2k";
            imageData = J2kImageData.FromImage(fileName);
            imageData.Encode(tiledFileName, tiledOptions);

            // You can specify compression quality using the encoding options
            var options30x = new J2kEncodingOptions
            {
                Codec = J2kCodec.J2k,
                QualityMode = J2kQualityMode.CompressionRatio,
                QualityValues = new float[] { 30 }
            };

            string fileName30x = @"lena_30x.j2k";
            imageData = J2kImageData.FromImage(fileName);
            imageData.Encode(fileName30x, options30x);
        }
    }
}
