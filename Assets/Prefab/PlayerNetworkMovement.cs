using System.Collections;
using System.Collections.Generic;

using Unity.Netcode;
using UnityEngine;
using Unity.Collections;


public class PlayerNetworkMovement : NetworkBehaviour
{
    public Rigidbody Rb;
    [SerializeField] private float speed;
    [SerializeField] private float maximumSpeed = 20;

    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(new MyCustomData{_int =56, _bool = true},NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


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
    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue._message); ;  
          };
    }
    private void Update()
    {
       
        if (!IsOwner) return;
        Movement();
            if (Input.GetKeyDown(KeyCode.T)) 
           {
                randomNumber.Value = new MyCustomData
                {
                    _int = 10,
                    _bool = false,
                    _message = " Test out Test out"
                };
            
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

