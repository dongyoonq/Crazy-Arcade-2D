using MySql.Data.MySqlClient;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultLine : MonoBehaviourPun
{
    private Image rankImg;

    [SerializeField] Sprite winImg;
    [SerializeField] Sprite loseImg;
    [SerializeField] Image resultImg;

    [SerializeField] TMP_Text idText;
    [SerializeField] TMP_Text expText;
    [SerializeField] TMP_Text moneyText;

    private Slider slider;

    public float prevMaxExp;
    private float exp;
    private float expMax;
    private float money;
    private int level;
    private int win;
    private int draw;
    private int lose;

    private string result;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
    }

    public void SetResultImg(string result)
    {
        this.result = result;
        resultImg.sprite = (result == "Win") ? winImg : loseImg;
    }

    public void SetID(string text)
    {
        idText.text = text;
    }

    public void SetInfo(string id)
    {
        GameManager.Data.ConnectDataBase();

        string sqlCommand = string.Format("SELECT Exp, ExpMax, Money, Level, Win, Draw, Lose FROM user_info WHERE Id ='{0}'", id);
        MySqlCommand cmd = new MySqlCommand(sqlCommand, GameManager.Data.Connection);
        GameManager.Data.reader = cmd.ExecuteReader();

        if (GameManager.Data.reader.HasRows)
        {
            while (GameManager.Data.reader.Read())
            {
                exp = float.Parse(GameManager.Data.reader["Exp"].ToString());
                expMax = float.Parse(GameManager.Data.reader["ExpMax"].ToString());
                prevMaxExp = expMax;
                money = float.Parse(GameManager.Data.reader["Money"].ToString());
                level = int.Parse(GameManager.Data.reader["Level"].ToString());
                win = int.Parse(GameManager.Data.reader["Win"].ToString());
                draw = int.Parse(GameManager.Data.reader["Draw"].ToString());
                lose = int.Parse(GameManager.Data.reader["Lose"].ToString());

                slider.maxValue = expMax;
                slider.value = exp;

                expText.text = $"{(exp / expMax) * 100.0f}%";
            }
        }

        if (!GameManager.Data.reader.IsClosed)
            GameManager.Data.reader.Close();
    }

    public void UpdateInfo(string id)
    {
        GameManager.Data.ConnectDataBase();

        if (result == "Win")
        {
            StartCoroutine(ApplyExpMoneyRoutine(300, 600, id));
            win += 1;
        }
        else if (result == "Lose")
        {
            StartCoroutine(ApplyExpMoneyRoutine(200, 400, id));
            lose += 1;
        }

    }

    IEnumerator ApplyExpMoneyRoutine(int dropExp, int money, string id)
    {
        this.money += money;
        moneyText.text = money.ToString();

        for (int i = 0; i < dropExp / 5; i++)
        {
            exp += 5;
            slider.value = exp;
            expText.text = $"{(Mathf.Round((exp / expMax) * 100) * 0.01f) * 100.0f}%";

            if (exp >= expMax)
            {
                level++;
                exp -= expMax;
                expMax *= 1.5f;
                slider.value = exp;
                slider.maxValue = expMax;
            }

            yield return new WaitForSeconds(0.00001f);
        }

        string sqlCommand = string.Format("UPDATE user_info SET Exp={0}, ExpMax={1}, Money={2}, Win={3}, Draw={4}, Lose={5}, Level={6} WHERE ID ='{7}'", exp, expMax, this.money, win, draw, lose, level, id);
        MySqlCommand cmd = new MySqlCommand(sqlCommand, GameManager.Data.Connection);
        GameManager.Data.reader = cmd.ExecuteReader();

        if (!GameManager.Data.reader.IsClosed)
            GameManager.Data.reader.Close();
    }
}
