namespace Assets.Rage.GSRAsset.SignalDevice
{
    public enum DataMode { Text, Hex };

    public interface ISignalDeviceController
    {
        void SelectCOMPort(string portName);

        int StartSignalsRecord();

        int StopSignalsRecord();

        void OpenPort();

        int SetSignalSamplerate(string speed);

        int GetSignalSampleRate();

        void SetSignalSettings();

        void GetSignalData(byte[] data);

        void SetSignalSamplerate();
    }
}
