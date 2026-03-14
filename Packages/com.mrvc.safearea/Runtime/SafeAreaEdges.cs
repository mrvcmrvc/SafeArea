using System;
using UnityEngine;

namespace Mrvc.SafeArea
{
    [Flags]
    public enum SafeAreaEdges
    {
        Left   = 1 << 0,
        Right  = 1 << 1,
        Top    = 1 << 2,
        Bottom = 1 << 3,
    }

    public readonly struct SafeAreaInsets
    {
        public SafeAreaInsets(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public float Left { get; }
        public float Right { get; }
        public float Top { get; }
        public float Bottom { get; }
    }

    public static class SafeAreaOrientationUtility
    {
        public static ScreenOrientation ResolveOrientation(
            ScreenOrientation reportedOrientation,
            float width,
            float height,
            ScreenOrientation fallbackOrientation = ScreenOrientation.Portrait)
        {
            if (IsExplicitOrientation(reportedOrientation))
            {
                return reportedOrientation;
            }

            ScreenOrientation sanitizedFallback = SanitizeOrientation(fallbackOrientation);

            if (width > height)
            {
                return IsLandscape(sanitizedFallback)
                    ? sanitizedFallback
                    : ScreenOrientation.LandscapeLeft;
            }

            if (height > width)
            {
                return IsPortrait(sanitizedFallback)
                    ? sanitizedFallback
                    : ScreenOrientation.Portrait;
            }

            return sanitizedFallback;
        }

#if UNITY_EDITOR
        public static ScreenOrientation ResolvePreviewOrientation(
            ScreenOrientation reportedOrientation,
            float width,
            float height,
            ScreenOrientation fallbackOrientation = ScreenOrientation.Portrait)
        {
            ScreenOrientation sanitizedFallback = SanitizeOrientation(fallbackOrientation);

            if (width > height)
            {
                if (IsLandscape(reportedOrientation))
                {
                    return reportedOrientation;
                }

                return IsLandscape(sanitizedFallback)
                    ? sanitizedFallback
                    : ScreenOrientation.LandscapeLeft;
            }

            if (height > width)
            {
                if (IsPortrait(reportedOrientation))
                {
                    return reportedOrientation;
                }

                return IsPortrait(sanitizedFallback)
                    ? sanitizedFallback
                    : ScreenOrientation.Portrait;
            }

            if (IsExplicitOrientation(reportedOrientation))
            {
                return reportedOrientation;
            }

            return sanitizedFallback;
        }
#endif

        public static SafeAreaInsets GetScreenInsets(Rect safeArea, float screenWidth, float screenHeight)
        {
            return new SafeAreaInsets(
                safeArea.xMin,
                screenWidth - safeArea.xMax,
                screenHeight - safeArea.yMax,
                safeArea.yMin);
        }

        public static bool IsLandscape(ScreenOrientation orientation)
        {
            return orientation is ScreenOrientation.LandscapeLeft or ScreenOrientation.LandscapeRight;
        }

        private static bool IsExplicitOrientation(ScreenOrientation orientation)
        {
            return orientation is
                ScreenOrientation.Portrait or
                ScreenOrientation.PortraitUpsideDown or
                ScreenOrientation.LandscapeLeft or
                ScreenOrientation.LandscapeRight;
        }

        private static bool IsPortrait(ScreenOrientation orientation)
        {
            return orientation is ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown;
        }

        private static ScreenOrientation SanitizeOrientation(ScreenOrientation orientation)
        {
            return IsExplicitOrientation(orientation) ? orientation : ScreenOrientation.Portrait;
        }
    }
}
