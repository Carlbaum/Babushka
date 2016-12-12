using UnityEngine;
using System.Collections;

public class MyGui : MonoBehaviour {
  //PRIVATE VARIABLES
  public gameState state;
  private GameController gameCTRL;

  public bool buildMode = false;
  private static int playerHealth;
  private static int killScore;
  private static int cash;
  private static int enemiesOnTheBoard;

  private Material originalMat;

  private GameObject lastHitObj;
  private Ray ray;
  private RaycastHit rayHit;
  private Camera camera;

  // PUBLIC VARIABLES
  public Transform placementGrid;
  public LayerMask placementGridLayer;

  public Material hoverMat;

  public GameObject[] structuresList;

  void Awake() {

  }

  void Start() {
    camera = FindObjectOfType<Camera>();
    gameCTRL = FindObjectOfType<GameController>();
    state = gameCTRL.state;
    buildMode = gameCTRL.buildMode;
  }

  void Update() {
    enemiesOnTheBoard = gameCTRL.enemiesOnTheBoard;
    playerHealth = gameCTRL.playerHealth;
//    state = gameCTRL.state;
//    switch (state) {
//      case gameState.Running:
//        break;
//      case gameState.Paused:
//        break;
//      case gameState.GameOver:
//        break;
//      case gameState.Victorious:
//        break;
//      default:
//        break;
//    }



  }

//  public int toolbarInt = 0;
//  public string[] toolbarStrings = new string[] {"Toolbar1", "Toolbar2", "Toolbar3"};



  void OnGUI() {
    
    switch (state) {
      case gameState.Running:
        GUI.Label(new Rect(Screen.width/2-60, 5, 120, 40), "Enemies left: " + enemiesOnTheBoard);
        GUI.Label(new Rect(5, 5, 100, 40), "Lives left: " + playerHealth);
        GUI.Label(new Rect(Screen.width - 105, 5, 100, 40), "Score: " + killScore);
        GUI.Label(new Rect(Screen.width - 105, Screen.height - 20, 200, 30), "Money: " + cash);

        if (!buildMode) {
          if (GUI.Button(new Rect(5, Screen.height - 45, 40, 40), "build")) {
            gameCTRL.changeBuildMode();
          }
        } else { // buildMode == true
          if (GUI.Button(new Rect(5, Screen.height - 45, 40, 40), "X")) {
            gameCTRL.changeBuildMode();
          }
          ray = camera.ScreenPointToRay(Input.mousePosition);

          if (Physics.Raycast(ray, out rayHit, 50f, placementGridLayer)) {
            if (lastHitObj) {
              lastHitObj.GetComponent<Renderer>().material = originalMat; // set material of previously hit tile, back to its original material
            }

            lastHitObj = rayHit.collider.gameObject; // get the tile we currently touch with the mouse
            originalMat = lastHitObj.GetComponent<Renderer>().material;
            lastHitObj.GetComponent<Renderer>().material = hoverMat;
          } else {
            if (lastHitObj) {
              lastHitObj.GetComponent<Renderer>().material = originalMat;
              lastHitObj = null; // no longer touching a tile
            }
          }

          // if still hovering over a tile which is free, place tower!
          if (Input.GetMouseButtonDown(0) && lastHitObj) {
            if (lastHitObj.tag == "placementTileVacant") {
              TileScript lastHitScript = lastHitObj.GetComponent<TileScript>();
              lastHitScript.setTower(structuresList[0]);
              lastHitScript.setAccessible(false);
              lastHitObj.tag = "placementTileOccupied";
              placementGrid.GetComponent<GridHandlerNew>().addTower(lastHitScript.getTileID());
              gameCTRL.changeBuildMode();
            }
          }
        }
        break;
      case gameState.Paused:
        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 20, 200, 35), "Resume Game")) {
          //        print ("Resuming game...");
          Time.timeScale = 1;
          gameCTRL.changeState(gameState.Running);
        }
        if (GUI.Button (new Rect (Screen.width/2-100,Screen.height/2+20,200,35), "Quit")) {
          //        print ("Quiting...");
        }
        break;
      case gameState.GameOver:
        break;
      case gameState.Victorious:
        break;
      default:
        break;
    }
  }

  public void setPlayerHealth(int hp) {
    playerHealth = hp;
  }

  public void setMoney(int money) {
    cash = money;
  }

  public void setScore(int score) {
    killScore = score;
  }

  public void setEnemiesLeft(int numberOfEnemies) {
    enemiesOnTheBoard = numberOfEnemies;
  }

  public void addToEnemiesLeft(int term) {
    enemiesOnTheBoard += term;
  }

//  public void EnemyKilled() {
//    killScore++;
//  }

//  public void addPlayerHealth(int hp) {
//    playerHealth += hp;
//  }
  public void setGridVisibility(bool b) {
    foreach (Transform PlacementTile in placementGrid) {
      PlacementTile.GetComponent<Renderer>().enabled = b;
    }
  }
}