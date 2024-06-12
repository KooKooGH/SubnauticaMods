﻿using System;
using RuntimeHandle;
using UnityEngine;

namespace ModStructureHelperPlugin.Tools;

public class ToolManager : MonoBehaviour
{
    public ToolBase[] tools;

    public RuntimeTransformHandle handle;

    /*
    private ToolType[] _toolTypes;

    private void Awake()
    {
        _toolTypes = (ToolType[]) Enum.GetValues(typeof(ToolType));
    }
    */

    private void OnValidate()
    {
        tools = GetComponentsInChildren<ToolBase>();
    }

    private void Update()
    {
        foreach (var tool in tools)
        {
            if (Input.GetKeyDown(GetKeyBindForTool(tool.Type)))
            {
                tool.OnToolButtonPressed();
            }
            if (tool.ToolEnabled) tool.UpdateTool();
        }
    }

    public KeyCode GetKeyBindForTool(ToolType tool)
    {
        switch (tool)
        {
            case ToolType.Select:
                return Plugin.ModConfig.SelectBind;
            case ToolType.Translate:
                return Plugin.ModConfig.TranslateBind;
            case ToolType.Rotate:
                return Plugin.ModConfig.RotateBind;
            case ToolType.Scale:
                return Plugin.ModConfig.ScaleBind;
            case ToolType.BrowseEntities:
                return Plugin.ModConfig.EntityEditorBind;
            case ToolType.Paint:
                return Plugin.ModConfig.PaintBrushBind;
            case ToolType.GlobalSpace:
                return Plugin.ModConfig.ToggleGlobalSpaceBind;
            case ToolType.Snapping:
                return Plugin.ModConfig.ToggleSnappingBind;
            case ToolType.Delete:
                return Plugin.ModConfig.DeleteBind;
            default:
                Plugin.Logger.LogWarning($"No keybind implemented for tool '{tool}'!");
                return KeyCode.None;
        }
    }
}