# Safe Area

`Safe Area` is a Unity UGUI package that fits a target `RectTransform` to the current device safe area. It is useful for keeping UI away from notches, rounded corners, and system gesture areas.

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

## Use

1. Add `CanvasHelper` to the same GameObject as your `Canvas`.
2. Assign the `RectTransform` that should stay inside the safe area.
3. Choose which edges should respect the safe area.
4. Optionally select a preview device in the custom inspector while working in the editor.