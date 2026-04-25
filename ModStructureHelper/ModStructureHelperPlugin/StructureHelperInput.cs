using UnityEngine;

namespace ModStructureHelperPlugin;

public readonly struct InputBinding
{
    public readonly KeyCode? Key;
    public readonly int? MouseButton;
    public readonly string Name;

    public InputBinding(KeyCode key, string name)
    {
        Key = key;
        MouseButton = null;
        Name = name;
    }

    public InputBinding(int mouseButton, string name)
    {
        Key = null;
        MouseButton = mouseButton;
        Name = name;
    }

    public bool GetKeyDown() => Key.HasValue ? Input.GetKeyDown(Key.Value) : Input.GetMouseButtonDown(MouseButton.Value);
    public bool GetKey() => Key.HasValue ? Input.GetKey(Key.Value) : Input.GetMouseButton(MouseButton.Value);
    public bool GetKeyUp() => Key.HasValue ? Input.GetKeyUp(Key.Value) : Input.GetMouseButtonUp(MouseButton.Value);

    public override string ToString() => Name;
}

public static class StructureHelperInput
{
    internal static void RegisterLocalization() { }

    // GENERAL
    public static readonly InputBinding ToggleStructureHelperKeyBind = new(KeyCode.F4, "F4");
    public static readonly InputBinding SaveKeyBind = new(KeyCode.S, "S");
    public static readonly InputBinding Interact = new(0, "Left Click");

    // TOOLS
    public static readonly InputBinding SelectBind = new(KeyCode.Q, "Q");
    public static readonly InputBinding TranslateBind = new(KeyCode.E, "E");
    public static readonly InputBinding RotateBind = new(KeyCode.R, "R");
    public static readonly InputBinding ScaleBind = new(KeyCode.T, "T");
    public static readonly InputBinding DragBind = new(KeyCode.F, "F");
    public static readonly InputBinding EntityEditorBind = new(KeyCode.Tab, "Tab");
    public static readonly InputBinding PaintBrushBind = new(KeyCode.B, "B");
    public static readonly InputBinding ToggleGlobalSpaceBind = new(KeyCode.G, "G");
    public static readonly InputBinding ToggleSnappingBind = new(KeyCode.P, "P");
    public static readonly InputBinding HoldToSnap = new(KeyCode.LeftControl, "Left Ctrl");
    public static readonly InputBinding PickObjectBind = new(KeyCode.K, "K");
    public static readonly InputBinding QuickPickEntity = new(2, "Middle Click");
    public static readonly InputBinding CableEditorBind = new(KeyCode.M, "M");
    public static readonly InputBinding DuplicateBind = new(KeyCode.D, "D");
    public static readonly InputBinding SelectAllBind = new(KeyCode.H, "H");
    public static readonly InputBinding UndoBind = new(KeyCode.Z, "Z");
    public static readonly InputBinding SelectLastSelectedBind = new(KeyCode.Z, "Z (Alt)");
    public static readonly InputBinding DeleteBind = new(KeyCode.Delete, "Delete");

    // MODIFIERS
    public static readonly InputBinding PrioritizeTriggers = new(KeyCode.LeftAlt, "Alt");
    public static readonly InputBinding ScaleUniform = new(KeyCode.LeftAlt, "Alt");
    public static readonly InputBinding SaveHotkeyModifier = new(KeyCode.LeftControl, "Left Ctrl");
    public static readonly InputBinding ToolHotkeyModifier = new(KeyCode.LeftControl, "Left Ctrl");
    public static readonly InputBinding AltToolHotkeyModifier = new(KeyCode.LeftAlt, "Left Alt");
    public static readonly InputBinding SelectMultipleModifier = new(KeyCode.LeftControl, "Left Ctrl");

    // OTHER
    public static readonly InputBinding BrushRotateLeft = new(KeyCode.LeftBracket, "[");
    public static readonly InputBinding BrushRotateRight = new(KeyCode.RightBracket, "]");
    public static readonly InputBinding BrushDecreaseScale = new(KeyCode.Minus, "-");
    public static readonly InputBinding BrushIncreaseScale = new(KeyCode.Equals, "=");
    public static readonly InputBinding UseGlobalUpNormal = new(KeyCode.Semicolon, ";");
    public static readonly InputBinding GoBack = new(3, "Mouse Back");
    public static readonly InputBinding GoForward = new(4, "Mouse Forward");
}
