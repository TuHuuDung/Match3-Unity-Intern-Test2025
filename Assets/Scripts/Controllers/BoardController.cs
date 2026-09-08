using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };

    public bool IsBusy { get; private set; }

    private Board m_board;

    private BottomTray m_tray;

    private GameManager m_gameManager;

    private Camera m_cam;

    private GameSettings m_gameSettings;

    private bool m_gameOver;

    private bool m_autoplayActive;

    private bool m_timeAttackMode;

    private Dictionary<Item, Cell> m_itemOrigin;

    public void StartGame(GameManager gameManager, GameSettings gameSettings, bool timeAttackMode = false)
    {
        m_gameManager = gameManager;
        m_gameSettings = gameSettings;
        m_timeAttackMode = timeAttackMode;
        m_itemOrigin = new Dictionary<Item, Cell>();

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);
        m_board.FillDivisibleByThree();

        float trayPosY = -gameSettings.BoardSizeY * 0.5f - 1.5f;
        m_tray = new BottomTray(this.transform, gameSettings.BottomCellsCount, trayPosY);
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.WIN:
            case GameManager.eStateGame.LOSE:
                m_gameOver = true;
                break;
        }
    }

    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;
        if (m_autoplayActive) return;

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                if (cell == null || cell.IsEmpty) return;

                if (!cell.IsTray)
                {
                    //Tapping a board item sends it to the tray.
                    TryMoveItemToTray(cell);
                }
                else if (m_timeAttackMode)
                {
                    //Attack Mode
                    TryReturnItemToBoard(cell);
                }
            }
        }
    }

    private void TryMoveItemToTray(Cell boardCell)
    {
        if (!m_tray.HasEmptySlot()) return;

        IsBusy = true;

        Cell slot = m_tray.GetFirstEmptySlot();
        Item item = boardCell.Item;

        boardCell.Free();
        m_itemOrigin[item] = boardCell;

        m_tray.PlaceItem(item, slot, () =>
        {
            OnMoveEvent();
            ResolveTray();
        });
    }

    private void TryReturnItemToBoard(Cell traySlot)
    {
        Item item = traySlot.Item;
        if (item == null) return;
        if (!m_itemOrigin.TryGetValue(item, out Cell originCell)) return;

        IsBusy = true;

        traySlot.Free();
        m_itemOrigin.Remove(item);

        originCell.Assign(item);
        item.View.DOMove(originCell.transform.position, 0.3f).OnComplete(() =>
        {
            IsBusy = false;
        });
    }

    private void ResolveTray()
    {
        List<Cell> triple = m_tray.FindTriple();

        if (triple.Count == 3)
        {
            StartCoroutine(ClearTripleCoroutine(triple));
        }
        else if (m_tray.IsFull && !m_timeAttackMode)
        {
         
            EndGame(false);
        }
        else
        {
            IsBusy = false;
        }
    }

    private IEnumerator ClearTripleCoroutine(List<Cell> triple)
    {
        foreach (var cell in triple)
        {
            if (cell.Item != null) m_itemOrigin.Remove(cell.Item);
        }

        m_tray.ClearSlots(triple);

        yield return new WaitForSeconds(0.2f);

        if (m_board.IsEmpty())
        {
            EndGame(true);
        }
        else
        {
            IsBusy = false;
        }
    }

    private void EndGame(bool won)
    {
        m_gameOver = true;
        m_gameManager.SetGameResult(won);
    }

    internal void Clear()
    {
        if (m_board != null) m_board.Clear();
        if (m_tray != null) m_tray.Clear();
    }

    

    public void StartAutoplay(bool goalWin)
    {
        m_autoplayActive = true;
        StartCoroutine(AutoplayCoroutine(goalWin));
    }

    private IEnumerator AutoplayCoroutine(bool goalWin)
    {
        while (!m_gameOver)
        {
            yield return new WaitUntil(() => !IsBusy);
            yield return new WaitForSeconds(0.5f);

            if (m_gameOver) yield break;

            Cell target = goalWin ? ChooseCellForWin() : ChooseCellForLose();
            if (target == null) yield break;

            TryMoveItemToTray(target);
        }
    }

    private Cell ChooseCellForWin()
    {
        Cell occupiedSlot = m_tray.Slots.FirstOrDefault(c => !c.IsEmpty);
        if (occupiedSlot != null)
        {
            NormalItem occupiedItem = occupiedSlot.Item as NormalItem;
            if (occupiedItem != null)
            {
                Cell match = m_board.FindCellOfType(occupiedItem.ItemType);
                if (match != null) return match;
            }
        }

        return m_board.FindAnyCellWithItem();
    }

    private Cell ChooseCellForLose()
    {
        HashSet<NormalItem.eNormalType> typesInTray = new HashSet<NormalItem.eNormalType>(
            m_tray.Slots.Where(c => !c.IsEmpty)
                        .Select(c => (c.Item as NormalItem).ItemType));

        Cell cell = m_board.FindCellOfTypeNotIn(typesInTray);
        if (cell != null) return cell;

        return m_board.FindAnyCellWithItem();
    }
}