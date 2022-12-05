using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetowrkMangerUI : MonoBehaviour
{
    [SerializeField] private Button ServerBtn;
    [SerializeField] private Button ClientBtn;
    [SerializeField] private Button HostBtn;


    private void Awake()
    {
        ServerBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        }); 
        HostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        ClientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
