using ModStructureHelperPlugin.StructureHandling;
using ModStructureHelperPlugin.UI;
using UnityEngine;

namespace ModStructureHelperPlugin.Mono;

public class InputHandler : MonoBehaviour
{
    private void Update()
    {
        if (StructureHelperInput.ToggleStructureHelperKeyBind.GetKeyDown())
        {
            StructureHelperUI.SetUIEnabled(!StructureHelperUI.IsActive);
        }

        if (!StructureHelperUI.main || !StructureHelperUI.main.isActiveAndEnabled) return;
        
        if (StructureHelperInput.SaveHotkeyModifier.GetKey() && StructureHelperInput.SaveKeyBind.GetKeyDown())
        {
            StructureInstance.TrySave();
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            StructureHelperUI.main.SetInputGroupOverride(false);
        }
        if (Input.GetMouseButtonUp(1))
        {
            StructureHelperUI.main.SetInputGroupOverride(true);
        }
    }
}