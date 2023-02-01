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

        //x���� ��������, ������ ���缭 y�� ��ġ ������
        targetPos = Vector2.right * targetPos.x + (other.transform.position.y + (targetPos.x - other.transform.position.x) * ratio) * Vector2.up;

        if (checkCanMove(targetPos))
        {
            transform.position = targetPos;
            center.setBoxCol();
        }
    }

    public void onTouchUp(Vector3 touchPos) { }

    //�����̰� ���� ��ġ�� targetPos�� �� �������� �������� �Ǵ��� �˻�.
    bool checkCanMove(Vector3 targetPos)
    {
        //���� ���� ������ �������� Ȯ��
        if (targetPos.x < limits[0].x || targetPos.x > limits[1].x || targetPos.y < limits[0].y || targetPos.y > limits[1].y) return false;

        //�ִ밡 �ּҺ��� ������ / ���� �ִ��� �˻�
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
        //�ڽ��� �ּ��� ���� ũ�� �̻����� ũ���� ����

        if (Vector2.Distance(targetPos, other.transform.position) < minimumSize) return false;

        return true;



    }

    /// <summary>
    /// �� vertex�� �����ϼ� �ִ� �Ѱ���� ����
    /// </summary>
    /// <param name="limits"></param>
    public void setLimit(Vector3[] limits)
    {
        this.limits[0] = limits[0];
        this.limits[1] = limits[1];
    }
    /// <summary>
    /// �� vertex�� ����/���� ���� ����
    /// </summary>
    /// <param name="ratio"></param>
    public void setRatio(float ratio)
    {
        this.ratio = ratio;
    }


    //������ ȸ�� ������ ���Ͽ� limit �缳�� �ؾ��Ҷ�, �缳�� �ϸ鼭 ȭ�� ������ ���� Vertex�� ��ġ ����.
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

        //�ڱ��ڽ��� ��ġ�� ȭ�� ������ ����
        transform.position = Vector2.right * targetX + Vector2.up * targetY;

        //�ٸ� ������ ��ġ�� ������ ���߾� ����.(�ݴ��� ������ ���� ������ ������ ��찡 ������, x���� �ΰ� y���� ���̴� ������� �̵��Ѵ�.
        other.transform.position = Vector2.right * (other.transform.position.x) + Vector2.up * (transform.position.y + (other.transform.position.x - transform.position.x) * ratio);
        
    }
}