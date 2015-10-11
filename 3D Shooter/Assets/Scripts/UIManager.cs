using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

    public GameObject combo, score, player;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        combo.GetComponent<Text>().text = player.GetComponent<Player>().combo.ToString() + "X";
        score.GetComponent<Text>().text = player.GetComponent<Player>().score.ToString() + " POINTS";
	}
}
