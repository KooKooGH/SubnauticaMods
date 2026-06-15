namespace ModStructureFormat;

[Serializable]
[Obsolete("Please use the ModStructureFormatV2")]
public class SavedVariable
{
    public string name;
    public string type;
    public string value;
}