using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BottomTray
{
    private Cell[] m_slots;

    private Transform m_root;

    public IEnumerable<Cell> Slots => m_slots;

    public bool IsFull => m_slots.All(c => !c.IsEmpty);

    public bool IsEmpty => m_slots.All(c => c.IsEmpty);

    public BottomTray(Transform root, int size, float posY)
    {
        m_root = root;
        m_slots = new Cell[size];

        CreateTray(size, posY);
    }

    private void CreateTray(int size, float posY)
    {
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);

        float startX = -size * 0.5f + 0.5f;

        for (int i = 0; i < size; i++)
        {
            GameObject go = GameObject.Instantiate(prefabBG);
            go.transform.position = new Vector3(startX + i, posY, 0f);
            go.transform.SetParent(m_root);

            Cell cell = go.GetComponent<Cell>();
            cell.Setup(i, 0, true); // true = this is a tray slot

            m_slots[i] = cell;
        }
    }

    public bool HasEmptySlot()
    {
        return m_slots.Any(c => c.IsEmpty);
    }

    public Cell GetFirstEmptySlot()
    {
        return m_slots.FirstOrDefault(c => c.IsEmpty);
    }

    public void PlaceItem(Item item, Cell slot, Action onComplete)
    {
        slot.Assign(item);

        item.View.DOMove(slot.transform.position, 0.3f).OnComplete(() => onComplete?.Invoke());
    }

    public List<Cell> FindTriple()
    {
        var occupied = m_slots.Where(c => !c.IsEmpty).ToList();

        var group = occupied
            .Where(c => c.Item is NormalItem)
            .GroupBy(c => ((NormalItem)c.Item).ItemType)
            .FirstOrDefault(g => g.Count() >= 3);

        return group != null ? group.Take(3).ToList() : new List<Cell>();
    }

    public void ClearSlots(List<Cell> cells)
    {
        foreach (var cell in cells)
        {
            cell.ExplodeItem();
        }
    }

    public void Clear()
    {
        for (int i = 0; i < m_slots.Length; i++)
        {
            m_slots[i].Clear();
            GameObject.Destroy(m_slots[i].gameObject);
            m_slots[i] = null;
        }
    }
}