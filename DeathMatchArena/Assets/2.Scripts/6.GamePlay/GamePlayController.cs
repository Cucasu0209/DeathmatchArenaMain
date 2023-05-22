using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GamePlayController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject newChar = PhotonNetwork.Instantiate("Character", Vector3.zero, Quaternion.identity);
        Physics2D.IgnoreLayerCollision(newChar.layer, newChar.layer);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
