namespace PodshellLeviathan.Mono.BasePieces;

public class ApplyPodshellFragment : PlayerTool
{
    public override string animToolName { get; } = TechType.ScrapMetal.AsString(true);

    public override bool OnRightHandDown()
    {
        return false;
    }
}