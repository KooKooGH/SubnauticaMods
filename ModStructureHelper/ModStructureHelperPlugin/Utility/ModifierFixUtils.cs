using UnityEngine;

namespace ModStructureHelperPlugin.Utility;

public static class ModifierFixUtils
{
    /// <summary>
    /// Checks if a modifier key is held. For Alt, checks both LeftAlt and RightAlt.
    /// </summary>
    public static bool GetModifierHeld(InputBinding binding)
    {
        if (!binding.GetKey()) return false;
        
        // Special handling for Alt key - check both left and right
        if (binding.Key == KeyCode.LeftAlt)
        {
            return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }
        
        return true;
    }
}
