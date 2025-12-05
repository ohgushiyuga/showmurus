using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Date/(Create StatusDate)")]
public class Character : ScriptableObject
{
    public string NAME;
    public int MAXHP;
    public int ATK;
    public int DEF;
    public int AGI;
}
