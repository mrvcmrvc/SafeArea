using UnityEngine;

namespace Mrvc.SafeArea
{
    public enum SafeAreaLookupResult
    {
        ExactMatch,
        PortraitFallback,
        FirstAvailableFallback,
        FullScreenFallback,
    }

    [System.Serializable]
    public struct SafeAreaOrientationData
    {
        [SerializeField] private int orientation;
        [SerializeField] private Rect safeArea;

        public ScreenOrientation Orientation => (ScreenOrientation)orientation;
        public Rect SafeArea => safeArea;

        public SafeAreaOrientationData(ScreenOrientation orientation, Rect safeArea)
        {
            this.orientation = (int)orientation;
            this.safeArea = safeArea;
        }
    }

    public class SafeAreaDeviceInfoAsset : ScriptableObject
    {
        [SerializeField] private string deviceName;
        [SerializeField] private int screenWidth;
        [SerializeField] private int screenHeight;
        [SerializeField] private SafeAreaOrientationData[] orientations = System.Array.Empty<SafeAreaOrientationData>();

        public string DeviceName => deviceName;
        public int ScreenWidth => screenWidth;
        public int ScreenHeight => screenHeight;
        public SafeAreaOrientationData[] Orientations => orientations;

        public bool TryGetSafeArea(ScreenOrientation orientation, out Rect safeArea)
        {
            if (TryGetSafeAreaData(orientation, out SafeAreaOrientationData data))
            {
                safeArea = data.SafeArea;
                return true;
            }

            safeArea = default;
            return false;
        }

        public bool TryGetSafeAreaData(ScreenOrientation orientation, out SafeAreaOrientationData data)
        {
            ScreenOrientation resolvedOrientation = SafeAreaOrientationUtility.ResolveOrientation(
                orientation,
                screenWidth,
                screenHeight,
                GetDefaultOrientation());

            for (int i = 0; i < orientations.Length; i++)
            {
                if (orientations[i].Orientation == resolvedOrientation)
                {
                    data = orientations[i];
                    return true;
                }
            }

            data = default;
            return false;
        }

        public SafeAreaOrientationData GetSafeAreaDataOrFallback(
            ScreenOrientation orientation,
            out SafeAreaLookupResult lookupResult)
        {
            if (TryGetSafeAreaData(orientation, out SafeAreaOrientationData data))
            {
                lookupResult = SafeAreaLookupResult.ExactMatch;
                return data;
            }

            if (TryGetSafeAreaData(ScreenOrientation.Portrait, out data))
            {
                lookupResult = SafeAreaLookupResult.PortraitFallback;
                return data;
            }

            if (orientations.Length > 0)
            {
                lookupResult = SafeAreaLookupResult.FirstAvailableFallback;
                return orientations[0];
            }

            lookupResult = SafeAreaLookupResult.FullScreenFallback;
            return new SafeAreaOrientationData(
                GetDefaultOrientation(),
                new Rect(0f, 0f, screenWidth, screenHeight));
        }

        public Vector2 GetScreenSize(ScreenOrientation orientation)
        {
            return GetScreenSizeForOrientation(orientation, screenWidth, screenHeight);
        }

        public void Init(
            string deviceName,
            int screenWidth,
            int screenHeight,
            SafeAreaOrientationData[] orientations)
        {
            this.deviceName = deviceName;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.orientations = orientations ?? System.Array.Empty<SafeAreaOrientationData>();
        }

        private ScreenOrientation GetDefaultOrientation()
        {
            return orientations.Length > 0 ? orientations[0].Orientation : ScreenOrientation.Portrait;
        }

        private static Vector2 GetScreenSizeForOrientation(
            ScreenOrientation orientation,
            int portraitWidth,
            int portraitHeight)
        {
            return SafeAreaOrientationUtility.IsLandscape(orientation)
                ? new Vector2(portraitHeight, portraitWidth)
                : new Vector2(portraitWidth, portraitHeight);
        }
    }
}
