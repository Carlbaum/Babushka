using UnityEngine;
using System.Collections;

public class InGameGUI : MonoBehaviour {
  //PRIVATE VARIABLES
  private static string guiMode = "InGame";
  private static string Health;
  private static string Score;
  private static string Money;

  private static string[] towerList = {"T3","T2","T1"};

  private static bool buildMode = false;
  private static bool buildingSelected = false;

  private static int selectedTower;// = 100;
  private static int PlayerHealth = 9;
  private static int killScore = 0;
  private static int cash = 1337;

  private float hSliderValue = 0.0F;
  private float vSliderValue = 0.0F;
  private float hSValue = 0.0F;
  private float vSValue = 0.0F;
  private int cont = 0;

  private Material originalMat;

  private GameObject lastHitObj;
  private GameObject tT2;
  GameObject tempTow;
  Ray cray;
  RaycastHit hit;
  private Camera camera;

  // PUBLIC VARIABLES
  public Transform placementGrid;
  public LayerMask placementGridLayer;

  public Material hoverMat;

  public GameObject[] TempTowers;
  public GameObject[] structuresList;
  public GameObject Tower1;

  public float cameraSpeed = 2.0f;
  public float cameraRotSpeed = 10.0f;

  public GUISkin[] s1;

  void Awake() {

  }

  void Start() {
    //placementGrid = GameObject.Find("placementGrid").collider;
    //TempTowers = new GameObject[];
    camera = FindObjectOfType<Camera>();
  }

  // Update is called once per frame
  void Update() {
//    if (Input.GetKeyDown(KeyCode.Space)) { cont++; }
    if (Input.GetMouseButtonDown(0)) { Debug.Log("Pressed left click."); }
    if (Input.GetMouseButtonDown(1)) { Debug.Log("Pressed right click."); }
    if (Input.GetMouseButtonDown(2)) { Debug.Log("Pressed middle click."); }

    if(Input.GetKeyUp(KeyCode.B)) {
      buildMode = buildMode ? false : true;
//      if (buildMode) { 
//        buildMode = false; 
//      } else if (!buildMode) {
//        buildMode = true;
//      }
    }


    //tempTow.transform.position=Input.mousePosition;
    /* if (Input.GetMouseButtonDown(0))// .GetButtonDown("Fire1")) 
    {
            Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
      
      Debug.DrawRay(ray.origin, ray.direction*5, Color.red,5f);
      Instantiate(tempTow, Input.mousePosition, transform.rotation);
      tempTow.transform.position=Input.mousePosition;
      
            if (Physics.Raycast(ray))
                Instantiate(particle, transform.position, transform.rotation);// as GameObject;
    }*/

    if(Input.GetKeyDown("escape")) {
      Time.timeScale = 0;
      guiMode = "Paused";
    }

  }

  public int toolbarInt = 0;
  public string[] toolbarStrings = new string[] {"Toolbar1", "Toolbar2", "Toolbar3"};




  void OnGUI() {
    /* GUI.skin = s1[cont % s1.Length];
        if (s1.Length == 0) {
            Debug.LogError("Assign at least 1 skin on the array");
            return;
        }
        GUI.Label(new Rect(10, 10, 100, 20), "Hello World!");
          
      GUI.Box(new Rect(10, 50, 50, 50), "A BOX");
          
        if (GUI.Button(new Rect(10, 110, 70, 30), "A button"))
                Debug.Log("Button has been pressed");
        
              hSliderValue = GUI.HorizontalSlider(new Rect(10, 150, 100, 30), hSliderValue, 0.0F, 10.0F);
            
            vSliderValue = GUI.VerticalSlider(new Rect(10, 170, 100, 30), vSliderValue, 10.0F, 0.0F);
        
              hSValue = GUI.HorizontalScrollbar(new Rect(10, 210, 100, 30), hSValue, 1.0F, 0.0F, 10.0F);
                  
                vSValue = GUI.VerticalScrollbar(new Rect(10, 230, 100, 30), vSValue, 1.0F, 10.0F, 0.0F);

                    toolbarInt = GUI.Toolbar(new Rect(25, 25, 250, 30), toolbarInt, toolbarStrings);
    */
    if(guiMode == "InGame") {
      Health = GUI.TextField(new Rect(5,5,100,40),"Lives left: " + PlayerHealth);
      Score = GUI.TextField(new Rect(Screen.width -105, 5,100,40),"Score: " + killScore);
      GUI.Label(new Rect(Screen.width - 105, Screen.height - 20, 200, 30),"Money: " + cash);

      if(buildMode == false) {
        foreach (Transform PlacementTile in placementGrid) {
          PlacementTile.GetComponent<Renderer>().enabled = false;
        }

        if (GUI.Button(new Rect(5,Screen.height-45,40,40), "build")) {
          print ("Choose tower");
          buildMode = true;
        }
      } else { // else buildMode == true
        cray = Camera.main.ScreenPointToRay(Input.mousePosition);
        foreach(Transform PlacementTile in placementGrid) {
          PlacementTile.GetComponent<Renderer>().enabled = true;
        }

        if(Physics.Raycast(cray,out hit,50f,placementGridLayer)) {
          if(lastHitObj) {
            lastHitObj.GetComponent<Renderer>().material = originalMat; // set material of previously hit tile, back to its original material
          }

          lastHitObj = hit.collider.gameObject; // get the tile we currently touch with the mouse
          originalMat = lastHitObj.GetComponent<Renderer>().material;
          lastHitObj.GetComponent<Renderer>().material = hoverMat;
        } else {
          if(lastHitObj) {
            lastHitObj.GetComponent<Renderer>().material = originalMat;
            lastHitObj = null; // no longer touching a tile
          }
        }

        // if still hovering over a tile which is free, place tower!
        if(Input.GetMouseButtonDown(0) && lastHitObj) {
          if(lastHitObj.tag == "placementTileVacant") {
            TileScript lastHitScript = lastHitObj.GetComponent<TileScript>();
            lastHitScript.setTower(structuresList[0]);
            lastHitScript.setAccessible(false);
            lastHitObj.tag = "placementTileOccupied";
            placementGrid.GetComponent<GenerateGrid>().addTower(lastHitScript.getTileID());
            buildMode = false;
          }
        }
        /*if(buildingSelected == false) {
          selectedTower = GUI.SelectionGrid(new Rect(5,Screen.height-135, 40, 130),2,towerList,1);
          
          
          switch(selectedTower) {
            case 0:
              if (placementGrid.Raycast(cray, out hit,20.0f)) {
                Debug.Log("Chose T1");
                buildingSelected = true;
                //tempTow = GameObject.Find("Towers/SelTower1");
                Instantiate(tempTow,new Vector3(Mathf.Round(hit.point.x),hit.point.y,Mathf.Round(hit.point.z)),transform.rotation);
                    tT2 = GameObject.FindGameObjectWithTag("TempTower");
              } 
              break;
            
            case 1:
              if (placementGrid.Raycast(cray, out hit,20.0f)) {
                Debug.Log("Chose T2");
                buildingSelected = true;
                //tempTow = GameObject.Find("Towers/SelTower1");
                Instantiate(tempTow,new Vector3(Mathf.Round(hit.point.x),hit.point.y,Mathf.Round(hit.point.z)),transform.rotation);
                    tT2 = GameObject.FindGameObjectWithTag("TempTower");
              } 
                break;
            
            case 2:
              if (placementGrid.Raycast(cray, out hit,20.0f)) {
                Debug.Log("Chose T3");
                buildingSelected = true;
                //tempTow = GameObject.Find("Towers/SelTower1");
                Instantiate(tempTow,new Vector3(Mathf.Round(hit.point.x),hit.point.y,Mathf.Round(hit.point.z)),transform.rotation);
                    tT2 = GameObject.FindGameObjectWithTag("TempTower");
              } 
              break;
            
            default:
              if (placementGrid.Raycast(cray, out hit,20.0f)) {
                Debug.Log("Chose Default!! DAFUQ?!?!");
                //tempTow = GameObject.Find("Towers/SelTower1");
                buildingSelected = true;
                Instantiate(tempTow,new Vector3(Mathf.Round(hit.point.x),hit.point.y,Mathf.Round(hit.point.z)),transform.rotation);
                    tT2 = GameObject.FindGameObjectWithTag("TempTower");
              } 
              break;          
          }
        } else if(buildingSelected==true) {
          //Debug.Log("Byggnad Vald");
          while (placementGrid.Raycast(cray, out hit,20.0f)) {
            placementGrid.renderer.materials.SetValue("placementTileMatHover,0");
            //tT2.transform.position = new Vector3(Mathf.Round(hit.point.x),hit.point.y,Mathf.Round(hit.point.z));
          }
          
          //Debug.Log("Torn följer muspekaren");
          if(Input.GetMouseButtonDown(0)) {
            Instantiate(Tower1,new Vector3(Mathf.Round(hit.point.x),hit.point.y,Mathf.Round(hit.point.z)),transform.rotation);
            Destroy(tT2);
            buildMode = false;
            buildingSelected = false;
          }
        }
          /*if (placementGrid.Raycast(cray, out hit,20.0f)) {
                  
            //Debug.DrawLine (cray.origin, hit.point, Color.blue);
            //Instantiate(particle,hit.point,transform.rotation);
            //Instantiate(tempTow,new Vector3(Mathf.Round(hit.point.x),hit.point.y,Mathf.Round(hit.point.z)),transform.rotation);
              } */
      }


      if(Input.GetKey(KeyCode.RightArrow)) {
        if (Input.GetKey(KeyCode.LeftAlt)) {
          camera.transform.Rotate(Vector3.up * cameraRotSpeed * Time.deltaTime, Space.World);
        } else {
          camera.transform.Translate(Vector3.right * cameraSpeed * Time.deltaTime, Space.World);
        }
      }
      if(Input.GetKey(KeyCode.LeftArrow)) {
        if (Input.GetKey(KeyCode.LeftAlt)) {
          camera.transform.Rotate(Vector3.down * cameraRotSpeed * Time.deltaTime, Space.World);
        } else {
          camera.transform.Translate(Vector3.left * cameraSpeed * Time.deltaTime, Space.World);
        }
      }
      if(Input.GetKey(KeyCode.DownArrow)) {
        if (Input.GetKey(KeyCode.LeftAlt)) {
          camera.transform.Translate(Vector3.back * cameraSpeed * Time.deltaTime);
        } else {
          camera.transform.Translate(Vector3.back * cameraSpeed * Time.deltaTime, Space.World);
        }
      }
      if(Input.GetKey(KeyCode.UpArrow)) {
        if (Input.GetKey(KeyCode.LeftAlt)) {
          camera.transform.Translate(Vector3.forward * cameraSpeed * Time.deltaTime);
        } else {
          camera.transform.Translate(Vector3.forward * cameraSpeed * Time.deltaTime, Space.World);
        }
      }
    }

    if(guiMode == "Paused") {
      if (GUI.Button (new Rect (Screen.width/2-100,Screen.height/2-20,200,35), "Resume Game")) {
        print ("Resuming game...");

        Time.timeScale = 1;
        guiMode = "InGame";
      }

      if (GUI.Button (new Rect (Screen.width/2-100,Screen.height/2+20,200,35), "Quit")) {
        print ("Quiting...");
      }

    }
  }

  public void EnemyKilled() {
    killScore += 1;
  }
}