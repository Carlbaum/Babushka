using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy : MonoBehaviour {
	
  public float maxSpeed         = 3.0f;
  public float currentSpeed     = 2.0f;
  public float maxTurnSpeed     = 30.0f;
  public float currentTurnSpeed = 20.0f;
  public float startingHP       = 100;
  public float currentHP        = 100;
  public Quaternion rotation;
  public GameController gameCTRL;
  public List<Enemy> enemies;
	

	// Use this for initialization
	void Start () {
    print("A NEW ENEMY HAS SPAWNED");
//    gameCTRL = FindObjectOfType<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  public void OnTriggerEnter(Collider collider) {
    if(collider.gameObject.tag=="Projectile") {
      currentHP -= collider.gameObject.GetComponent<ProjectileScript>().GetDamage();
//      barLength = enemyHP/startHP;
      //      Debug.Log ("Enemy hit!!\n" + enemyHP + " HP left!");
      if(currentHP <= 0) {
        EnemyDeath();
      } 
    }
  }

  public void EnemyDeath()  {
    gameCTRL.EnemyKilled(gameObject);
//    print("pre:" + enemies.Count);
//    enemies.Remove(this);
//    print("post:" + enemies.Count);
//    Destroy(gameObject); 
  }

  public void EnemyReachedTarget() {
    gameCTRL.EnemyReachedTarget(gameObject);
  }

//  string ToString() {
//    return "jahaja";
//  }
}
