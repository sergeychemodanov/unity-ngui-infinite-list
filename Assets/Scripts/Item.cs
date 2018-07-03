using UnityEngine;

public class Item : MonoBehaviour
{
    public int Id { get; private set; }

    [SerializeField] private UILabel _label;
    [SerializeField] private UISprite _background;

    public void Initialize(string text, int id)
    {
        _label.text = text;
        Id = id;
        gameObject.name = "Item " + id;
    }

    public bool IsVisibleBy(UIPanel panel)
    {
        return panel.IsVisible(_background);
    }
}