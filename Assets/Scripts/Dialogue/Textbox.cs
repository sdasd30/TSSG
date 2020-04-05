using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum DialogueSound { TYPED, SPOKEN, RECORDED};

public class Textbox : MonoBehaviour {

	public DialogueUnit MasterSequence;
	[SerializeField]
	private string RawText;
	[SerializeField]
	private string CurrentText;
	public float TimeBetweenType = 0.01f;
	public float PauseAfterTextboxDone = 2f;
	public Color TextboxColor;
	public Dictionary<MovementBase, bool> FrozenCharacters;
	public DialogueSound TypingSound;

	TextMeshProUGUI m_Text;
	private float m_sinceLastChar = 0f;
	private float m_sinceLastSound = 0f;
	private float m_pauseTime = 0f;
	private float m_timeSinceStop = 0f;
	private Vector3 m_lastPos;
	private string m_processedText;
	private int m_lastCharacterIndex;
	private bool m_isTyping;

	private GameObject m_targetedObj;

	public void SkipSection()
	{
		skipAll();
		Destroy(gameObject);
	}
	// Use this for initialization
	void Start () {
		m_Text = GetComponentInChildren<TextMeshProUGUI> ();
		if (!m_isTyping)
			skipAll();
	}

	void OnDestroy() {
		if (MasterSequence != null) {
			MasterSequence.parseNextElement ();
		}
		//TextboxManager.Instance.removeTextbox (gameObject);
		/*if (m_targetedObj.GetComponent<Character> ()) {
			m_targetedObj.GetComponent<Character> ().onTBComplete ();
		}*/
	}
		
	public void SetPause(float pt) {
		m_pauseTime = pt;
	}
	public void SetTargetObj(GameObject gameObj) {
		m_targetedObj = gameObj;
		if (m_targetedObj != null)
			m_lastPos = gameObj.transform.position;
	}
	public void SetTypeMode(bool type) {
		m_isTyping = type;
		if (type) {
			CurrentText = "";
			m_lastCharacterIndex = 0;
		} else {
			CurrentText = m_processedText;
		}
	}
	public void setText(string text) {
		RawText = text;
		m_processedText = RawText;
	}
	public void FreezeCharacter(MovementBase bm, bool isFrozen = true) {
		if (!FrozenCharacters.ContainsKey (bm))
			FrozenCharacters.Add (bm, bm.IsPlayerControl);
		bm.GetComponent<MovementBase>().SetCanMove(!isFrozen);
	}
		
	/* Cutscene scripting guide:
	 *  Normal text is shown as dialogue for the starting character.
	 * 	Using the '|' character or the newline character will create a new textbox.
	 *  At the start of a new textbox if the colon character is found within the first 18 characters, the game will attempt to search
	 *  For the character and make the dialogue come from that character instead.
	 *  
	 *  The characters < and > surrounds a special block. This can call commands in a simple lisp like language
	 * 
	 * 
	 * A number will result in a pause for a certain amount of time.
	 * $ will change the text speed
	 * 
	 * */

	public string ParseSection(string section)
	{
		string retString = "";
		string remainingParseStr = section;
		while (remainingParseStr.Length > 1)
		{
			char nextChar = remainingParseStr.ToCharArray()[0];
			remainingParseStr = remainingParseStr.Substring(1);
			if (nextChar != '<')
			{
				retString += nextChar;
			} else
			{
				remainingParseStr = ParseSpecialSection(remainingParseStr);
			}
		}
		if (remainingParseStr.Length > 0)
			retString += remainingParseStr;
		return retString;
	}
	public string ParseSpecialSection(string section) {
		string actStr = "";
		int charNum = 0;
		char nextChar = section.ToCharArray () [charNum];
		actStr += nextChar;
		int numSpecials = 1;
		string retString = "";
		while (numSpecials > 0 && charNum < section.Length - 1) {
			charNum++;
			nextChar = section.ToCharArray () [charNum];
			if (nextChar == '>') {
				numSpecials--;
				continue;
			} else if (nextChar == '<') {
				numSpecials++;
				continue;
			}
			actStr += nextChar;
		}

		List<DialogueAction> executedActions = TextboxManager.ValidActions(actStr);
		foreach (DialogueAction da in executedActions)
			retString += da.PerformAction (actStr, this);
		charNum++;
		return retString + section.Substring(charNum);
	}


	private void processSpecialSection(bool skip = false) {
		string actStr = "";
		char nextChar = RawText.ToCharArray () [m_lastCharacterIndex];
		int numSpecials = 1;
		while (numSpecials > 0 && m_lastCharacterIndex < RawText.Length - 1) {
			actStr += nextChar;
			m_lastCharacterIndex++;
			nextChar = RawText.ToCharArray () [m_lastCharacterIndex];
			if (nextChar == '>')
				numSpecials--;
			else if (nextChar == '<')
				numSpecials++;
		}
		List<DialogueAction> executedActions = TextboxManager.ValidActions(actStr);
		string retStr = "";
		foreach (DialogueAction da in executedActions)
		{
			if (skip)
				retStr += da.SkipAction(actStr, this);
			else
				retStr += da.PerformAction(actStr, this);
		}
		m_lastCharacterIndex++;
		m_processedText = m_processedText.Substring(0, m_lastCharacterIndex) + retStr + m_processedText.Substring(m_lastCharacterIndex);
	}

	private void processNormalChar(char nextChar, bool skip = false) {
		if (m_sinceLastSound > 0.15f && !skip) {
			m_sinceLastSound = 0f;
			playSound ();
		}
		CurrentText += nextChar;
		m_Text.text = CurrentText;
		m_sinceLastChar = 0f;
	}

	private void processChar(bool skip = false) {
		m_lastCharacterIndex++;
		char nextChar = m_processedText.ToCharArray () [m_lastCharacterIndex - 1];
		if (nextChar == '<') {
			processSpecialSection (skip);
		} else {
			processNormalChar (nextChar);
		}
	}

	void Update () {
		if (m_targetedObj != null) {
			transform.position += m_targetedObj.transform.position-m_lastPos;
			m_lastPos = m_targetedObj.transform.position;
		}
		if (m_isTyping ) {
			if (m_lastCharacterIndex < m_processedText.Length) { 
				m_sinceLastChar += Time.deltaTime;
				m_sinceLastSound += Time.deltaTime;
				if (m_pauseTime > 0f) {
					m_pauseTime -= Time.deltaTime;
				} else if (m_sinceLastChar > TimeBetweenType) {
					processChar ();
				}
			} else {
				m_timeSinceStop += Time.deltaTime;
				if (m_timeSinceStop > PauseAfterTextboxDone) {
					Destroy (gameObject);
				}
			}
		}
	}
	private void skipAll()
	{
		while (m_lastCharacterIndex < m_processedText.Length)
		{
			if (m_lastCharacterIndex < m_processedText.Length)
			{
				processChar(true);
			}
			else
			{
				m_timeSinceStop += Time.deltaTime;
				if (m_timeSinceStop > PauseAfterTextboxDone)
				{
					Destroy(gameObject);
				}
			}
		}
	}
	private void playSound() {
		/*switch (TypingSound) {
		case DialogueSound.RECORDED:
			FindObjectOfType<AudioManager> ().PlayClipAtPos (UIList.Instance.SFXDialogueStatic, FindObjectOfType<CameraFollow> ().transform.position, 0.15f, 0f, 0.1f);
			break;
		case DialogueSound.TYPED:
			FindObjectOfType<AudioManager> ().PlayClipAtPos (UIList.Instance.SFXDialogueClick, FindObjectOfType<CameraFollow> ().transform.position, 0.2f, 0f, 0.25f);
			break;
		case DialogueSound.SPOKEN:
			FindObjectOfType<AudioManager> ().PlayClipAtPos (UIList.Instance.SFXDialogueSpeak, FindObjectOfType<CameraFollow> ().transform.position, 0.2f, 0f, 0.25f);
			break;
		default:
			break;
		}*/
	}
}

/*
private void playAnimation(string targetChar) {
		string[] chars = targetChar.Split(',');
		if (chars.Length < 2)
			return;
		GameObject character = GameObject.Find (chars [0]);
		string anim = chars [1];
		if (character != null && character.GetComponent<AnimatorSprite>()) {
			bool res = character.GetComponent<AnimatorSprite> ().Play (anim);
		}
	}
*/