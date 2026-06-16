using ModStructureHelperPlugin.StructureHandling;

namespace ModStructureHelperPlugin.UndoSystem;

public readonly struct AddEntityMemento : IMemento
{
    private string Id { get; }

    public int SaveFrame { get; }
    public bool Invalid => false;
    
    public void RestoreSync()
    {
        foreach (var entity in StructureInstance.Main.GetAllManagedEntities())
        {
            if (entity.Id == Id)
            {
                StructureInstance.Main.DeleteEntity(entity.EntityInstance.ManagedEntity, false);
                return;
            }
        }
    }

    public AddEntityMemento(string id, int saveFrame)
    {
        Id = id;
        SaveFrame = saveFrame;
    }
}