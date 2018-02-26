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

using Assets.Rage.RealTimeArousalDetectionUsingGSRAsset.Utils;
using System.Collections.Generic;

namespace Assets.Rage.RealTimeArousalDetectionUsingGSRAsset.SignalDevice
{
    /// <summary>
    /// For each channel (GSR channel and HR channel) we save the data received by the used device in a cache
    /// </summary>
    public class Cache
    {
        private readonly static int CACHE_MAX_SIZE = (int)(RealTimeArousalDetectionAssetSettings.Instance.DefaultTimeWindow * (1000/RealTimeArousalDetectionAssetSettings.Instance.Samplerate));
        private static Dictionary<int, List<double>> channelsCache = new Dictionary<int, List<double>>();

        public Cache()
        {
            //super();

        }

        /// <summary>
        /// Insert value into the cache using
        /// appropriate number of channel and the value
        /// </summary>
        /// <param name="cacheValue">The current data received by the device
        /// </param>
        /// <param name="channel">Name of the channel</param>
        public static void AddChannelCacheValue(int channel, int cacheValue)
        {
            RemoveEldestChannelValue(channel);
            if (DoesKeyExist(channel))
            {
                channelsCache[channel].Add(cacheValue);
            }
            else
            {
                channelsCache.Add(channel, new List<double>() { cacheValue });
            }
        }

        /// <summary>
        /// Insert list of values into the cache using
        /// appropriate number of channel and the value
        /// </summary>
        /// <param name="cacheValue">The current set of data received by the device
        /// </param>
        /// <param name="channel">Name of the channel</param>
        public static void AddChannelCacheValue(int channel, List<double> cacheValues)
        {
            RemoveEldestChannelValues(channel, cacheValues.Count);
            if (DoesKeyExist(channel))
            {
                channelsCache[channel].AddRange(cacheValues);
            }
            else
            {
                channelsCache.Add(channel, cacheValues);
            }
        }

        /// <summary>
        /// Remove the eldest value in the cache in order to save the current value instead of it
        /// </summary>
        /// <param name="channel">Name of the channel</param>
        private static void RemoveEldestChannelValue(int channel)
        {
            if (DoesKeyExist(channel) && IsMaxSizeCacheHit(channel))
            {
                channelsCache[channel].Remove(0);
            }
        }

        /// <summary>
        /// Remove the eldest value in the cache in order to save the current value instead of it
        /// </summary>
        /// <param name="channel">Name of the channel</param>
        private static void RemoveEldestChannelValues(int channel, int numberOfValues)
        {
            if (DoesKeyExist(channel) && WillMaxSizedCacheHit(channel, numberOfValues))
            {
                int extraNumbers = channelsCache[channel].Count + numberOfValues - CACHE_MAX_SIZE;
                for (int i = 0; i < extraNumbers; i++)
                {
                    channelsCache[channel].Remove(0);
                }
            }
        }

        private static bool WillMaxSizedCacheHit(int channel, int numberOfValues)
        {
            return (channelsCache[channel].Count + numberOfValues > CACHE_MAX_SIZE);
        }

        /// <summary>
        /// Check for item in cache
        /// </summary>
        /// <param name="key">Name of cached item</param>
        /// <returns>True if the key already exists in the cache</returns>
        private static bool DoesKeyExist(int key)
        {
            return channelsCache.ContainsKey(key);
        }

        private static bool IsMaxSizeCacheHit(int channel)
        {
            return (channelsCache[channel].Count + 1 > CACHE_MAX_SIZE);
        }

        /// <summary>
        /// Gets all cached items as a list by their key.
        /// </summary>
        /// <returns>A list of all cached items</returns>
        public static List<double> GetAllForChannel(int channel)
        {
            if (channelsCache != null && channelsCache.ContainsKey(channel))
            {
                return channelsCache[channel];
            }

            return null;
        }

        /// <summary>
        /// Gets all cached items as a list by their key.
        /// </summary>
        /// <returns>A list of all cached items</returns>
        public static Dictionary<int, List<double>> GetChannelsCache()
        {
            return channelsCache;
        }

    }
}
