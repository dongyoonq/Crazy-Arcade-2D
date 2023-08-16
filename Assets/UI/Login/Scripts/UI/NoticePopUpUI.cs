using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoticePopUpUI : PopUpUI
{
    [SerializeField] public TMP_Text notice;

    protected override void Awake()
    {
        base.Awake();

        buttons["OkButton"].onClick.AddListener(() => { GameManager.Sound.Onclick(); GameManager.UI.ClosePopUpUI(); });
    }
}
