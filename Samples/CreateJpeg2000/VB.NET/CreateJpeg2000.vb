Imports BitMiracle.Jpeg2k

Module CreateJpeg2000

    Sub Main()
        ' The library won't work without a license. You can get free time limited license
        ' at https://bitmiracle.com/jpeg2000/
        LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license")

        Dim fileName As String = "Sample data/lena.tif"
        Dim imageData = J2kImageData.FromImage(fileName)
        Console.WriteLine("Image size is {0}x{1}", imageData.Width, imageData.Height)

        ' For a lossless encoding with default options you would not need options.
        Dim j2kFileName As String = "lena.j2k"
        imageData.Encode(j2kFileName)

        ' You can produce tiled images too. 
        Dim tiledOptions = New J2kEncodingOptions()
        tiledOptions.Codec = J2kCodec.J2k
        tiledOptions.ProduceTiledImage = True
        tiledOptions.TileWidth = 128
        tiledOptions.TileHeight = 256

        Dim tiledFileName As String = "lena_tiled.j2k"
        imageData = J2kImageData.FromImage(fileName)
        imageData.Encode(tiledFileName, tiledOptions)

        ' You can specify compression quality using the encoding options
        Dim options30x = New J2kEncodingOptions()
        options30x.Codec = J2kCodec.J2k
        options30x.QualityMode = J2kQualityMode.CompressionRatio
        options30x.QualityValues = New Single() {30}

        Dim fileName30x As String = "lena_30x.j2k"
        imageData = J2kImageData.FromImage(fileName)
        imageData.Encode(fileName30x, options30x)
    End Sub

End Module
