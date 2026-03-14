# MRVC Safe Area

`MRVC Safe Area` is a Unity UGUI package that fits a target `RectTransform` to the current device safe area. It is useful for keeping UI away from notches, rounded corners, and system gesture areas.

## Install

Install it through OpenUPM:

```bash
openupm add com.mrvc.safearea
```

If you prefer to edit the manifest yourself, add the OpenUPM registry and then install `com.mrvc.safearea`:

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.mrvc"
      ]
    }
  ],
  "dependencies": {
    "com.mrvc.safearea": "0.1.0"
  }
}
```

## Use

1. Add `CanvasHelper` to the same GameObject as your `Canvas`.
2. Assign the `RectTransform` that should stay inside the safe area.
3. Choose which edges should respect the safe area.
4. Optionally select a preview device in the custom inspector while working in the editor.
