using NaughtyAttributes;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(menuName = "Game Config")]
    public class GameConfigs : ScriptableObject
    {
        [BoxGroup("Initial Scene Configs")] public string gameName;
        [BoxGroup("Initial Scene Configs")] public Color gameNameColor;
        [BoxGroup("Initial Scene Configs")] public Sprite gameIcon;

        [BoxGroup("Application Settings")] public int targetFrameRate = 240;
        [BoxGroup("Application Settings")] public int vSyncCount = 0;
    }
}
