using BitMiracle.Jpeg2k;
using System;

namespace ImageInfo
{
    class ImageInfo
    {
        static void Main(string[] args)
        {
            // The library won't work without a license. You can get free time limited license
            // at https://bitmiracle.com/jpeg2000/
            LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license"); 

            string fileName = @"Sample data/a1_mono.j2c";
            using (var image = new J2kImage(fileName))
            {
                Console.WriteLine("Size = {0}x{1}", image.Width, image.Height);
                Console.WriteLine("Color space = {0}", image.ColorSpace);

                Console.WriteLine("Component count = {0}", image.ComponentsInfo.Count);
                int componentIndex = 0;
                foreach (var component in image.ComponentsInfo)
                {
                    Console.WriteLine("Component {0} offset = {1}x{2}", componentIndex, component.Left, component.Top);
                    Console.WriteLine("Component {0} size = {1}x{2}", componentIndex, component.Width, component.Height);
                    Console.WriteLine("Component {0} bytes per pixel = {1}", componentIndex, component.BitsPerPixel);
                    componentIndex++;
                }

                if (image.TileCount > 1)
                {
                    Console.WriteLine("Tile count = {0}", image.TileCount);
                    Console.WriteLine("Tile size = {0}x{1}", image.TileWidth, image.TileHeight);
                }

                Console.WriteLine("Default tile coding style = {0}", image.DefaultTileInfo.CodingStyle);
                Console.WriteLine("Default tile progression order = {0}", image.DefaultTileInfo.ProgressionOrder);
                Console.WriteLine("Default tile quality layers count = {0}", image.DefaultTileInfo.QualityLayerCount);
            }
        }
    }
}
