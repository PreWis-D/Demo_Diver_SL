using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private AbstractHandItem[] _abstractHandItems;

    private InputProcessingService _input;

    public List<AbstractHandItem> OpenedItems { get; private set; } = new List<AbstractHandItem>();
    public AbstractHandItem CurrentHandItem { get; private set; }

    public void Init()
    {
        _input = SL.Get<InputProcessingService>();
        HandItemsConfig config = SL.Get<GeneralComponentsService>().GetHandItemsConfig();

        for (int i = 0; i < _abstractHandItems.Length; i++)
        {
            HandItemData data = config.GetData(_abstractHandItems[i].Type);
            _abstractHandItems[i].Init(data);
            if (_abstractHandItems[i].IsOpened)
                OpenedItems.Add(_abstractHandItems[i]);
            _abstractHandItems[i].ChangeEnabledState(false);
        }

        CurrentHandItem = OpenedItems[0];
        CurrentHandItem.ChangeEnabledState(true);

        _input.SwitchTargetItem += OnSwitchTargetItem;
        _input.SwitchPreviousItem += OnSwitchPreviousItem;
        _input.SwitchNextItem += OnSwitchNextItem;
    }

    private void OnSwitchTargetItem(int index)
    {
        CurrentHandItem.ChangeEnabledState(false);

        if (index < OpenedItems.Count)
            CurrentHandItem = OpenedItems[index];

        CurrentHandItem.ChangeEnabledState(true);
        SL.Get<EventProcessingService>().HandItemChangedInvoke(CurrentHandItem.Data);
    }

    private void OnSwitchPreviousItem()
    {
        int currentIndex = 0;

        for (int i = 0; i < OpenedItems.Count; i++)
            if (CurrentHandItem == OpenedItems[i])
                currentIndex = i;

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = OpenedItems.Count - 1;

        OnSwitchTargetItem(currentIndex);

    }

    private void OnSwitchNextItem()
    {
        int currentIndex = 0;

        for (int i = 0; i < OpenedItems.Count; i++)
            if (CurrentHandItem == OpenedItems[i])
                currentIndex = i;

        currentIndex++;
        if (currentIndex > OpenedItems.Count - 1)
            currentIndex = 0;

        OnSwitchTargetItem(currentIndex);
    }

    public Transform GetItemLeftHandPoint()
    {
        return CurrentHandItem switch
        {
            Harpoon harpoon => harpoon.LeftHand,
            Jackhammer jackhammer => jackhammer.LeftHand,
            GasSuction gasSuction => gasSuction.LeftHand,
            _ => null
        };
    }

    public Transform GetItemRightHandPoint()
    {
        return CurrentHandItem switch
        {
            Harpoon harpoon => harpoon.RightHand,
            Jackhammer jackhammer => jackhammer.RightHand,
            GasSuction gasSuction => gasSuction.RightHand,
            _ => null
        };
    }

    public bool HasItemVisible()
    {
        return CurrentHandItem.HasVisible();
    }

    public AbstractHandItem GetHandItem(HandItemType type)
    {
        return OpenedItems.FirstOrDefault(item => item.Data.Type == type);
    }

    public void OnDestroy()
    {
        _input.SwitchTargetItem -= OnSwitchTargetItem;
        _input.SwitchPreviousItem -= OnSwitchPreviousItem;
        _input.SwitchNextItem -= OnSwitchPreviousItem;
    }
}