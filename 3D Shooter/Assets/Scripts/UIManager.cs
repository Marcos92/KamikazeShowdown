using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

    public GameObject health, combo, score, ammo, player;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (player.GetComponent<Player>().combo > 1) combo.GetComponent<Text>().text = player.GetComponent<Player>().combo.ToString() + "x";
        else combo.GetComponent<Text>().text = "";

        score.GetComponent<Text>().text = player.GetComponent<Player>().score.ToString() + " POINTS";

        health.GetComponent<Text>().text = player.GetComponent<Player>().health.ToString();

        if(player.GetComponent<Player>().gunController.equippedGun.infinite) ammo.GetComponent<Text>().text = "";
        else ammo.GetComponent<Text>().text = player.GetComponent<Player>().gunController.equippedGun.ammo.ToString() + " BULLETS";
    }
}
