using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutBox_Vertex : MonoBehaviour, Object_Interactive
{
    Vector3 mOffset = Vector3.zero;

    public CutBox_Vertex other;
    public CutBox_Center center;
    public bool isMax;

    Vector3[] limits = new Vector3[2];

    float ratio = 1.0f;

    static float minimumSize = 5f;
    public void onTouchDown(Vector3 touchPos)
    {
        mOffset = transform.position - touchPos;
    }

    public void onTouchDrag(Vector3 touchPos)
    {
        Vector2 targetPos = touchPos + mOffset;

        //x축을 기준으로, 비율에 맞춰서 y축 위치 재조정
        targetPos = Vector2.right * targetPos.x + (other.transform.position.y + (targetPos.x - other.transform.position.x) * ratio) * Vector2.up;

        if (checkCanMove(targetPos))
        {
            transform.position = targetPos;
            center.setBoxCol();
        }
    }

    public void onTouchUp(Vector3 touchPos) { }

    //움직이고 싶은 위치인 targetPos로 이 꼭짓점이 움직여도 되는지 검사.
    bool checkCanMove(Vector3 targetPos)
    {
        //사진 범위 밖으로 나가는지 확인
        if (targetPos.x < limits[0].x || targetPos.x > limits[1].x || targetPos.y < limits[0].y || targetPos.y > limits[1].y) return false;

        //최대가 최소보다 오른쪽 / 위에 있는지 검사
        if (isMax)
        {
            if (targetPos.y < other.transform.position.y) return false;
            if (targetPos.x < other.transform.position.x) return false;
        }
        else
        {
            if (targetPos.y > other.transform.position.y) return false;
            if (targetPos.x > other.transform.position.x) return false;
        }
        //박스가 최소한 일정 크기 이상으로 크도록 조정

        if (Vector2.Distance(targetPos, other.transform.position) < minimumSize) return false;

        return true;



    }

    /// <summary>
    /// 이 vertex가 움직일수 있는 한계범위 설정
    /// </summary>
    /// <param name="limits"></param>
    public void setLimit(Vector3[] limits)
    {
        this.limits[0] = limits[0];
        this.limits[1] = limits[1];
    }
    /// <summary>
    /// 이 vertex의 가로/세로 범위 설정
    /// </summary>
    /// <param name="ratio"></param>
    public void setRatio(float ratio)
    {
        this.ratio = ratio;
    }


    //사진의 회전 등으로 인하여 limit 재설정 해야할때, 재설정 하면서 화면 밖으로 나간 Vertex의 위치 조정.
    public void initpos()
    {
        float targetX;
        float targetY;

        if (transform.position.x > limits[1].x)
        {
            targetX = limits[1].x;
        }
        else if (transform.position.x < limits[0].x)
        {
            targetX = limits[0].x;
        }
        else
        {
            targetX = transform.position.x;
        }

        if (transform.position.y > limits[1].y)
        {
            targetY = limits[1].y;
        }
        else if (transform.position.y < limits[0].y)
        {
            targetY = limits[0].y;
        }
        else
        {
            targetY = transform.position.y;
        }

        //자기자신의 위치를 화면 안으로 조정
        transform.position = Vector2.right * targetX + Vector2.up * targetY;

        //다른 정점의 위치를 비율에 맞추어 조절.(반대쪽 정점이 사진 밖으로 나가는 경우가 없도록, x축을 두고 y축을 줄이는 방식으로 이동한다.
        other.transform.position = Vector2.right * (other.transform.position.x) + Vector2.up * (transform.position.y + (other.transform.position.x - transform.position.x) * ratio);
        
    }
}