using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class UIController : MonoBehaviour
{
    [SerializeField] Image[] lifes;
    [SerializeField] Text stageText;
    [SerializeField] GameObject clearText, gameOverText,missText,titlePanel,controllerPanel;
    [SerializeField] Button StartButton,reStartButton,Abutton;
    public static UIController I;
    Canvas canvas;
    public UnityAction ReStart,ChangeMode;
    public UnityAction<int> PushArrow;
    Arrow[] arrows;


    private void Awake()
    {
        GameManager.I.ui = this;
    }

    private void Start()
    {
        
        for (int i = 0; i < GameManager.I.life; i++)
        {
            lifes[i].gameObject.SetActive(true);
        }
        canvas = GetComponent<Canvas>();
        reStartButton.onClick.AddListener(() => ReStart?.Invoke());
        arrows = GetComponentsInChildren<Arrow>();
        Abutton.onClick.AddListener(() => ChangeMode?.Invoke());
        //arrows = controllerPanel.GetComponentsInChildren<Button>();
        //for (int i = 0; i < arrows.Length; i++)
        //{
        //    int idx = i;
        //    arrows[idx].onClick.AddListener(()=> PushArrow?.Invoke(idx) );
        //}
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit && hit.collider.GetComponent<Arrow>())
            {
                for (int i = 0; i < arrows.Length; i++)
                {
                    if(hit.collider.gameObject == arrows[i].gameObject)
                    {
                        PushArrow?.Invoke(i);
                    }
                }
                
            }
        }
    }


    public void ShowLife()
    {
        for (int i = 0; i < GameManager.I.life; i++)
        {
            lifes[i].gameObject.SetActive(true);
        }
    }


    public void HideLife(int idx)
    {
        lifes[idx].gameObject.SetActive(false);
    }

    public void UpdateStageText(int _stage)
    {
        stageText.text = $"STAGE {_stage}";
    }

    public void ShowClearText()
    {
        clearText.SetActive(true);
    }
    public void HideClearText()
    {
        clearText.SetActive(false);
    }

    public void ShowGameOverText()
    {
        gameOverText.SetActive(true);
    }
    public void HideGameOverText()
    {
        gameOverText.SetActive(false);
    }

    public void ShowMissText()
    {
        missText.SetActive(true);
    }
    public void HideMissText()
    {
        missText.SetActive(false);
    }



    public void UIInit()
    {
        
        HideClearText();
        HideGameOverText();
        
        Camera cam = Camera.main;
        canvas.worldCamera = cam;
    }
}
