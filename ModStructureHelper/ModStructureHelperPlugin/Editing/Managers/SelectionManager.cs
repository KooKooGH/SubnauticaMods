using System.Collections.Generic;
using System.Linq;
using ModStructureHelperPlugin.Handle;
using ModStructureHelperPlugin.Handle.Handles;
using ModStructureHelperPlugin.Interfaces;
using ModStructureHelperPlugin.OutlineShit;
using ModStructureHelperPlugin.OutlineShit.Rendering;
using ModStructureHelperPlugin.StructureHandling;
using UnityEngine;

namespace ModStructureHelperPlugin.Editing.Managers;

public static class SelectionManager
{
    private static List<Transform> _targets = new();

    private static readonly Stack<Transform> SelectionHistory = new();

    public static bool IsSelected(Transform transform) => _targets.Contains(transform);
    
    public static void SetSelectedObject(GameObject obj)
    {
        SetSelectedObject(obj.transform);
    }
    
    public static void SetSelectedObject(Transform transform)
    {
        _targets.ForEach(old => OnTargetRemovedInternal(old));
        _targets = new List<Transform> { transform };
        OnTargetAddedInternal(transform);
        OnUpdateTargetInternal();
    }

    public static void ClearSelection()
    {
        _targets.ForEach(obj => OnTargetRemovedInternal(obj));
        _targets.Clear();
        OnUpdateTargetInternal();
    }
    
    public static void DeselectDeletedObjects()
    {
        var newTargetsList = new List<Transform>();
        _targets.ForEach(obj =>
        {
            if (obj != null)
                newTargetsList.Add(obj);
        });
        _targets = newTargetsList;
        OnUpdateTargetInternal();
    }
    
    public static void AddSelectedObject(Transform obj)
    {
        _targets.Add(obj);
        OnTargetAddedInternal(obj);
        OnUpdateTargetInternal();
    }
    
    public static void RemoveSelectedObject(Transform obj)
    {
        OnTargetRemovedInternal(obj);
        _targets.Remove(obj);
        OnUpdateTargetInternal();
    }

    public static void SelectLastSelected()
    {
        var lastSelected = GetLastSelectedGameObject();
        if (lastSelected != null)
        {
            SetSelectedObject(lastSelected);
        }
        else
        {
            ErrorMessage.AddMessage("No last selected object found");
        }
    }

    public static Transform GetLastSelectedGameObject()
    {
        while (SelectionHistory.Count > 0)
        {
            var selected = SelectionHistory.Pop();
            if (selected != null && !_targets.Contains(selected))
                return selected;
        }

        return null;
    }

    public static int NumberOfSelectedObjects => _targets.Count;
    
    public static IEnumerable<Transform> SelectedObjects => _targets;

    private static void OnTargetAddedInternal(Transform newTarget)
    {
        if (newTarget == null) return;
        var outline = AddOutline(newTarget);
        if (outline.OutlineRenderers.Count == 0)
        {
            ErrorMessage.AddMessage($"Selecting {newTarget.gameObject}, which has no active renderers!");
        }
        foreach (var selectionListener in newTarget.GetComponents<ISelectionListener>())
            selectionListener.OnObjectSelected();
        SelectionHistory.Push(newTarget);
    }
    
    private static void OnTargetRemovedInternal(Transform target)
    {
        if (target == null) return;
        Object.DestroyImmediate(target.GetComponent<OutlineBehaviour>());
        foreach (var selectionListener in target.GetComponents<ISelectionListener>())
            selectionListener.OnObjectDeselected();
    }

    private static OutlineBehaviour AddOutline(Transform obj)
    {
        var existing = obj.GetComponent<OutlineBehaviour>();
        if (existing) return existing;
        
        var outlineBehaviour = obj.gameObject.AddComponent<OutlineBehaviour>();
        outlineBehaviour.OutlineResources = Plugin.OutlineResources;
        
        outlineBehaviour.OutlineColor = Color.yellow;
        outlineBehaviour.OutlineWidth = 8;
        outlineBehaviour.OutlineRenderMode = OutlineRenderFlags.Blurred;
        return outlineBehaviour;
    }

    public static ObjectRootResult TryGetObjectRoot(GameObject obj, out GameObject root, SelectionFilterMode filter)
    {
        if (obj.GetComponentInParent<Player>() != null || obj.GetComponentInChildren<Player>() != null)
        {
            root = null;
            return ObjectRootResult.Failed;
        }
        
        if (obj.GetComponentInParent<HandleBase>() != null)
        {
            root = null;
            return ObjectRootResult.Failed;
        }

        if (filter.HasFlag(SelectionFilterMode.AllowTransformableObjects) && obj.GetComponentInParent<TransformableObject>() != null)
        {
            root = obj;
            return ObjectRootResult.Success;
        }
        
        var componentInParent = obj.GetComponentInParent<PrefabIdentifier>();
        if (componentInParent)
        {
            root = componentInParent.gameObject;
            if (!filter.HasFlag(SelectionFilterMode.NoStructureRequired) && StructureInstance.Main != null)
            {
                if (!StructureInstance.Main.IsEntityPartOfStructure(componentInParent.Id))
                {
                    ErrorMessage.AddMessage($"Cannot edit '{root.gameObject.name}'; this is not part of the currently selected structure.");
                    return ObjectRootResult.Failed;
                }
            }

            return ObjectRootResult.Success;
        }

        root = null;
        return ObjectRootResult.NoSelection;
    }

    private static void OnUpdateTargetInternal()
    {
        var runtimeTransformHandle = RuntimeTransformHandle.main;
        if (!runtimeTransformHandle)
        {
            Plugin.Logger.LogError("Cannot update selection - the runtime transform handle does not exist!");
        }

        // remove null elements
        _targets.RemoveAll(target => target == null);
        
        // update the transformation handle
        switch (_targets.Count)
        {
            case 0:
                runtimeTransformHandle.SetTarget(null);
                break;
            case 1:
                runtimeTransformHandle.SetTarget(_targets[0].transform);
                break;
            case > 1:
                runtimeTransformHandle.SetTargets(_targets);
                break;
        }
    }
    
    public enum ObjectRootResult
    {
        NoSelection,
        Success,
        Failed
    }

    [System.Flags]
    public enum SelectionFilterMode
    {
        Default,
        NoStructureRequired = 1,
        AllowTransformableObjects = 2,
    }
}