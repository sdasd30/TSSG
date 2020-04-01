using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FactionType { IMMUNE, ALLIES, ENEMIES, NEUTRAL };


public class FactionHolder : MonoBehaviour
{
    void Start()
    {
        if (FindObjectOfType<FactionManager>() != null && GetComponent<Attackable>() != null)
            FindObjectOfType<FactionManager>().RegisterAttackable(this);
        GetComponent<PersistentItem>()?.InitializeSaveLoadFuncs(storeData, loadData);
    }
    void OnDestroy()
    {
        if (FindObjectOfType<FactionManager>() != null && GetComponent<Attackable>() != null)
            FindObjectOfType<FactionManager>().DeregisterAttackable(this);
    }
    public FactionType Faction = FactionType.NEUTRAL;
    public bool CanAttack(FactionType otherFaction)
    {
        return (otherFaction == FactionType.NEUTRAL || Faction == FactionType.NEUTRAL || otherFaction != Faction);
    }
    public bool CanAttack(FactionHolder f)
    {
        return CanAttack(f.Faction);
    }
    public bool CanAttack(Attackable otherObj)
    {
        
        //Debug.Log(otherObj);
        return CanAttack(otherObj.GetComponent<FactionHolder>().Faction);
    }
    public bool CanAttack(GameObject go)
    {
        if (go.GetComponent<FactionHolder>())
            return CanAttack(go.GetComponent<FactionHolder>().Faction);
        return true;
    }
    public void SetFaction(GameObject go)
    {
        if (go.GetComponent<FactionHolder>() == null)
            go.AddComponent<FactionHolder>();
        go.GetComponent<FactionHolder>().Faction = Faction;
        for(int i =0;i < go.transform.childCount;i++)
        {
            SetFaction(go.transform.GetChild(i).gameObject);
        }
    }
    public List<FactionType> GetOpposingFactions()
    {
        List<FactionType> ft = new List<FactionType>();
        ft.Add(FactionType.NEUTRAL);
        if (Faction == FactionType.ALLIES)
            ft.Add(FactionType.ENEMIES);
        else if (Faction == FactionType.ENEMIES)
            ft.Add(FactionType.ALLIES);
        return ft;
    }
    public Transform GetNearestEnemy(float MaxYDiff = -1f)
    {
        FactionHolder go = FindObjectOfType<FactionManager>().GetClosestOpposingFactionObj(this);
        if (go != null)
            return go.transform;
        return null;
    }

    private void storeData(CharData d)
    {
        d.SetInt("Faction", (int)Faction);
    }

    private void loadData(CharData d)
    {
        Faction = (FactionType)d.GetInt("Faction");
    }
}
