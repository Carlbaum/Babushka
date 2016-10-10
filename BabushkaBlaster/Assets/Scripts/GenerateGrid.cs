using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateGrid : MonoBehaviour {

//----PUBLIC VARIABLES----
	public int numColumns   = 17;
	public int numRows      = 17;
	public int startSquare  = -1;
	public int targetSquare = -1;

  public int[] obstacles;

  public Transform prefabTile;
  public Transform prefabObstacle;
 
  public Material matTileOccupied;
  public Material matTilePath;
  public Material matTileStart; 
  public Material matTileTarget;


//----PRIVATE VARIABLES----
	private int tileCount;
  private bool isPathFound = false;

  private int[] movementCost = new int[2] {10, 14};

  List<int> closedList, openList;
  Stack<Vector3> shortestPath;

  void CreateGrid () {

    if (startSquare == -1) {  startSquare = numColumns/2 + 1 ; print("startSquare: " + startSquare + "\n"); }
    if (targetSquare == -1) {  targetSquare = numRows * numColumns -( numColumns/2 ); print("targetSquare: " + targetSquare + "\n"); }
    // use offset to make sure the grid center is actually in the center of the grid
    float offsetX = -(numColumns-1) / 2.0f; // assumes width of tiles is =1
    float offsetY = (numRows-1) / 2.0f ; // assumes height of tiles is =1
    int targetColumn = targetSquare % numColumns -1; // 1st row and column are both at index = 0
    int targetRow = (targetSquare - 1) / numColumns;
    float targetCoordinateX = targetColumn + offsetX;
    float targetCoordinateY = -targetRow + offsetY;
    print("targetCoords: (" + targetCoordinateX + ", " + targetCoordinateY + ")\n");
    print(targetCoordinateX + ", " + targetCoordinateY);
    for (int row = 0; row < numRows; row++) {
      for (int column = 0; column < numColumns; column++) {
        ++tileCount;
        float coordinateX = column + offsetX;
        float coordinateY = -row + offsetY;
        Transform newGameObject = Instantiate(prefabTile, new Vector3(coordinateX, 0, coordinateY), Quaternion.Euler(new Vector3(90, 0, 0))) as Transform;
        TileScript nGOScript = newGameObject.GetComponent<TileScript>();

        newGameObject.Find("idText").GetComponent<TextMesh>().text = tileCount.ToString();
        newGameObject.name = "Tile#" + tileCount;
        newGameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.05f));
        newGameObject.transform.parent = transform;

        nGOScript.setTileID(tileCount);
        nGOScript.setCoordinates();
        nGOScript.setAccessible(true);
        nGOScript.setHValue((int)(Mathf.Abs(coordinateX-targetCoordinateX)+Mathf.Abs(coordinateY-targetCoordinateY))*movementCost[0]);

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

        newGameObject.GetComponent<TileScript>().setAdjacent(new Vector4(northTileID,eastTileID,southTileID,westTileID));

      }
    }
    transform.Find ("Tile#" + startSquare).GetComponent<Renderer>().material.SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.2f));
  }

  bool checkAdjecentTiles(int currentTileID) {
    Transform currentTileTransform = transform.Find ("Tile#" + currentTileID);
    TileScript currentTileScript = currentTileTransform.GetComponent<TileScript>();
    //if currentTile is next to a target
    if(currentTileScript.getHValue() < 15) {
      //print ("checkAdjacent:1 First IF");
      transform.Find("Tile#" + (int)targetSquare).GetComponent<TileScript>().setParentNumber(currentTileID);
      return true;
    }

    //print ("checkAdjacent:2 Start.. CHECKING NUMBER: " +currentTileID);
    //adjacentTile will contain what tile# that is ( north, east, south and west ) of start tile. 0 means no tile in that direction
    Vector4[] adjacentTiles = currentTileTransform.GetComponent<TileScript>().getAdjacentTilesNumber();

    closedList.Add(currentTileID);

    if (openList.Contains(currentTileID)) {
      openList.Remove(currentTileID);
    }

    bool[] freeTiles = new bool[4] { false, false, false, false }; // north, east, south, west

    for (int i = 0; i < 2; i++) { // i=0 => north, east, south & west neighbours.. i=1 => diagonal neighbours
      for (int j = 0; j < 4; j++) {
        int tileIDToCheck = (int)adjacentTiles[i][j];
        //TODO: squash these ifs into one.. but keep like this for now, for easier debug purposes
        // check that there is a neighbour in the direction we are currently treating
        if (tileIDToCheck > 0) { // NOTE: that the neighbour id is set to 0 if no neighbour is present
          // check that the tile isn't in the closedList
          if (!closedList.Contains(tileIDToCheck)) {
            Transform tileToCheck = transform.Find("Tile#" + tileIDToCheck);
            TileScript tileToCheckScript = tileToCheck.GetComponent<TileScript>();
            if (tileToCheckScript.getAccessible()) {
              freeTiles[j] = true;
              int currentGValue = currentTileScript.getGValue();
              int tileToCheckHValue = tileToCheckScript.getHValue();
              int newGValue = movementCost[i] + currentGValue;
              int newFValue = newGValue + tileToCheckHValue;

              if (openList.Contains(tileIDToCheck)) {
                if (tileToCheckScript.getFValue() > (newFValue)) {
                  tileToCheckScript.setGValue(newGValue);
                  tileToCheckScript.setParentNumber(currentTileID);
                  tileToCheckScript.setFValue(newFValue);
                }
              } else {
                openList.Add(tileIDToCheck);
                tileToCheckScript.setGValue(newGValue);
                tileToCheckScript.setParentNumber(currentTileID);
                tileToCheckScript.setFValue(newFValue);
              }
            }
          }
        }
      }
    }

    bool[] freeDiagonalTiles = new bool[4] { false, false, false, false }; // {NE, SE, SW, NW}
    if (freeTiles[0]) {
      if (freeTiles[1]) { freeDiagonalTiles[0] = true; } // north-east
      if (freeTiles[3]) { freeDiagonalTiles[3] = true; } // north-west
    }

    if (freeTiles[2]) {
      if (freeTiles[1]) { freeDiagonalTiles[1] = true; } // south-east
      if (freeTiles[3]) { freeDiagonalTiles[2] = true; } // south-west
    }

    return false;
  }

  void findPath() {
    print ("findPathStart");
    closedList = new List<int>();
    openList= new List<int>();

    transform.Find ("Tile#" + startSquare).GetComponent<TileScript> ().setGValue(0);
    isPathFound = checkAdjecentTiles(startSquare);


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
      isPathFound = checkAdjecentTiles(tileWithSmallestFValue);
    }

    shortestPath = new Stack<Vector3>();


    int tempTileID = targetSquare;
    while(tempTileID != startSquare) {
      Transform tempTileTransform = transform.Find("Tile#" + tempTileID);
      tempTileTransform.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.0f, 1.0f, 0.0f, 0.2f));
//      tempTileTransform.GetComponent<Renderer>().material = pathTileMat;
      shortestPath.Push(tempTileTransform.position);
      tempTileID = tempTileTransform.GetComponent<TileScript>().getParentNumber();
    } 
    shortestPath.Push(transform.Find ("Tile#" + tempTileID).position);

//    activateEnemy();

    print(shortestPath.ToString());

    print ("findPatchEnd");

  }

	// Use this for initialization
	void Start () {
    tileCount = 0;
    CreateGrid();
	}
	
	// Update is called once per frame
	void Update () {
    if (Input.GetKeyDown ("space") && !isPathFound) {
      findPath();
    }
	}
}
