using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalProps
{
    public static Dictionary<long, SPlayer> players = new Dictionary<long, SPlayer>();
}
public class ServerInit : MonoBehaviour
{
    public Vector3 m_playerPos;
    public Dictionary<int, Vector3> m_otherPosDic;
    public void Awake()
    {
        MsgCenter.Ins.AddListener("MovePos", (notify) =>
         {
             if (notify.msg.Equals("Player"))
             {
                 m_playerPos = (Vector3)notify.data[0];
             }
             else if (notify.msg.Equals("Other"))
             {
                 if (m_otherPosDic == null)
                 {
                     m_otherPosDic = new Dictionary<int, Vector3>();
                 }
                 int insid = (int)notify.data[0];
                 Vector3 pos = (Vector3)notify.data[1];
                 if (!m_otherPosDic.ContainsKey(insid))
                 {
                     m_otherPosDic.Add(insid, pos);
                 }
                 else
                 {
                     m_otherPosDic[insid] = pos;
                 }
             }
         });

        MsgCenter.Ins.AddListener("ServerMsg", (notify) =>
         {
             if (notify.msg.Equals("gather"))
             {
                 Debug.Log($"点击采集按钮  Insid : {(int)notify.data[0]}");
                 int insid = (int)notify.data[0];

                 notify.Refresh("gather_callback", insid, 2);
                 MsgCenter.Ins.SendMsg("ServerMsg", notify);

                 MsgCenter.Ins.SendMsg("GatherAction", notify);
             }
             if (notify.msg.Equals("AcceptTask"))
             {
                 int taskid = (int)notify.data[0];
                 foreach (var item in LocalProps.players)
                 {
                     if (item.Key == 1)
                     {
                         item.Value.components.Add(ComPonentType.task, new TaskComponent());
                         item.Value.components[ComPonentType.task].Init();
                     }
                 }
             }
         });

        SPlayer splayer = new SPlayer();
        splayer.InitPlayer();
        splayer.m_insid = 1;
        splayer.Hp = 100;
        splayer.components.Add(ComPonentType.battle, new BattleComponent());
        LocalProps.players.Add(splayer.m_insid, splayer);
        if (LocalProps.players == null) return;
        foreach (var item in LocalProps.players)      
        {
            foreach (var item1 in item.Value.components)
            {
                item1.Value.GetPlayerById = GetPlayer;
                item1.Value.Init();
            }
        }
        
    }
    private SPlayer GetPlayer(long id)
    {
        using (var tmp = LocalProps.players.GetEnumerator())
        {
            while (tmp.MoveNext())
            {
                if (tmp.Current.Key == id)
                {
                    return tmp.Current.Value;
                }
            }
            return null;
        }
        
    }
}
public enum ComPonentType:byte
{
    nil = 0,
    task,
    battle,
}
public class SkillProp
{
    public float range;
}
public class SPlayer
{
    public long m_insid;
    public Vector3 m_pos;
    public float Hp;
    public float Mp;
    public float Atk;
    public List<int> buffs;
    public List<SkillProp> skills;
    public Dictionary<ComPonentType, SComponent> components;

    public void InitPlayer()
    {
        buffs = new List<int>();
        skills = new List<SkillProp>();
        components = new Dictionary<ComPonentType, SComponent>();
    }
    public void PropOperation(int type,float value)
    {
        switch (type)
        {
            case 1:
                Hp += value;
                break;
            case 2:
                Hp += value;
                break;
        }
        Notification m_notify = new Notification();
        m_notify.Refresh("ByServer", type, value);
        MsgCenter.Ins.SendMsg("propchange", m_notify);
    }
}
public class SComponent
{
    public Func<long, SPlayer> GetPlayerById;
    Notification m_notify;
    public virtual void S2CMsg(string cmd,object value)
    {
        if (m_notify == null)
        {
            m_notify = new Notification();
        }
        m_notify.Refresh("ByServer", value);
        MsgCenter.Ins.SendMsg(cmd, m_notify);
        
    }
    public virtual void Init()
    {

    }
}
public class TaskComponent:SComponent
{
    public List<int> Tasks;
    public override void Init()
    {
        MsgCenter.Ins.AddListener("GatheraAction", (notify) =>
         {
             Debug.Log("处理采集进度");
         });
    }
}
public class BattleComponent:SComponent
{
    public override void Init()
    {
        MsgCenter.Ins.AddListener("ByClent_Battle", (notify) =>
         {
             if (notify.msg.Equals("atkOther"))
             {
                 int atkId = (int)notify.data[0];
                 int targetID = (int)notify.data[1];
                 int skillID = (int)notify.data[2];
                 AtkPlayer(atkId, targetID, skillID);
             }
         });
    }
    private void AtkPlayer(long atk,long target,int skillid)
    {
        SPlayer pl = GetPlayerById(atk);
        SPlayer p2 = GetPlayerById(target);
        S2CMsg("atkActionPlay", skillid);
    }
}