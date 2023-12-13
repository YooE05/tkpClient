using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelTrapSettings", menuName ="ScriptableObjects/TrapLevelSettings")]
public class LevelTrapSettings : ScriptableObject
{
    public bool needRandomCount;

    public int trapCount;
    public int lasersCount;
    public int cannonsCount;
}
