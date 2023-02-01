using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutBox_Center : MonoBehaviour, Object_Interactive
{
    Vector3 mOffset = Vector3.zero;

    [SerializeField] Transform[] vertexTransform = new Transform[2];
    [SerializeField] BoxCollider2D col;

    //center에 대한 vertex들의 상대적인 위치.
    Vector2[] vertexOffset = new Vector2[2];

    //vertex들이 움직일 수 있는 제한범위 설정
    Vector3[] limits = new Vector3[2];


    public void onTouchDown(Vector3 touchPos)
    {
        mOffset = transform.position - touchPos;

        vertexOffset[0] = vertexTransform[0].position - transform.position;
        vertexOffset[1] = vertexTransform[1].position - transform.position;
    }

    public void onTouchDrag(Vector3 touchPos)
    {
        Vector2 targetPos = touchPos + mOffset;

        if (checkCanMove(targetPos))
        {
            transform.position = Vector3.right * targetPos.x + Vector3.up * targetPos.y + Vector3.forward;
            vertexTransform[0].position = targetPos + vertexOffset[0];
            vertexTransform[1].position = targetPos + vertexOffset[1];
        }
    }

    public void onTouchUp(Vector3 touchPos){}



    //각 vertex가 움직일수 있는 한계점 설정.
    public void setLimit(Vector3[] limits)
    {
        this.limits[0] = limits[0];
        this.limits[1] = limits[1];
    }

    /// <summary>
    /// drag를 통해 움직였을때, 이동한 꼭지점 위치에 따라 cutbox center의 collider 크기 및 위치 재조정
    /// </summary>
    public void setBoxCol()
    {
        transform.position = (vertexTransform[0].position + vertexTransform[1].position) / 2;
        col.size = Vector2.right * (vertexTransform[1].position.x - vertexTransform[0].position.x) + Vector2.up * (vertexTransform[1].position.y - vertexTransform[0].position.y);

    }
    bool checkCanMove(Vector2 targetPos)
    {
        Vector2 targetVec1Pos = targetPos + vertexOffset[0];

        //vertex1이 범위 밖으로 나가는지 확인
        if (targetVec1Pos.x < limits[0].x || targetVec1Pos.x > limits[1].x || targetVec1Pos.y < limits[0].y || targetVec1Pos.y > limits[1].y) return false;

        Vector2 targetVec2Pos = targetPos + vertexOffset[1];

        //vertex1이 범위 밖으로 나가는지 확인
        if (targetVec2Pos.x < limits[0].x || targetVec2Pos.x > limits[1].x || targetVec2Pos.y < limits[0].y || targetVec2Pos.y > limits[1].y) return false;


        return true;
    }


}
