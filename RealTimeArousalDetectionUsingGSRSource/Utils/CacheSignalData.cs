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

using System;
using System.Collections.Generic;
using Assets.Rage.GSRAsset.Utils;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Globalization;
using AssetPackage;
using AssetManagerPackage;

public class CacheSignalData
{
    private readonly static int CACHE_MAX_SIZE = 1000;
    private static List<SignalDataByTime> signalValuesCache;
    private static ILog  logger = (ILog)AssetManager.Instance.Bridge;

    private RealTimeArousalDetectionAssetSettings config;
    private static String applicationMode;
    private static int sampleRate;
    private static CacheSignalData instance;
    private static FilterButterworth butterworthHighPassFilter;
    private static FilterButterworth butterworthLowPassFilter;
    private static double HIGHPASS_ADJUSTING_VARIABLE = 3000.00;

    public static CacheSignalData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CacheSignalData();
                            }
            return instance;
        }
    }

    private CacheSignalData()
    {
        signalValuesCache = new List<SignalDataByTime>();
        config = RealTimeArousalDetectionAssetSettings.Instance;
        double butterworthPhasicFrequency = config.ButterworthPhasicFrequency;
        double butterworthTonicFrequency = config.ButterworthTonicFrequency;
        sampleRate = config.Samplerate;
        butterworthHighPassFilter = new FilterButterworth(butterworthPhasicFrequency, sampleRate, ButterworthPassType.Highpass, "compute EDA high");
        butterworthLowPassFilter = new FilterButterworth(butterworthTonicFrequency, sampleRate, ButterworthPassType.Lowpass, "compute EDA low");

        if ("TestWithoutDevice".Equals(config.ApplicationMode))
        {
            FillTestGSRDataFromFile();
        }

        applicationMode = config.ApplicationMode;
    }

    private void FillTestGSRDataFromFile()
    {
        
        using (StreamReader sr = new StreamReader(config.TestData, Encoding.Default))
        {
            string text = sr.ReadToEnd();
            string[] lines = text.Split(';');
            for(int i = 0; i < lines.Length; i++)
            {
                if(!String.IsNullOrEmpty(lines[i])) AddSignalValue(Int32.Parse(Regex.Replace(lines[i], @"\s+", ""), CultureInfo.InvariantCulture));
            }
        }
        
    }

    /// <summary>
    /// Insert value into the cache using the value
    /// </summary>
    /// <param name="cacheValue">The current data received by the device
    /// </param>
    public static void AddSignalValue(double cacheValue)
    {
        if (IsMaxSizeCacheHit())
        {
            RemoveEldestCacheValue();
        }

        lock (signalValuesCache)
        {
            double milliseconds = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            signalValuesCache.Add(new SignalDataByTime(milliseconds, cacheValue, 
                                                       butterworthHighPassFilter.GetFilterValue(cacheValue) + HIGHPASS_ADJUSTING_VARIABLE, 
                                                       butterworthLowPassFilter.GetFilterValue(cacheValue)));
        }        
    }

    /// <summary>
    /// Remove the eldest value in the cache in order to save the current value instead of it
    /// </summary>
    /// <param name="channel">Name of the channel</param>
    private static void RemoveEldestCacheValue()
    {
            signalValuesCache.RemoveAt(0);
    }

    private static bool IsMaxSizeCacheHit()
    {
        return signalValuesCache != null && (signalValuesCache.Count > CACHE_MAX_SIZE);
    }

    /// <summary>
    /// Gets all cached items as a list by their key.
    /// </summary>
    /// <returns>A list of all cached items</returns>
    public static SignalDataByTime[] GetCacheData()
    {
        SignalDataByTime[] signalValuesArray;
        lock (signalValuesCache)
        {
            signalValuesArray = signalValuesCache.ToArray();
        }

        int numberNotNullListItem = 0;
        for (int i = 0; i < signalValuesArray.Length - 100; i++)
        {
            if (signalValuesArray[i] != null) numberNotNullListItem++;
        }

        SignalDataByTime[] result = new SignalDataByTime[numberNotNullListItem];
        if (numberNotNullListItem == signalValuesArray.Length) result = signalValuesArray;
        else
        {
            int j = 0;
            for (int i = 0; i < signalValuesArray.Length - 100; i++)
            {
                if (signalValuesArray[i] != null)
                {
                    result[j] = signalValuesArray[i];
                    if ("TestWithoutDevice".Equals(applicationMode) && j > 0)
                    {
                        result[j].Time = result[j - 1].Time + sampleRate;
                    }
                    else if ("TestWithoutDevice".Equals(applicationMode) && j == 0)
                    {
                        result[j].Time = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                    }
                    j++;
                }
            }
        }

        return result;
    }

    private List<SignalDataByTime> CopyListItems(SignalDataByTime[] list)
    {
        List<SignalDataByTime> result = new List<SignalDataByTime>();
        foreach (SignalDataByTime listItem in list)
        {
            if (listItem != null) result.Add(listItem);
        }

        return result;
    }
}
