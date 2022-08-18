using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthArrangement : MonoBehaviour
{
    public List<Transform> targetList;
    
    void Update ()
    {
        foreach (Transform target in targetList)
        {
            target.position = new Vector3(target.position.x,
                                          target.position.y,
                                          target.position.y);
        }
    }
}
