using ModStructureHelperPlugin.UI;
using ModStructureHelperPlugin.Utility;

namespace ModStructureHelperPlugin.Editing.Tools;

public class ObjectPickerTool : ToolBase
{
    public override ToolType Type => ToolType.ObjectPicker;
    public override bool IncompatibleWithSelectTool => true;

    protected override void OnToolEnabled()
    {

    }

    protected override void OnToolDisabled()
    {

    }

    protected override string GetBindString()
    {
        var quickBindString = StructureHelperInput.QuickPickEntity.ToString();
        return $"{base.GetBindString()} (quick: {quickBindString})";
    }

    public override void UpdateTool()
    {
        if (StructureHelperInput.Interact.GetKeyDown() && !StructureHelperUI.main.IsCursorHoveringOverExternalWindows)
        {
            ObjectPickingUtils.PickObjectAtCursor();
        }   
    }

    private void Update()
    {
        if (StructureHelperInput.QuickPickEntity.GetKeyDown() && !StructureHelperUI.main.IsCursorHoveringOverExternalWindows)
        {
            ObjectPickingUtils.PickObjectAtCursor();
        }   
    }
}