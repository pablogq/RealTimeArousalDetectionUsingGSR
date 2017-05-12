#pragma once

#using <System.dll>
#using <Utils.dll>

using namespace System;
using namespace System::IO;
using namespace System::ComponentModel;
using namespace System::Collections;
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
	private: CacheSignalData^ cacheData;

	public:HMDevice(void);
	protected:~HMDevice();
	private: static void port_PinChanged(Object^ sender, SerialPinChangedEventArgs^ e);
	private: void DataReceivedHandler(Object^ sender, SerialDataReceivedEventArgs^ e);
	public:	void StartSignalsTransfer();
	public: void StopSignalsTransfer();
	public: int GetSignalSampleRate();
	public: void SelectCOMPort(String ^portName);
	public: void SelectCOMPortSampleRate(String ^portName, int sampleRate);
	public: void SetSignalSamplerate(int sampleRate);
	public: bool IsPortOpen();
	public: void OpenPort();
	public: void OpenPort(int sampleRate);
	};
}