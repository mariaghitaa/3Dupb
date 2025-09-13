# 2D Path-based Animation Editor

A Unity tool for creating and playing animations along 2D trajectories.  
The editor allows users to **draw curves**, adjust control points, and animate characters (e.g., a robot) with customizable motion profiles.

---

## ✨ Features

- **Interactive Path Editing**
  - Click-to-add **draggable control points**.
  - Live `LineRenderer` preview with optional **Closed Loop**.
  - Three curve types:
    - **Polyline** – straight segments.
    - **Catmull–Rom** – smooth interpolation through all points.
    - **Cubic Bézier** – approximation with smart handles.

- **Playback & Motion**
  - `PathFollower` component drives objects along paths.
  - Time-based playback with selectable motion profiles:
    - Constant
    - Ease In/Out
    - Stop & Go
  - Supports looping and orientation modes:
    - Look Forward (tangent to path)
    - Look at Target (focuses on a given object)

- **Persistence**
  - Save/Load paths at runtime as JSON.
  - Export/Import `CurveAsset` in JSON format directly from the Inspector.
  - Schema stores curve type, closed flag, and control points.

- **Path Chaining**
  - `PathChain` asset lets you sequence multiple curves.
  - Per-segment **Reverse** option.
  - Duration distributed proportionally to segment length.

- **UI Integration**
  - Lightweight TMP-based panel with dropdowns, toggles, and sliders for curve and motion selection.

---

## 🛠️ Tech Stack

- **Engine**: Unity  
- **Language**: C#  
- **Core Components**: LineRenderer, ScriptableObject, TextMeshPro UI  
- **Data Format**: JSON  

---

## 📂 Project Structure

