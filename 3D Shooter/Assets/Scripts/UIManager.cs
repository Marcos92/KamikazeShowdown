using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

    public GameObject combo, score, ammo, player;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        combo.GetComponent<Text>().text = player.GetComponent<Player>().combo.ToString() + "x";

        score.GetComponent<Text>().text = player.GetComponent<Player>().score.ToString() + " POINTS";

        if(player.GetComponent<Player>().gunController.equippedGun.infinite) ammo.GetComponent<Text>().text = "";
        else ammo.GetComponent<Text>().text = player.GetComponent<Player>().gunController.equippedGun.ammo.ToString() + " BULLETS";
    }
}
