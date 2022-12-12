using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battlesys : UIbase
{
    private Button m_gatherBtn;
    private Slider m_gatherSlider;
    private int m_gatherInsid;
    private bool isok = false;
    private float ServerNum = 0;

    public override void DoCreate(string path)
    {
        base.DoCreate(path);
        
        MsgCenter.Ins.AddListener("ClientMsg", RefreshBtn);
        MsgCenter.Ins.AddListener("ServerMsg", ServerNotify);
    }
    public void ServerNotify(Notification obj)
    {
        if (obj.msg.Equals("gather_callback"))
        {
            ServerNum = (int)obj.data[1];
            isok = true;
            m_gatherSlider.gameObject.SetActive(true);
            //ServerNum -= Time.deltaTime;
            //if (ServerNum>=2)
            //{
            //    m_gatherSlider.value = ServerNum / 2;
            //}
        }
    }
    public override void Update()
    {
        base.Update();
        if (isok)
        {
            ServerNum -= Time.deltaTime;
            if (ServerNum >= 2)
            {
                m_gatherSlider.value = ServerNum / 2;
            }
        }
    }
    public override void DoShow(bool active)
    {
        base.DoShow(active);
        m_gatherBtn = m_go.transform.Find("gather_btn").GetComponent<Button>();
        m_gatherBtn.onClick.AddListener(() => {
            //½»»¥·þÎñÆ÷
            Notification notify = new Notification();
            notify.Refresh("gather", 1);
            MsgCenter.Ins.SendMsg("ServerMsg", notify);
        });
        m_gatherSlider = m_go.transform.Find("gather_slider").GetComponent<Slider>();
        m_gatherBtn.gameObject.SetActive(false);
        m_gatherSlider.gameObject.SetActive(false);
    }

    public override void Destory()
    {
        base.Destory();
        MsgCenter.Ins.RemoveListener("ClientMsg", RefreshBtn);
    }
    private void RefreshBtn(Notification notiy)
    {
        if (notiy.msg.Equals("gather_trigger"))
        {
            m_gatherInsid = (int)notiy.data[0];
            m_gatherBtn.gameObject.SetActive(true);
        }
    }
}

