namespace HITSBlazor
{
    public static class AppEnvironment
    {
#if DEBUG || DEBUGAPI
        public static bool IsLogEnabled => true;
#else
        public static bool IsLogEnabled => false;
#endif
    }
}
