using UnityEngine;

namespace Game
{
    public static class GameConstants 
    {
        private const string _device01 = "d97404f1d7f4fc9e4264571689606ea4dddfb247"; //PhuongND PC
        private const string _device02 = "";

        private const string _device03 = "";
        private const string _device04 = "";

        private const string _device05 = "";

        private static string[] _developerDevicesID = new string[] { _device01, _device02, _device03, _device04, _device05 };

        private static string _cashIconString = "<sprite=\"CashIcon\" index=0>";
        private static string _adsIconString = "<sprite=\"AdsIcon\" index=0>";
        private static string _noInternetIconString = "<sprite=\"NoInternetIcon\" index=0>";
        private static string _assetStoreIconString = "<sprite=\"AssetStoreIcon\" index=0>";

        public static string CashIconString => _cashIconString;
        public static string AdsIconString => _adsIconString;
        public static string NoInternetIconString => _noInternetIconString;
        public static string AssetStoreIconString => _assetStoreIconString;

        public static bool IsDeveloperDevice()
        {
            if(Debug.isDebugBuild)
                return true;

            string deviceID = SystemInfo.deviceUniqueIdentifier;
            foreach (string device in _developerDevicesID)
            {
                if (device == deviceID)
                    return true;
            }
            return false;
        }
    }
}