using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObj : ObjectBase
{
    public player_info m_info;
    public PlayerObj(player_info info)
    {
        m_info = info;
    }
    public override void SetPos(Vector3 pos)
    {
        base.SetPos(pos);
    }
    public void SetPos(Vector3 pos,float speed)
    {

    }
    public override void OnCreate()
    {
        base.OnCreate();
        //´´½¨UIµÄ
        m_pate = m_go.AddComponent<UIPate>();
        m_pate.InitPate();
        m_pate.m_gather.SetActive(false);
        m_pate.SetData(m_info.m_name, m_info.m_HP / m_info.m_hpMax, m_info.m_MP / m_info.m_mpMax);
    }
    public void AddBuff(string path)
    {

    }
}
public class HostPlayer : PlayerObj
{
    Player player;
    public HostPlayer(player_info info):base(info)
    {
        m_insID = info.ID;
        m_moderlPath = info.m_res;
    }
    public override void CreateObj(MonsterType type)
    {
        SetPos(m_info.m_pos);
        base.CreateObj(type);
    }
    public override void OnCreate()
    {
        base.OnCreate();
        player = m_go.AddComponent<Player>();
        player.InitData();

        MsgCenter.Ins.AddListener("atkActionPlay", (notify) =>
         {
             if (notify.msg.Equals("ByServer"))
             {
                 int skillid = (int)notify.data[0];
                 player.SetData(skillid.ToString());
                 player.play();
             }
         });
    }
    Notification notify = new Notification();
    public void JoystickHandlerMoving(float h,float v)
    {
        if (Mathf.Abs(h) > 0.05f || (Mathf.Abs(v) > 0.05f))
        {
            MoveByTranslate(new Vector3(m_go.transform.position.x + h, m_go.transform.position.y, m_go.transform.position.z + v), Vector3.forward * Time.deltaTime * 1);
            notify.Refresh("Player", m_go.transform.position);
            MsgCenter.Ins.SendMsg("MovePos", notify);
        }
    }
    public void JoyButtonHandler(string btnName)
    {
        //List<SkillBase> componentList;
        switch (btnName)
        {
            case "attack":
                Notification m_notify = new Notification();
                m_notify.Refresh("atkOther", 1, 2, 1);
                MsgCenter.Ins.SendMsg("ByClent_Battle", m_notify);
                break;
        }
    }
}
public class OtherPlayer : PlayerObj
{
    public OtherPlayer(player_info info):base(info)
    {
        m_insID = info.ID;
        m_moderlPath = info.m_res;
    }
    public override void OnCreate()
    {
        SetPos(m_info.m_pos);
        base.OnCreate();
    }
}