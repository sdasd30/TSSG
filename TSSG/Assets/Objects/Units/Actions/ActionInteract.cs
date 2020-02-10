using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InteractInfo
{
    public Vector3 HitboxScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 HitboxOffset = new Vector3(0f, 0f);
    public float HitboxDuration = 0.5f;
}

public class ActionInteract : ActionInfo
{
    [SerializeField]
    private List<InteractInfo> m_InteractData;

    protected override void OnAttack()
    {
        base.OnAttack();
        if (m_InteractData.Count != 0)
        {
            createInteract();
        }
    }

    protected void createInteract()
    {
        //m_hitboxMaker.AddHitType(HitType);
        foreach (InteractInfo pi in m_InteractData)
        {
            CreateInteract(pi);
        }
    }

    private void CreateInteract(InteractInfo i)
    {
        if (ListHitboxes.Instance == null)
        {
            Debug.LogWarning("LIstHitboxes not found. Please add a Gameobject with the ListHItboxes prefab");
            return;
        }
        Vector3 newPos = transform.position + (Vector3)i.HitboxOffset;
        //Debug.Log("Instantiated at: " + newPos);
        var go = GameObject.Instantiate(ListHitboxes.Instance.InteractBox, newPos, Quaternion.identity);
        go.transform.localScale = i.HitboxScale;
    }
}
