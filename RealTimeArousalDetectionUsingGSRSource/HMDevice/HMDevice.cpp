#include "HMDevice.h"

namespace HMDevice {

	HMDevice::HMDevice()
	{
		this->_serialPort = (gcnew System::IO::Ports::SerialPort());

		this->_serialPort->PortName = L"COM6";
		this->_serialPort->ReadTimeout = 500;
		this->_serialPort->WriteTimeout = 500;

		this->_serialPort->DataReceived += gcnew SerialDataReceivedEventHandler(this, &HMDevice::DataReceivedHandler);
		this->_serialPort->PinChanged += gcnew SerialPinChangedEventHandler(port_PinChanged);
		cacheData = CacheSignalData::Instance;
	}

	HMDevice::~HMDevice()
	{

	}

	void HMDevice::port_PinChanged(Object^ sender, SerialPinChangedEventArgs^ e)
	{
		
	}

	void HMDevice::DataReceivedHandler(Object^ sender, SerialDataReceivedEventArgs^ e)
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

					if (GSvalue <= 0)
					{
						GSvalue = 1;
					}

					GSres = 500 * GSvalue;	//resistance
					GSsi = 1000000000 / GSres;

					if (dataRxEnable) //eliminate first wrong data and data after fs is closed
					{
						if (GSsi > 0)
							cacheData->AddSignalValue(GSsi);
					}
				}
			}
			dataRxEnable = true;
		}
	}


    void HMDevice::StartSignalsTransfer()
	{
		_serialPort->WriteLine("B");	//Begin data transfer
	}

	System::Void HMDevice::StopSignalsTransfer()
	{
		this->_serialPort->WriteLine("E");	//End data transfer + delay
		counter = 0;
		byteReceived = 3;
		dataRxEnable = false;
	}

	int HMDevice::GetSignalSampleRate()
	{
		return this->sampleRate;
	}

	System::Void HMDevice::SelectCOMPort(String ^portName)
	{
		if (portName->Equals("N.A."))
		{
			array<String^>^ objectArray = SerialPort::GetPortNames();
			this->_serialPort->PortName = objectArray[0];
		}
		else
		{
			this->_serialPort->PortName = portName;
		}
	}

	System::Void HMDevice::SelectCOMPortSampleRate(String ^portName, int sampleRate)
	{
		if (portName->Equals("N.A."))
		{
			array<String^>^ objectArray = SerialPort::GetPortNames();
			this->_serialPort->PortName = objectArray[0];
		}
		else
		{
			this->_serialPort->PortName = portName;
		}

		SetSignalSamplerate(sampleRate);
	}


	System::Void HMDevice::SetSignalSamplerate(int sampleRate)
	{
		this->sampleRate = sampleRate;
		auto dataSend = gcnew cli::array<System::Byte> { 0xF0 };

		if (sampleRate == 10)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xF0 };
		}
		if (sampleRate == 20)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xF1 };
		}
		if (sampleRate == 40)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xF2 };
		}
		if (sampleRate == 50)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xF3 };
		}
		if (sampleRate == 100)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xF4 };
		}
		if (sampleRate == 200)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xF5 };
		}
		if (sampleRate == 250)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xF6 };
		}
		if (sampleRate == 400)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xF7 };
		}
		if (sampleRate == 500)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xF8 };
		}
		if (sampleRate == 1000)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xF9 };
		}
		if (sampleRate == 2000)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xFA };
		}
		if (sampleRate == 2500)
		{
			dataSend = gcnew cli::array<System::Byte> { 0xFB };
		}

		_serialPort->Write(dataSend, 0, dataSend->Length);
	}

	bool HMDevice::IsPortOpen()
	{
		return this->_serialPort->IsOpen;
	}

	System::Void HMDevice::OpenPort()
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
		catch (UnauthorizedAccessException^) 
		{
			// "UnauthorizedAccess";
		}
	}

	System::Void HMDevice::OpenPort(int sampleRate)
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
				SetSignalSamplerate(sampleRate);
			}
			else
			{
				//"Port isn't openned";
			}
		}
		catch (UnauthorizedAccessException^) 
		{
			// "UnauthorizedAccess";
		}
	}
}