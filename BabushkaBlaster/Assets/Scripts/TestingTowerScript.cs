using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestingTowerScript : MonoBehaviour {
	
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
  private float projectileSpeed;// = ProjectileScript.mySpeed;
  private float targetSpeed;
  private float targetDistance;
  private float errorAmount = 0.2f;
  
  private Vector3 aimError;
  private Vector3 targetAnticipatedPos;
  private Vector3 targetDirection;
	
//  float maxBarrelRot = -64.0f;
//  float minBarrelRot = 1.3f;
  Quaternion desiredRotation;
  Quaternion desiredBarrelRot;
	
	public GameObject myProjectile;
	
	private GameObject myTargetObj;
//  public Stack<GameObject> enemyStack;
  public List<GameObject> enemyList;
	
	void Awake () {
		
	}
	
	void Start () {
    enemyList = new List<GameObject>();
//    enemyStack = new Stack<GameObject>();
    projectileSpeed = myProjectile.GetComponent<ProjectileScript>().GetSpeed();
	}
	
	void Update () {
    if (enemyList.Count > 0) {
//      Debug.Log("List contains " + enemyList.Count + " elements!");
      if (enemyList[0]) {
//        Debug.Log("VALID!");
        if(Time.time >= nextMoveTime) {
          //CalculateAimPosition(myTarget);
          myTarget =  enemyList[0].transform;
          targetSpeed = myTarget.GetComponent<Boid>().getSpeed();
          targetDirection = myTarget.forward;

          Vector3 scaledDirectionVec = targetDirection * targetSpeed / projectileSpeed;
          targetAnticipatedPos = myTarget.position + scaledDirectionVec;
          targetDistance = Vector3.Distance(aimHorizontal.position, targetAnticipatedPos);
          Vector3 aimPoint = targetAnticipatedPos + scaledDirectionVec * targetDistance;
//          Vector3 horizontalDirection = (((myTarget.position - aimHorizontal.position) / Time.deltaTime + targetSpeed * targetDirection) /projectileSpeed).normalized;
//          Vector3 aimPoint = aimHorizontal.position + horizontalDirection * projectileSpeed;

          Vector3 temp = aimPoint + aimError;
          aimHorizontal.LookAt(temp);
          aimHorizontal.eulerAngles = new Vector3(0, aimHorizontal.eulerAngles.y, 0);
          aimVertical.LookAt(temp);

          towerTurnPart.rotation = Quaternion.Slerp(towerTurnPart.rotation, aimHorizontal.rotation, Time.deltaTime*turnSpeed);

          aimHorizontal.LookAt(projectileSpawnPoint);
          aimHorizontal.eulerAngles = new Vector3(0, aimHorizontal.eulerAngles.y, 0);

          barrel.rotation = Quaternion.Slerp(barrel.rotation , aimVertical.rotation , Time.deltaTime*turnSpeed);

          if(barrel.eulerAngles.x < -64.0f) {
            barrel.transform.Rotate(-64.0f, 0f, 0f);
          } else if (barrel.eulerAngles.x > 4.5f) {
//            barrel.transform.Rotate(4.5f, 0f, 0f);
          }
        }

        if(Time.time >= nextFireTime) {
          FireProjectile();
        }      
      } else {
//        Debug.Log("NON-VALID!");
        enemyList.RemoveAt(0);
      }
    }
//    if (false){//myTarget) {	
			
//		}
	}
	
	void OnTriggerEnter (Collider collider) {
//    nextFireTime = Time.time+(reloadTime*0.5f);
    if(collider.gameObject.tag == "Enemy") {
      myTargetObj = collider.gameObject;
      myTarget = myTargetObj.transform;
//      Debug.Log("Aquired new target: " + myTarget.name );
//      enemyStack.Push(collider.gameObject);
      if (myTargetObj.GetComponent<Boid>().avoidanceDistance == 0.25) {
        int i = 0;
        while (i < enemyList.Count && enemyList[i] && enemyList[i].GetComponent<Boid>().avoidanceDistance == 0.25) {
          i++;
        }
        enemyList.Insert(i,collider.gameObject);
      } else {
        enemyList.Add(collider.gameObject);
      }

//			myTargetObj = collider.gameObject;
//			myTarget = myTargetObj.transform;
//      targetSpeed = myTarget.GetComponent<EnemyScript>().getSpeed();
////			Debug.Log("Aquired new target: " + myTarget.name + "\nSpeed: " + targetSpeed);
		}
	}
	
	void OnTriggerExit (Collider collider) {
    int idx = enemyList.IndexOf(collider.gameObject);
    if (idx != -1) {
//      Debug.Log("Found object at index " + idx + ", removing it!");
      enemyList.RemoveAt(idx);
    }
//		if(collider.gameObject.transform == myTarget) {
//			myTarget = null;
//			Debug.Log("**Danger**\nLost track of target!!");
//		}
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
    aimError.x = Random.Range(-errorAmount, errorAmount);
    aimError.y = Random.Range(-errorAmount, errorAmount);
    aimError.z = Random.Range(-errorAmount, errorAmount);
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










/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestingTowerScript : MonoBehaviour {

  Transform myTarget;

  public Transform towerTurnPart;
  public Transform barrel;
  public Transform projectileSpawnPoint;
  public Transform aimHorizontal;
  public Transform aimVertical;


  public float turnSpeed = 10.0f;
  public float reloadTime = 0.7f;
  public float firePauseTime = 0.2f;

  private float nextFireTime;
  private float nextMoveTime;
  private float projectileSpeed = ProjectileScript.mySpeed;
  private float targetSpeed;
  private Vector3 targetSpeedVec;
  private float targetDistance;
  private float aimError;
  private float errorAmount = 0.25f;

  private Vector3 targetAnticipatedPos;
  private Vector3 targetDirection;

  //  float maxBarrelRot = -64.0f;
  //  float minBarrelRot = 1.3f;
  Quaternion desiredRotation;
  Quaternion desiredBarrelRot;

  public GameObject myProjectile;

  private GameObject myTargetObj;
  public Stack<GameObject> enemyStack;
  //  private Stack<Enemy> enemyStack;

  void Awake () {

  }

  void Start () {
    enemyStack = new Stack<GameObject>();
    //    enemyStack = new Stack<Enemy>();
  }

  void Update () {
    if (myTarget) { 
      if(Time.time >= nextMoveTime) {
        //CalculateAimPosition(myTarget);

        targetDirection = myTarget.forward;

        targetAnticipatedPos = myTarget.position + targetSpeedVec / projectileSpeed;
        //        targetAnticipatedPos = myTarget.position + targetDirection * targetSpeed / projectileSpeed;
        targetDistance = Vector3.Distance( aimHorizontal.position, targetAnticipatedPos);
        Vector3 aimPoint = targetAnticipatedPos + targetSpeedVec * targetDistance / projectileSpeed;

        Vector3 temp = aimPoint + new Vector3(aimError, aimError, aimError);
        aimHorizontal.LookAt(temp);
        aimHorizontal.eulerAngles = new Vector3(0, aimHorizontal.eulerAngles.y, 0);
        aimVertical.LookAt(temp);

        towerTurnPart.rotation = Quaternion.Slerp(towerTurnPart.rotation, aimHorizontal.rotation, Time.deltaTime*turnSpeed);

        aimHorizontal.LookAt(projectileSpawnPoint);
        aimHorizontal.eulerAngles = new Vector3(0, aimHorizontal.eulerAngles.y, 0);

        barrel.rotation = Quaternion.Slerp(barrel.rotation , aimVertical.rotation , Time.deltaTime * turnSpeed);

        if(barrel.eulerAngles.x < -64.0f) {
          barrel.transform.Rotate(-64.0f,0f,0f);
        }
      }

      if(Time.time >= nextFireTime) {
        FireProjectile();
        nextFireTime = Time.time+(reloadTime*0.5f);

      }
    } else if (enemyStack.Count > 0) {
      enemyStack.Pop();
      if (enemyStack.Count > 0) {
        myTargetObj = enemyStack.Peek();
        myTarget = myTargetObj.transform;
        targetSpeedVec = myTargetObj.GetComponent<Boid>().getSpeedVector();
      }
    }
  }

  void OnTriggerEnter (Collider collider) {
    if(collider.gameObject.tag == "Enemy") {
      enemyStack.Push(collider.gameObject);
      if (!myTarget) {
        myTargetObj = GetComponent<Collider>().gameObject;
        myTarget = myTargetObj.transform;
        targetSpeedVec = myTargetObj.GetComponent<Boid>().getSpeedVector();
      }
      //      Debug.Log("Aquired new target: " + myTarget.name + "\nSpeed: " + targetSpeed);
    }
  }

  void OnTriggerExit (Collider collider) {
    if(collider.gameObject.transform == myTarget) {
      myTarget = null;
      //      Debug.Log("**Danger**\nLost track of target!!");
    }
  }

//  void CalculateAimPosition (Vector3 targetPos) {
//    Vector3 targetAimVec = targetPos - towerTurnPart.parent.parent.position;
//    Vector3 verticalAim = targetAimVec;
//    //new Vector3(0.0f, targetPos.y, 0.0f);
//    desiredBarrelRot = Quaternion.LookRotation(verticalAim);
//    
//    targetAimVec.y = 0.0f;
//    desiredRotation = Quaternion.LookRotation(targetAimVec);
//  }

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
*/