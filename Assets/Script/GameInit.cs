using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInit : MonoBehaviour
{
    public GameObject[] DontDestory;
    public List<ETCButton> Attack;
    public ETCJoystick Joystick;
    public GameObject uiroot;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < DontDestory.Length; i++)
        {
            GameObject.DontDestroyOnLoad(DontDestory[i]);
        }
        GameSceneUtils.LoadSceneAsync("Lobby", () =>
        {
            JoyStickMgr.Ins.m_joyGO = DontDestory[0];
            JoyStickMgr.Ins.m_joystick = Joystick;
            JoyStickMgr.Ins.m_skillBtn = Attack;

            GameData.Ins.InitByRoleName("Teddy");
            GameData.Ins.InitTaskData();

            World.Ins.Init();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
