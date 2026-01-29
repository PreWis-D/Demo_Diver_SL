using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbstractUIManager : MonoBehaviour
{
    [SerializeField] private Transform panelsContainer;
    [SerializeField] private Transform popupsContainer;
    [SerializeField] private AbstractPanel[] panelPrefabs;
    [SerializeField] private AbstractPopup[] popupPrefabs;

    public List<AbstractPanel> Panels { get; protected set; } = new List<AbstractPanel>();
    public List<AbstractPopup> Popups { get; protected set; } = new List<AbstractPopup>();

    public virtual void Init()
    {
        InitPanels();
        InitPopups();

        HideAllPanels();
        HideAllPopups();
    }

    private void InitPanels()
    {
        foreach (var prefab in panelPrefabs)
        {
            var panel = Instantiate(prefab, panelsContainer);
            Panels.Add(panel);
            panel.Init();
        }
    }

    private void InitPopups()
    {
        foreach (var prefab in popupPrefabs)
        {
            var popup = Instantiate(prefab, popupsContainer);
            Popups.Add(popup);
            popup.Init();
        }
    }

    protected AbstractPanel GetPanel(PanelType type)
    {
        return Panels.First(p => p.Type == type);
    }

    public void HideAllPanels()
    {
        foreach (var panel in Panels)
            panel.Hide();
    }

    protected AbstractPopup GetPopup(PopupType type)
    {
        return Popups.First(p => p.Type == type);
    }

    public void HideAllPopups()
    {
        foreach (var popup in Popups)
            popup.ForceHide();
    }
}
