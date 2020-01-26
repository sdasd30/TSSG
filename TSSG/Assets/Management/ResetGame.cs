using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class ResetGame : MonoBehaviour
{
    //StatTracker stats;

    private void Start()
    {
        //stats = FindObjectOfType<StatTracker>();
    }

    public void resetGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.sav"))
        {
            File.Delete(Application.persistentDataPath + "/gamesave.sav");
            Debug.Log("Deleted game data at " + Application.persistentDataPath + "/gamesave.sav");
            //stats.initFromClean();
            //FindObjectOfType<AchievementManager>().ResetAchievements();
            FindObjectOfType<SaveGame>().WriteSaveToFile();
            FindObjectOfType<LoadGame>().LoadSaveFromFile();
            return;
        }

        else
        {
            Debug.LogWarning("Deletion Failed! Maybe you need to create a savegame first?");
        }
    }


}
