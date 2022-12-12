using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    public float xMap, yMap;
    public float xoffset, yoffset;

    private Transform player;
    Dictionary<MonsterType, Transform> monsterdic = new Dictionary<MonsterType, Transform>();

    List<ObjectBase> otherGoPos = new List<ObjectBase>();
    Vector3 playerpos = new Vector3(0, 0, 0);
    List<Vector3> otherPos = new List<Vector3>();

    private void Awake()
    {
        xMap = this.gameObject.GetComponent<RectTransform>().sizeDelta.x;
        yMap = this.gameObject.GetComponent<RectTransform>().sizeDelta.y;
        xoffset = xMap / World.Ins.xlength;
        yoffset = yMap / World.Ins.ylength;

        player = transform.Find("player");
        monsterdic.Add(MonsterType.Gather, transform.Find("gather"));
        monsterdic.Add(MonsterType.Normal, transform.Find("monster"));
        monsterdic.Add(MonsterType.NPC, transform.Find("npc"));
    }
    private void Update()
    {
        if (World.Ins.m_insDic.Count != otherGoPos.Count)
        {
            otherGoPos.Clear();
            otherPos.Clear();
            foreach (var item in World.Ins.m_insDic)
            {
                otherGoPos.Add(item.Value);
                otherPos.Add(new Vector3(0, 0, 0));
            }
        }
        if (player && World.Ins.m_player.m_go)
        {
            playerpos.Set(World.Ins.m_player.m_go.transform.position.x * xoffset, World.Ins.m_player.m_go.transform.position.z * yoffset, 0);
            player.localPosition = playerpos;
        }
        if (otherGoPos!=null && otherGoPos.Count > 0)
        {
            for (int i = 0; i < otherGoPos.Count; i++)
            {
                otherPos[i] = new Vector3(otherGoPos[i].m_go.transform.position.x * xoffset, otherGoPos[i].m_go.transform.position.z * yoffset, 0);
                monsterdic[otherGoPos[i].m_type].transform.localPosition = otherPos[i];
            }
        }
    }
    private void OnDestroy()
    {
        CancelInvoke("UpdateMap");
    }
}
