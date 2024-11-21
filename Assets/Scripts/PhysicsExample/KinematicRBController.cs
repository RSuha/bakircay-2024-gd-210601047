using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicRBController : MonoBehaviour
{
    public Rigidbody controlledRigidbody;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            controlledRigidbody.MovePosition(controlledRigidbody.position + Vector3.up * Time.deltaTime);
        }
    }

}
