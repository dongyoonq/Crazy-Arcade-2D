using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    //private EventSystem eventSystem;
    private Canvas popUpCanvas;
    public Stack<PopUpUI> popUpStack;
    public bool isPopUpOpened;

    private void Awake()
    {
        //eventSystem = GameManager.Resource.Instantiate<EventSystem>("UI/EventSystem");
        //eventSystem.transform.parent = transform;
        isPopUpOpened = false;
        Init();

    }

    public void Init()
    {
        popUpCanvas = GameManager.Resource.Instantiate<Canvas>("UI/UICanvas");
        popUpCanvas.gameObject.name = "PopUpCanvas";
        popUpCanvas.sortingOrder = 100;
        popUpStack = new Stack<PopUpUI>();
    }

    public T ShowPopUpUI<T>(T popUpui) where T : PopUpUI
    {
        if (popUpStack.Count > 0)
        {
            PopUpUI prevUI = popUpStack.Peek();
            prevUI.gameObject.SetActive(false);
        }

        T ui = GameManager.Pool.GetUI(popUpui);
        ui.transform.SetParent(popUpCanvas.transform, false);

        popUpStack.Push(ui);
        isPopUpOpened = true;
        Time.timeScale = 0;

        return ui;
    }

    public T ShowPopUpUI<T>(string path) where T : PopUpUI
    {
        T ui = GameManager.Resource.Load<T>(path);
        return ShowPopUpUI(ui);
    }

    public void ClosePopUpUI()
    {
        PopUpUI ui = popUpStack.Pop();
        isPopUpOpened = false;
        GameManager.Pool.Release(ui.gameObject);
        if (popUpStack.Count > 0)
        {
            PopUpUI curUI = popUpStack.Peek();
            curUI.gameObject.SetActive(true);
        }
        if (popUpStack.Count == 0)
        {
            Time.timeScale = 1;
        }
    }
}
