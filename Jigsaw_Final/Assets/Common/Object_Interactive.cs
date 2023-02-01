using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Object_Interactive
{
    public void onTouchDown(Vector3 touchPos);
    public void onTouchDrag(Vector3 touchPos);
    public void onTouchUp(Vector3 touchPos);


}
