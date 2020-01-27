using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


public class LoadGame : MonoBehaviour
{
    // Start is called before the first frame update
    //StatTracker stats;
    void Start()
    {
        //stats = FindObjectOfType<StatTracker>();
        LoadSaveFromFile();
    }

    // Update is called once per frame
    public void LoadSaveFromFile()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.sav"))
        {
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(Application.persistentDataPath + "/gamesave.sav", FileMode.Open);
            //SaveObject oldSave = (SaveObject)bf.Deserialize(file);
            //stats.LoadFromSaveObject(oldSave);
            //file.Close();

            string json = File.ReadAllText(Application.persistentDataPath + "/gamesave.sav");
            SaveObject oldSave = JsonUtility.FromJson<SaveObject>(json);
            //stats.LoadFromSaveObject(oldSave);
            Debug.Log("Loaded game from " + Application.persistentDataPath + "/gamesave.sav");
            return;
        }

        else
        {
            Debug.LogWarning("Load Failed! File does not exist.");
        }
    }
}
