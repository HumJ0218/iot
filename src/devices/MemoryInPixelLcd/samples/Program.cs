// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Device.Gpio;
using System.Device.Spi;
using Iot.Device.MemoryInPixelLcd;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

// *** LSxxxB7DHxx's chip select is high-level votage enabled ***
// You can use default ChipSelectLineActiveState value and a NOT gate to inverse this signal
// If you use gpio pins to control SCS, ChipSelectLineActiveState is optional
SpiDevice spi = SpiDevice.Create(new SpiConnectionSettings(0, 0)
{
    ChipSelectLineActiveState = 1, // optional
    ClockFrequency = 2_000_000,
    DataFlow = DataFlow.MsbFirst,
    Mode = 0
});

// You can fix DISP and EXTCOMIN in your circuit and use SPI's CE line to economize the GPIO pins
// DISP -- HIGH
// EXTCOMIN -- LOW
// EXTMODE -- LOW
// LSxxxB7DHxx mlcd = new LS027B7DH01(spi);
GpioController gpio = new GpioController(PinNumberingScheme.Logical);
LSxxxB7DHxx mlcd = new LS027B7DH01(spi, gpio, false, 25, 24, 23);

Clear();
Console.ReadLine();

RandomDots();
Console.ReadLine();

ShowImage();

void Clear()
{
    Console.WriteLine();
    Console.WriteLine("Clear");

    var image = new Image<Rgb24>(mlcd.PixelWidth, mlcd.PixelHeight);
    mlcd.ShowImage(image, split: 4);
}

void RandomDots()
{
    Console.WriteLine("Random dots");

    var image = new Image<Rgb24>(mlcd.PixelWidth, mlcd.PixelHeight);

    var random = new Random();
    for (var x = 0; x < image.Width; x++)
    {
        for (var y = 0; y < image.Height; y++)
        {
            image[x, y] = new Rgb24((byte)random.Next(), (byte)random.Next(), (byte)random.Next());
        }
    }

    mlcd.ShowImage(image, split: 4);
}

void ShowImage()
{
    Console.WriteLine("Show image");

    var image = Image.Load<Rgb24>("./dotnet-bot.png");
    mlcd.ShowImage(image, split: 4);
}
