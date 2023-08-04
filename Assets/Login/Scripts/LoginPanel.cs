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

    [SerializeField] GameObject noticePopUp;
    [SerializeField] GameObject okPopUp;
    [SerializeField] GameObject checkIdPopUp;
    [SerializeField] GameObject enterIdPopUp;

    [SerializeField] GameObject chatManager;
    [SerializeField] GameObject chatingArea;
    [SerializeField] GameObject loginPanel;

    [SerializeField] GameObject chatingView;
    [SerializeField] GameObject chatingHide;

    [SerializeField] GameObject speakerPopUp;

    private MySqlConnection connection;
    private MySqlDataReader reader;

    private void OnEnable()
    {
        noticePopUp.SetActive(false);
        okPopUp.SetActive(false);
        checkIdPopUp.SetActive(false);
        enterIdPopUp.SetActive(false);
        speakerPopUp.SetActive(false);
    }

    private void Start()
    {
        ConnectDataBase();
    }

    private void ConnectDataBase()
    {
        try
        {
            string serverInfo = "Server=127.0.0.1; Database=crazyarcade; Uid=root; Pwd=pkb7018; Port=3306; CharSet=utf8;";
            connection = new MySqlConnection(serverInfo);
            connection.Open();

            Debug.Log("DataBase Connect Success");
        }
        catch (InvalidCastException e)
        {
            Debug.Log(e.Message);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void OnSignUpButtonClicked()
    {
        try
        {
            string id = idInputField.text;
            string password = passwordInputField.text;

            if (string.IsNullOrEmpty(id))
            {
                enterIdPopUp.SetActive(true);
                return;
            }

            string sqlCommand = string.Format("SELECT ID FROM user_info WHERE ID ='{0}'", id);

            MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                Debug.Log("ID is already exist");

                noticePopUp.SetActive(true);

                if (!reader.IsClosed)
                    reader.Close();

                return;
            }
            else
            {
                if (!reader.IsClosed)
                    reader.Close();

                sqlCommand = string.Format("INSERT INTO user_info(Id, Password, Exp, Money) VALUES ('{0}', '{1}', '{2}', '{3}');", id, password, 0f, 0f);

                cmd = new MySqlCommand(sqlCommand, connection);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    Debug.Log("Success");
                    okPopUp.SetActive(true);
                    //OnLoginButtonClicked();
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
        try
        {
            string id = idInputField.text;
            string password = passwordInputField.text;

            if (string.IsNullOrEmpty(id))
            {
                checkIdPopUp.SetActive(true);
                return;
            }

            string sqlCommand = string.Format("SELECT ID,Password FROM user_info WHERE ID ='{0}'", id);
            MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string readID = reader["ID"].ToString();
                    string readPassword = reader["Password"].ToString();

                    Debug.Log($"Id : {readID}, Pass : {readPassword}");

                    if (password == readPassword)
                    {
                        PhotonNetwork.LocalPlayer.NickName = id;
                        PhotonNetwork.ConnectUsingSettings();

                        ActiveChatManager();

                        if (!reader.IsClosed)
                            reader.Close();
                        return;
                    }
                    else
                    {
                        Debug.Log("Wrong Password");
                        checkIdPopUp.SetActive(true);
                        if (!reader.IsClosed)
                            reader.Close();
                        return;
                    }
                }
            }
            else
            {
                Debug.Log("There is no player id");
            }
            if (!reader.IsClosed)
                reader.Close();
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
        loginPanel.SetActive(false);
    }

    public void Quit()
    {
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
        if (chatingHideButtonCheck == false)
        {
            chatingHide.transform.Translate(Vector3.up * 330);
            chatingView.SetActive(true);
        }
        chatingHideButtonCheck = true;
    }

    public void OnDownButtonClicked()
    {
        if (chatingHideButtonCheck == true)
        {
            chatingHide.transform.Translate(Vector3.down * 330);
            chatingView.SetActive(false);
        }
        chatingHideButtonCheck = false;
    }

    public void OnSpeakerButtonClicked()
    {
        if (speakerPopUp.active == false)
        {
            speakerPopUp.SetActive(true);
        }
    }
}