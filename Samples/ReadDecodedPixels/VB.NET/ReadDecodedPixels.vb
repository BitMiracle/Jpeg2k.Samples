Imports System
Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports BitMiracle.Jpeg2k

Module ReadDecodedPixels
    Sub Main()
        ' The library won't work without a license. You can get free time limited license
        ' at https://bitmiracle.com/jpeg2000/
        LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license")

        Dim fileName = "Sample data/p1_04.j2k"
        Using image = New J2kImage(fileName)
            Dim imageData As J2kImageData = image.Decode()

            Dim components = imageData.Components
            Dim comp0 = components(0)
            If comp0.BitsPerPixel < 8 Then
                Console.WriteLine("Unsupported bits per pixel value: {0}. At least 8 bpp required.", comp0.BitsPerPixel)
                Return
            End If

            Dim comp1 As J2kImageComponent = Nothing
            Dim comp2 As J2kImageComponent = Nothing
            If components.Count >= 3 Then
                comp1 = components(1)
                comp2 = components(2)
            End If

            Dim w = comp0.Width
            Dim h = comp0.Height
            If components.Count >= 3 AndAlso
                comp0.HorizontalSeparation = comp1.HorizontalSeparation AndAlso
                comp1.HorizontalSeparation = comp2.HorizontalSeparation AndAlso
                comp0.VerticalSeparation = comp1.VerticalSeparation AndAlso
                comp1.VerticalSeparation = comp2.VerticalSeparation AndAlso
                comp0.BitsPerPixel = comp1.BitsPerPixel AndAlso
                comp1.BitsPerPixel = comp2.BitsPerPixel Then

                ' 24 bpp
                Dim adjustR = 0
                If comp0.BitsPerPixel > 8 Then adjustR = comp0.BitsPerPixel - 8

                Dim adjustG = 0
                If comp1.BitsPerPixel > 8 Then adjustG = comp1.BitsPerPixel - 8

                Dim adjustB = 0
                If comp2.BitsPerPixel > 8 Then adjustB = comp2.BitsPerPixel - 8

                Dim redPixels As J2kPixels = comp0.GetPixels()
                Dim redAdjustment = If(comp0.PixelsAreSigned, 1 << comp0.BitsPerPixel - 1, 0)

                Dim greenPixels As J2kPixels = comp1.GetPixels()
                Dim greenAdjustment = If(comp1.PixelsAreSigned, 1 << comp1.BitsPerPixel - 1, 0)

                Dim bluePixels As J2kPixels = comp2.GetPixels()
                Dim blueAdjustment = If(comp2.PixelsAreSigned, 1 << comp2.BitsPerPixel - 1, 0)

                Using bitmap = New Bitmap(w, h, PixelFormat.Format24bppRgb)
                    Dim bd As BitmapData = bitmap.LockBits(New Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, bitmap.PixelFormat)

                    Dim dest = New Byte(bd.Stride * bd.Height - 1) {}
                    Marshal.Copy(bd.Scan0, dest, 0, dest.Length)

                    Dim y = h - 1
                    While y >= 0
                        Dim start = y * w

                        Dim x = 0
                        While x < w
                            Dim r = redPixels(start + x) + redAdjustment
                            r = (r >> adjustR) + (r >> adjustR - 1) Mod 2
                            If r > 255 Then
                                r = 255
                            ElseIf r < 0 Then
                                r = 0
                            End If

                            Dim g = greenPixels(start + x) + greenAdjustment
                            g = (g >> adjustG) + (g >> adjustG - 1) Mod 2
                            If g > 255 Then
                                g = 255
                            ElseIf g < 0 Then
                                g = 0
                            End If

                            Dim b = bluePixels(start + x) + blueAdjustment
                            b = (b >> adjustB) + (b >> adjustB - 1) Mod 2
                            If b > 255 Then
                                b = 255
                            ElseIf b < 0 Then
                                b = 0
                            End If

                            Dim baseOffset = y * bd.Stride + x * 3
                            dest(baseOffset + 0) = CByte(b)
                            dest(baseOffset + 1) = CByte(g)
                            dest(baseOffset + 2) = CByte(r)

                            x += 1
                        End While

                        y -= 1
                    End While

                    Marshal.Copy(dest, 0, bd.Scan0, dest.Length)

                    bitmap.UnlockBits(bd)

                    Const OutputFileName = "result_24bpp.png"
                    bitmap.Save(OutputFileName, ImageFormat.Png)
                    Process.Start(OutputFileName)
                End Using
            Else
                ' 8 bpp grayscale
                Dim adjustG = 0
                If comp0.BitsPerPixel > 8 Then adjustG = comp0.BitsPerPixel - 8

                Dim rowPixels As J2kPixels = comp0.GetPixels()
                Using bitmap = New Bitmap(w, h, PixelFormat.Format8bppIndexed)
                    Dim pal = bitmap.Palette
                    For i = 0 To 255
                        pal.Entries(i) = Color.FromArgb(i, i, i)
                    Next

                    bitmap.Palette = pal

                    Dim bd As BitmapData = bitmap.LockBits(New Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, bitmap.PixelFormat)

                    Dim dest = New Byte(bd.Stride * bd.Height - 1) {}
                    Marshal.Copy(bd.Scan0, dest, 0, dest.Length)

                    Dim y = h - 1
                    While y >= 0
                        Dim start = y * w

                        Dim x = 0
                        While x < w
                            Dim g = rowPixels(start + x) + adjustG
                            g = (g >> adjustG) + (g >> adjustG - 1) Mod 2
                            If g > 255 Then
                                g = 255
                            ElseIf g < 0 Then
                                g = 0
                            End If

                            dest(y * bd.Stride + x) = CByte(g)

                            x += 1
                        End While

                        y -= 1
                    End While

                    Marshal.Copy(dest, 0, bd.Scan0, dest.Length)

                    bitmap.UnlockBits(bd)

                    Const OutputFileName = "result_8bpp.png"
                    bitmap.Save(OutputFileName, ImageFormat.Png)
                    Process.Start(OutputFileName)
                End Using
            End If
        End Using
    End Sub
End Module
