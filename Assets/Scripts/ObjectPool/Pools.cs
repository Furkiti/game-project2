using System;
using UnityEngine;

namespace ObjectPool
{
    public class Pools : MonoBehaviour
    {
        public enum Types
        {
            ProductionMenuItem,
            Tile,
            Soldier,
            Barrack,
            PowerPlant,
           
        }
        
        public static string GetTypeStr(Types poolType)
        {
            return Enum.GetName(typeof(Types), poolType);
        }
    }
}

