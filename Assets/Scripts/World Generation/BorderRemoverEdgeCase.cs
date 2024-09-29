using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderRemoverEdgeCase : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        Debug.Log(collision.gameObject.name + "Testing");
    }
}
