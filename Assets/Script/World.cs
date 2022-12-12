using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : Singleton<World>
{
    public Dictionary<int, ObjectBase> m_insDic = new Dictionary<int, ObjectBase>();
    public HostPlayer m_player;
    private GameObject npcroot;
    public Camera m_main;

    public float xlength;
    public float ylength;

    public void Init()
    {
        GameObject plan = GameObject.Find("Plane");
        Vector3 length = plan.GetComponent<MeshFilter>().mesh.bounds.size;
        xlength = length.x * plan.transform.lossyScale.x;
        ylength = length.x * plan.transform.lossyScale.z;
        Debug.Log($"地图的尺寸为  x:{xlength}  y:{ylength}");

        m_main = GameObject.Find("Main Camera").GetComponent<Camera>();
        npcroot = GameObject.Find("NPC_Root");
        UIMgr.Ins.Init(GameObject.Find("UIRoot"), GameObject.Find("HUD"));

        player_info info = new player_info();
        info.ID = 0;
        info.m_name = "tony";
        info.m_level = 9;
        info.m_pos = Vector3.zero;
        info.m_res = "Teddy";
        info.m_HP = 2000;
        info.m_MP = 1000;
        info.m_hpMax = 2000;
        info.m_mpMax = 2000;

        m_player = new HostPlayer(info);
        m_player.CreateObj(MonsterType.Null);
        JoyStickMgr.Ins.SetJoyArg(m_main, m_player);
        JoyStickMgr.Ins.JoyActive = true;

        CreatIns();
        MsgCenter.Ins.AddListener("AutoMove", (notify) =>
         {
             
         });
    }
    private void CreatIns()
    {
        JsonData data = MonsterCfg.Instance.GetJsonDate();
        object_info info;
        for (int i = 0; i < data.datas.Count; i++)
        {
            info = new object_info();
            info.ID = m_insDic.Count + 1;
            info.m_name = string.Format("{0}({1})", data.datas[i].name, info.ID);
            info.m_res = data.datas[i].name;
            info.m_pos = new Vector3(data.datas[i].x, data.datas[i].y, data.datas[i].z);
            info.m_type = data.datas[i].type;
            CreatObj(info);
        }
    }
    ObjectBase monster = null;
    private void CreatObj(object_info info)
    {
        monster = null;
        if (info !=null)
        {
            if (info.m_type == MonsterType.Normal)
            {
                monster = new Normal(info);
            }
            else if (info.m_type == MonsterType.Gather)
            {
                monster = new Gather(info);
            }
            else if (info.m_type == MonsterType.NPC)
            {
                monster = new NPCObj(1, info);
            }
        }
        if (monster != null)
        {
            monster.CreateObj(info.m_type);
            monster.m_go.transform.SetParent(npcroot.transform, false);
            m_insDic.Add(info.ID, monster);
        }
        else
        {
            Debug.Log("生成失败!!!!");
        }
    }
    public void AutoMoveByInsId(int target,Vector3 pos)
    {
        using (var tmp = m_insDic.GetEnumerator())
        {
            while(tmp.MoveNext())
            {
                if (target == tmp.Current.Key)
                {
                    tmp.Current.Value.AutoMove(pos, pos);
                }
            }
        }
    }
}
