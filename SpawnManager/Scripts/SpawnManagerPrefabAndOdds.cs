// Defines the drop chance of an item for spawn generation.
using System;
using UnityEngine;

[Serializable]
public class SpawnManagerPrefabAndOdds
{
    public ItemDrop itemPrefab;
    [Range(0,1)] public float probability;
}
