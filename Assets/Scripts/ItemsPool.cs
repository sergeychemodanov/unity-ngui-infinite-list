using System.Linq;
using UnityEngine;

public class ItemsPool : MonoBehaviour
{
    public int Size { get { return _items.Length; } }

    [SerializeField] private Item _itemPrefab;
    [SerializeField] private Transform _itemsContainer;

    private Item[] _items;


    public void Initialize(int size)
    {
        _items = new Item[size];

        for (var i = 0; i < size; i++)
            InstantiateItem(i);
    }

    public Item this[int index]
    {
        get
        {
            if (index < 0 || index >= _items.Length)
                return null;

            return _items[index];
        }
    }

    public Item GetItemWithId(int id)
    {
        return _items.FirstOrDefault(i => i.Id == id);
    }


    private void InstantiateItem(int poolIndex)
    {
        var item = Instantiate(_itemPrefab, _itemsContainer, false);
        item.gameObject.SetActive(false);
        _items[poolIndex] = item;
    }
}