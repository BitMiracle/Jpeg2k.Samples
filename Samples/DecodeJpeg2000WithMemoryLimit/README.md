# Decode JPEG 2000 image with a memory limit
This sample shows how to decode JPEG 2000 images by tiles with fixed memory consumption.

The decoding of the entire JPEG 2000 image requires a lot of memory when the image is large. You can restrict memory consumption using portioned decoding. Use J2kImage.DecodeArea method to decode parts of JPEG 2000 images. Then render decoded parts or merge them into a tiled TIFF image.

This sample uses [LibTiff.Net](https://bitmiracle.com/libtiff/) library to produce tiled TIFF.
