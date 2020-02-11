using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class SaveGame : MonoBehaviour
{
    //StatTracker stats;
    // Start is called before the first frame update
    void Start()
    {
        //stats = FindObjectOfType<StatTracker>();
    }

    // Update is called once per frame
    public void WriteSaveToFile()
    {
        //SaveObject newSave = stats.TransferToSaveObject();

        //string json = JsonUtility.ToJson(newSave);
        //File.WriteAllText(Application.persistentDataPath + "\\gamesave.sav",json);
        //Debug.Log("Game Saved");
    }
}
