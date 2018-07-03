using System.Collections.Generic;
using UnityEngine;

public class InfiniteList : MonoBehaviour
{
    [SerializeField] private int _itemsCount;
    [SerializeField] private float _itemHeight;
    [SerializeField] private ItemsPool _itemsPool;
    [SerializeField] private UITable _table;
    [SerializeField] private UIScrollView _scrollView;

    private List<string> _data;
    private const int _itemsBuffer = 4;


    private void Start()
    {
        _data = new List<string>();
        for (var i = 0; i < _itemsCount; i++)
        {
            var item = "Item " + i;
            _data.Add(item);
        }

        InitializePool();
        InitializeItems();
    }

    private void Update()
    {
        var currentMomentum = Mathf.Abs(_scrollView.currentMomentum.y);
        if (currentMomentum <= 0)
            return;

        for (var i = 0; i < _itemsPool.Size; i++)
        {
            var item = _itemsPool[i];
            if (!IsVisible(item))
                Check(item);
        }
    }


    private void InitializePool()
    {
        var screenItemsCount = Mathf.CeilToInt(_scrollView.panel.baseClipRegion.w / _itemHeight);
        
        var poolSize = screenItemsCount + _itemsBuffer;
        poolSize = Mathf.Clamp(poolSize, 0, _itemsCount);
        
        _itemsPool.Initialize(poolSize);
    }

    private void InitializeItems()
    {
        var poolIndex = 0;
        for (var id = 0; id < _data.Count; id++)
        {
            var item = _itemsPool[poolIndex];
            if (item == null)
                break;

            InitializeItem(item, id);
            poolIndex++;
        }

        SetItemsPositions();
    }

    private void InitializeItem(Item item, int id)
    {
        item.gameObject.SetActive(true);
        item.Initialize(_data[id], id);
    }

    private void SetItemsPositions()
    {
        for (var i = 0; i < _itemsPool.Size; i++)
        {
            var item = _itemsPool[i];
            
            var newPosition = item.transform.localPosition;
            newPosition.y = -(_itemHeight / 2 + i * _itemHeight);

            item.transform.localPosition = newPosition;
        }
        
        _table.Reposition();
        _scrollView.SetDragAmount(0, 0, false);
    }

    private void ReplaceItem(Item item, int id, int oldId)
    {
        var newY = _itemsPool.Size * _itemHeight;

        var offset = Vector3.zero;
        offset.y = id < oldId ? newY : -newY;

        item.transform.localPosition += offset;
        item.Initialize(_data[id], id);
    }

    private void Check(Item item)
    {
        if (_data.Count <= _itemsPool.Size)
            return;

        var nextItem = _itemsPool.GetItemWithId(item.Id + 1);
        if (nextItem != null && IsVisible(nextItem))
        {
            var topBufferItemId = item.Id - _itemsBuffer / 2;
            for (var i = topBufferItemId; i >= 0; i--)
            {
                var topBufferItem = _itemsPool.GetItemWithId(i);
                if (topBufferItem == null)
                    break;

                var newId = _itemsPool.Size + i;
                if (newId < _data.Count)
                    ReplaceItem(topBufferItem, newId, i);
            }
        }

        var previousItem = _itemsPool.GetItemWithId(item.Id - 1);
        if (previousItem != null && IsVisible(previousItem))
        {
            var bottomBufferItemId = item.Id + _itemsBuffer / 2;
            for (var i = bottomBufferItemId; i < _data.Count; i++)
            {
                var bottomBufferItem = _itemsPool.GetItemWithId(i);
                if (bottomBufferItem == null)
                    break;

                var newId = i - _itemsPool.Size;
                if (newId > -1 && i < _data.Count)
                    ReplaceItem(bottomBufferItem, newId, i);
            }
        }
    }

    private bool IsVisible(Item item)
    {
        return item.IsVisibleBy(_scrollView.panel);
    }
}