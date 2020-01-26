using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SaveObject
{
    [System.Serializable] public class DictionaryOfStringAndInt : SerializableDictionary<string, int> { }

    [System.Serializable] public class DoubleStringAndIntDict : SerializableDictionary<string, DictionaryOfStringAndInt> { }
    public List<string> savAchivementsDone;
    public DoubleStringAndIntDict savMaxEnemyWeaponKills;
    public DictionaryOfStringAndInt savMaxEnemykills;
    public DictionaryOfStringAndInt savMaxWeaponKills;
    public DictionaryOfStringAndInt savMaxWeaponScores;

    public DoubleStringAndIntDict savLifetimeEnemyKills;
    public DictionaryOfStringAndInt savLifetimeWeaponKills;
    public DictionaryOfStringAndInt savLifetimeWeaponScores;

    public DictionaryOfStringAndInt savMaxWeaponSwitches;
    public DictionaryOfStringAndInt savLifetimeWeaponSwitches;

    public DictionaryOfStringAndInt savMaxWeaponUsedAtLevel;

    public DictionaryOfStringAndInt savMaxPowerups;

    public int savmaxScore;
    public int savmaxKills;
    public int savmaxSwitches;
}
