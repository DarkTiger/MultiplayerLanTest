using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerCustom : NetworkBehaviour
{
   [SyncVar] public bool newPlayer = false;


   private void Awake()
   {
        
   }
}
