using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RHResponseCSVParser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<RHResponseString> parseExcelFile(string path)
    {
        List<RHResponseString> responses = new List<RHResponseString>();
        string fileData = System.IO.File.ReadAllText(path);
        string[] lines = fileData.Split("\n"[0]);
        List<string> lines_list = new List<string>(lines);
        lines_list.RemoveAt(0);
        foreach (string l in lines_list)
        {
            string[] lineData = (l.Trim()).Split(","[0]);
            if (lineData[0] == "")
                continue;

            Color c = new Color((lineData[9].Length > 0) ? float.Parse(lineData[9]) : 1f,
                (lineData[10].Length > 0) ? float.Parse(lineData[10]) : 1f,
                (lineData[11].Length > 0) ? float.Parse(lineData[11]) : 1f);
            int ft = (lineData[8].Length > 0) ? int.Parse(lineData[8]) : 12;

            RHResponseString rrs = new RHResponseString(lineData[0],ft,c);
            string statement = (lineData[1].Length > 0) ? lineData[1] : "";
            string previousStatement = (lineData[2].Length > 0) ? lineData[2] : "";
            string speaker = (lineData[1].Length > 0) ? lineData[1] : "";
            string listener = (lineData[1].Length > 0) ? lineData[1] : "";
            float diff = (lineData[1].Length > 0) ? float.Parse(lineData[1]) : -999f;
            rrs.m_refreshTime = (lineData[1].Length > 0) ? float.Parse(lineData[1]) : 60f;
            rrs.isPausing = (lineData[7].Length > 0) && (lineData[7].Substring(0, 1).ToLower() == "y" || lineData[7].Substring(0, 1).ToLower() == "t");
            rrs.SetConditions(statement, listener, speaker, previousStatement, diff);
        }
        return responses;
    }
}
