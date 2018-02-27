/*
 * Copyright 2016-2018 Sofia University
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union's Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 
 using System;

namespace Assets.Rage.RealTimeArousalDetectionUsingGSRAsset
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
