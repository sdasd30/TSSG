﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOption : MonoBehaviour {
	public delegate void SelectFunction(DialogueOption dop);
	public string SelectionText = "";
	public string remainderText = "";
	public SelectFunction OnSelect;
	public DialogueOptionBox MasterBox;
    public bool CloseDialogueWindow;

    public void OnSelection() {
		OnSelect (this);
        if (CloseDialogueWindow)
            GameObject.Destroy(MasterBox.gameObject);
    }
}
