using UnityEngine;
using System.Collections;

public class TestingTowerScriptOld : MonoBehaviour {
	
	Transform myTarget;
	
  public Transform towerTurnPart;
  public Transform barrel;
  public Transform projectileSpawnPoint;
  public Transform aimHorizontal;
  public Transform aimVertical;
	
  public float turnSpeed = 10.0f;
  public float reloadTime = 0.7f;
  public float firePauseTime = 0.2f;
	
  private float	nextFireTime;
  private float nextMoveTime;
  private float projectileSpeed = ProjectileScript.mySpeed;
  private float targetSpeed;
  private float targetDistance;
  private float aimError;
  private float errorAmount = 0.5f;
	
  private Vector3 targetAnticipatedPos;
  private Vector3 targetDirection;
	
//  float maxBarrelRot = -64.0f;
//  float minBarrelRot = 1.3f;
  Quaternion desiredRotation;
  Quaternion desiredBarrelRot;
	
	public GameObject myProjectile;
	
	private GameObject myTargetObj;
	
	void Awake () {
		
	}
	
	void Start () {
		
	}
	
	void Update () {
		if (myTarget) {	
			if(Time.time >= nextMoveTime)	{
				//CalculateAimPosition(myTarget);
				
				targetDirection = myTarget.forward;
				
				targetAnticipatedPos = myTarget.position+targetDirection*targetSpeed/projectileSpeed;
				targetDistance = Vector3.Distance(aimHorizontal.position,targetAnticipatedPos);
				Vector3 aimPoint = targetAnticipatedPos+targetDirection*targetSpeed*targetDistance/projectileSpeed;
				
        Vector3 temp = aimPoint + new Vector3(aimError, aimError, aimError);
				aimHorizontal.LookAt(temp);
				aimHorizontal.eulerAngles = new Vector3(0, aimHorizontal.eulerAngles.y, 0);
				aimVertical.LookAt(temp);
				
				towerTurnPart.rotation = Quaternion.Slerp(towerTurnPart.rotation, aimHorizontal.rotation, Time.deltaTime*turnSpeed);
				
				aimHorizontal.LookAt(projectileSpawnPoint);
				aimHorizontal.eulerAngles = new Vector3(0, aimHorizontal.eulerAngles.y, 0);
				
				barrel.rotation = Quaternion.Slerp(barrel.rotation , aimVertical.rotation , Time.deltaTime*turnSpeed);
				
				if(barrel.eulerAngles.x < -64.0f) {
					barrel.transform.Rotate(-64.0f,0f,0f);
				}
			}
			
			if(Time.time >= nextFireTime) {
				FireProjectile();
			}
		}
	}
	
	void OnTriggerEnter (Collider collider) {
		if(collider.gameObject.tag == "Enemy") {
			
			nextFireTime = Time.time+(reloadTime*0.5f);
			myTargetObj = collider.gameObject;
			myTarget = myTargetObj.transform;
      targetSpeed = myTarget.GetComponent<EnemyScript>().getSpeed();
//			Debug.Log("Aquired new target: " + myTarget.name + "\nSpeed: " + targetSpeed);
		}
	}
	
	void OnTriggerExit (Collider collider) {
		if(collider.gameObject.transform == myTarget) {
			myTarget = null;
//			Debug.Log("**Danger**\nLost track of target!!");
		}
	}
	
	/*void CalculateAimPosition (Vector3 targetPos) {
		Vector3 targetAimVec = targetPos - towerTurnPart.parent.parent.position;
		Vector3 verticalAim = targetAimVec;
		//new Vector3(0.0f, targetPos.y, 0.0f);
		desiredBarrelRot = Quaternion.LookRotation(verticalAim);
		
		targetAimVec.y = 0.0f;
		desiredRotation = Quaternion.LookRotation(targetAimVec);
	}*/
	
	private void CalculateAimError() {
		aimError = Random.Range(-errorAmount, errorAmount);
	}
	
	void FireProjectile () {
		GetComponent<AudioSource>().Play();
		
		CalculateAimError();
		
		nextFireTime = Time.time+reloadTime;
		nextMoveTime = Time.time+firePauseTime;
		
		Instantiate(myProjectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
	}

  public float GetFireRadius() {
    return gameObject.GetComponent<SphereCollider>().radius;
  }

  public void SetProjectileSpeed(float newSpeed) { 
    projectileSpeed = newSpeed;
    myProjectile.GetComponent<ProjectileScript>().SetSpeed(newSpeed);
  }

  public void SetAttackPower(float attackPower) { 
    myProjectile.GetComponent<ProjectileScript>().SetDamage(attackPower);
  }

  public float GetAttackPower() { 
    return myProjectile.GetComponent<ProjectileScript>().GetDamage();
  }
}
