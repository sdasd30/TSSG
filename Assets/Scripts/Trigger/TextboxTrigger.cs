using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextboxTrigger : Interactable {
	
	public bool typeText = true;

	public bool ClearAllSequence = true;
	public bool Skippable = false;
	public DialogueSound soundType = DialogueSound.SPOKEN;

	public string textFilePath;
	[TextArea(3,8)]
	public string TextboxString;

	
	void Start() {
		base.init ();
	}

	void Update () { destroyAfterUse (); }

	void OnDrawGizmos() {
		Gizmos.color = new Color (1, 0, 1, .5f);
		Gizmos.DrawCube (transform.position, transform.localScale);
	}
	internal void OnTriggerEnter(Collider other)
	{
		//Debug.Log("ON trigger enter: " + other.gameObject);
		if (interactableObjectInfo.autoTrigger && other.gameObject.GetComponent<CharacterBase>())
		{
			//Debug.Log("Starting on Trigger");
			TriggerWithCoolDown(other.gameObject);
		}
		if (other.gameObject.GetComponent<CharacterBase>() != null)
		{
			/*m_prompt.text = "Press " + TextboxManager.GetKeyString("Interact") + " to " + InteractionPrompt;
            FindObjectOfType<GUIHandler>().ReplaceText(m_prompt);*/
			//Actor = other.gameObject.GetComponent<CharacterBase>();
			// Actor.PromptedInteraction = this;
		}
	}
	protected override void onTrigger(GameObject interactor) {
        Debug.Log("On Trigger");
		if (ClearAllSequence)
			TextboxManager.ClearAllSequences ();
		TextboxManager.SetSoundType (soundType);
		if (ClearAllSequence)
			TextboxManager.ClearAllSequences ();
		string txt = TextboxString;
		if (textFilePath.Length > 0)
		{
			TextAsset mytxtData = (TextAsset)Resources.Load("MyText");
			txt = mytxtData.text;
		}
		TextboxManager.StartSequence (txt,null,Skippable);
	}

	private void storeData(CharData d) {
		d.SetBool("TriggerUsed", TriggerUsed);
		d.SetString( "TextboxString", TextboxString);
	}

	private void loadData(CharData d) {
		TriggerUsed = d.GetBool("TriggerUsed");
		TextboxString = d.GetString("TextboxString");
	}
}