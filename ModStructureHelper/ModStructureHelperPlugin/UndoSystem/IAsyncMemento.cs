using System.Collections;

namespace ModStructureHelperPlugin.UndoSystem;

public interface IAsyncMemento : IMemento
{
    public IEnumerator RestoreAsync();
}