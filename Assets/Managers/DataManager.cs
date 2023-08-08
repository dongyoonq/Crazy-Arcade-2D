using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public MySqlConnection Connection { get; private set; }
    public MySqlDataReader reader;


    private void Awake()
    {
        ConnectDataBase();
    }

    public void ConnectDataBase()
    {
        try
        {
            string serverInfo = "Server=127.0.0.1; Database=userdata; Uid=root; Pwd=1234; Port=3306; CharSet=utf8;";
            Connection = new MySqlConnection(serverInfo);
            Connection.Open();

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
}
