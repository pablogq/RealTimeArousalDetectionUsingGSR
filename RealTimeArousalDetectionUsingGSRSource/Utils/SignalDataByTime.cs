/*
 * Copyright 2016 Sofia University
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

public class SignalDataByTime
{
    #region Fields
    private double time;
    private double signalValue;
    private double highPassValue;
    private double lowPassValue;
    #endregion Fields

    public SignalDataByTime()
    {
        //
    }

    public SignalDataByTime(double time, double signalValue)
    {
        this.time = time;
        this.signalValue = signalValue;
    }

    public SignalDataByTime(double time, double signalValue, double highPassValue, double lowPassValue)
    {
        this.time = time;
        this.signalValue = signalValue;
        this.highPassValue = highPassValue;
        this.lowPassValue = lowPassValue;
    }

    #region Properties
    public double Time
    {
        get
        {
            return time;
        }

        set
        {
            time = value;
        }
    }

    public double SignalValue
    {
        get
        {
            return signalValue;
        }

        set
        {
            signalValue = value;
        }
    }

    public double HighPassValue
    {
        get
        {
            return highPassValue;
        }

        set
        {
            highPassValue = value;
        }
    }

    public double LowPassValue
    {
        get
        {
            return lowPassValue;
        }

        set
        {
            lowPassValue = value;
        }
    }
    #endregion Properties
}
