using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxScripts : MonoBehaviour
{
    public float damage;
    public float knockBack = 10;
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
                pnm.gameObject.GetComponent<Rigidbody>().AddForce((pnm.gameObject.transform.position - transform.position).normalized * knockBack, ForceMode.Impulse);

            }

      
    }

    }
        
}
