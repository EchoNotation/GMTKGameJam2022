using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    bool isMoving = false;
    float animateMoveTime = 1f;

    public void SetMove(Vector3 pos)
    {
        // todo make it move smoothly
        transform.position = pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
