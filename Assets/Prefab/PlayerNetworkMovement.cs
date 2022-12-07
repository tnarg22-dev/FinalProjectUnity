using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;


public class PlayerNetworkMovement : NetworkBehaviour
{
    public GameObject otherCharacter;
    public Rigidbody Rb;
    public GameObject spawnpoint1;
    public GameObject spawnpoint2;
    public bool facingLeft;
    public Vector3 OtherPlayerLocation;
    public Vector3 PlayerLocation;
    public Vector3 ThisPlayerXpos;
    public float Health = 100;
    [SerializeField] private float speed;
    [SerializeField] private float maximumSpeed = 5;
    public Animator m_Animator;
    public float jumpForce = 10f;

    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(new MyCustomData { _int = 56, _bool = true }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public bool onground;

    public bool Hit;
    public bool Blocking;
    public int BlockCounter;
    public bool GuardBroken;
    public bool CanBlock;

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;

        public FixedString128Bytes _message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref _message);
        }
    }
    public void Start()
    {
        spawnpoint1 = GameObject.FindGameObjectWithTag("SpawnPoint1");
        spawnpoint2 = GameObject.FindGameObjectWithTag("SpawnPoint2");

        OtherPlayerLocation = otherCharacter.transform.position;
        PlayerLocation = this.transform.position;


        if (IsClient && !IsHost)
        {

            this.gameObject.transform.position = spawnpoint2.transform.position;
            this.gameObject.tag = "Player2";


        }
        else if (IsHost)
        {

            this.gameObject.transform.position = spawnpoint1.transform.position;
            this.gameObject.tag = "Player1";
        }


    }
    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue._message); ;
        };
    }
    private void Update()
    {


        if (!IsOwner) return;
        m_Animator.SetFloat("Speed", (float)(speed * 0.1));

        if (IsClient && !IsHost)
        {
            this.gameObject.tag = "Player2";

            otherCharacter = GameObject.FindGameObjectWithTag("Player");

        }
        if (IsHost)
        {



            otherCharacter = GameObject.FindGameObjectWithTag("Player2");
        }

        Movement();

        SpeedRegulation();

        if (Input.GetKeyDown(KeyCode.J))
        {
            m_Animator.SetTrigger("Punch"); 
           Rb.velocity = new Vector3(0, 0, 0);
        } 
        if (Input.GetKeyDown(KeyCode.K))
        {
            m_Animator.SetTrigger("Punch2");
           Rb.velocity = new Vector3(0, 0, 0);
        }
        if(Health <= 0)
        {
            Destroy(this.gameObject);
        }
        HandleHitBlocking(Hit, Blocking);
    }
    public void SpeedRegulation()
    {
        if (speed > maximumSpeed)
        {
            float brakeSpeed = speed - maximumSpeed;  // calculate the speed decrease
            Vector3 normalisedVelocity = Rb.velocity.normalized;
            Vector3 brakeVelocity = normalisedVelocity * brakeSpeed;  // make the brake Vector3 value
            Rb.AddForce(-brakeVelocity);  // apply opposing brake force
        }

    }
    public void Movement()
    {

        speed = Vector3.Magnitude(Rb.velocity);
        Debug.Log(speed);

        Vector3 moveDir = new Vector3(0, 0, 0);
        Jump();
        if (Input.GetKey(KeyCode.A))
        {
            if (!facingLeft)
            {
                facingLeft = true;
                transform.RotateAround(transform.position, transform.up, 180f);
            }
            moveDir.x = -1f;


        }

        if (Input.GetKey(KeyCode.D))
        {
            if (facingLeft)
            {
                facingLeft = false;
                transform.RotateAround(transform.position, transform.up, 180f);
            }

            moveDir.x = +1f;

        }

        CheckGrounded();
        float moveSpeed = 10f;


        Rb.AddForce(moveDir * moveSpeed, ForceMode.Force);
        Debug.Log("moving");


    }

    // Draws raycast gizmo
   
    // Raycast that checks if the object is on the ground
    void CheckGrounded()
    {
        RaycastHit hit;
        Vector3 raycastDirection = Vector3.down;
        float raycastDistance = 1;



        if (Physics.Raycast(transform.position, raycastDirection, out hit, raycastDistance))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                onground = true;
            } 
            if (!hit.collider.gameObject.CompareTag("Ground"))
            {
                onground = false;
            }
        }
    } 
    void OnDrawGizmos()
    {
        Vector3 raycastDirection = Vector3.down;
        float raycastDistance = .25f;

        // Draws gizmo in scene for debugging
        Gizmos.color = (onground) ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, raycastDirection * raycastDistance);
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && onground == true)
        {
            Rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (Input.GetKey(KeyCode.Space))
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up * (jumpForce * Time.deltaTime), ForceMode.Impulse);
            }
        }
    }

    public void HandleHitBlocking(bool hit, bool blocking)
    {
      if(CanBlock == true)
        {
            if (hit == true && blocking == true)
                 {
                    BlockCounter += 1;
                    Hit = false;
                 }
        }

            if (BlockCounter >= 3)
            {
                GuardBroken = true;
            
            }

            if (GuardBroken)
            {
                CanBlock = false;
                Debug.Log("Guard is broken");
            }

            if (!CanBlock)
            {
                StartCoroutine(WaitForTime(1.5f));
            }

        
        
    }

    private IEnumerator WaitForTime(float time)
    {
        yield return new WaitForSeconds(time);
        CanBlock = true;
        BlockCounter = 0;
        GuardBroken = false;
    }

}

