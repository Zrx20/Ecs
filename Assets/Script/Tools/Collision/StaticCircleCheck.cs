using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaticCircleCheck : MonoBehaviour
{
    public float m_checkRangeActive = 3f;
    public float m_tirggerRange = 0.2f;
    public GameObject m_targer;
    public Action<bool> m_call;
    public bool m_isTirgger = true;
    private CricleCollision cricle1;
    private CricleCollision cricle2;
    public void Start()
    {
        cricle1 = new CricleCollision(this.transform.position.x, this.transform.position.z, m_tirggerRange);
        cricle2 = new CricleCollision(0, 0, m_tirggerRange);
    }
    private void Update()
    {
        if (m_targer)
        {
            if (Vector3.Distance(this.transform.position,m_targer.transform.position)<=m_checkRangeActive)
            {
                cricle2.RefreshPos(m_targer.transform.position.x, m_targer.transform.position.z);
                if (CircleCollision.CricleCollisionCheck(cricle1,cricle2))
                {
                    if (m_isTirgger)
                    {
                        m_call(true);
                        m_isTirgger = false;
                    }
                }
                else
                {
                    if (!m_isTirgger)
                    {
                        m_isTirgger = true;
                    }
                }
            }
        }
    }
}
