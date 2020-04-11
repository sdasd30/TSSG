using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldUISystem : MonoBehaviour
{

    public GameObject CanvasObject;
    public string TargetObjectTypeStr;
    public bool PlayerCharacterOnly = false;
    public bool DrawInHUD = false;
    public System.Type TargetObjectType;

    // Start is called before the first frame update
    void Start()
    {
        UIManager.AddUISystem(this);
    }

    public virtual void AcceptFunction(GameObject target)
    {
        if (CanvasObject == null || (PlayerCharacterOnly && (target.GetComponent<MovementBase>() == null ||
            !target.GetComponent<MovementBase>().IsPlayerControl)))
            return;
        if (DrawInHUD)
        {
            UIManager.AddToHUD(CanvasObject,target);
        } else
        {
            UIWorldCanvas wc = GetCanvas(target);
            if (wc != null)
                wc.AddUIObject(CanvasObject);
        }
    }

    protected UIWorldCanvas GetCanvas(GameObject targetObj)
    {
        UIWorldCanvasPointer g = targetObj.GetComponent<UIWorldCanvasPointer>();
        if (g == null)
        {
            GameObject newCanvas = Instantiate(UIManager.GetWorldCanvasPrefab());
            newCanvas.name = targetObj.name + "_WorldCanvas";
            UIWorldCanvasPointer newPointer = targetObj.AddComponent<UIWorldCanvasPointer>();
            newPointer.pointedObject = newCanvas;
            newCanvas.GetComponent<UIFollowGameObject>().TargetObject = targetObj;
            return newCanvas.GetComponent<UIWorldCanvas>();
        } else
        {
            return g.pointedObject.GetComponent<UIWorldCanvas>();
        }
            
    }
}
