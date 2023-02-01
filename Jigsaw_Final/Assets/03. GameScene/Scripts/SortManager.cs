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
    /// sp를 리스트에 넣었다가 뺌으로서, sp를 리스트의 마지막 위치로 넣은 뒤에, List에서의 index 순서로 sortingOrder를 정리함.
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
