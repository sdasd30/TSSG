using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
public class WebExportSave : MonoBehaviour
{
    public GameObject TextBoxWindow;
    public InputField SaveText;
    // Start is called before the first frame update
    void Start()
    {
        int i = SaveText.characterLimit;
        SaveText.characterLimit = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ExportSave()
    {
        if (!TextBoxWindow.activeSelf)
        {
            //string sav = SerializeDataToString(FindObjectOfType<StatTracker>().TransferToSaveObject());
            //SaveText.text = sav;
            //TextBoxWindow.SetActive(true);
        }
    }
    private string SerializeDataToString(SaveObject staticOptions)
    {
        //MemoryStream ms = new MemoryStream();
        //BinaryFormatter bf = new BinaryFormatter();
        //BinaryWriter tw = new BinaryWriter(ms, Encoding.UTF8);
        //bf.Serialize(ms, staticOptions);
        string json = JsonUtility.ToJson(staticOptions);
        //ms = (MemoryStream)tw.BaseStream;
        //string utfString = UtfToString(ms.ToArray());
        return json;
    }
    //private string UtfToString(byte[] bytes)
    //{
    //    UTF8Encoding encoding = new UTF8Encoding();
    //    string constructedString = encoding.GetString(bytes);
    //    return (constructedString);
    //}
}
