using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
    public UIController ui;
    public MapGenerator map;
    Player player;
    int stage = 1;
    public int life = 3;
    public int w, h;
    
    public enum GAME_STATE
    {
        WAIT,
        PLAY,
        SETBLOCK,
        GAMEOVER
    }

    public GAME_STATE state;

    private void Awake()
    {
        if(I == null)
        {
            I = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitGame();
    }

    private void InitGame()
    {
        map.InitMap();
        ui.ReStart = ResetGame;
        ui.ChangeMode = _changeMode;
        state = GAME_STATE.PLAY;
        ui.UpdateStageText(stage);
        player = map.player;
        ui.PushArrow = _pushArrow;
    }

    private void _pushArrow(int _idx)
    {
        if (state == GAME_STATE.WAIT || state == GAME_STATE.GAMEOVER ) return;
        switch (state)
        {
            case GAME_STATE.PLAY:
                player.Move(_idx);
                break;
            case GAME_STATE.SETBLOCK:
                player.PutGround(_idx);
                state = GAME_STATE.PLAY;
                break;

        }
        
    }

    private void _changeMode()
    {
        if(state == GAME_STATE.PLAY)
        {
            state = GAME_STATE.SETBLOCK;
            player.ShowCursor();
        }else if(state == GAME_STATE.SETBLOCK)
        {
            state = GAME_STATE.PLAY;
            player.HideCursor();
        }
        
    }


    public void ResetGame()
    {
        SoundManager.I.PlaySE(SESoundData.SE.CLICK);
        stage = 1;
        life = 3;
        w = 0;
        h = 0;
        SceneManager.LoadScene("Main");
        Destroy(gameObject);
    }

    public void StageClear()
    {
        stage++;
        w += 2;
        h += 2;
        StartCoroutine(_reloadScene());
    }
    public void Miss()
    {
        SoundManager.I.PlaySE(SESoundData.SE.MISS);
        ui.ShowMissText();
        state = GAME_STATE.WAIT;
        life--;
        ui.HideLife(life);
        player.HideCursor();
    }

    public void GameOver()
    {
        state = GAME_STATE.GAMEOVER;
        SoundManager.I.PlaySE(SESoundData.SE.DEAD);
      
        ui.ShowGameOverText();
    }


    //Ž€‚ñ‚¾‚Æ‚«
    public void Restart()
    {
        StartCoroutine(_restart());

    }

    IEnumerator _restart()
    {
        yield return new WaitForSeconds(1f);
        ui.HideMissText();
        state = GAME_STATE.PLAY;
        
    }


    IEnumerator _reloadScene()
    {
        ui.ShowClearText();
        state = GAME_STATE.WAIT;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main");
        yield return new WaitForSeconds(0.1f);
        InitGame();
        ui.UIInit();
    }

    


}
