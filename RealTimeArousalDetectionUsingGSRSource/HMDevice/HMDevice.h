// HMDevice.h

#pragma once
#include <windows.h>
#include <iostream>
#include <string>
#include <cstring>
#include <sstream>
#include <fstream>
#include <ctime>

using namespace System;
using namespace std;
using namespace System;
using namespace System::IO;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Data;
using namespace System::IO::Ports;


namespace HMDevice {

	public ref class HMDevice
	{
	    private: System::IO::Ports::SerialPort^  _serialPort;
		private: static const int topDataCounter = 12000;
		private: static int GSvalue = 1;
		private: static int GSres = 1;
		private: static double GSsi = 1;
		private: static unsigned char counter = 0;
		private: static unsigned char byteReceived = 3;
		private: static bool dataRxEnable = false;
		private: int tempDevider;
	    private: int sampleRate;

		public:HMDevice(void)
			{
				// 
				// _serialPort
				// 
				this->_serialPort = (gcnew System::IO::Ports::SerialPort());
				
				this->_serialPort->PortName = L"COM6";
				this->_serialPort->ReadTimeout = 500;
				this->_serialPort->WriteTimeout = 500;

				this->_serialPort->DataReceived += gcnew SerialDataReceivedEventHandler(this, &HMDevice::DataReceivedHandler);
				this->_serialPort->PinChanged += gcnew SerialPinChangedEventHandler(port_PinChanged);
			}

		protected:~HMDevice() 		
			{
			
			}

		// find available ports
		private: static void port_PinChanged(Object^ sender, SerialPinChangedEventArgs^ e)
		{
			//this->Invoke(new ThreadStart(())
		}

	private:
		void DataReceivedHandler(Object^ sender, SerialDataReceivedEventArgs^ e)
		{
			if (!this->_serialPort->IsOpen) return;
			else
			{
				// Obtain the number of bytes waiting in the port's buffer
				int bytes = _serialPort->BytesToRead;
				// Create a byte array buffer to hold the incoming data
				cli::array<System::Byte>^ buffer = gcnew cli::array<System::Byte>(bytes);
				// Read the data from the port and store it in our buffer
				this->_serialPort->Read(buffer, 0, bytes);
				System::String^ s = "";
				for each(System::Byte b in buffer)
				{
					int readValue = (int)b;
					if (readValue < 63 && readValue >= 32 && byteReceived == 3)
					{
						byteReceived = 2;
						GSvalue = (readValue - 32) * 256;
					}
					if (readValue < 32 && readValue >= 16 && byteReceived == 2)
					{
						byteReceived = 1;
						GSvalue += (readValue - 16) * 16;
					}
					if (readValue < 16 && readValue >= 0 && byteReceived == 1)
					{
						byteReceived = 3;
						GSvalue += readValue;
						tempDevider = 5250 - GSvalue;
						if (tempDevider <= 0) tempDevider = 1;
						GSres = 300 * GSvalue / tempDevider;  
						if (GSres <= 0) GSres = 1;
						GSsi = 1000000 / GSres;
						if (dataRxEnable) //eliminate first wrong data and data after fs is closed
						{
							CacheSignalData c;
							if (GSsi > 0)
								c.AddChannelCacheValue(0, GSsi);
						}
						//s += GSsi.ToString() + " ";
					}
				}
				dataRxEnable = true;
			}
		}

		public: System::Void StartSignalsTransfer()
		{
			this->_serialPort->WriteLine("B");	//Begin data transfer
		}

		public: System::Void StopSignalsTransfer()
		{
			this->_serialPort->WriteLine("E");	//End data transfer + maybe delay?
			counter = 0;
			byteReceived = 3;
			dataRxEnable = false;
		}

		public: int GetSignalSampleRate()
		{
			return this->sampleRate;
		}

		public: System::Void SelectCOMPort(String ^portName)
		{
			this->_serialPort->PortName = portName;
		}

		public: System::Void SetSignalSamplerate(int sampleRate)
		{
			this->sampleRate = sampleRate;
			auto dataSend = gcnew cli::array<System::Byte> { 0xF0 };
			
			if (sampleRate == 10)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xF0 };
			}
			if (sampleRate == 20)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xF1 };
			}
			if (sampleRate == 40)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xF2 };
			}
			if (sampleRate == 50)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xF3 };
			}
			if (sampleRate == 100)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xF4 };
			}
			if (sampleRate == 200)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xF5 };
			}
			if (sampleRate == 250)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xF6 };
			}
			if (sampleRate == 400)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xF7 };
			}
			if (sampleRate == 500)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xF8 };
			}
			if (sampleRate == 1000)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xF9 };
			}
			if (sampleRate == 2000)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xFA };
			}
			if (sampleRate == 2500)
			{
				auto dataSend = gcnew cli::array<System::Byte> { 0xFB };
			}
			
			_serialPort->Write(dataSend, 0, dataSend->Length);
		}

		public: bool IsPortOpen()
		{
			return this->_serialPort->IsOpen;
		}
		
	    public: System::Void OpenPort()
	    {
				try
				{
					// make sure port isn't open	
					if (!this->_serialPort->IsOpen)
					{
						dataRxEnable = false;
						this->_serialPort->BaudRate = 115200;
						//open serial port 
						this->_serialPort->Open();
					}
					else
					{
						//"Port isn't openned";
					}
				}
				catch (UnauthorizedAccessException^) {
					// "UnauthorizedAccess";
				}
			
	    }

	};
}
