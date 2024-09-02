using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour {

    [SyncVar] public int skinIndex = 1;
    [SyncVar] public string playerName;

    NetworkManagerCustom networkManager;
    public GameObject myCamera;
    int rayRange = 3;
    // Use this for initialization


    private void Awake()
    {
        networkManager = GameObject.Find("GameObject").GetComponent<NetworkManagerCustom>();
    }


    IEnumerator InitializePlayer()
    {
        yield return new WaitForSeconds(1);
        CmdShowName(Environment.MachineName, gameObject);
        CmdChangeSkin(skinIndex, gameObject);
    }

    public override void OnStartClient()
    {
        CmdSetNewPlayerBool(true);
    }

    void Start()
    {
        if (isLocalPlayer)
        {
            myCamera = transform.Find("Main Camera").gameObject;
            myCamera.GetComponent<Camera>().enabled = true;
            StartCoroutine(InitializePlayer());
        }

        //CmdShowName(Environment.MachineName, gameObject);
        //CmdChangeSkin(skinIndex, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (networkManager.newPlayer)
        {
            CmdChangeSkin(skinIndex, gameObject);
            CmdShowName(Environment.MachineName, gameObject);
            CmdSetNewPlayerBool(false);
            Debug.Log("Player state update called");
        }

        if (isLocalPlayer)
        {
            float x = Input.GetAxis("Horizontal") * Time.deltaTime * 200.0f;
            float z = Input.GetAxis("Vertical") * Time.deltaTime * 5.0f;

            transform.Rotate(0, x, 0);
            transform.Translate(0, 0, z);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * 500);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                CmdChangeSkin(skinIndex, gameObject);
                CmdShowName(Environment.MachineName, gameObject);
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, rayRange))
                {
                    if (hit.transform.tag == "Player")
                    {
                        CmdHit(hit.transform.gameObject, transform.forward);
                    }
                }
            }

        }

    }

    [Command]
    void CmdSetNewPlayerBool(bool newPlayer)
    {
        networkManager.newPlayer = newPlayer;
    }
        

    [Command]
    void CmdShowName(string playerName, GameObject player)
    {
        RpcShowName(playerName, player);
    }

    [ClientRpc]
    void RpcShowName(string playerName, GameObject player)
    {
        player.name = playerName;
        player.transform.Find("PlayerCanvas").GetChild(0).GetComponent<Text>().text = playerName;

        if (!isLocalPlayer)
        {
            GameObject newCamera = myCamera;
            //transform.Find("PlayerCanvas").GetComponent<Canvas>().worldCamera = myCamera.GetComponent<Camera>();
            transform.Find("PlayerCanvas").GetChild(0).gameObject.SetActive(true);
        }
    }

    [Command]
    void CmdHit(GameObject target, Vector3 dir)
    {
        RpcHit(target, dir);
    }

    [ClientRpc]
    void RpcHit (GameObject target, Vector3 dir)
    {
        target.GetComponent<Rigidbody>().AddForce(Vector3.up * 100);
        target.GetComponent<Rigidbody>().AddForce(dir * 500);
    }

    [Command] 
    void CmdChangeSkin (int skin, GameObject player)
    {
        if(skin == 0)
        {
            player.GetComponent<PlayerController>().skinIndex = 1;
        }
        else if(skin == 1)
        {
            player.GetComponent<PlayerController>().skinIndex = 0;
        }

        RpcChangeSkin(skin, player);

    }

    [ClientRpc]
    void RpcChangeSkin (int skinIndex, GameObject player)
    {
        if (skinIndex == 0)
        {
            player.transform.GetChild(1).gameObject.SetActive(false);
            player.transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (skinIndex == 1)
        {
            player.transform.GetChild(0).gameObject.SetActive(false);
            player.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
