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
namespace devMobile.IoT.Rfm9x
{
	using System;
	using System.Diagnostics;
	using GHIElectronics.TinyCLR.Devices.Gpio;
	using GHIElectronics.TinyCLR.Devices.Spi;
	using GHIElectronics.TinyCLR.Pins;

	public sealed class RegisterManager
	{
		private SpiDevice rfm9XLoraModem;
		private const byte RegisterAddressReadMask = 0X7f;
		private const byte RegisterAddressWriteMask = 0x80;

		public RegisterManager(int chipSelectPin)
		{
			var settings = new SpiConnectionSettings()
			{
				ChipSelectType = SpiChipSelectType.Gpio,
				ChipSelectLine = chipSelectPin,
				Mode = SpiMode.Mode0,
				ClockFrequency = 500000,
				DataBitLength = 8,
				ChipSelectActiveState = false,
			};

			SpiController spiController = SpiController.FromName(FEZ.SpiBus.Spi1);

			rfm9XLoraModem = spiController.GetDevice(settings);
		}

		public Byte ReadByte(byte registerAddress)
		{
			byte[] writeBuffer = new byte[] { registerAddress &= RegisterAddressReadMask, 0x0 };
			byte[] readBuffer = new byte[writeBuffer.Length];
			Debug.Assert(rfm9XLoraModem != null);

			rfm9XLoraModem.TransferFullDuplex(writeBuffer, readBuffer);

			return readBuffer[1];
		}

		public ushort ReadWord(byte address)
		{
			byte[] writeBuffer = new byte[] { address &= RegisterAddressReadMask, 0x0, 0x0 };
			byte[] readBuffer = new byte[writeBuffer.Length];
			Debug.Assert(rfm9XLoraModem != null);

			rfm9XLoraModem.TransferFullDuplex(writeBuffer, readBuffer);

			return (ushort)(readBuffer[2] + (readBuffer[1] << 8));
		}

		public byte[] Read(byte address, int length)
		{
			byte[] writeBuffer = new byte[length + 1];
			byte[] readBuffer = new byte[length + 1];
			byte[] repyBuffer = new byte[length];
			Debug.Assert(rfm9XLoraModem != null);

			writeBuffer[0] = address &= RegisterAddressReadMask;

			rfm9XLoraModem.TransferFullDuplex(writeBuffer, readBuffer);

			Array.Copy(readBuffer, 1, repyBuffer, 0, length);

			return repyBuffer;
		}

		public void WriteByte(byte address, byte value)
		{
			byte[] writeBuffer = new byte[] { address |= RegisterAddressWriteMask, value };
			Debug.Assert(rfm9XLoraModem != null);

			rfm9XLoraModem.Write(writeBuffer);
		}

		public void WriteWord(byte address, ushort value)
		{
			byte[] valueBytes = BitConverter.GetBytes(value);
			byte[] writeBuffer = new byte[] { address |= RegisterAddressWriteMask, valueBytes[0], valueBytes[1] };
			Debug.Assert(rfm9XLoraModem != null);

			rfm9XLoraModem.Write(writeBuffer);
		}

		public void Write(byte address, byte[] bytes)
		{
			byte[] writeBuffer = new byte[1 + bytes.Length];
			Debug.Assert(rfm9XLoraModem != null);

			Array.Copy(bytes, 0, writeBuffer, 1, bytes.Length);
			writeBuffer[0] = address |= RegisterAddressWriteMask;

			rfm9XLoraModem.Write(writeBuffer);
		}

		public void Dump(byte start, byte finish)
		{
			Debug.WriteLine("Register dump");
			for (byte registerIndex = 0; registerIndex <= 0x42; registerIndex++)
			{
				byte registerValue = this.ReadByte(registerIndex);

				Debug.WriteLine($"Register 0x{registerIndex:x2} - Value 0X{registerValue:x2}");
			}
		}
	}
}
