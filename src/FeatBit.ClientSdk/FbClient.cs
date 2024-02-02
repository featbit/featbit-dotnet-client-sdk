using FeatBit.ClientSdk.Concurrent;
using FeatBit.ClientSdk.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace FeatBit.ClientSdk
{
    public class FbClient : IFbClient
    {
        public bool Initialized => throw new NotImplementedException();

        public void Bootstrap(List<FeatureFlag> initFeatureFlagsValue)
        {
            
        }

        public List<FeatureFlag> ExportFeatureFlags()
        {
            throw new NotImplementedException();
        }

        public bool BoolVariation(string key, bool defaultValue = false)
        {
            FeatureFlag ff = new FeatureFlag();
            if (FeatureFlagsCollection<String, FeatureFlag>.Instance.TryGetValue(key, out ff) == true)
            {
                if(ff.VariationType.ToLower() == "boolean")
                {
                    return ff.Variation == "true";
                }
                else
                {
                    throw new Exception("Variation type is not boolean");
                }
            }
            else
            {
                FeatureFlagsCollection<String, FeatureFlag>.Instance.AddOrUpdate(key, new FeatureFlag
                {
                    Id = key,
                    Variation = defaultValue.ToString().ToLower(),
                    VariationType = "boolean"
                });
                return defaultValue;
            }
        }

        public double DoubleVariation(string key, double defaultValue = 0)
        {
            FeatureFlag ff = new FeatureFlag();
            if (FeatureFlagsCollection<String, FeatureFlag>.Instance.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() == "number")
                {
                    return Convert.ToDouble(ff.Variation);
                }
                else
                {
                    throw new Exception("Variation type is not Double");
                }
            }
            else
            {
                FeatureFlagsCollection<String, FeatureFlag>.Instance.AddOrUpdate(key, new FeatureFlag
                {
                    Id = key,
                    Variation = defaultValue.ToString().ToLower(),
                    VariationType = "number"
                });
                return defaultValue;
            }
        }

        public float FloatVariation(string key, float defaultValue = 0)
        {
            FeatureFlag ff = new FeatureFlag();
            if (FeatureFlagsCollection<String, FeatureFlag>.Instance.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() == "number")
                {
                    return float.Parse(ff.Variation.ToLower(), CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    throw new Exception("Variation type is not Float");
                }
            }
            else
            {
                FeatureFlagsCollection<String, FeatureFlag>.Instance.AddOrUpdate(key, new FeatureFlag
                {
                    Id = key,
                    Variation = defaultValue.ToString().ToLower(),
                    VariationType = "number"
                });
                return defaultValue;
            }
        }

        public object? ObjectVariation(string key, object defaultValue = null)
        {
            FeatureFlag ff = new FeatureFlag();
            if (FeatureFlagsCollection<String, FeatureFlag>.Instance.TryGetValue(key, out ff) == true)
            {
                if (ff.VariationType.ToLower() == "string")
                {
                    return JsonConvert.DeserializeObject(ff.Variation);
                }
                else
                {
                    throw new Exception("Variation type is not Json");
                }
            }
            else
            {
                FeatureFlagsCollection<String, FeatureFlag>.Instance.AddOrUpdate(key, new FeatureFlag
                {
                    Id = key,
                    Variation = defaultValue.ToString().ToLower(),
                    VariationType = "string"
                });
                return defaultValue;
            }
            throw new NotImplementedException();
        }

        public int IntVariation(string key, int defaultValue = 0)
        {
            throw new NotImplementedException();
        }

        public string StringVariation(string key, string defaultValue = "")
        {
            throw new NotImplementedException();
        }

        public void Track(FbIdentity user, string eventName)
        {
            throw new NotImplementedException();
        }

        public void Track(FbIdentity user, string eventName, double metricValue)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public bool FlushAndWait(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }
    }
}
