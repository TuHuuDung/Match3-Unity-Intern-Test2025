using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };

    public enum eLevelMode
    {
        TIMER,
        MOVES
    }

    public enum eStateGame
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        WIN,
        LOSE,
    }

    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;

            StateChangedAction(m_state);
        }
    }


    private GameSettings m_gameSettings;


    private BoardController m_boardController;

    private UIMainManager m_uiMenu;

    private LevelCondition m_levelCondition;

    private void Awake()
    {
        State = eStateGame.SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);
    }

    void Start()
    {
        State = eStateGame.MAIN_MENU;
    }

 
    void Update()
    {
        if (m_boardController != null) m_boardController.Update();
    }


    internal void SetState(eStateGame state)
    {
        State = state;

        if (State == eStateGame.PAUSE)
        {
            DOTween.PauseAll();
        }
        else
        {
            DOTween.PlayAll();
        }
    }

    public void LoadLevel(eLevelMode mode)
    {
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings);

        if (mode == eLevelMode.MOVES)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelMoves>();
            m_levelCondition.Setup(m_gameSettings.LevelMoves, m_uiMenu.GetLevelConditionView(), m_boardController);
        }
        else if (mode == eLevelMode.TIMER)
        {
            m_levelCondition = this.gameObject.AddComponent<LevelTime>();
            m_levelCondition.Setup(m_gameSettings.LevelTime, m_uiMenu.GetLevelConditionView(), this);
        }

        m_levelCondition.ConditionCompleteEvent += OnConditionRanOut;

        State = eStateGame.GAME_STARTED;
    }

  
    public void LoadLevelAutoplay(bool goalWin)
    {
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings);
        m_boardController.StartAutoplay(goalWin);

        State = eStateGame.GAME_STARTED;
    }

  
    public void LoadLevelTimeAttack()
    {
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings, true);

        m_levelCondition = this.gameObject.AddComponent<LevelTime>();
        m_levelCondition.Setup(m_gameSettings.TimeAttackDuration, m_uiMenu.GetLevelConditionView(), this);
        m_levelCondition.ConditionCompleteEvent += OnConditionRanOut;

        State = eStateGame.GAME_STARTED;
    }

    private void OnConditionRanOut()
    {
     
        SetGameResult(false);
    }

   
    public void SetGameResult(bool won)
    {
        StartCoroutine(WaitBoardController(won));
    }

    internal void ClearLevel()
    {
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }
    }

    private IEnumerator WaitBoardController(bool won)
    {
        yield return new WaitForSeconds(1f);

        State = won ? eStateGame.WIN : eStateGame.LOSE;

        if (m_levelCondition != null)
        {
            m_levelCondition.ConditionCompleteEvent -= OnConditionRanOut;

            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }
}