using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private InventoryHandItemView[] _views;

    private EventProcessingService _eventProcessingService;

    public void Init()
    {
        _eventProcessingService = SL.Get<EventProcessingService>();
        PlayerCharacter player = SL.Get<CharactersService>().GetPlayerCharacter();
        PlayerInventory inventory = player.PlayerInventory;

        for (int i = 0; i < _views.Length; i++)
        {
            _views[i].ChangeOutlineVisibleState(false);
            _views[i].gameObject.SetActive(i < inventory.OpenedItems.Count);
            if (_views[i].gameObject.activeSelf)
                _views[i].Init(inventory.OpenedItems[i].Data);
        }

        _views[0].ChangeOutlineVisibleState(true);

        _eventProcessingService.HandItemChanged += OnHandItemChanged;
    }

    private void OnHandItemChanged(HandItemData data)
    {
        for (int i = 0; i < _views.Length; i++)
        {
            bool isVisible = _views[i].gameObject.activeSelf && _views[i].HandItemType == data.Type;
            _views[i].ChangeOutlineVisibleState(isVisible);
        }
    }

    private void OnDestroy()
    {
        _eventProcessingService.HandItemChanged -= OnHandItemChanged;
    }
}