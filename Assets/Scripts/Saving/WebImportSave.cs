using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebImportSave : MonoBehaviour
{
    public GameObject TextBoxWindow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ImportSave()
    {
        if (!TextBoxWindow.activeSelf)
        {
            TextBoxWindow.SetActive(true);
        }
    }
}
