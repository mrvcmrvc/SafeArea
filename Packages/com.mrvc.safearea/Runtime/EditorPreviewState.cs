#if UNITY_EDITOR
using UnityEngine;

namespace Mrvc.SafeArea
{
    public readonly struct EditorPreviewState
    {
        public ScreenOrientation RequestedOrientation { get; }
        public ScreenOrientation ResolvedOrientation { get; }
        public SafeAreaOrientationData SafeAreaData { get; }
        public SafeAreaLookupResult LookupResult { get; }
        public Vector2 PreviewScreenSize { get; }

        public EditorPreviewState(
            ScreenOrientation requestedOrientation,
            ScreenOrientation resolvedOrientation,
            SafeAreaOrientationData safeAreaData,
            SafeAreaLookupResult lookupResult,
            Vector2 previewScreenSize)
        {
            RequestedOrientation = requestedOrientation;
            ResolvedOrientation = resolvedOrientation;
            SafeAreaData = safeAreaData;
            LookupResult = lookupResult;
            PreviewScreenSize = previewScreenSize;
        }
    }
}
#endif
