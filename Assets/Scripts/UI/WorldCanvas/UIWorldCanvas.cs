using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldCanvas : MonoBehaviour
{
    public GameObject container;

    public void AddUIObject(GameObject newUIObject)
    {
        GameObject go = Instantiate(newUIObject);
        go.transform.SetParent(container.transform);
        go.GetComponent<WUIBase>().Target = GetComponent<UIFollowGameObject>().TargetObject;
        go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.localRotation = Quaternion.identity;
    }

    public void deleteObject(string name)
    {
        Transform t = container.transform.Find(name);
        if (t != null)
            Destroy(t.gameObject);
    }
}
