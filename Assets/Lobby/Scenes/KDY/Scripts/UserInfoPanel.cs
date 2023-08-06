using MySql.Data.MySqlClient;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoPanel : MonoBehaviour
{
    [SerializeField] TMP_Text userName;
    [SerializeField] TMP_Text userLevel;
    [SerializeField] Button okButton;

    private MySqlConnection connection;
    private MySqlDataReader reader;

    private void OnEnable()
    {
        okButton.onClick.AddListener(() => Destroy(gameObject));
    }

    public void ConnectDataBase()
    {
        try
        {
            string serverInfo = "Server=127.0.0.1; Database=userdata; Uid=root; Pwd=1234; Port=3306; CharSet=utf8;";
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

    public void ReadSqlData(string username)
    {
        string sqlCommand = string.Format("SELECT Level FROM user_info WHERE ID ='{0}'", username);
        userName.text = username;

        MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);
        reader = cmd.ExecuteReader();

        if (reader.HasRows)
        {
            while (reader.Read())
            {
                userLevel.text = $"Level : {reader["Level"]}";
            }

            if (!reader.IsClosed)
                reader.Close();

            return;
        }
    }
}
