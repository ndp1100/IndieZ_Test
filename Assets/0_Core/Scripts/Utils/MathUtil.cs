using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Text;
using Random = System.Random;

namespace Utilities
{
    public static class MathUtil
    {
        private static float secPerYear;

        public static void SetSecPerYear(float value)
        {
            secPerYear = value;
        }

        public static float YearsToSec(float years)
        {
            return years * secPerYear;
        }
        public static float SecToYears(float value)
        {
            return value / secPerYear;
        }

        public static string FloatToString(float value, int decim)
        {
            string _string = "F" + decim;
            return value.ToString(_string);
        }

        public static float SecToMos(float value)
        {
            float _years = value / secPerYear;
            float _mos = _years * 12;
            return _mos;
        }

        public static string TwoDigitCash(float cash)
        {
            return ZString.Concat("$", cash.ToString("F2"));
        }

        public static string NiceCash(int cash)
        {
            string[] suffixes = { "", "k", "m", "b" };
            int suffixIndex;
            int digits;
            if (cash == 0)
            {
                suffixIndex = 0;
                digits = cash.ToString().Length;
            }
            else if (cash > 0)
            {
                suffixIndex = (int)(Mathf.Log10(cash) / 3);
                digits = cash.ToString().Length;
            }
            else
            {
                suffixIndex = (int)(Mathf.Log10(Math.Abs(cash)) / 3);
                digits = Math.Abs(cash).ToString().Length;
            }

            var dividor = Mathf.Pow(10, suffixIndex * 3);
            var text = "";

            if (digits < 4)
                text = (cash / dividor).ToString() + suffixes[suffixIndex];
            else if (digits >= 4 && digits < 7)
                text = (cash / dividor).ToString("F1") + suffixes[suffixIndex];
            else
                text = (cash / dividor).ToString("F2") + suffixes[suffixIndex];
            return text;
        }

        public static long IntToLong(int value)
        {
            return Convert.ToInt64(value);
        }

        public static float HoursToSeconds(float value)
        {
            return value * 3600;
        }

        public static string SecondsToHours(float value)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(value);
            string timerFormatted;
            if (timeSpan.Days == 0)
            {
                timerFormatted = $"{timeSpan.Hours:D1}h {timeSpan.Minutes:D1}m {timeSpan.Seconds:D1}s";
            }
            else timerFormatted =
                $"{timeSpan.Days:D1}d {timeSpan.Hours:D1}h {timeSpan.Minutes:D1}m {timeSpan.Seconds:D1}s";
            return timerFormatted;
        }

        public static string SecondsToMinutes(float value)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(value);
            string timerFormatted;
            string _minutes = "min";
            string _seconds = "s";

            if (timeSpan.Minutes == 0)
            {
                timerFormatted = string.Format("{0:D1}" + _seconds, timeSpan.Seconds);
            }
            else if (timeSpan.Seconds == 0)
            {
                timerFormatted = string.Format("{0:D1}" + _minutes, timeSpan.Minutes);
            }
            else timerFormatted = string.Format("{0:D1}" + _minutes + "{1:D1}" + _seconds, timeSpan.Minutes, timeSpan.Seconds);
            return timerFormatted;
        }

        public static int Seed { get; private set; }

        private static System.Random _random;

        static MathUtil()
        {
            _random = new System.Random();
        }

        public static void InitSeed(int seed)
        {
            Seed = seed;
            _random = new System.Random(seed);
            UnityEngine.Random.InitState(seed);
        }

        public static float GetAngle(Vector3 start, Vector3 end)
        {
            return Mathf.Atan2(start.z - end.z, start.x - end.x) * Mathf.Rad2Deg;
        }

        public static float GetAngle(Vector2 start, Vector2 end)
        {
            return Mathf.Atan2(start.y - end.y, start.x - end.x) * Mathf.Rad2Deg;
        }

        public static long Lerp(double a, double b, float t)
        {
            return (long)(a + (b - a) * Mathf.Clamp01(t));
        }

        public static int Sign(double value)
        {
            return (value >= 0) ? 1 : -1;
        }

        public static int RandomSystem(int min, int max)
        {
            return _random.Next(min, max + 1);
        }

        public static float RandomSystem(float min, float max)
        {
            return (float)_random.NextDouble() * (max + .0001f - min) + min;
        }

        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max + 1);
        }

        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max + .0001f);
        }

        public static string IntToHex(uint crc)
        {
            return $"{crc:X}";
        }

        public static uint HexToInt(string crc)
        {
            return uint.Parse(crc, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        public static bool RandomBool => UnityEngine.Random.value > 0.5f;
        public static int RandomSign => RandomBool ? 1 : -1;

        public static Vector2 AddRotateAround(Vector2 center, Vector2 point, float angleInDegree)
        {
            var angle = GetAngle(point, center);
            angleInDegree += angle;
            var radius = Vector2.Distance(center, point);
            return center + new Vector2(Mathf.Cos(angleInDegree * Mathf.Deg2Rad), Mathf.Sin(angleInDegree * Mathf.Deg2Rad)) * radius;
        }

        public static Color IntToColor(int value)
        {
            Color c;

            c.a = ((value >> 24) & 0xff) / 255f;
            c.r = ((value >> 16) & 0xff) / 255f;
            c.g = ((value >> 8) & 0xff) / 255f;
            c.b = ((value) & 0xff) / 255f;

            return c;
        }

        public static int ColorToInt(Color color)
        {
            byte a = (byte)(color.a * 255);
            byte r = (byte)(color.r * 255);
            byte g = (byte)(color.g * 255);
            byte b = (byte)(color.b * 255);

            return (a & 0xff) << 24 | (r & 0xff) << 16 | (g & 0xff) << 8 | (b & 0xff);
        }

        public static Vector3 PowVector3(Vector3 vector, float p)
        {
            return new Vector3
            {
                x = Mathf.Pow(vector.x, p),
                y = Mathf.Pow(vector.y, p),
                z = Mathf.Pow(vector.z, p)
            };
        }

        public static Vector3 SqrtVector3(Vector3 vector)
        {
            return new Vector3
            {
                x = Mathf.Sqrt(vector.x),
                y = Mathf.Sqrt(vector.y),
                z = Mathf.Sqrt(vector.z)
            };
        }

        public static Vector3 AbsVector3(Vector3 vector)
        {
            return new Vector3
            {
                x = Mathf.Abs(vector.x),
                y = Mathf.Abs(vector.y),
                z = Mathf.Abs(vector.z)
            };
        }

        public static float ConvertWithClamp(float value, float in_min, float in_max, float out_min, float out_max)
        {
            return (value - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }


        //"Position": "(-12.52, 0.65, 39.69)",
        //Convert string to Vector3
        public static Vector3 StringToVector3(string value)
        {
            value = value.Replace("(", "").Replace(")", "");
            // string[] sArray = value.Split(',');
            // return new Vector3(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]));

            string[] sArray = value.Split(new string[] { "," }, StringSplitOptions.None);

            float x = ParseGlobalFloat(sArray[0]);
            float y = ParseGlobalFloat(sArray[1]);
            float z = ParseGlobalFloat(sArray[2]);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Performs an in-place shuffle of an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static T[] Shuffle<T>(this T[] instance)
        {
            for (int i = 0; i < instance.Length; ++i)
            {
                int j = rng.Next(i, instance.Length); // select a random j such that i <= j < instance.Length

                // swap instance[i] and instance[j]
                T x = instance[j];
                instance[j] = instance[i];
                instance[i] = x;

            }

            return instance;
        }
        private static readonly Random rng = new Random();
        public static Quaternion StringToQuaternion(string value)
        {
            value = value.Replace("(", "").Replace(")", "");
            // string[] sArray = value.Split(',');
            // return new Quaternion(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]), float.Parse(sArray[3]));

            string[] sArray = value.Split(new string[] { "," }, StringSplitOptions.None);

            float x = ParseGlobalFloat(sArray[0]);
            float y = ParseGlobalFloat(sArray[1]);
            float z = ParseGlobalFloat(sArray[2]);
            float w = ParseGlobalFloat(sArray[3]);

            return new Quaternion(x, y, z, w);
        }

        public static float ParseGlobalFloat(string value)
        {
            float result;
            if (!float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result))
            {
                result = 0;
            }
            return result;
        }

        public static List<T> SelectNRandomElements<T>(List<T> list, int n)
        {
            Random rand = new Random();
            return list.OrderBy(x => rand.Next()).Take(n).ToList();
        }
    }
}