using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private static PoolManager poolManager;
    private static ResourceManager resource;
    private static UIManager ui;
    private static SoundManager soundManager;
    private static DataManager dataManager;

    public static GameManager Instance { get { return instance; } }
    public static PoolManager Pool { get { return poolManager; } }
    public static ResourceManager Resource { get { return resource; } }
    public static UIManager UI { get { return ui; } }
    public static SoundManager Sound { get { return soundManager; } }
    public static DataManager Data { get { return dataManager; } }


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
        InitManagers();

    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private void InitManagers()
    {
        GameObject resourceObj = new GameObject();
        resourceObj.name = "ResourceManager";
        resourceObj.transform.parent = transform;
        resource = resourceObj.AddComponent<ResourceManager>();

        GameObject poolObj = new GameObject();
        poolObj.name = "PoolManager";
        poolObj.transform.parent = transform;
        poolManager = poolObj.AddComponent<PoolManager>();

        GameObject UIObj = new GameObject();
        UIObj.name = "UIManager";
        UIObj.transform.parent = transform;
        ui = UIObj.AddComponent<UIManager>();

        GameObject soundObj = new GameObject();
        soundObj.name = "SoundManager";
        soundObj.transform.parent = transform;
        soundManager = soundObj.AddComponent<SoundManager>();

        GameObject dataObj = new GameObject();
        dataObj.name = "DataManager";
        dataObj.transform.parent = transform;
        dataManager = dataObj.AddComponent<DataManager>();
    }
}
