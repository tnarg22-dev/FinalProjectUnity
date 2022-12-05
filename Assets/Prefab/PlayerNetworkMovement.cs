using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerNetworkMovement : NetworkBehaviour
{
    public Rigidbody Rb;
    [SerializeField] private float speed;
    [SerializeField] private float maximumSpeed = 20;

    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            throw new System.NotImplementedException();
        }
    }
    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (int previousValue, int newValue) => {
            Debug.Log(OwnerClientId + "; Random " + randomNumber.Value);  
          };
    }
    private void Update()
    {
       
        if (!IsOwner) return;
        Movement();
            if (Input.GetKeyDown(KeyCode.T)) 
           {
            randomNumber.Value = Random.Range(0,100);
            
           }
        SpeedRegulation();
         
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
          Vector3 moveDir = new Vector3(0,0,0);

          if (Input.GetKey(KeyCode.A)) moveDir.x = -1f; 
          if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

             float moveSpeed = 10f;
             Rb.AddForce(moveDir * moveSpeed, ForceMode.Force);
    }
}
