using Story;

namespace IceDragon;

public static class ModStory
{
    public static StoryGoal FrozenIceDragonRoar { get; private set; }

    public static void Register()
    {
        FrozenIceDragonRoar = new StoryGoal("FrozenIceDragonRoarHallucination", Story.GoalType.Story, 0);
    }
}