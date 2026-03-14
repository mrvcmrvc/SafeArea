using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Mrvc.SafeArea.Editor
{
    [CustomEditor(typeof(CanvasHelper))]
    public class CanvasHelperEditor : UnityEditor.Editor
    {
        private CanvasHelper _canvasHelper;
        private SerializedProperty _safeAreaTransform;
        private SerializedProperty _appliedEdges;
        private SerializedProperty _editorDeviceOverride;

        private void OnEnable()
        {
            _canvasHelper = (CanvasHelper)target;
            _safeAreaTransform = serializedObject.FindProperty("safeAreaTransform");
            _appliedEdges = serializedObject.FindProperty("appliedEdges");
            _editorDeviceOverride = serializedObject.FindProperty("editorDeviceOverride");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_safeAreaTransform);
            EditorGUILayout.PropertyField(_appliedEdges);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Preview", EditorStyles.boldLabel);

            DrawDeviceDropdown();

            SafeAreaDeviceInfoAsset current = _editorDeviceOverride.objectReferenceValue as SafeAreaDeviceInfoAsset;
            if (current != null)
            {
                DrawDeviceInfo(current);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDeviceDropdown()
        {
            SafeAreaDeviceInfoAsset current = _editorDeviceOverride.objectReferenceValue as SafeAreaDeviceInfoAsset;

            string buttonLabel = current != null
                ? $"{current.DeviceName}  ({current.ScreenWidth}\u00d7{current.ScreenHeight})"
                : "None";

            Rect rect = EditorGUILayout.GetControlRect();
            Rect labelRect = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
            Rect buttonRect = new Rect(
                labelRect.xMax + 2f,
                rect.y,
                rect.width - EditorGUIUtility.labelWidth - 2f,
                rect.height);

            EditorGUI.LabelField(labelRect, new GUIContent("Device", "Select a device to preview safe area insets in edit mode."));

            if (EditorGUI.DropdownButton(buttonRect, new GUIContent(buttonLabel), FocusType.Keyboard))
            {
                DeviceDropdown dropdown = new DeviceDropdown(new AdvancedDropdownState(), _editorDeviceOverride);
                dropdown.Show(buttonRect);
            }
        }

        private void DrawDeviceInfo(SafeAreaDeviceInfoAsset safeAreaDevice)
        {
            if (!_canvasHelper.TryGetEditorPreviewState(out EditorPreviewState previewState))
            {
                return;
            }

            Rect safeArea = previewState.SafeAreaData.SafeArea;
            Vector2 screenSize = previewState.PreviewScreenSize;
            SafeAreaInsets insets = SafeAreaOrientationUtility.GetScreenInsets(safeArea, screenSize.x, screenSize.y);

            EditorGUILayout.HelpBox(
                $"Screen: {Mathf.RoundToInt(screenSize.x)} \u00d7 {Mathf.RoundToInt(screenSize.y)} px\n" +
                $"Requested orientation: {previewState.RequestedOrientation}\n" +
                $"Resolved orientation: {previewState.ResolvedOrientation}\n" +
                $"Safe area orientation: {previewState.SafeAreaData.Orientation}\n" +
                $"Lookup: {FormatLookup(previewState.LookupResult)}\n" +
                $"Applied edges: {FormatEdges((SafeAreaEdges)_appliedEdges.intValue)}\n" +
                $"Insets: Left {Mathf.RoundToInt(insets.Left)} px   Right {Mathf.RoundToInt(insets.Right)} px   Top {Mathf.RoundToInt(insets.Top)} px   Bottom {Mathf.RoundToInt(insets.Bottom)} px",
                MessageType.Info);
        }

        private static string FormatEdges(SafeAreaEdges edges)
        {
            return edges == 0 ? "None" : edges.ToString();
        }

        private static string FormatLookup(SafeAreaLookupResult lookupResult)
        {
            return lookupResult switch
            {
                SafeAreaLookupResult.ExactMatch => "Exact match",
                SafeAreaLookupResult.PortraitFallback => "Portrait fallback",
                SafeAreaLookupResult.FirstAvailableFallback => "First available fallback",
                _ => "Full screen fallback",
            };
        }
    }

    internal class DeviceDropdown : AdvancedDropdown
    {
        private readonly SerializedProperty _property;
        private readonly List<SafeAreaDeviceInfoAsset> _devices = new List<SafeAreaDeviceInfoAsset>();

        public DeviceDropdown(AdvancedDropdownState state, SerializedProperty property) : base(state)
        {
            _property = property;
            minimumSize = new Vector2(340, 360);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Device");

            root.AddChild(new DeviceItem("None", null));

            string[] guids = AssetDatabase.FindAssets($"t:{nameof(SafeAreaDeviceInfoAsset)}");
            SortedDictionary<string, List<SafeAreaDeviceInfoAsset>> groups = new SortedDictionary<string, List<SafeAreaDeviceInfoAsset>>();

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SafeAreaDeviceInfoAsset asset = AssetDatabase.LoadAssetAtPath<SafeAreaDeviceInfoAsset>(path);

                if (asset == null)
                {
                    continue;
                }

                _devices.Add(asset);

                string group = ExtractGroup(path);
                if (!groups.ContainsKey(group))
                {
                    groups[group] = new List<SafeAreaDeviceInfoAsset>();
                }
                groups[group].Add(asset);
            }

            string[] preferredOrder = { "Apple", "Android" };
            IEnumerable<string> orderedKeys = preferredOrder
                .Where(groups.ContainsKey)
                .Concat(groups.Keys.Where(k => !preferredOrder.Contains(k)));

            foreach (string groupName in orderedKeys)
            {
                AdvancedDropdownItem groupItem = new AdvancedDropdownItem(groupName);
                foreach (SafeAreaDeviceInfoAsset asset in groups[groupName].OrderBy(a => a.DeviceName))
                {
                    string displayName = string.IsNullOrEmpty(asset.DeviceName) ? asset.name : asset.DeviceName;
                    groupItem.AddChild(new DeviceItem(
                        $"{displayName}  ({asset.ScreenWidth}\u00d7{asset.ScreenHeight})",
                        asset));
                }
                root.AddChild(groupItem);
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            DeviceItem deviceItem = (DeviceItem)item;
            _property.objectReferenceValue = deviceItem.SafeAreaDevice;
            _property.serializedObject.ApplyModifiedProperties();
        }

        private static string ExtractGroup(string assetPath)
        {
            string[] parts = assetPath.Split('/');
            for (int i = parts.Length - 2; i >= 0; i--)
            {
                if (parts[i] is "Apple" or "Android")
                {
                    return parts[i];
                }
            }
            return "Other";
        }

        private class DeviceItem : AdvancedDropdownItem
        {
            public SafeAreaDeviceInfoAsset SafeAreaDevice { get; }

            public DeviceItem(string name, SafeAreaDeviceInfoAsset safeAreaDevice) : base(name)
            {
                SafeAreaDevice = safeAreaDevice;
            }
        }
    }
}
