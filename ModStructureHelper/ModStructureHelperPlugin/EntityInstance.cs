﻿using ModStructureFormat;
using UnityEngine;

namespace ModStructureHelperPlugin;

public class EntityInstance : MonoBehaviour
{
    public ManagedEntity ManagedEntity;
    
    // Non-changeable properties
    public Entity.CellLevel CellLevel { get; private set; }
    public string ClassId { get; private set; }
    public string Id { get; private set; }
    
    // Changeable properties
    public Entity.Priority Priority { get; set; }

    private Rigidbody rigidbody;
    
    private void Awake()
    {
        var lwe = gameObject.GetComponent<LargeWorldEntity>();
        if (lwe)
            CellLevel = (Entity.CellLevel) (int) lwe.cellLevel;
        else
            CellLevel = Entity.CellLevel.Unknown;
        var prefabIdentifier = gameObject.GetComponent<PrefabIdentifier>();
        if (prefabIdentifier)
        {
            ClassId = prefabIdentifier.ClassId;
            Id = prefabIdentifier.Id;
        }

        StructureInstance.OnStructureInstanceChanged += OnStructureInstanceChanged;

        AccountForLackOfCollisions();

        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody) rigidbody.isKinematic = true;

        // DestroyImmediate(lwe);
    }

    private void AccountForLackOfCollisions()
    {
        if (!HasSolidCollisions())
        {
            gameObject.AddComponent<SphereCollider>().radius = 2;
        }
    }

    private bool HasSolidCollisions()
    {
        if (GetComponentInChildren<Collider>() == null)
        {
            return false;
        }

        foreach (var collision in GetComponentsInChildren<Collider>())
        {
            if (!collision.isTrigger) return true;
        }

        return false;
    }
    
    public Entity GetEntityDataStruct()
    {
        return new Entity(ClassId, Id, transform.position, transform.rotation, transform.localScale, CellLevel, Priority);
    }

    private void Update()
    {
        if (rigidbody) rigidbody.isKinematic = true;
    }

    private void OnDestroy()
    {
        ManagedEntity.RemoveCurrentEntityInstance();
        StructureInstance.OnStructureInstanceChanged -= OnStructureInstanceChanged;
    }

    private void OnStructureInstanceChanged(StructureInstance instance)
    {
        if (instance == null)
            Destroy(this);
    }
}