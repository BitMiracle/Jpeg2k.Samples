Imports BitMiracle.Jpeg2k

Module ImageInfo

    Sub Main()
        ' The library won't work without a license. You can get free time limited license
        ' at https://bitmiracle.com/jpeg2000/
        LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license")

        Dim fileName As String = "Sample data/a1_mono.j2c"
        Using image = New J2kImage(fileName)
            Console.WriteLine("Size = {0}x{1}", image.Width, image.Height)
            Console.WriteLine("Color space = {0}", image.ColorSpace)

            Console.WriteLine("Component count = {0}", image.ComponentsInfo.Count)
            Dim componentIndex As Integer = 0

            For Each component In image.ComponentsInfo
                Console.WriteLine("Component {0} offset = {1}x{2}", componentIndex, component.Left, component.Top)
                Console.WriteLine("Component {0} size = {1}x{2}", componentIndex, component.Width, component.Height)
                Console.WriteLine("Component {0} bytes per pixel = {1}", componentIndex, component.BitsPerPixel)
                componentIndex = (componentIndex + 1)
            Next

            If (image.TileCount > 1) Then
                Console.WriteLine("Tile count = {0}", image.TileCount)
                Console.WriteLine("Tile size = {0}x{1}", image.TileWidth, image.TileHeight)
            End If

            Console.WriteLine("Default tile coding style = {0}", image.DefaultTileInfo.CodingStyle)
            Console.WriteLine("Default tile progression order = {0}", image.DefaultTileInfo.ProgressionOrder)
            Console.WriteLine("Default tile quality layers count = {0}", image.DefaultTileInfo.QualityLayerCount)
        End Using
    End Sub

End Module
