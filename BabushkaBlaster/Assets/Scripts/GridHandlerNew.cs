using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridHandlerNew : MonoBehaviour {

//----PUBLIC VARIABLES----
	public int numColumns   = 17;
	public int numRows      = 17;
	public int startSquare  = -1;
  public int targetSquare = -1;

  //TODO: should be in enemy class
  public float damageAvoidanceFactor = 1;
	public float distanceAvoidanceFactor = 1;

//  public int[] obstacles;

  public Transform prefabTile;
  public Transform prefabObstacle;
  public Transform prefabEnemy;
 
  public Material matTileDefault;
  public Material matTileOccupied;
  public Material matTilePath;
  public Material matTileStart; 
  public Material matTileTarget;

  public LayerMask placementGridLayer;

  public bool isEnemiesMovingHorizontally;
  public bool isUsingTowerPenalties;


//----PRIVATE VARIABLES----
	private int tileCount;

  private int[] movementCost = new int[2] {10, 14};
  private List<int> towerTiles;
  private List<int> closedList;
  private List<int> openList;
 
  private bool isPathFound = false;

  private Vector3 defaultSpawnPosition;

  private Stack<Vector3> shortestPath;


//----FUNCTIONS----

  void CreateGrid () {
    // IF ENEMIES should go from left to right use these default start & target squares
    if (isEnemiesMovingHorizontally) {
      if (startSquare == -1) { startSquare = (numRows / 2) * numColumns + 1; }
      if (targetSquare == -1) { targetSquare = (numRows / 2 + 1) * numColumns; }
    } else {
      // IF ENEMIES should go from top to bottom use these default start & target squares
      if (startSquare == -1) { startSquare = numColumns / 2 + 1; }
      if (targetSquare == -1) { targetSquare = numRows * numColumns - (numColumns / 2); }
    }
    // use offset to make sure the grid center is actually in the center of the grid
    float offsetX = -(numColumns-1) / 2.0f; // assumes width of tiles is =1
    float offsetY = (numRows-1) / 2.0f ; // assumes height of tiles is =1

    int targetColumn = (targetSquare - 1) % numColumns; // 1st row and column are both at index = 0
    int targetRow = (targetSquare - 1) / numColumns;
    float targetCoordinateX = targetColumn + offsetX;
    float targetCoordinateY = -targetRow + offsetY;
//    print("targetCoords: (" + targetCoordinateX + ", " + targetCoordinateY + ")\ntarget column,row = (" + targetColumn + ", " + targetRow+ ")");
//    print(targetCoordinateX + ", " + targetCoordinateY);
    for (int row = 0; row < numRows; row++) {
      for (int column = 0; column < numColumns; column++) {
        float coordinateX = column + offsetX;
        float coordinateY = -row + offsetY;
        Transform newTileObject = Instantiate(prefabTile, new Vector3(coordinateX, 0.01f, coordinateY), Quaternion.Euler(new Vector3(0, 0, 0))) as Transform;
        ++tileCount;
        newTileObject.gameObject.layer = LayerMask.NameToLayer("GridLayer");
        newTileObject.Find("idText").GetComponent<TextMesh>().text = tileCount.ToString();
        newTileObject.name = "Tile#" + tileCount;
        //        newTileObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.05f));
        newTileObject.transform.parent = transform;
        newTileObject.tag = "placementTileVacant";

        TileScript newTileScript = newTileObject.GetComponent<TileScript>();
        newTileScript.setTileID(tileCount);
        newTileScript.setCoordinates();
        newTileScript.setAccessible(true);
        newTileScript.setHValue((int)(Mathf.Abs(coordinateX-targetCoordinateX)+Mathf.Abs(coordinateY-targetCoordinateY))*movementCost[0]);
        newTileScript.setTowerPenalty(0);

        // Neighbouring tile's IDs
        int northTileID = 0, eastTileID = 0, southTileID = 0, westTileID = 0;
        if (row > 0) {
          northTileID = tileCount - numColumns;
        }
        if (column < numColumns - 1) {
          eastTileID = tileCount + 1;
        } 
        if (row < numRows - 1) {
          southTileID = tileCount + numColumns;
        }
        if (column > 0) {
          westTileID = tileCount - 1;
        }

        newTileObject.GetComponent<TileScript>().setAdjacent(new Vector4(northTileID,eastTileID,southTileID,westTileID));

      }
    }
//    transform.Find ("Tile#" + startSquare).GetComponent<Renderer>().material.SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.99f));

    Vector2 startTileCoordinates = transform.Find("Tile#" + startSquare).GetComponent<TileScript>().getCoordinates();
    defaultSpawnPosition = new Vector3(startTileCoordinates.x, 0, startTileCoordinates.y);
    if (isEnemiesMovingHorizontally) {
      defaultSpawnPosition.x -= 5.0f; 
    } else {
      defaultSpawnPosition.z += 5.0f;
    }
  }

  bool CheckAdjecentTiles(int currentTileID) {
    if (currentTileID == targetSquare) {
      return true;
    }

    Transform currentTileTransform = transform.Find ("Tile#" + currentTileID);
    TileScript currentTileScript = currentTileTransform.GetComponent<TileScript>();

    //adjacentTile will contain what tile# that is ( north, east, south and west ) of start tile. 0 means no tile in that direction
    Vector4[] adjacentTiles = currentTileTransform.GetComponent<TileScript>().getAdjacentTilesNumber();

    closedList.Add(currentTileID);

    if (openList.Contains(currentTileID)) {
      openList.Remove(currentTileID);
    }

    bool[] freeTiles = new bool[4] { false, false, false, false }; // north, east, south, west

    // Check horizontal and vertical neighbours
    for (int j = 0; j < 4; j++) {
      int tileIDToCheck = (int)adjacentTiles[0][j];
      //TODO: squash these ifs into one.. but keep like this for now.. easier for debug purposes
      // check that there is a neighbour in the direction we are currently treating
      if (tileIDToCheck > 0) { // NOTE: neighbour id is set to 0 if no neighbour is present
        // check that the tile isn't in the closedList
        if (!closedList.Contains(tileIDToCheck)) {
          Transform tileToCheck = transform.Find("Tile#" + tileIDToCheck);
          TileScript tileToCheckScript = tileToCheck.GetComponent<TileScript>();
          if (tileToCheckScript.getAccessible()) {
            freeTiles[j] = true;
            int currentGValue = currentTileScript.getGValue();
            int tileToCheckHValue = tileToCheckScript.getHValue();
            int newGValue = movementCost[0] + currentGValue;
            int newTowerPenalty = 0;
            int newFValue = Mathf.RoundToInt(newGValue * distanceAvoidanceFactor) + tileToCheckHValue;
            if (isUsingTowerPenalties) {
              newTowerPenalty = Mathf.RoundToInt(CalculateTowerPenalties(tileToCheckScript.getCoordinates())*damageAvoidanceFactor) + tileToCheckScript.getTowerPenalty();
              newFValue += newTowerPenalty;
            }

            if (openList.Contains(tileIDToCheck)) { 
              if (tileToCheckScript.getFValue() > (newFValue)) {
                tileToCheckScript.setGValue(newGValue);
                tileToCheckScript.setParentNumber(currentTileID);
                tileToCheckScript.setFValue(newFValue);
                if (isUsingTowerPenalties) {
                  tileToCheckScript.setTowerPenalty(newTowerPenalty);
                }
              }
            } else {
              openList.Add(tileIDToCheck);
              tileToCheckScript.setGValue(newGValue);
              tileToCheckScript.setParentNumber(currentTileID);
              tileToCheckScript.setFValue(newFValue);
              if (isUsingTowerPenalties) {
                tileToCheckScript.setTowerPenalty(newTowerPenalty);
              }
            }
          }
        }
      }
    }
    // Check diagonal neighbours
    // TODO: do all of this in the same loop as above.. just add a nested for(int i = 0; i<2;i++)
    bool[] freeDiagonalTiles = new bool[4] { false, false, false, false }; // {NE, SE, SW, NW}
    if (freeTiles[0]) {
      if (freeTiles[1]) { freeDiagonalTiles[0] = true; } // north-east
      if (freeTiles[3]) { freeDiagonalTiles[3] = true; } // north-west
    }

    if (freeTiles[2]) {
      if (freeTiles[1]) { freeDiagonalTiles[1] = true; } // south-east
      if (freeTiles[3]) { freeDiagonalTiles[2] = true; } // south-west
    }

    for (int j = 0; j < 4; j++) { // j=0=>NE, j=1=>SE, j=2=>SW, j=3=>NW
      int tileIDToCheck = (int)adjacentTiles[1][j];
      //TODO: squash these ifs into one.. but keep like this for now, for easier debug purposes
      // check that there is a neighbour in the direction we are currently treating
      if (tileIDToCheck > 0 && freeDiagonalTiles[j]) { // NOTE: neighbour id is set to 0 if no neighbour is present
        // check that the tile isn't in the closedList
        if (!closedList.Contains(tileIDToCheck)) {
          Transform tileToCheck = transform.Find("Tile#" + tileIDToCheck);
          TileScript tileToCheckScript = tileToCheck.GetComponent<TileScript>();
          if (tileToCheckScript.getAccessible()) {
            int currentGValue = currentTileScript.getGValue();
            int tileToCheckHValue = tileToCheckScript.getHValue();
            int newGValue = movementCost[1] + currentGValue;
            int newTowerPenalty = 0;
            int newFValue = Mathf.RoundToInt(newGValue * distanceAvoidanceFactor) + tileToCheckHValue;
            if (isUsingTowerPenalties) {
              newTowerPenalty = Mathf.RoundToInt(CalculateTowerPenalties(tileToCheckScript.getCoordinates())*damageAvoidanceFactor) + tileToCheckScript.getTowerPenalty();
              newFValue += newTowerPenalty;
            }

            if (openList.Contains(tileIDToCheck)) {
              if (tileToCheckScript.getFValue() > (newFValue)) {
                tileToCheckScript.setGValue(newGValue);
                tileToCheckScript.setParentNumber(currentTileID);
                tileToCheckScript.setFValue(newFValue);
                if (isUsingTowerPenalties) {
                  tileToCheckScript.setTowerPenalty(newTowerPenalty);
                }
              }
            } else {
              openList.Add(tileIDToCheck);
              tileToCheckScript.setGValue(newGValue);
              tileToCheckScript.setParentNumber(currentTileID);
              tileToCheckScript.setFValue(newFValue);
              if (isUsingTowerPenalties) {
                tileToCheckScript.setTowerPenalty(newTowerPenalty);
              }
            }
          }
        }
      }
    }
    return false;
  }

  // A* pathfinding
  void FindPath() {
    print ("FindPathStart\n");
    closedList = new List<int>();
    openList= new List<int>();

    transform.Find("Tile#" + startSquare).GetComponent<TileScript>().setGValue(0);
    isPathFound = CheckAdjecentTiles(startSquare);


    while (!isPathFound) {
      int tileWithSmallestFValue = openList[0];
      int smallestFValue = transform.Find ("Tile#" + openList[0]).GetComponent<TileScript>().getFValue();

      foreach(int i in openList) {
        int fValue = transform.Find ("Tile#" + i).GetComponent<TileScript>().getFValue();
        if(fValue < smallestFValue) {
          smallestFValue = fValue;
          tileWithSmallestFValue = i;
        }

      }
      isPathFound = CheckAdjecentTiles(tileWithSmallestFValue);
    }

    shortestPath = new Stack<Vector3>();

    int tempTileID = targetSquare;
    while(tempTileID != startSquare) {
//      Debug.Log("ID: " + tempTileID);
      Transform tempTileTransform = transform.Find("Tile#" + tempTileID);
//      tempTileTransform.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.2f));
      tempTileTransform.GetComponent<Renderer>().material = matTilePath;
      shortestPath.Push(tempTileTransform.position);
      tempTileID = tempTileTransform.GetComponent<TileScript>().getParentNumber();
    } 
    shortestPath.Push(transform.Find ("Tile#" + tempTileID).position);
  }

  float CalculateTowerPenalties(Vector2 tileCoordinates) {
    float newTowerPenaltyCost = 0;
    foreach (int i in towerTiles) {
      TileScript towerTileScript = transform.Find("Tile#" + i).GetComponent<TileScript>();
      float towerFireRadius = towerTileScript.getFireRadius() + 2.5f; // add some xtra for enemy bounding box
      Vector2 towerDirection = tileCoordinates - towerTileScript.getCoordinates();
      float distanceToTower = towerDirection.magnitude;
      if (distanceToTower < towerFireRadius) {
        newTowerPenaltyCost += (towerFireRadius-distanceToTower) * towerTileScript.getAttackPower();
      }
    }
    return newTowerPenaltyCost;
  }

  void ResetGrid() {
    for (int i = 1; i <= tileCount ; i++ ) {
      Transform tileTransform = transform.Find ("Tile#" + i);
      tileTransform.GetComponent<Renderer>().material = matTileDefault;

      TileScript tileScript = tileTransform.GetComponent<TileScript>();
      tileScript.setGValue(99999); // just set it to a very high number so it is guaranteed to be updated
      tileScript.setFValue(99999); // just set it to a very high number so it is guaranteed to be updated
      tileScript.setTowerPenalty(0);
      tileScript.setParentNumber(0);
    }
  }

  void spawnEnemy() {
    Instantiate(prefabEnemy,defaultSpawnPosition,Quaternion.identity);
  }

  public bool GetIsPathFound() {
    return isPathFound;
  }

  public Stack<Vector3> getShortestPath(){
    return new Stack<Vector3>(shortestPath); // TODO: fix this UGLY ASS solution
  }

  public void addTower(int tileId) {
//    print("Added a tower to tile #" + tileId);
    towerTiles.Add(tileId);
  }

	// Use this for initialization
	void Start () {
//    tileCount = 0;
//    CreateGrid();
//    towerTiles = new List<int>();
	}
	
	// Update is called once per frame
	void Update () {
//    if (Input.GetKeyDown ("space")) {
//      if (isPathFound) {
//        isPathFound = false;
//        ResetGrid();
//      }
//      FindPath();
//    }
//
//    if (Input.GetKeyDown ("return") && isPathFound) {
//      spawnEnemy();
//    }
  }
}
