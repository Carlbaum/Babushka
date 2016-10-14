using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	
  public static float	myRange = 10.0f;
  public static float mySpeed = 10.0f;
  public static float myDamage = 20.0f;
	
	private float myDist;
	
	public GameObject explosion;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.forward * Time.deltaTime * mySpeed);
		myDist += Time.deltaTime * mySpeed;
		if(myDist >= myRange)
			Destroy(gameObject);
	}
	
	public float GetSpeed () { return mySpeed; }
  public float GetDamage () { return myDamage; print("attackPOWERint:" + (int)myDamage);}

  public void SetSpeed (float speed) { mySpeed = speed; }
  public void SetDamage (float damage) { myDamage = damage; }

	
	void OnTriggerEnter (Collider collider) {
		if(collider.gameObject.tag == "Enemy") {
			Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
			//collider.GetComponent<
			Destroy(gameObject);
		}
	}
}