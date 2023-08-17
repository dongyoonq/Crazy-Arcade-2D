using MySql.Data.MySqlClient;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyPage : MonoBehaviour
{
    [SerializeField] TMP_Text id1Text;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text id2Text;
    [SerializeField] TMP_Text expPerText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text infoText;
    
    [SerializeField] TMP_Text recordText;
    [SerializeField] TMP_Text oddText;
    [SerializeField] TMP_Text expText;

    // private MySqlConnection connection;
    private MySqlDataReader reader;

    private void OnEnable()
    {
        if (!GameManager.Data.reader.IsClosed)
            GameManager.Data.reader.Close();

        GameManager.Data.ConnectDataBase();

        string id = PhotonNetwork.LocalPlayer.NickName;

        // reader = GameManager.Data.reader;

        string sqlCommand = string.Format("SELECT Id, Level, Exp, ExpMax, Money, Win, Draw, Lose FROM user_info WHERE Id ='{0}'", id);
        MySqlCommand cmd = new MySqlCommand(sqlCommand, GameManager.Data.Connection);
        GameManager.Data.reader = cmd.ExecuteReader();

        if (GameManager.Data.reader.HasRows)
        {
            while (GameManager.Data.reader.Read())
            {
                string readID = GameManager.Data.reader["Id"].ToString();
                string readLevel = GameManager.Data.reader["Level"].ToString();
                string readExp = GameManager.Data.reader["Exp"].ToString();
                string readExpMax = GameManager.Data.reader["ExpMax"].ToString();
                string readMoney = GameManager.Data.reader["Money"].ToString();
                string readWin = GameManager.Data.reader["Win"].ToString();
                int readWin2 = int.Parse(readWin);
                string readDraw = GameManager.Data.reader["Draw"].ToString();
                int readDraw2 = int.Parse(readDraw);
                string readLose = GameManager.Data.reader["Lose"].ToString();
                int readLose2 = int.Parse(readLose);

                float sum = readWin2 + readDraw2 + readLose2;
                float avg = (readWin2 / sum) * 100.0f;

                float exp2 = float.Parse(readExp);
                float expMax2 = float.Parse(readExpMax);

                float expPer = (exp2 / expMax2) * 100.0f;

                id1Text.text = readID;
                moneyText.text = readMoney;
                id2Text.text = readID;
                levelText.text = readLevel;

                recordText.text = $"{readWin}½Â {readDraw}¹« {readLose}ÆÐ";
                oddText.text = $"Á¾ÇÕ½Â·ü : {avg}%";
                expPerText.text = $"{expPer}%";
                expText.text = $"{readExp} / {readExpMax}";
            }

            if (!GameManager.Data.reader.IsClosed)
                GameManager.Data.reader.Close();
        }
    }

    public void OnButtonClicked()
    {
        GameManager.Sound.Onclick();
    }
}
