#pragma strict

var myProjectile : GameObject;
var reloadTime : float = 1f;
var turnSpeed : float = 10f;
var firePauseTime : float = .5f;
var errorAmount : float = .001;
var myTarget : Transform;
var projectileSpawnPosition : Transform[];
var towerTurnPart : Transform;
var barrelRot : Transform;

private var nextFireTime : float;
private var nextMoveTime : float;
private var desiredRotation : Quaternion;
private var desiredBarrelRot : Quaternion;
private var aimError : float;

function Update () {
	if(myTarget) {
		Debug.Log("FIRE!!");
		if(Time.time >= nextMoveTime) {
			CalculateAimPosition(myTarget.position);
			towerTurnPart.rotation = Quaternion.Lerp(towerTurnPart.rotation, desiredRotation, Time.deltaTime*turnSpeed);
//			barrelRot.rotation.x     = myTarget.//Quaternion.Lerp(barrelRot.rotation,     desiredBarrelRot,Time.deltaTime*turnSpeed);
		}
	}
}

function OnTriggerEnter(something : Collider) {
	if(something.gameObject.tag == "Enemy") {
		Debug.Log("ENEMY SPOTTED!!");
		nextFireTime = Time.time+(reloadTime*.5);
		myTarget = something.gameObject.transform;
	}
	
}

function OnTriggerExit(something : Collider) {
	if(something.gameObject.transform == myTarget) {
		myTarget = null;
		Debug.Log("Lost sight of the enemy");
	}
}

function CalculateAimPosition(targetPos : Vector3) {
	var aimPoint = Vector3(targetPos.x+aimError, 0.0, targetPos.z+aimError);
	desiredRotation = Quaternion.LookRotation(aimPoint);
	var barrelAimPoint = Vector3(targetPos.y+aimError,aimPoint.x,aimPoint.z);
	desiredBarrelRot = Quaternion.LookRotation(barrelAimPoint);
	
}

function CalculateAimError() {
	aimError = Random.Range(-errorAmount, errorAmount);
}

function FireProjectile() {
	nextFireTime = Time.time+reloadTime;
	nextMoveTime = Time.time+firePauseTime;
	CalculateAimError();
	
	for(theSpawnPos in projectileSpawnPosition) {
		Instantiate(myProjectile, theSpawnPos.position,theSpawnPos.rotation);
	}
}