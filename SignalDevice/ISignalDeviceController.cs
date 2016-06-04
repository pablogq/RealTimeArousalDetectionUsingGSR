namespace SignalDevice
{
    public enum DataMode { Text, Hex };

    public interface ISignalDeviceController
    {
        void SelectCOMPort(string portName);

        void StartSignalsRecord();

        void StopSignalsRecord();

        void OpenPort();

        void SetSignalSamplerate(string speed);
        int GetSignalSampleRate();

        void SetSignalSettings();

        void GetSignalData(byte[] data);

        void SetSignalSamplerate();
    }
}
