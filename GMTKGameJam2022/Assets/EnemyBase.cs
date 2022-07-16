using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float animateMoveTime = 0.5f;

    bool isMoving = false;
    Vector3 desiredMove;
    float timeStartMove;
    Vector3 prevPosition;

    public void SetMove(Vector3 pos)
    {
        desiredMove = pos;
        timeStartMove = Time.realtimeSinceStartup;
        prevPosition = transform.position;
    }

    private void Start()
    {
        prevPosition = transform.position;
        desiredMove = prevPosition;
    }

    public void Attack(Vector3 pos)
    {

    }

    public void TakeDamage()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position != desiredMove)
        {
            float dt = Mathf.Clamp01((Time.realtimeSinceStartup - timeStartMove) / animateMoveTime);
            transform.position = Vector3.Lerp(prevPosition, desiredMove, dt);
    }
    }
        
}
