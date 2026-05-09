namespace NeoModLoader.AndroidCompatibilityModule;

public class SmoothLoaderHelper
{
    #if IL2CPP
    public static void add(
        Action pAction,
        string pId,
        bool pSkipFrame = false,
        float pNewWaitTimerValue = 0.001f,
        bool pToEnd = false)
    {
        SmoothLoader.add(pAction, pId, pSkipFrame, pNewWaitTimerValue, pToEnd);
    }
    #endif
    public static void add(
        MapLoaderAction pAction,
        string pId,
        bool pSkipFrame = false,
        float pNewWaitTimerValue = 0.001f,
        bool pToEnd = false)
    {
        SmoothLoader.add(pAction, pId, pSkipFrame, pNewWaitTimerValue, pToEnd);
    }
}

public static class ActionHelper
{
    public static readonly WorldAction Default = Converter.C<WorldAction>((BaseSimObject _, WorldTile _) => true);
}