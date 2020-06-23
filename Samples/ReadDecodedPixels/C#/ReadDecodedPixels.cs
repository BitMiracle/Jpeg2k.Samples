using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using BitMiracle.Jpeg2k;

namespace ReadDecodedPixels
{
    class ReadDecodedPixels
    {
        static void Main(string[] args)
        {
            // The library won't work without a license. You can get free time limited license
            // at https://bitmiracle.com/jpeg2000/
            LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license");

            string fileName = @"Sample data/p1_04.j2k";
            using (var image = new J2kImage(fileName))
            {
                J2kImageData imageData = image.Decode();

                ReadOnlyCollection<J2kImageComponent> components = imageData.Components;
                J2kImageComponent comp0 = components[0];
                if (comp0.BitsPerPixel < 8)
                {
                    Console.WriteLine("Unsupported bits per pixel value: {0}. At least 8 bpp required.", comp0.BitsPerPixel);
                    return;
                }

                J2kImageComponent comp1 = null;
                J2kImageComponent comp2 = null;
                if (components.Count >= 3)
                {
                    comp1 = components[1];
                    comp2 = components[2];
                }

                int w = comp0.Width;
                int h = comp0.Height;
                if (components.Count >= 3 &&
                    comp0.HorizontalSeparation == comp1.HorizontalSeparation &&
                    comp1.HorizontalSeparation == comp2.HorizontalSeparation &&
                    comp0.VerticalSeparation == comp1.VerticalSeparation &&
                    comp1.VerticalSeparation == comp2.VerticalSeparation &&
                    comp0.BitsPerPixel == comp1.BitsPerPixel &&
                    comp1.BitsPerPixel == comp2.BitsPerPixel)
                {
                    // 24 bpp
                    int adjustR = 0;
                    if (comp0.BitsPerPixel > 8)
                        adjustR = comp0.BitsPerPixel - 8;

                    int adjustG = 0;
                    if (comp1.BitsPerPixel > 8)
                        adjustG = comp1.BitsPerPixel - 8;

                    int adjustB = 0;
                    if (comp2.BitsPerPixel > 8)
                        adjustB = comp2.BitsPerPixel - 8;

                    J2kPixels redPixels = comp0.GetPixels();
                    int redAdjustment = comp0.PixelsAreSigned ? (1 << (comp0.BitsPerPixel - 1)) : 0;

                    J2kPixels greenPixels = comp1.GetPixels();
                    int greenAdjustment = comp1.PixelsAreSigned ? (1 << (comp1.BitsPerPixel - 1)) : 0;

                    J2kPixels bluePixels = comp2.GetPixels();
                    int blueAdjustment = comp2.PixelsAreSigned ? (1 << (comp2.BitsPerPixel - 1)) : 0;

                    using (var bitmap = new Bitmap(w, h, PixelFormat.Format24bppRgb))
                    {
                        BitmapData bd = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                        byte[] dest = new byte[bd.Stride * bd.Height];
                        Marshal.Copy(bd.Scan0, dest, 0, dest.Length);

                        for (int y = h - 1; y >= 0; --y)
                        {
                            int start = y * w;
                            for (int x = 0; x < w; ++x)
                            {
                                int r = redPixels[start + x] + redAdjustment;
                                r = (r >> adjustR) + ((r >> (adjustR - 1)) % 2);
                                if (r > 255)
                                    r = 255;
                                else if (r < 0)
                                    r = 0;

                                int g = greenPixels[start + x] + greenAdjustment;
                                g = (g >> adjustG) + ((g >> (adjustG - 1)) % 2);
                                if (g > 255)
                                    g = 255;
                                else if (g < 0)
                                    g = 0;

                                int b = bluePixels[start + x] + blueAdjustment;
                                b = (b >> adjustB) + ((b >> (adjustB - 1)) % 2);
                                if (b > 255)
                                    b = 255;
                                else if (b < 0)
                                    b = 0;

                                int baseOffset = y * bd.Stride + x * 3;
                                dest[baseOffset + 0] = (byte)b;
                                dest[baseOffset + 1] = (byte)g;
                                dest[baseOffset + 2] = (byte)r;
                            }
                        }

                        Marshal.Copy(dest, 0, bd.Scan0, dest.Length);

                        bitmap.UnlockBits(bd);

                        const string OutputFileName = "result_24bpp.png";
                        bitmap.Save(OutputFileName, ImageFormat.Png);
                        Process.Start(OutputFileName);
                    }
                }
                else
                {
                    // 8 bpp grayscale
                    int adjustG = 0;
                    if (comp0.BitsPerPixel > 8)
                        adjustG = comp0.BitsPerPixel - 8;

                    J2kPixels rowPixels = comp0.GetPixels();
                    using (var bitmap = new Bitmap(w, h, PixelFormat.Format8bppIndexed))
                    {
                        ColorPalette pal = bitmap.Palette;
                        for (int i = 0; i <= 255; i++)
                            pal.Entries[i] = Color.FromArgb(i, i, i);

                        bitmap.Palette = pal;

                        BitmapData bd = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                        byte[] dest = new byte[bd.Stride * bd.Height];
                        Marshal.Copy(bd.Scan0, dest, 0, dest.Length);

                        for (int y = h - 1; y >= 0; --y)
                        {
                            int start = y * w;
                            for (int x = 0; x < w; ++x)
                            {
                                int g = rowPixels[start + x] + adjustG;
                                g = (g >> adjustG) + ((g >> (adjustG - 1)) % 2);
                                if (g > 255)
                                    g = 255;
                                else if (g < 0)
                                    g = 0;

                                dest[y * bd.Stride + x] = (byte)g;
                            }
                        }

                        Marshal.Copy(dest, 0, bd.Scan0, dest.Length);

                        bitmap.UnlockBits(bd);

                        const string OutputFileName = "result_8bpp.png";
                        bitmap.Save(OutputFileName, ImageFormat.Png);
                        Process.Start(OutputFileName);
                    }
                }
            }
        }
    }
}
