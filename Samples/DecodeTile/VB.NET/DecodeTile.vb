Imports BitMiracle.Jpeg2k

Module DecodeTile

    Sub Main()
        ' The library won't work without a license. You can get free time limited license
        ' at https://bitmiracle.com/jpeg2000/
        LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license")

        Dim fileName As String = "Sample data/p1_04.j2k"
        Dim tileFileName As String = "tile"

        Using image = New J2kImage(fileName)
            Dim options = New J2kDecodingOptions()
            options.ForceRgbColorSpace = False

            For Each i As Integer In New Integer() {5, 25, 45}
                Dim imageData = image.DecodeTile(i, options)
                Dim outputName As String = String.Format("{0}_{1}.tif", tileFileName, i)
                imageData.Save(outputName, J2kOutputFormat.Tiff)

                Process.Start(outputName)
            Next
        End Using
    End Sub

End Module
