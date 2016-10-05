using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NewGenerateGrid : MonoBehaviour {

	public Transform TilePrefab,ObstaclePrefab;

	public int 	columns = 6, 
				rows = 7,
				tileCount;
	public int startSquare, targetSquare;

	//public int amountOfObstacles;
	public int[] obstacles;
	Vector2 destinationCoords,startCoords;
	int[] movementCost = new int[2] {10, 14};
	public bool foundPath = false;

	List<int> closedList, openList;

	public Material startTileMat, targetTileMat, occupiedTileMat, pathTileMat;
	
	Stack<Vector3> shortestPath;

	void Start () {
		tileCount = 0;
		CreateGrid ();
	}

	void CreateGrid() {
		for (int r = 0; r < rows; r++) {
			for (int c = 0; c < columns; c++) {
				Transform newGO = Instantiate(TilePrefab, new Vector3(c ,0,-r), Quaternion.Euler(new Vector3(90, 0, 0)) ) as Transform;
				++tileCount;
				
				newGO.transform.parent = transform;
				newGO.name = "Tile#"+tileCount;
				newGO.GetComponentInChildren<NewTileScript>().tileNumber = (tileCount);
				newGO.GetComponentInChildren<NewTileScript>().setCoordinates(new Vector2(r+1,c+1));
				//newGO.GetComponentInChildren<TextMesh>().text = tileCount.ToString();
				newGO.Find("idText").GetComponent<TextMesh>().text = tileCount.ToString();
				newGO.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 0.5f,0.0f,0.4f));
				newGO.GetComponent<NewTileScript>().accessible = true;
				//newGO.renderer.enabled = false;
				int north = 0, east = 0, south=0, west=0;
				if(r > 0)
					north = tileCount-columns;
				if(c < columns-1)
					east = tileCount+1;
				if(r < rows-1)
					south = tileCount+columns;
				if(c > 0)
					west = tileCount-1;
//				print (north + "  "  + east + "   " + south + "  " + west);
				newGO.GetComponent<NewTileScript>().setAdjacent(new Vector4(north,east,south,west));

			}		
		}
		this.transform.Translate(new Vector3(-columns/2,0,rows/2));

		if (startSquare == 0) {
			startSquare = 24;
			startCoords = transform.Find ("Tile#" + startSquare).GetComponent<NewTileScript> ().getCoordinates ();
		}

		if (targetSquare == 0) {
			targetSquare = columns * rows-columns/2;
		}
		
		transform.Find ("Tile#" + startSquare).GetComponent<Renderer>().material = startTileMat	;
		transform.Find ("Tile#" + targetSquare).GetComponent<Renderer>().material = targetTileMat;
		//		destinationCoords = new Vector2(rows + 4, Mathf.Ceil(columns/2));
//		destinationCoords = new Vector2(rows + 4, Mathf.Ceil(columns/2));
		destinationCoords = transform.Find ("Tile#" + targetSquare).GetComponent<NewTileScript> ().getCoordinates ();

		//obstacles = new int[amountOfObstacles];

		foreach (int i in obstacles) {
			Transform temp = transform.Find ("Tile#" + i);
			temp.GetComponent<Renderer>().material = occupiedTileMat;
			temp.GetComponent<NewTileScript>().accessible = false;
//			Transform newGO = Instantiate(ObstaclePrefab, new Vector3(temp.GetComponent<NewTileScript>().getCoordinates().y-columns/2-1,0,rows/2+1-temp.GetComponent<NewTileScript>().getCoordinates().x), Quaternion.Euler(new Vector3(0, 0, 0)) ) as Transform;
		}

		calculateHeuristicValue();
	}

	void OnMouseDown() {
		transform.Translate (new Vector3 (1, 6, 2));
	}

	void calculateHeuristicValue() {

		foreach (Transform tileChild in transform) {
			//Debug.Log(tileChild.name);
			Vector2 curPos = tileChild.GetComponent<NewTileScript> ().getCoordinates ();
			tileChild.GetComponent<NewTileScript>().setHValue((int)(Mathf.Abs(curPos.x-destinationCoords.x)+Mathf.Abs(curPos.y-destinationCoords.y)));
//			tileChild.GetComponent<NewTileScript>().setHValue((int)(Mathf.Abs(curPos.x-destinationCoords.x)+Mathf.Abs(curPos.y-destinationCoords.y)));
		//	tileChild.Find("hValueText").GetComponent<TextMesh>().text = tileCount.ToString();
			
			//Debug.Log ("max(abs(dx),abs(dy)): " + ((int)(Mathf.Abs(curPos.x-destinationCoords.x)+Mathf.Abs(curPos.y-destinationCoords.y))));

		}
	}
    /*void calculateMovementCosts() {
		
		foreach (Transform tileChild in transform) {
			Debug.Log(tileChild.name);
			Vector2 curPos = tileChild.GetComponent<NewTileScript> ().getCoordinates ();
			tileChild.GetComponent<NewTileScript>().setGValue((int)(Mathf.Abs(curPos.x-startCoords.x)+Mathf.Abs(curPos.y-startCoords.y)));
			//	tileChild.Find("gValueText").GetComponent<TextMesh>().text = tileCount.ToString();
			
			//Debug.Log ("max(abs(dx),abs(dy)): " + ((int)(Mathf.Abs(curPos.x-startCoords.x)+Mathf.Abs(curPos.y-startCoords.y))));
			
		}
	}*/
    int checkAdjecentTiles(int currentTile)
    {
        //print ("checkAdjacent:1 Start.. CHECKING NUMBER: " +currentTile);
        //adjacentTile will contain what tile# that is ( north, east, south and west ) of start tile. 0 means no tile in that direction
        Vector4[] adjacentTiles = transform.Find("Tile#" + currentTile).GetComponent<NewTileScript>().getAdjacentTilesNumber();
//        	print ("The current tile is tile#" + currentTile + 
//                   "\nAbove it       is tile#" + adjacentTiles[0].x +
//                   "\nNorth-east     is tile#" + adjacentTiles[1].x +
//                   "\nTo the right   is tile#" + adjacentTiles[0].y +
//                   "\nSouth-east     is tile#" + adjacentTiles[1].y +
//                   "\nBelow  it      is tile#" + adjacentTiles[0].z +
//                   "\nSouth-west     is tile#" + adjacentTiles[1].z +
//                   "\nTo the left    is tile#" + adjacentTiles[0].w + 
//                   "\nNorth-west     is tile#" + adjacentTiles[1].w);

        //if currentTile is next to a target // should maybe be changed to last row
        /*if(transform.Find ("Tile#" + currentTile).GetComponent<NewTileScript>().getHValue() <= 1) {
			//print ("checkAdjacent:2 First IF");
			transform.Find ("Tile#" + (int)targetSquare).GetComponent<NewTileScript>().setParentNumber(currentTile);
			foundPath = true;
			return;
		}*/


        closedList.Add(currentTile);

        if (openList.Contains(currentTile))
        {
            //print ("checkAdjacent:3 second IF");
            openList.Remove(currentTile);
        }
        bool[] freeTiles = new bool[4] { false, false, false, false }, freeDiagonalTiles = new bool[4] { true, true, true, true };

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                //print (i+"checkAdjacent:4 for-loop");

                //check... 
                //...if there is a tile in the direction, 
                //...that the tile isn't in the closed list
                //...that the tile is accessible	
                //print ("freeDiagonalTiles[j] = " + freeDiagonalTiles[j]);
                //print ("adjacentTiles[i][j]  = " + adjacentTiles[i][j]);
                if (freeDiagonalTiles[j] && adjacentTiles[i][j] > 0 && !closedList.Contains((int)adjacentTiles[i][j]) && transform.Find("Tile#" + (int)adjacentTiles[i][j]).GetComponent<NewTileScript>().accessible)
                {
                    //print (i+"checkAdjacent:5 if in for");
                    Transform tileToCheck = transform.Find("Tile#" + (int)adjacentTiles[i][j]);

                    freeTiles[j] = true;

					if (tileToCheck.GetComponent<NewTileScript>().getHValue() == 1) {
                        //print (i+"checkAdjacent:6 first if in if in for");
                        tileToCheck.GetComponent<NewTileScript>().setParentNumber(currentTile);
                        //transform.Find ("Tile#" + (int)targetSquare).GetComponent<NewTileScript>().setParentNumber((int)adjacentTiles[i][j]);
                        foundPath = true;
                        return (int)adjacentTiles[i][j];
                    }

                    int curGValue = transform.Find("Tile#" + currentTile).GetComponent<NewTileScript>().getGValue();
                    int tileToCheckHValue = tileToCheck.GetComponent<NewTileScript>().getHValue();

                    if (openList.Contains((int)adjacentTiles[i][j])) {
                        //print (i+"checkAdjacent:7 scnd if in if in for");
                        if (tileToCheck.GetComponent<NewTileScript>().getFValue() > (movementCost[i] + curGValue + tileToCheckHValue)) {
                            //print (i+"checkAdjacent:8 if in scnd if in if in for");
                            tileToCheck.GetComponent<NewTileScript>().setGValue(curGValue + movementCost[i]);
                            tileToCheck.GetComponent<NewTileScript>().setParentNumber(currentTile);
                            tileToCheck.GetComponent<NewTileScript>().setFValue(movementCost[i] + curGValue + tileToCheckHValue);
                        }
                    } else {
                        //print (i+"checkAdjacent:9 else in if in for");
                        openList.Add((int)adjacentTiles[i][j]);
                        tileToCheck.GetComponent<NewTileScript>().setGValue(curGValue + movementCost[i]);
                        tileToCheck.GetComponent<NewTileScript>().setParentNumber(currentTile);
                        tileToCheck.GetComponent<NewTileScript>().setFValue(movementCost[i] + curGValue + tileToCheckHValue);
                        //print (i+"checkAdjacent:10 new item in open list");
                    }
                }

            }
            freeDiagonalTiles = new bool[4] { false, false, false, false };
            if (freeTiles[0])
            {
                if (freeTiles[1])
                    freeDiagonalTiles[0] = true;
                if (freeTiles[3])
                    freeDiagonalTiles[3] = true;
            }

            if (freeTiles[2])
            {
                if (freeTiles[1])
                    freeDiagonalTiles[1] = true;
                if (freeTiles[3])
                    freeDiagonalTiles[2] = true;
            }
            //print (i+"checkAdjacent:11 freeDiagonalTiles\n"+freeDiagonalTiles[0]+"\n"+freeDiagonalTiles[1]+"\n"+freeDiagonalTiles[2]+"\n"+freeDiagonalTiles[3]+"\n");
        }

        //print ("checkAdjacent:12 End");
        return 0;
    }

    void findPath() {
		/*
		using System.Collections.Generic;
		List<type> name = new List<type>();
		 */
		print ("findPathStart");
		closedList = new List<int>();
		openList= new List<int>();

		transform.Find ("Tile#" + startSquare).GetComponent<NewTileScript> ().setGValue(0);
		checkAdjecentTiles(startSquare);


		while (foundPath == false) {
            int tileWithSmallestFValue = openList[0];
			int smallestFValue = transform.Find ("Tile#" + openList[0]).GetComponent<NewTileScript>().getFValue();

			foreach(int i in openList) {
				//print ("i = " + i + "\nsmallest FV: " + smallestFValue + "is in Tile #" + tileWithSmallestFValue);
				if(transform.Find ("Tile#" + i).GetComponent<NewTileScript>().getFValue() < smallestFValue) {
					smallestFValue = transform.Find ("Tile#" + i).GetComponent<NewTileScript>().getFValue();
					tileWithSmallestFValue = i;
				}
			}
			checkAdjecentTiles(tileWithSmallestFValue);	
		}
		
		shortestPath = new Stack<Vector3>();
		

		int tempTile = targetSquare;  //TODO: change this row
		while(tempTile != startSquare) {
			transform.Find ("Tile#" + tempTile).GetComponent<Renderer>().material = pathTileMat;
        	shortestPath.Push(transform.Find ("Tile#" + tempTile).position);
			tempTile = transform.Find ("Tile#" + tempTile).GetComponent<NewTileScript>().getParentNumber();
		}	
		shortestPath.Push(transform.Find ("Tile#" + tempTile).position);
		
		activateEnemy();
		
		print(shortestPath.ToString());

		print ("findPatchEnd");


	}


	
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space") && foundPath == false) {
			//print ("space key was pressed");
			findPath();
		}
		/*if(walk) {
			
		}*/
	}
	
	void activateEnemy() {
		//walk = true	;
	}
	
	public Stack<Vector3> getShortestPath(){
		return shortestPath;
	}
	
	/*

	Vector3[] createSquareCoordinates(float size) {
		float s = size/2.0f;
		Vector3[] temp = new Vector3[4];
		temp[0] = new Vector3(-s, -s, 0.0f);
		temp[1] = new Vector3( s, -s, 0.0f);
		temp[2] = new Vector3( s,  s, 0.0f);
		temp[3] = new Vector3(-s,  s, 0.0f);

		return temp;
	}*/
}
/*
http://answers.unity3d.com/questions/139808/creating-a-plane-mesh-directly-from-code.html
var size : float;
function Awake() {
	var m : Mesh = new Mesh();
	m.name = "Scripted_Plane_New_Mesh";
	m.vertices = [Vector3(-size, -size, 0.01), Vector3(size, -size, 0.01), Vector3(size, size, 0.01), Vector3(-size, size, 0.01) ];
	m.uv = [Vector2 (0, 0), Vector2 (0, 1), Vector2(1, 1), Vector2 (1, 0)];
	m.triangles = [0, 1, 2, 0, 2, 3];
	m.RecalculateNormals();
	var obj : GameObject = new GameObject("New_Plane_Fom_Script", MeshRenderer, MeshFilter, MeshCollider);
	obj.GetComponent(MeshFilter).mesh = m;
}*/