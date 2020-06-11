using System;
using System.Diagnostics;
using BitMiracle.Jpeg2k;
using BitMiracle.LibTiff.Classic;

namespace DecodeJpeg2000WithMemoryLimit
{
    class DecodeJpeg2000WithMemoryLimit
    {
        static void Main(string[] args)
        {
            // The library won't work without a license. You can get free time limited license
            // at https://bitmiracle.com/jpeg2000/
            LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license");

            const string OutputFileName = "output.tiff";
            using (var image = new J2kImage(@"Sample data/map.jp2"))
            {
                using (Tiff outputTiff = Tiff.Open(OutputFileName, "w"))
                {
                    // Tile size must be a multiple of 16.
                    int tileWidth = roundUp(Math.Min(2000, image.Width / 5), 16);
                    int tileHeight = roundUp(Math.Min(2000, image.Height / 5), 16);

                    int tileStripSize = 0;
                    byte[] tileData = null;
                    var options = new J2kDecodingOptions();
                    for (int y = 0; y < image.Height; y += tileHeight)
                    {
                        for (int x = 0; x < image.Width; x += tileWidth)
                        {
                            const string TileFileName = "tile.tiff";
                            var imageData = image.DecodeArea(x, y, tileWidth, tileHeight, options);
                            imageData.Save(TileFileName, J2kOutputFormat.Tiff);
                            using (var tileTiff = Tiff.Open(TileFileName, "r"))
                            {
                                if (x == 0 && y == 0)
                                {
                                    int samplesPerPixel = tileTiff.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();
                                    int bitsPerSample = tileTiff.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                                    int photometric = tileTiff.GetField(TiffTag.PHOTOMETRIC)[0].ToInt();

                                    outputTiff.SetField(TiffTag.IMAGEWIDTH, image.Width);
                                    outputTiff.SetField(TiffTag.IMAGELENGTH, image.Height);
                                    outputTiff.SetField(TiffTag.BITSPERSAMPLE, bitsPerSample);
                                    outputTiff.SetField(TiffTag.SAMPLESPERPIXEL, samplesPerPixel);
                                    outputTiff.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);
                                    outputTiff.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                                    outputTiff.SetField(TiffTag.PHOTOMETRIC, photometric);
                                    outputTiff.SetField(TiffTag.TILEWIDTH, tileWidth);
                                    outputTiff.SetField(TiffTag.TILELENGTH, tileHeight);

                                    tileStripSize = tileTiff.StripSize();
                                    tileData = new byte[tileTiff.NumberOfStrips() * tileStripSize];
                                }

                                int offset = 0;
                                int stripCount = tileTiff.NumberOfStrips();
                                for (int i = 0; i < stripCount; ++i)
                                {
                                    int bytesRead = tileTiff.ReadRawStrip(i, tileData, offset, tileData.Length - offset);
                                    offset += bytesRead;

                                    // pad the rightmost column of tiles if necessary
                                    offset += (tileStripSize - bytesRead);
                                }

                                outputTiff.WriteTile(tileData, x, y, 0, 0);
                            }

                            Console.WriteLine($"Processed tile {{{x}, {y}, {imageData.Width}, {imageData.Height}}}");
                        }
                    }
                }
            }

            Process.Start(OutputFileName);
        }

        private static int roundUp(int numToRound, int multiple)
        {
            if (multiple == 0)
                return numToRound;

            int remainder = numToRound % multiple;
            if (remainder == 0)
                return numToRound;

            return numToRound + multiple - remainder;
        }
    }
}
