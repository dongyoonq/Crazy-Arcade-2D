using Gangbie;
using MySql.Data.MySqlClient;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using System;
using TMPro;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [SerializeField] NoticePopUpUI noticePopUp;

    [SerializeField] GameObject chatManager;
    [SerializeField] GameObject chatingArea;
    [SerializeField] GameObject loginPanel;

    [SerializeField] GameObject chatingView;
    [SerializeField] GameObject chatingHide;

    [SerializeField] GameObject speakerPopUp;

    [SerializeField] KDY.LobbyPanel lobbyPanel;

    [SerializeField] LobbyManager lobbyManager;

    private void OnEnable()
    {
        noticePopUp.gameObject.SetActive(false);
    }

    private void Start()
    {
        GameManager.Data.ConnectDataBase();
    }

    public void OnSignUpButtonClicked()
    {
        GameManager.Sound.Onclick();
        try
        {
            string id = idInputField.text;
            string password = passwordInputField.text;

            if (string.IsNullOrEmpty(id))
            {
                noticePopUp.notice.text = "아이디를 입력해주세요.";
                GameManager.UI.popUpStack.Push(noticePopUp);
                noticePopUp.gameObject.SetActive(true);
                return;
            }

            string sqlCommand = string.Format("SELECT ID FROM user_info WHERE ID ='{0}'", id);

            MySqlCommand cmd = new MySqlCommand(sqlCommand, GameManager.Data.Connection);
            GameManager.Data.reader = cmd.ExecuteReader();

            if (GameManager.Data.reader.HasRows)
            {
                Debug.Log("ID is already exist");

                noticePopUp.notice.text = "해당 아이디는 이미 사용중입니다.";
                GameManager.UI.popUpStack.Push(noticePopUp);
                noticePopUp.gameObject.SetActive(true);

                if (!GameManager.Data.reader.IsClosed)
                    GameManager.Data.reader.Close();

                return;
            }
            else
            {
                if (!GameManager.Data.reader.IsClosed)
                    GameManager.Data.reader.Close();

                sqlCommand = string.Format("INSERT INTO user_info(Id, Password, Exp, Money) VALUES ('{0}', '{1}', '{2}', '{3}');", id, password, 0f, 0f);

                cmd = new MySqlCommand(sqlCommand, GameManager.Data.Connection);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    Debug.Log("Success");
                    noticePopUp.notice.text = "회원가입이 완료되었습니다.\n로그인 후 이용하세요.";
                    noticePopUp.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Fail");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void OnLoginButtonClicked()
    {
        GameManager.Sound.Onclick();
        try
        {
            string id = idInputField.text;
            string password = passwordInputField.text;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                noticePopUp.notice.text = "아이디와 비밀번호를 확인해주세요.";
                GameManager.UI.popUpStack.Push(noticePopUp);
                noticePopUp.gameObject.SetActive(true);
                return;
            }

            string sqlCommand = string.Format("SELECT ID,Password FROM user_info WHERE ID ='{0}'", id);
            MySqlCommand cmd = new MySqlCommand(sqlCommand, GameManager.Data.Connection);
            GameManager.Data.reader = cmd.ExecuteReader();

            if (GameManager.Data.reader.HasRows)
            {
                while (GameManager.Data.reader.Read())
                {
                    string readID = GameManager.Data.reader["ID"].ToString();
                    string readPassword = GameManager.Data.reader["Password"].ToString();

                    Debug.Log($"Id : {readID}, Pass : {readPassword}");

                    if (password == readPassword)
                    {
                        idInputField.text = "";
                        passwordInputField.text = "";

                        PhotonNetwork.LocalPlayer.NickName = id;
                        PhotonNetwork.ConnectUsingSettings();

                        ActiveChatManager();

                        lobbyManager.SetActivePanel(LobbyManager.Panel.Lobby);

                        if (!GameManager.Data.reader.IsClosed)
                            GameManager.Data.reader.Close();
                        return;
                    }
                    else
                    {
                        Debug.Log("Wrong Password");
                        noticePopUp.notice.text = "아이디와 비밀번호를 확인해주세요.";
                        GameManager.UI.popUpStack.Push(noticePopUp);
                        noticePopUp.gameObject.SetActive(true);
                        // GameManager.UI.ShowPopUpUI<PopUpUI>("UI/NoticePopUp");

                        if (!GameManager.Data.reader.IsClosed)
                            GameManager.Data.reader.Close();
                        return;
                    }
                }
            }
            else
            {
                Debug.Log("There is no player id");
            }

            if (!GameManager.Data.reader.IsClosed)
                GameManager.Data.reader.Close();
        }
        
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void ActiveChatManager()
    {
        chatManager.SetActive(true);
        chatingArea.SetActive(true);
    }

    public void Quit()
    {
        GameManager.Sound.Onclick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
#else
            Application.Quit();
#endif
    }

    private bool chatingHideButtonCheck = true;

    public void OnUpButtonClicked()
    {
        GameManager.Sound.Onclick();
        if (chatingHideButtonCheck == false)
        {
            chatingHide.transform.Translate(Vector3.up * 330);
            chatingView.SetActive(true);
        }
        chatingHideButtonCheck = true;
    }

    public void OnDownButtonClicked()
    {
        GameManager.Sound.Onclick();
        if (chatingHideButtonCheck == true)
        {
            chatingHide.transform.Translate(Vector3.down * 330);
            chatingView.SetActive(false);
        }
        chatingHideButtonCheck = false;
    }

    public void OnSpeakerButtonClicked()
    {
        GameManager.Sound.Onclick();
        if (speakerPopUp.active == false)
        {
            speakerPopUp.SetActive(true);
        }
    }
}