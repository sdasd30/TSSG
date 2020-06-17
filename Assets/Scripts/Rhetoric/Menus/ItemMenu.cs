using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public delegate void ItemMenuOptionCallbacks(Item i);
public class ItemMenu : MonoBehaviour
{
    public Item HighlightedItem;
    [SerializeField]
    private GameObject MenuOption;
    // Start is called before the first frame update
    void Start()
    {
        HighlightedItem.AddItemOptions(this);
    }
    void Update()
    {
        HideIfClickedOutside();
    }
    public void AddOption(ItemMenuOptionCallbacks callBack,string optionName)
    {
        GameObject newOption = Instantiate(MenuOption, transform);
        newOption.GetComponentInChildren<TextMeshProUGUI>().text = optionName;
        newOption.GetComponent<Button>().onClick.AddListener(delegate { callBack(HighlightedItem); });
    }
    private void dropItem(Item i)
    {
        i.GetContainer().DropItem(i.GetSlot());
    }
    private void HideIfClickedOutside()
    {
        
        if (Input.GetMouseButton(0) && !RectTransformUtility.RectangleContainsScreenPoint(
                gameObject.GetComponent<RectTransform>(),
                Input.mousePosition,
                null)) {
        
            Destroy(gameObject);
        }
    }
}
