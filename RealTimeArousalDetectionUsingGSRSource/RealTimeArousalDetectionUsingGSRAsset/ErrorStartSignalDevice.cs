using System;

namespace Assets.Rage.GSRAsset.Integrator
{
    public class ErrorStartSignalDevice
    {
        #region Field
        private string errorContent;
        private ErrorType errorType1;
        private Exception exception;
        #endregion Field

        #region Contructors
        public ErrorStartSignalDevice()
        {

        }

        public ErrorStartSignalDevice(string errorContent, ErrorType errorType, Exception exception)
        {
            this.ErrorContent = errorContent;
            this.ErrorType1 = errorType;
            this.Exception = exception;
        }
        #endregion Constructors

        #region Properties
        public string ErrorContent
        {
            get
            {
                return errorContent;
            }

            set
            {
                errorContent = value;
            }
        }

        public ErrorType ErrorType1
        {
            get
            {
                return errorType1;
            }

            set
            {
                errorType1 = value;
            }
        }

        public Exception Exception
        {
            get
            {
                return exception;
            }

            set
            {
                exception = value;
            }
        }
        #endregion Properties

        public enum ErrorType
        {
            None,
            ErrorComPort,
            ErrorOpenPort,
            ErrorStartSignalDevice
        };
    }
}
