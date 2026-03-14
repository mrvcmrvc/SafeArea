using System;
using UnityEngine;

namespace Mrvc.SafeArea
{
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas))]
    public class CanvasHelper : MonoBehaviour
    {
        [SerializeField]
        private RectTransform safeAreaTransform;

        [SerializeField]
        private SafeAreaEdges appliedEdges = SafeAreaEdges.Left | SafeAreaEdges.Right | SafeAreaEdges.Top | SafeAreaEdges.Bottom;

#if UNITY_EDITOR
        [Header("Editor Preview")]
        [Tooltip("Assign a DeviceInfoAsset to preview safe area insets in edit mode.")]
        [SerializeField]
        private SafeAreaDeviceInfoAsset editorDeviceOverride;

        private ScreenOrientation _lastPreviewOrientation = ScreenOrientation.Portrait;
#endif

        private Rect _lastSafeArea;
        private SafeAreaEdges _lastAppliedEdges;
        private ScreenOrientation _lastOrientation = ScreenOrientation.Portrait;
        private Canvas _canvas;

        public event Action OnSafeAreaUpdatedEvent;

        private void Awake()
        {
            EnsureCanvas();
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (editorDeviceOverride != null)
                {
                    ApplyDeviceOverride();
                }
                return;
            }
#endif
            ApplySafeArea();
        }

        private void Update()
        {
            EnsureCanvas();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (editorDeviceOverride != null)
                {
                    ApplyDeviceOverride();
                }
                return;
            }
#endif

            Rect pixelRect = _canvas.pixelRect;
            ScreenOrientation orientation = ResolveCurrentOrientation(pixelRect.width, pixelRect.height);

            if (Screen.safeArea == _lastSafeArea &&
                appliedEdges == _lastAppliedEdges &&
                orientation == _lastOrientation)
            {
                return;
            }

            ApplySafeArea();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null)
                {
                    return;
                }

                if (editorDeviceOverride != null)
                {
                    ApplyDeviceOverride();
                }
                else
                {
                    ResetAnchors();
                }
            };
        }

        private void ApplyDeviceOverride()
        {
            if (!TryGetEditorPreviewState(out EditorPreviewState previewState))
            {
                return;
            }

            ApplyAnchors(
                previewState.SafeAreaData.SafeArea,
                previewState.PreviewScreenSize.x,
                previewState.PreviewScreenSize.y);
        }

        private void ResetAnchors()
        {
            safeAreaTransform.anchorMin = Vector2.zero;
            safeAreaTransform.anchorMax = Vector2.one;
        }

        public bool TryGetEditorPreviewState(out EditorPreviewState previewState)
        {
            previewState = default;

            if (editorDeviceOverride == null)
            {
                return false;
            }

            EnsureCanvas();

            Rect pixelRect = _canvas.pixelRect;
            ScreenOrientation requestedOrientation = SafeAreaOrientationUtility.ResolveOrientation(
                Screen.orientation,
                pixelRect.width,
                pixelRect.height,
                _lastPreviewOrientation);
            ScreenOrientation resolvedOrientation = SafeAreaOrientationUtility.ResolvePreviewOrientation(
                Screen.orientation,
                pixelRect.width,
                pixelRect.height,
                _lastPreviewOrientation);

            SafeAreaOrientationData safeAreaData = editorDeviceOverride.GetSafeAreaDataOrFallback(
                resolvedOrientation,
                out SafeAreaLookupResult lookupResult);
            Vector2 previewScreenSize = editorDeviceOverride.GetScreenSize(safeAreaData.Orientation);

            _lastPreviewOrientation = resolvedOrientation;
            previewState = new EditorPreviewState(
                requestedOrientation,
                resolvedOrientation,
                safeAreaData,
                lookupResult,
                previewScreenSize);
            return true;
        }
#endif

        private void ApplySafeArea()
        {
            EnsureCanvas();

            _lastSafeArea = Screen.safeArea;
            _lastAppliedEdges = appliedEdges;

            Rect pixelRect = _canvas.pixelRect;
            _lastOrientation = ResolveCurrentOrientation(pixelRect.width, pixelRect.height);

            ApplyAnchors(
                _lastSafeArea,
                pixelRect.width,
                pixelRect.height);
        }

        private void ApplyAnchors(Rect safeArea, float screenWidth, float screenHeight)
        {
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= screenWidth;
            anchorMin.y /= screenHeight;
            anchorMax.x /= screenWidth;
            anchorMax.y /= screenHeight;

            if ((appliedEdges & SafeAreaEdges.Left) == 0)
            {
                anchorMin.x = 0f;
            }

            if ((appliedEdges & SafeAreaEdges.Bottom) == 0)
            {
                anchorMin.y = 0f;
            }

            if ((appliedEdges & SafeAreaEdges.Right) == 0)
            {
                anchorMax.x = 1f;
            }

            if ((appliedEdges & SafeAreaEdges.Top) == 0)
            {
                anchorMax.y = 1f;
            }

            safeAreaTransform.anchorMin = anchorMin;
            safeAreaTransform.anchorMax = anchorMax;

            OnSafeAreaUpdatedEvent?.Invoke();
        }

        private void EnsureCanvas()
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }
        }

        private ScreenOrientation ResolveCurrentOrientation(float width, float height)
        {
            return SafeAreaOrientationUtility.ResolveOrientation(Screen.orientation, width, height, _lastOrientation);
        }
    }
}
