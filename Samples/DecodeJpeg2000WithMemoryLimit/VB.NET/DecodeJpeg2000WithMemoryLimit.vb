Imports System
Imports System.Diagnostics
Imports BitMiracle.Jpeg2k
Imports BitMiracle.LibTiff.Classic

Module DecodeJpeg2000WithMemoryLimit
    Sub Main(ByVal args As String())
        ' The library won't work without a license. You can get free time limited license
        ' at https://bitmiracle.com/jpeg2000/
        LicenseManager.SetTrialLicense("contact support@bitmiracle.com for a license")

        Const OutputFileName As String = "output.tiff"
        Using image = New J2kImage("Sample data/map.jp2")

            Using outputTiff As Tiff = Tiff.Open(OutputFileName, "w")
                Dim tileWidth As Integer = roundUp(Math.Min(2000, image.Width / 5), 16)
                Dim tileHeight As Integer = roundUp(Math.Min(2000, image.Height / 5), 16)

                Dim tileStripSize As Integer = 0
                Dim tileData As Byte() = Nothing
                Dim options = New J2kDecodingOptions()

                Dim y As Integer = 0
                While y < image.Height

                    Dim x As Integer = 0
                    While x < image.Width
                        Const TileFileName As String = "tile.tiff"
                        Dim imageData = image.DecodeArea(x, y, tileWidth, tileHeight, options)
                        imageData.Save(TileFileName, J2kOutputFormat.Tiff)

                        Using tileTiff = Tiff.Open(TileFileName, "r")

                            If x = 0 AndAlso y = 0 Then
                                Dim samplesPerPixel As Integer = tileTiff.GetField(TiffTag.SAMPLESPERPIXEL)(0).ToInt()
                                Dim bitsPerSample As Integer = tileTiff.GetField(TiffTag.BITSPERSAMPLE)(0).ToInt()
                                Dim photometric As Integer = tileTiff.GetField(TiffTag.PHOTOMETRIC)(0).ToInt()
                                outputTiff.SetField(TiffTag.IMAGEWIDTH, image.Width)
                                outputTiff.SetField(TiffTag.IMAGELENGTH, image.Height)
                                outputTiff.SetField(TiffTag.BITSPERSAMPLE, bitsPerSample)
                                outputTiff.SetField(TiffTag.SAMPLESPERPIXEL, samplesPerPixel)
                                outputTiff.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT)
                                outputTiff.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG)
                                outputTiff.SetField(TiffTag.PHOTOMETRIC, photometric)
                                outputTiff.SetField(TiffTag.TILEWIDTH, tileWidth)
                                outputTiff.SetField(TiffTag.TILELENGTH, tileHeight)
                                tileStripSize = tileTiff.StripSize()
                                tileData = New Byte(tileTiff.NumberOfStrips() * tileStripSize - 1) {}
                            End If

                            Dim offset As Integer = 0
                            Dim stripCount As Integer = tileTiff.NumberOfStrips()

                            For i As Integer = 0 To stripCount - 1
                                Dim bytesRead As Integer = tileTiff.ReadRawStrip(i, tileData, offset, tileData.Length - offset)
                                offset += bytesRead
                                offset += (tileStripSize - bytesRead)
                            Next

                            outputTiff.WriteTile(tileData, x, y, 0, 0)
                        End Using

                        Console.WriteLine($"Processed tile {{{x}, {y}, {imageData.Width}, {imageData.Height}}}")

                        x += tileWidth
                    End While

                    y += tileHeight
                End While
            End Using
        End Using

        Process.Start(OutputFileName)
    End Sub

    Private Function roundUp(ByVal numToRound As Integer, ByVal multiple As Integer) As Integer
        If multiple = 0 Then Return numToRound

        Dim remainder As Integer = numToRound Mod multiple
        If remainder = 0 Then Return numToRound

        Return numToRound + multiple - remainder
    End Function
End Module