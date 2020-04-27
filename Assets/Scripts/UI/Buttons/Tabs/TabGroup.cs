using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TabGroup : MonoBehaviour
{
    private List<TabButton> _tabButtons = null;

    [SerializeField]
    private Color _idleColor = Color.white;
    [SerializeField]
    private Color _hoverColor = Color.white;
    [SerializeField]
    private Color _selectedColor = Color.white;

    [SerializeField]
    private List<GameObject> _tabMenus = null;

    private TabButton _selectedTab;

    public void BindTab(TabButton tab)
    {
        if (_tabButtons == null)
            _tabButtons = new List<TabButton>();
        _tabButtons.Add(tab);
    }

    public void OnTabEnter(TabButton tab)
    {
        ResetTabs();
        if (_selectedTab != null && _selectedTab == tab) { return; }
        tab.TabImage.color = _hoverColor;
    }

    public void OnTabExit(TabButton tab)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton tab)
    {
        if (_selectedTab != null)
        {
            _selectedTab.Deselect();
            CloseTabMenu(_selectedTab);
        }

        ResetTabs();

        _selectedTab = tab;
        tab.TabImage.color = _selectedColor;
        tab.Select();
        OpenTabMenu(tab);
    }

    public void OnTabDeselected(TabButton tab)
    {
        tab.Deselect();
        CloseTabMenu(tab);
        _selectedTab = null;
        ResetTabs();
    }

    public void ResetTabs()
    {
        foreach (TabButton tab in _tabButtons)
        {
            if (_selectedTab != null && _selectedTab == tab) { continue; }
                tab.TabImage.color = _idleColor;
        }
    }

    private void OpenTabMenu(TabButton tab)
    {
        int index = tab.transform.GetSiblingIndex();
        _tabMenus[index].SetActive(true);
        _tabMenus[index].transform.DOScale(1, 0.4f)
                                  .SetEase(Ease.OutBack)
                                  .SetUpdate(true);
    }
    private void CloseTabMenu(TabButton tab)
    {
        int index = tab.transform.GetSiblingIndex();
        _tabMenus[index].transform.DOScale(0, 0.4f)
                                  .SetEase(Ease.InBack)
                                  .SetUpdate(true);
        StartCoroutine(DisableMenu(_tabMenus[index]));
    }

    private IEnumerator DisableMenu(GameObject menu)
    {
        yield return new WaitForSeconds(0.4f);
        menu.SetActive(false);
    }
}
