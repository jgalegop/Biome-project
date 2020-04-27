using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonMenu)), CanEditMultipleObjects]
public class ButtonMenuEditor : Editor
{
    private ButtonMenu _menu;

    private void OnEnable()
    {
        _menu = (ButtonMenu)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _menu.SetBackgroundSize();
    }
}
