using UnityEngine;

public class MainFirstScenePanel : AbstractPanel
{
    [SerializeField] private InventoryView _inventoryView;

    public override PanelType Type => PanelType.MainFirstScene;

    public override void Init()
    {
        base.Init();

        _inventoryView.Init();
    }
}