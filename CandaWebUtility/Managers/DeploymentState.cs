public static class DeploymentState
{
    public static bool IsDebug
    {
        get
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }

    public static bool IsRelease
    {
        get
        {
            return !IsDebug;
        }
    }
}