using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]

public class GUITest : MonoBehaviour {

	void OnGUI () {
		if (GUI.Button (new Rect (Screen.width/2-100,Screen.height/2-20,200,35), "Start Game")) {
			print ("Loading game...");
		}
		
		if (GUI.Button (new Rect (Screen.width/2-100,Screen.height/2+20,200,35), "Quit")) {
			print ("Exiting application..");
		}
	}
}