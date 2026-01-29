using UnityEngine;
using UnityEngine.UI;

public class InventoryHandItemView : MonoBehaviour
{
    [SerializeField] private Image _outline;
    [SerializeField] private Image _icon;

    public HandItemType HandItemType { get; private set; }

    public void Init(HandItemData handItemData)
    {
        HandItemType = handItemData.Type;
        _icon.sprite = SL.Get<SpritesService>().GetHandItemSprite(HandItemType);
    }

    public void ChangeOutlineVisibleState(bool isVisible)
    {
        _outline.gameObject.SetActive(isVisible);
    }
}