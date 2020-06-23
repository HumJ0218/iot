﻿using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Drawing;
using Iot.Device.MemoryLcd;

namespace MemoryLcd.Samples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // LSxxxB7DHxx's chip select is high-level votage enabled
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
            GpioController gpio = new GpioController(PinNumberingScheme.Logical);
            LSxxxB7DHxx mlcd = new LS027B7DH01(spi, gpio, 25, 24, 23);

            Random random = new Random();

            Console.WriteLine("Clear. Press any key to continue...");
            Console.ReadKey();

            mlcd.AllClear();

            Console.WriteLine("DataUpdate1Line. Press any key to continue...");
            Console.ReadKey();

            byte[] lineBuffer = new byte[mlcd.BytesPerLine];
            for (int i = 0; i < mlcd.PixelHeight; i++)
            {
                random.NextBytes(lineBuffer);
                mlcd.DataUpdate1Line((byte)i, lineBuffer);
            }

            Console.WriteLine("Clear(inverse). Press any key to continue...");
            Console.ReadKey();

            mlcd.AllClear(true);

            Console.WriteLine("DataUpdateMultipleLines. Press any key to continue...");
            Console.ReadKey();

            byte[] lineNumberBuffer = Enumerable.Range(0, mlcd.PixelHeight).Select(m => (byte)m).ToArray();
            byte[] frameBuffer = new byte[mlcd.BytesPerLine * mlcd.PixelHeight];

            // LS027B7DH01 needs split bytes into 3 spans on RaspberryPi (buffer length is 4096 bytes)
            int frameSplit = mlcd is LS027B7DH01 ? 3 : 1;
            int linesToSend = mlcd.PixelHeight / frameSplit;
            int bytesToSend = frameBuffer.Length / frameSplit;

            random.NextBytes(frameBuffer);
            for (int fs = 0; fs < frameSplit; fs++)
            {
                Span<byte> lineNumbers = lineNumberBuffer.AsSpan(linesToSend * fs, linesToSend);
                Span<byte> bytes = frameBuffer.AsSpan(bytesToSend * fs, bytesToSend);

                mlcd.DataUpdateMultipleLines(lineNumbers, bytes);
            }

            Console.WriteLine("All done. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
