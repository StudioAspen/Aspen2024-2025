using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqaureManager : MonoBehaviour
{

    [Header("Square Stats")]
    [SerializeField] private int sqaureLevel;
    public Vector3 islandRiseSpeed;
    public bool openSides = true;
    public int sqaureScore;
    public int wallHitScore;
    public int wallHitScore2;
    public int wallHitScore3;
    public int wallHitScore4;

    [SerializeField] LayerMask WallLayerMask;
    public Vector3 raycastoffset;

    RaycastHit wallHit;
    RaycastHit wallHit2;
    RaycastHit wallHit3;
    RaycastHit wallHit4;





    void Start()
    {
   
    }

    
    void Update()
    {
        Ray rayFront = new Ray(transform.position + raycastoffset, transform.TransformDirection(Vector3.forward));
        Ray rayRight = new Ray(transform.position, transform.TransformDirection(Vector3.right));
        Ray rayLeft = new Ray(transform.position, transform.TransformDirection(Vector3.left));
        Ray rayBack = new Ray(transform.position, transform.TransformDirection(Vector3.back));

        if (gameObject.transform.position.y < -5) 
        {
              GetComponent<Transform>().position += islandRiseSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SqaureCheck();
        }
                
       
        if (sqaureScore > 0) 
        {
        
            if (Physics.Raycast(rayFront, out wallHit, 100f))
            {
                Debug.DrawRay(transform.position + raycastoffset, transform.TransformDirection(Vector3.forward) * wallHit.distance, Color.red);
                wallHitScore = 1;
                Debug.Log(wallHit.point);

         
            }
            else { wallHitScore = 0; }


            if (Physics.Raycast(rayRight, out wallHit2, 100f)) 
            {
                Debug.DrawRay(transform.position + raycastoffset, transform.TransformDirection(Vector3.right) * wallHit2.distance, Color.red);
                wallHitScore2 = 1;
                Debug.Log(wallHit2.point);
            }
            else { wallHitScore2 = 0; }

            if (Physics.Raycast(rayLeft, out wallHit3, 100f)) 
            {
                Debug.DrawRay(transform.position + raycastoffset, transform.TransformDirection(Vector3.left) * wallHit3.distance, Color.red);
                wallHitScore3 = 1;
                Debug.Log(wallHit3.point);
            }
            else { wallHitScore3 = 0; }


            if (Physics.Raycast(rayBack, out wallHit4, 100f))
            {
                Debug.DrawRay(transform.position + raycastoffset, transform.TransformDirection(Vector3.back) * wallHit4.distance, Color.red);
                wallHitScore4 = 1;
                Debug.Log(wallHit4.point);
            }
            else { wallHitScore4 = 0; }
        
        }

     

        if (sqaureScore <= 0)
        {
            openSides = false;
        }

        sqaureScore = wallHitScore + wallHitScore2 + wallHitScore3 + wallHitScore4;

        Debug.Log(sqaureScore);
    }

    public void SqaureCheck() 
    {
        if (openSides == true) 
        {
         
        }
    }
}
    