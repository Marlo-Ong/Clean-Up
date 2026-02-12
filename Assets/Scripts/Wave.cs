using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "CleanUp/Wave")]
public class Wave : ScriptableObject
{
    public GameObject ObjectPrefab;
    public float Duration;
    public string InstructionText;
}