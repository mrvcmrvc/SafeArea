using System;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Mrvc.SafeArea.Editor
{
    [ScriptedImporter(3, "deviceinfo")]
    public class DeviceInfoImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            string json = System.IO.File.ReadAllText(ctx.assetPath);
            DeviceDefinition definition = JsonUtility.FromJson<DeviceDefinition>(json);

            SafeAreaDeviceInfoAsset asset = ScriptableObject.CreateInstance<SafeAreaDeviceInfoAsset>();

            Screen screen = definition.screens[0];
            SafeAreaOrientationData[] orientations = BuildOrientationData(screen);

            asset.Init(definition.friendlyName, screen.width, screen.height, orientations);

            ctx.AddObjectToAsset("main", asset);
            ctx.SetMainObject(asset);
        }

        private static SafeAreaOrientationData[] BuildOrientationData(Screen screen)
        {
            if (screen.orientations == null || screen.orientations.Length == 0)
            {
                return Array.Empty<SafeAreaOrientationData>();
            }

            SafeAreaOrientationData[] result = new SafeAreaOrientationData[screen.orientations.Length];

            for (int i = 0; i < screen.orientations.Length; i++)
            {
                Orientation orientation = screen.orientations[i];
                result[i] = new SafeAreaOrientationData(
                    (ScreenOrientation)orientation.orientation,
                    new Rect(
                        orientation.safeArea.x,
                        orientation.safeArea.y,
                        orientation.safeArea.width,
                        orientation.safeArea.height));
            }

            return result;
        }

        [Serializable]
        private struct DeviceDefinition
        {
            public string friendlyName;
            public Screen[] screens;
        }

        [Serializable]
        private struct Screen
        {
            public int width;
            public int height;
            public Orientation[] orientations;
        }

        [Serializable]
        private struct Orientation
        {
            public int orientation;
            public SafeAreaRect safeArea;
        }

        [Serializable]
        private struct SafeAreaRect
        {
            public float x;
            public float y;
            public float width;
            public float height;
        }
    }
}
