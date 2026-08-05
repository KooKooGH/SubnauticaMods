using System.Collections;
using UnityEngine;

namespace KallieʼsPropPack.Utility;

public static class ChildPrefabUtils
{
    /// <summary>
    /// Instantiates the prefab with the given Class ID as a child of <paramref name="parentObject"/>, removing the specified components.
    /// </summary>
    /// <param name="parentObject">The parent GameObject to instantiate the child under.</param>
    /// <param name="classId">The Class ID of the entity to instantiate as the child object.</param>
    /// <param name="result">If provided, receives a reference to the spawned child object. You may want to use an instance of the <see cref="TaskResult{T}"/> class.</param>
    /// <param name="stripComponents">If this parameter is true, this utility destroys any <see cref="LargeWorldEntity"/>, <see cref="PrefabIdentifier"/>, <see cref="Rigidbody"/>, <see cref="WorldForces"/>, and <see cref="TechTag"/> components on the root.</param>
    /// <param name="stripSkyAppliers">If this parameter is true, all <see cref="SkyApplier"/> components on the object and its children will be destroyed.</param>
    /// <remarks>
    /// <para>The instantiated child object will be given default values (0, 0, 0) for its local position and rotation.</para>
    /// <para>This method must be called asynchronously (using coroutines).</para>
    /// </remarks>
    public static IEnumerator AddChildPrefab(GameObject parentObject, string classId, IOut<GameObject> result = null, bool stripComponents = true, bool stripSkyAppliers = true)
    {
        var task = UWE.PrefabDatabase.GetPrefabAsync(classId);
        yield return task;

        if (!task.TryGetPrefab(out var prefab) || prefab == null)
        {
            Plugin.Logger.LogError(
                $"Could not load prefab by ClassId '{classId}' to add to parent '{parentObject}'");
            result?.Set(null);
            yield break;
        }

        AddPrefabAsChildInternal(parentObject, prefab, result, stripComponents, stripSkyAppliers);
    }
    
    /// <summary>
    /// Instantiates the prefab with the given Class ID as a child of <paramref name="parentObject"/>, removing the specified components.
    /// </summary>
    /// <param name="parentObject">The parent GameObject to instantiate the child under.</param>
    /// <param name="fileName">The full path to the child, e.g. "WorldEntities/Doodads/Debris/Wrecks/Decoration/starship_souvenir.prefab".</param>
    /// <param name="result">If provided, receives a reference to the spawned child object. You may want to use an instance of the <see cref="TaskResult{T}"/> class.</param>
    /// <param name="stripComponents">If this parameter is true, this utility destroys any <see cref="LargeWorldEntity"/>, <see cref="PrefabIdentifier"/>, <see cref="Rigidbody"/>, <see cref="WorldForces"/>, and <see cref="TechTag"/> components on the root.</param>
    /// <param name="stripSkyAppliers">If this parameter is true, all <see cref="SkyApplier"/> components on the object and its children will be destroyed.</param>
    /// <remarks>
    /// <para>The instantiated child object will be given default values (0, 0, 0) for its local position and rotation.</para>
    /// <para>This method must be called asynchronously (using coroutines).</para>
    /// </remarks>
    public static IEnumerator AddChildPrefabByFileName(GameObject parentObject, string fileName, IOut<GameObject> result = null, bool stripComponents = true, bool stripSkyAppliers = true)
    {
        var task = UWE.PrefabDatabase.GetPrefabForFilenameAsync(fileName);
        yield return task;

        if (!task.TryGetPrefab(out var prefab) || prefab == null)
        {
            Plugin.Logger.LogError(
                $"Could not load prefab by file name '{fileName}' to add to parent '{parentObject}'");
            result?.Set(null);
            yield break;
        }

        AddPrefabAsChildInternal(parentObject, prefab, result, stripComponents, stripSkyAppliers);
    }
    
    /// <summary>
    /// Instantiates the prefab with the given <see cref="TechType"/> as a child of <paramref name="parentObject"/>, removing the specified components.
    /// </summary>
    /// <param name="parentObject">The parent GameObject to instantiate the child under.</param>
    /// <param name="techType">The <see cref="TechType"/> of the entity to instantiate as the child object.</param>
    /// <param name="result">If provided, receives a reference to the spawned child object. You may want to use an instance of the <see cref="TaskResult{T}"/> class.</param>
    /// <param name="stripComponents">If this parameter is true, this utility destroys any <see cref="LargeWorldEntity"/>, <see cref="PrefabIdentifier"/>, <see cref="Rigidbody"/>, <see cref="WorldForces"/>, and <see cref="TechTag"/> components on the root.</param>
    /// <param name="stripSkyAppliers">If this parameter is true, all <see cref="SkyApplier"/> components on the object and its children will be destroyed.</param>
    /// <remarks>
    /// <para>The instantiated child object will be given default values (0, 0, 0) for its local position and rotation.</para>
    /// <para>This method must be called asynchronously (using coroutines).</para>
    /// </remarks>
    public static IEnumerator AddChildPrefab(GameObject parentObject, TechType techType, IOut<GameObject> result = null, bool stripComponents = true, bool stripSkyAppliers = true)
    {
        var task = CraftData.GetPrefabForTechTypeAsync(techType);
        yield return task;
        var prefab = task.GetResult();
        
        if (prefab == null)
        {
            Plugin.Logger.LogError(
                $"Could not load prefab by TechType '{techType}' to add to parent '{parentObject}'");
            result?.Set(null);
            yield break;
        }

        AddPrefabAsChildInternal(parentObject, prefab, result, stripComponents, stripSkyAppliers);
    }

    private static void AddPrefabAsChildInternal(GameObject parentObject, GameObject prefab, IOut<GameObject> result, bool stripComponents, bool stripSkyAppliers)
    {
        if (parentObject == null)
        {
            Plugin.Logger.LogError($"Failed to instantiate '{prefab}'; parent is null");
            return;
        }

        var child = UWE.Utils.InstantiateDeactivated(prefab);
        var childTransform = child.transform;

        childTransform.SetParent(parentObject.transform);
        childTransform.localPosition = Vector3.zero;
        childTransform.localRotation = Quaternion.identity;

        if (stripComponents)
        {
            Object.DestroyImmediate(child.GetComponent<LargeWorldEntity>());
            Object.DestroyImmediate(child.GetComponent<TechTag>());
            Object.DestroyImmediate(child.GetComponent<PrefabIdentifier>());
            Object.DestroyImmediate(child.GetComponent<WorldForces>());
            Object.DestroyImmediate(child.GetComponent<Rigidbody>());
            Object.DestroyImmediate(child.GetComponent<Pickupable>());
        }

        if (stripSkyAppliers)
        {
            foreach (var skyApplier in child.GetComponentsInChildren<SkyApplier>(true))
            {
                Object.DestroyImmediate(skyApplier);
            }
        }

        child.SetActive(true);
        
        result?.Set(child);
    }
}