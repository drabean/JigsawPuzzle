using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileMovement : MonoBehaviour, Object_Interactive
{
    public Tile tile { get; set; }
    SpriteRenderer sp;

    public bool canInteract;

    float answerDist = 100;

    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();

        SortManager.Inst.sps.Add(sp);
    }
    private void OnDestroy()
    {
        if(SortManager.Inst != null) SortManager.Inst.sps.Remove(sp);
    }
    private Vector3 GetCorrectPosition()
    {
        return new Vector3(tile.xIndex * Tile.TileSize, tile.yIndex * Tile.TileSize, 0.0f);
    }

    #region 이동

    /// <summary>
    /// 목적지와 이동시간을 받아, 이동시간에 걸쳐 목적지로 이동.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="time"></param>
    public void Move_Time(Vector3 destination, float time)
    {
        float speed = Vector3.Distance(destination, transform.position) / time;
        StartCoroutine(CO_Move(destination, speed));
    }
    
    public IEnumerator CO_Move(Vector3 destination, float speed)
    {
        while (transform.position != destination)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
    }
    #endregion


    #region Object_Interactive
    private Vector3 mOffset = new Vector3(0.0f, 0.0f, 0.0f);

    public void onTouchDown(Vector3 touchPos)
    {
        if (!canInteract) return;

        mOffset = transform.position - touchPos;
        SortManager.Inst.sort(sp);
    }

    public void onTouchDrag(Vector3 touchPos)
    {
        if (!canInteract) return;

        transform.position = touchPos + mOffset;
    }

    public void onTouchUp(Vector3 touchPos)
    {
        if (!canInteract) return;

        float dist = (transform.position - GetCorrectPosition()).magnitude;
        if (dist < answerDist)
        {
            transform.position = GetCorrectPosition();
            canInteract = false;
            SortManager.Inst.sps.Remove(sp);

            sp.sortingOrder = 0;

            if (SortManager.Inst.sps.Count == 0) GameManager.Inst.StartDraw();
            Destroy(GetComponent<BoxCollider2D>());
            Destroy(this);
        }
    }

    #endregion
}