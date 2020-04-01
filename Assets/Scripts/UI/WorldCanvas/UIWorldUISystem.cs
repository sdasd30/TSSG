using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldUISystem : MonoBehaviour
{

    public GameObject CanvasObject;
    public string TargetObjectTypeStr;
    public System.Type TargetObjectType;

    // Start is called before the first frame update
    void Start()
    {
        UIWorldCanvasManager.AddUISystem(this);
    }

    public virtual void AcceptFunction(GameObject target)
    {
        UIWorldCanvas wc = GetCanvas(target);
        if (wc != null && CanvasObject != null)
            wc.AddUIObject(CanvasObject);
    }

    protected UIWorldCanvas GetCanvas(GameObject targetObj)
    {
        UIWorldCanvasPointer g = targetObj.GetComponent<UIWorldCanvasPointer>();
        if (g == null)
        {
            GameObject newCanvas = Instantiate(UIWorldCanvasManager.GetWorldCanvasPrefab());
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
