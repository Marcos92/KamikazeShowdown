using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIManager : MonoBehaviour {

    public GameObject health, combo, score, ammo, player, bossHealth;
    Enemy boss;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (player.GetComponent<Player>().combo > 1) combo.GetComponent<Text>().text = player.GetComponent<Player>().combo.ToString() + "x";
            else combo.GetComponent<Text>().text = "";

            score.GetComponent<Text>().text = player.GetComponent<Player>().score.ToString() + " POINTS";

            health.GetComponent<Text>().text = player.GetComponent<Player>().health.ToString();

            if (player.GetComponent<Player>().gunController.equippedGun.infinite) ammo.GetComponent<Text>().text = "";
            else ammo.GetComponent<Text>().text = player.GetComponent<Player>().gunController.equippedGun.ammo.ToString() + " BULLETS";
        }
        if (gameObject.GetComponent<LevelGenerator>().currentRoom.boss)
        { 
            if (GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>().type == Enemy.Type.Boss1)
                boss = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
            else
                boss = null;
        }
        if (boss != null)
        {
            bossHealth.GetComponent<Text>().text = boss.health.ToString();
        }
        else bossHealth.GetComponent<Text>().text = "";
    }
}
