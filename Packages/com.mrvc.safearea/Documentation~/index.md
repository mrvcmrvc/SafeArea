# Safe Area

`Safe Area` helps Unity UI canvases respect the visible area of devices with cutouts, rounded corners, or system gesture insets.

## How to install

<summary>Add from OpenUPM by scoped registry</summary>

- open `Edit/Project Settings/Package Manager`
- add a new Scoped Registry:
  ```
  Name: OpenUPM
  URL:  https://package.openupm.com/
  Scope(s): com.mrvc
  ```
- click <kbd>Save</kbd>
- open Package Manager
- click <kbd>+</kbd>
- select <kbd>Add from Git URL</kbd>
- paste `com.mrvc.safearea`
- click <kbd>Add</kbd>

## Runtime Setup

1. Add `CanvasHelper` to a GameObject with a `Canvas` component.
2. Assign `Safe Area Transform` to the `RectTransform` you want clamped to the device safe area.
3. Select the edges that should respect the safe area.

At runtime, `CanvasHelper` reads `Screen.safeArea`, converts it into normalized anchors, and applies those anchors to the configured transform.

## Editor Preview

The custom inspector includes an `Editor Preview` section with bundled device profiles for common Apple and Android devices.

Use it to:

- preview portrait and landscape safe area insets in edit mode
- verify which orientation profile was selected
- inspect the resulting inset values before entering play mode

The preview assets are packaged under `Editor/Devices`, so they stay editor-only and do not become part of player builds.

## Package Layout

- `Runtime/`: runtime components, data structures, and orientation helpers
- `Editor/`: custom inspector, importer, and bundled editor preview device profiles
- `Documentation~/`: package documentation shown by Unity Package Manager

## Repository Notes

This repository also contains a small Unity project used to validate the package locally. The demo scene remains in `Assets/Scenes/DemoScene.unity`, while the distributable package lives under `Packages/com.mrvc.safearea`.
