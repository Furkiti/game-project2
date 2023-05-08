namespace Managers
{
    public static class LevelManager
    {
        private static string levelStr = "level";
        private static int initialLevel = 1;
    
        public static int CurrentLevel
        {
            get
            {
                if (Save.Save.CheckSavedData(levelStr))
                {
                    return Save.Save.GetSavedIntData(levelStr);
                }

                Save.Save.SetSavedData(levelStr,initialLevel);
                return initialLevel;
            }

            set => Save.Save.SetSavedData(levelStr, value);
        }
    }
}
