// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Iot.Device.MemoryLcd
{
    /// <summary>
    /// Extend methods for LSxxxB7DHxx
    /// </summary>
    public static class LSxxxB7DHxxImageSending
    {
        /// <summary>
        /// Method for whether pixel is lit or not.
        /// </summary>
        /// <param name="pixel">Input pixel</param>
        /// <returns>True if lit.</returns>
        public delegate bool BrightnessConvertor(IPixel pixel);

        /// <summary>
        /// Show image to device
        /// </summary>
        /// <param name="mlcd">Memory LCD device</param>
        /// <param name="image">Image to show</param>
        /// <param name="brightnessConvertor">Method for whether pixel is lit or not. Use <see cref="LinearBrightnessConvertor"/> if null.</param>
        /// <param name="split">Number to splits<br/>To avoid buffer overflow exceptions, it needs to split one frame into multiple sends for some device.</param>
        /// <param name="frameInversion">Frame inversion flag</param>
        public static void ShowImage<TPixel>(this LSxxxB7DHxx mlcd, Image<TPixel> image, BrightnessConvertor? brightnessConvertor = null, int split = 1, bool frameInversion = false)
                where TPixel : unmanaged, IPixel<TPixel>
        {
            mlcd.FillImageBufferWithImage(image, brightnessConvertor ?? LinearBrightnessConvertor);

            int linesToSend = mlcd.PixelHeight / split;
            int bytesToSend = mlcd._frameBuffer.Length / split;

            for (int fs = 0; fs < split; fs++)
            {
                Span<byte> lineNumbers = mlcd._lineNumberBuffer.AsSpan(linesToSend * fs, linesToSend);
                Span<byte> bytes = mlcd._frameBuffer.AsSpan(bytesToSend * fs, bytesToSend);

                mlcd.DataUpdateMultipleLines(lineNumbers, bytes, frameInversion);
            }
        }

        /// <summary>
        /// Lit if average value of RGB is greater than half.
        /// </summary>
        /// <param name="pixel">Input pixel</param>
        /// <returns>True if average value of RGB is greater than half.</returns>
        public static bool LinearBrightnessConvertor(IPixel pixel)
        {
            var vector = pixel.ToScaledVector4();
            return vector.X + vector.Y + vector.Z > 1.5;
        }

        private static void FillImageBufferWithImage<TPixel>(this LSxxxB7DHxx mlcd, Image<TPixel> image, BrightnessConvertor brightnessConvertor)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            if (image.Width != mlcd.PixelWidth)
            {
                throw new ArgumentException($"The width of the image should be {mlcd.PixelWidth}", nameof(image));
            }

            if (image.Height != mlcd.PixelHeight)
            {
                throw new ArgumentException($"The height of the image should be {mlcd.PixelHeight}", nameof(image));
            }

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x += 8)
                {
                    int bx = x / 8;
                    byte dataByte = (byte)(
                        (brightnessConvertor(image[x + 0, y]) ? 0b10000000 : 0) |
                        (brightnessConvertor(image[x + 1, y]) ? 0b01000000 : 0) |
                        (brightnessConvertor(image[x + 2, y]) ? 0b00100000 : 0) |
                        (brightnessConvertor(image[x + 3, y]) ? 0b00010000 : 0) |
                        (brightnessConvertor(image[x + 4, y]) ? 0b00001000 : 0) |
                        (brightnessConvertor(image[x + 5, y]) ? 0b00000100 : 0) |
                        (brightnessConvertor(image[x + 6, y]) ? 0b00000010 : 0) |
                        (brightnessConvertor(image[x + 7, y]) ? 0b00000001 : 0));

                    mlcd._frameBuffer[bx + y * mlcd.BytesPerLine] = dataByte;
                }
            }
        }
    }
}
