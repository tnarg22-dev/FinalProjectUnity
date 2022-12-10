using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxScripts : MonoBehaviour
{
    public float damage;
    public float knockBack = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Player2"))
        {
            PlayerNetworkMovement pnm = other.GetComponent<PlayerNetworkMovement>();

            if (pnm.Blocking == true)
            {
                pnm.Hit = true;
               
                Debug.Log("Hit Blocked");
            }
            else
            {
                pnm.Health -= damage;
                Debug.Log("Hit Enemy"); 
            if(other.gameObject.transform.position.x < transform.position.x)
                {
                    Vector3 awayDirection = transform.position - other.transform.position * knockBack;
                     other.transform.position += awayDirection * Time.deltaTime;

                } 
                else if(other.gameObject.transform.position.x > transform.position.x)
                {
                    Vector3 awayDirection = other.transform.position - transform.position * knockBack;
                     other.transform.position += awayDirection * Time.deltaTime;

                }
               

            }

      
    }

    }
        
}
