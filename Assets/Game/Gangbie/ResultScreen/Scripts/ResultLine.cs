using MySql.Data.MySqlClient;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultLine : MonoBehaviour
{
    private Image rankImg;

    [SerializeField] Image winImg;
    [SerializeField] Image loseImg;
    private Image resultImg;

    [SerializeField] TMP_Text idText;
    [SerializeField] TMP_Text expText;
    [SerializeField] TMP_Text moneyText;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();

        GameManager.Data.ConnectDataBase();

        string id = PhotonNetwork.LocalPlayer.NickName;

        string sqlCommand = string.Format("SELECT Id, Exp, ExpMax, FROM user_info WHERE Id ='{0}'", id);
        MySqlCommand cmd = new MySqlCommand(sqlCommand, GameManager.Data.Connection);
        GameManager.Data.reader = cmd.ExecuteReader();

        if (GameManager.Data.reader.HasRows)
        {
            while (GameManager.Data.reader.Read())
            {
                string readID = GameManager.Data.reader["Id"].ToString();
                string readExp = GameManager.Data.reader["Exp"].ToString();
                string readExpMax = GameManager.Data.reader["ExpMax"].ToString();

                float exp2 = float.Parse(readExp); // + expGet
                float expMax2 = float.Parse(readExpMax);

                // if (exp2 >= expMax2)
                // {
                //     exp2 = exp2 - expMax2;
                //     expMax2 *= 1.1f;
                //     // �����ͺ��̽� ���� or ���ʿ� �̰� ������ �� ���� ���� �Ǻ��� �� �����ͺ��̽� ���� ���
                // }
                slider.maxValue = expMax2;
                slider.value = exp2;

                float expPer = (exp2 / expMax2) * 100.0f;

                idText.text = readID;
                // �Ӵϴ� ���Ӵ� ȹ���� �Ӵ�
                // moneyText.text = 

                expText.text = $"{expPer}%";
            }
        }

        // rankImg�� �׳� Text�� �ص� �ɵ�

        // if �̰����� resultImg = winImg; else loseImg
    }
}
