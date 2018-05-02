Imports BitMiracle.Jpeg2k

Module DecodeJpeg2000

    Sub Main()
        ' The library won't work without a license. You can get free time limited license
        ' at https://bitmiracle.com/jpeg2000/
        LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license")

        Dim fileName As String = "Sample data/a2_colr.j2c"
        Dim bitmapFileName As String = "output.bmp"
        Dim tiffFileName As String = "output.tiff"

        Using image = New J2kImage(fileName)
            ' there some decoding options that you can setup before you decode the image
            Dim options = New J2kDecodingOptions()
            options.UpsampleComponents = True

            Dim imageData = image.Decode(options)

            ' decoded image data can be saved as BMP or TIFF file
            imageData.Save(bitmapFileName, J2kOutputFormat.Bmp)
            imageData.Save(tiffFileName, J2kOutputFormat.Tiff)
        End Using

        Process.Start(bitmapFileName)
        Process.Start(tiffFileName)
    End Sub

End Module
