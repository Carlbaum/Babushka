using UnityEngine;
using System.Collections;

public class TileScript : MonoBehaviour {
  // PUBLIC VARIABLES
  int westNeighbour, eastNeighbour, northNeighbour, southNeighbour;
  int northEastNeighbour, southWestNeighbour, southEastNeighbour, northWestNeighbour;

  Vector2 coordinates;
  public int hValue, gValue, fValue;
  public int parentNumber;

  // PRIVATE VARIABLES
  private int tileID;
  private bool isAccessible;

  // GETTER FUNCTIONS
  public int getHValue() { return hValue; }
  public int getParentNumber() { return parentNumber; }
  public int getGValue() { return gValue; }
  public int getFValue() { return fValue; }
  public int getTileID() { return tileID; }
  public bool getAccessible() { return isAccessible; }
  public Vector2 getCoordinates(){ return coordinates; }
  public Vector4[] getAdjacentTilesNumber() {
    return new Vector4[2]{new Vector4(northNeighbour,eastNeighbour,southNeighbour,westNeighbour), 
      new Vector4(northEastNeighbour, southEastNeighbour, southWestNeighbour, northWestNeighbour)};
  }

  // SETTERS
  public void setTileID( int id ) { tileID = id; }
  public void setCoordinates() { coordinates = new Vector2(transform.position.x,transform.position.z); }
  public void setHValue( int newH ){ hValue = newH;
    transform.Find("hValueText").GetComponent<TextMesh>().text = hValue.ToString(); // TODO: COMPLETELY REMOVE THIS FROM PREFAB, ONLY FOR DEBUGGING
  }
  public void setParentNumber(int newParentNumber ){ parentNumber = newParentNumber;
    transform.Find("parentText").GetComponent<TextMesh>().text = parentNumber.ToString();  // TODO: COMPLETELY REMOVE THIS FROM PREFAB, ONLY FOR DEBUGGING
  }
  public void setGValue( int newG ){ gValue = newG;
    transform.Find("gValueText").GetComponent<TextMesh>().text = gValue.ToString();  // TODO: COMPLETELY REMOVE THIS FROM PREFAB, ONLY FOR DEBUGGING
  }
  public void setFValue( int newF ){ fValue = newF;
    transform.Find("fValueText").GetComponent<TextMesh>().text = fValue.ToString();  // TODO: COMPLETELY REMOVE THIS FROM PREFAB, ONLY FOR DEBUGGING
  }
  public void setAccessible( bool accessible ) { isAccessible = accessible; }
  public void setAdjacent(Vector4 adjacentTiles) {
    northNeighbour = (int) adjacentTiles.x;
    eastNeighbour  = (int) adjacentTiles.y;
    southNeighbour = (int) adjacentTiles.z;
    westNeighbour  = (int) adjacentTiles.w;

    if (northNeighbour > 0 && eastNeighbour > 0) {
      northEastNeighbour = northNeighbour + 1;
    }

    if (southNeighbour > 0 && eastNeighbour > 0) {
      southEastNeighbour = southNeighbour + 1;
    }

    if (southNeighbour > 0 && westNeighbour > 0) {
      southWestNeighbour = southNeighbour - 1;
    }

    if (northNeighbour > 0 && westNeighbour > 0) {
      northWestNeighbour = northNeighbour - 1;
    }
  }

	void Start () {
    
	}

  void Update () {
    
  }
}
