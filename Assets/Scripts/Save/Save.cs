using UnityEngine;

namespace Save
{
    public static class Save
    {
        public static bool CheckSavedData(string playerPrefsString)
        {
            if (PlayerPrefs.HasKey(playerPrefsString))
            {
                return true;
            }
            return false;
        }

        public static float GetSavedFloatData(string playerPrefsString)
        {
            return PlayerPrefs.GetFloat(playerPrefsString);
        }
        
        public static int GetSavedIntData(string playerPrefsString)
        {
            return PlayerPrefs.GetInt(playerPrefsString);
        }
    
        public static void SetSavedData(string playerPrefsString,float newData)
        {
            PlayerPrefs.SetFloat(playerPrefsString, newData);
        }
        
        public static void SetSavedData(string playerPrefsString,int newData)
        {
            PlayerPrefs.SetInt(playerPrefsString, newData);
        }
        
    }
}



