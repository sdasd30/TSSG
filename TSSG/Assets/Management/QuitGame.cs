using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void makeGameQuit()
    {
        FindObjectOfType<SaveGame>().WriteSaveToFile();
        Application.Quit();
    }
}
