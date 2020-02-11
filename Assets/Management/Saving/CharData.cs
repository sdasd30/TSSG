using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class StringDictionary : SerializableDictionary<string, string> { }

[Serializable]
public class CharData {

	public string regID = "Not Assigned";
	public string prefabPath;
	public string name;
	public Vector3 pos;

	[Serializable] public class DictionaryOfStringAndInt : SerializableDictionary<string, int> {}
	public DictionaryOfStringAndInt PersistentInt;

	[Serializable] public class DictionaryOfStringAndFloat : SerializableDictionary<string, float> {}
    public DictionaryOfStringAndFloat PersistentFloats;

	[Serializable] public class DictionaryOfStringAndString : SerializableDictionary<string, string> {}
    public DictionaryOfStringAndString PersistentStrings;

	[Serializable] public class DictionaryOfStringAndBool : SerializableDictionary<string, bool> {}
    public DictionaryOfStringAndBool PersistentBools;

	public Direction targetDir;
	public string targetID;

    public void SetInt(string key, int value) { PersistentInt[key] = value; }
    public int GetInt(string key, int defaultValue = 0)
    {
        if (PersistentInt.ContainsKey(key))
            return PersistentInt[key];
        else
            return defaultValue;
    }

    public void SetFloat(string key, float value) { PersistentFloats[key] = value; }
    public float GetFloat(string key, float defaultValue = 0f)
    {
        if (PersistentFloats.ContainsKey(key))
            return PersistentFloats[key];
        else
            return defaultValue;
    }

    public void SetString(string key, string value) { PersistentStrings [key] = value; }
    public string GetString(string key, string defaultValue = "")
    {
        if (PersistentStrings.ContainsKey(key))
            return PersistentStrings[key];
        else
            return defaultValue;
    }

    public void SetBool(string key, bool value) { PersistentBools[key] = value; }
    public bool GetBool(string key, bool defaultValue = false)
    {
        if (PersistentBools.ContainsKey(key))
            return PersistentBools[key];
        else
            return defaultValue;
    }
    public void ClearString(string key)
    {
        if (PersistentStrings.ContainsKey(key))
            PersistentStrings.Remove(key);
    }
}