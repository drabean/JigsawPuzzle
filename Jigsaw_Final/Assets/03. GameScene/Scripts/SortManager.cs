using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortManager : MonoBehaviour
{
    public static SortManager Inst;

   public  List<SpriteRenderer> sps;
    private void Awake()
    {
        Inst = this;
    }

    /// <summary>
    /// sp�� ����Ʈ�� �־��ٰ� �����μ�, sp�� ����Ʈ�� ������ ��ġ�� ���� �ڿ�, List������ index ������ sortingOrder�� ������.
    /// </summary>
    /// <param name="sp"></param>
    public void sort(SpriteRenderer sp)
    {
        sps.Remove(sp);
        sps.Add(sp);

        for(int i = 0; i < sps.Count; i++)
        {
            sps[i].sortingOrder = i+1;
        }
    }

}
