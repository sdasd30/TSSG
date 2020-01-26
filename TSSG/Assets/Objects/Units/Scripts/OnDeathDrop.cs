using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeathDropItem
{
    public GameObject Item;
    public Vector2 Offset;
    public Vector2 XOffsetRange;
    public Vector2 YOffsetRange;
    public float Probability = 100f;
    public bool UseExcludeCategory = false;
    public int ExcludeCategory = 1;
    public bool InheritFaction = true;
}
public class OnDeathDrop : MonoBehaviour
{
    public List<DeathDropItem> DeathItems = new List<DeathDropItem>();

    private List<DeathDropItem> no_exclude_items = new List<DeathDropItem>();
    private Dictionary<int, List<DeathDropItem>> m_excludeItems = new Dictionary<int, List<DeathDropItem>>();
    // Start is called before the first frame update

    private void Start()
    {
        foreach ( DeathDropItem item in DeathItems)
        {
            if (!item.UseExcludeCategory)
                no_exclude_items.Add(item);
            else
            {
                if (!m_excludeItems.ContainsKey(item.ExcludeCategory))
                    m_excludeItems[item.ExcludeCategory] = new List<DeathDropItem>();
                m_excludeItems[item.ExcludeCategory].Add(item);
            }
        }
    }
    private void OnDestroy()
    {
        foreach (DeathDropItem item in no_exclude_items)
        {
            if (Random.Range(0f,100f) < item.Probability)
            {
                spawnItem(item);
            }      
        }
        foreach(int i in m_excludeItems.Keys)
        {
            SpawnExcludeItems(m_excludeItems[i]);
        }
    }
    private void SpawnExcludeItems(List<DeathDropItem> list)
    {
        float maxProbability = 0f;
        foreach( DeathDropItem it in list)
        {
            maxProbability += it.Probability;
        }
        float roll = Random.Range(0f, Mathf.Max(100f, maxProbability));
        float total = 0f;
        foreach(DeathDropItem it in list)
        {
            total += it.Probability;
            if (roll < total)
            {
                spawnItem(it);
                break;
            }
        }
    }
    private void spawnItem(DeathDropItem item)
    {
        Vector3 pos = new Vector3(transform.position.x + item.Offset.x + Random.Range(item.XOffsetRange.x, item.XOffsetRange.y),
                    transform.position.y + item.Offset.y + Random.Range(item.YOffsetRange.x, item.YOffsetRange.y), transform.position.z);
        GameObject go = Instantiate(item.Item, pos, Quaternion.identity);
        if (item.InheritFaction)
        {
            GetComponent<FactionHolder>().SetFaction(go);
        } else if (go.GetComponent<FactionHolder>() == null)
        {
            go.AddComponent<FactionHolder>();
            go.GetComponent<FactionHolder>().Faction = FactionType.NEUTRAL;
        }
        if (go.GetComponent<Hitbox>() != null && GetComponent<Hitbox>() != null)
            go.GetComponent<Hitbox>().OriginSource = GetComponent<Hitbox>().OriginSource;
    }
}
