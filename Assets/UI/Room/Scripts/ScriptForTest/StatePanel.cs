using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace RoomUI.ScriptForTest
{
    public class StatePanel : MonoBehaviour
    {
        [SerializeField] RectTransform content;
        [SerializeField] TMP_Text logPrefab;

        private ClientState state;

        void Update()
        {
            if (state == PhotonNetwork.NetworkClientState)
                return;

            state = PhotonNetwork.NetworkClientState;

            TMP_Text newLog = Instantiate(logPrefab, content);
            newLog.text = string.Format("[Photon] {0} : {1}", System.DateTime.Now.ToString("HH:mm:ss.ff"), state);
            Debug.Log(string.Format("[Photon] {0}", state));
        }

        public void AddMessage(string message)
        {
            TMP_Text newLog = Instantiate(logPrefab, content);
            newLog.text = string.Format("[Photon] {0} : {1}", System.DateTime.Now.ToString("HH:mm:ss.ff"), message);
            Debug.Log(string.Format("[Photon] {0}", message));
        }
    }
}