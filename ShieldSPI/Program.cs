﻿//---------------------------------------------------------------------------------
// Copyright (c) March 2020, devMobile Software
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
//---------------------------------------------------------------------------------
namespace devMobile.IoT.Rfm9x.ShieldSpi
{
   using System;
   using System.Diagnostics;

   using System.Threading;
   using GHIElectronics.TinyCLR.Devices.Spi;
   using GHIElectronics.TinyCLR.Pins;

   class Program
   {
      static void Main()
      {
         var settings = new SpiConnectionSettings()
         {
            ChipSelectType = SpiChipSelectType.Gpio,
            ChipSelectLine = FEZ.GpioPin.D10,
            Mode = SpiMode.Mode0,
            //Mode = SpiMode.Mode1,
            //Mode = SpiMode.Mode2,
            //Mode = SpiMode.Mode3,
            ClockFrequency = 500000,
            DataBitLength = 8,
            //ChipSelectActiveState = true
            ChipSelectActiveState = false,
            //ChipSelectHoldTime = new TimeSpan(0, 0, 0, 0, 500),
            //ChipSelectSetupTime = new TimeSpan(0, 0, 0, 0, 500),
         };

         var controller = SpiController.FromName(FEZ.SpiBus.Spi1);
         var device = controller.GetDevice(settings);

         Thread.Sleep(500);

         while (true)
         {
            byte register;
            byte[] writeBuffer;
            byte[] readBuffer;

            // Silicon Version info
            register = 0x42; // RegVersion expecting 0x12

            // Frequency
            //register = 0x06; // RegFrfMsb expecting 0x6C
            //register = 0x07; // RegFrfMid expecting 0x80
            //register = 0x08; // RegFrfLsb expecting 0x00

            //register = 0x17; //RegPayoadLength expecting 0x47

            // Preamble length 
            //register = 0x18; // RegPreambleMsb expecting 0x32
            //register = 0x19; // RegPreambleLsb expecting 0x3E

            //register <<= 1;
            //register |= 0x80;

            //writeBuffer = new byte[] { register };
            writeBuffer = new byte[] { register,  0x0 };
            //writeBuffer = new byte[] { register, register, 0x0 };

            readBuffer = new byte[writeBuffer.Length];

            //device.TransferSequential(writeBuffer, readBuffer);
            device.TransferFullDuplex(writeBuffer, readBuffer);

            Debug.WriteLine("Value = 0x" + BytesToHexString(readBuffer));

            Thread.Sleep(1000);
         }
      }

      private static string BytesToHexString(byte[] bytes)
      {
         string hexString = string.Empty;

         // Create a character array for hexidecimal conversion.
         const string hexChars = "0123456789ABCDEF";

         // Loop through the bytes.
         for (byte b = 0; b < bytes.Length; b++)
         {
            if (b > 0)
               hexString += "-";

            // Grab the top 4 bits and append the hex equivalent to the return string.        
            hexString += hexChars[bytes[b] >> 4];

            // Mask off the upper 4 bits to get the rest of it.
            hexString += hexChars[bytes[b] & 0x0F];
         }

         return hexString;
      }
   }
}