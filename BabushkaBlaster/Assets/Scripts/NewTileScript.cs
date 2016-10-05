using UnityEngine;
using System.Collections;

public class NewTileScript : MonoBehaviour {

	public int tileNumber;
	public bool accessible;

	int westNeighbour, eastNeighbour, northNeighbour, southNeighbour;
	int northEastNeighbour, southWestNeighbour, southEastNeighbour, northWestNeighbour;

	Vector2 coordinates;
	public int hValue, gValue, fValue;
	public int parentNumber;

	void Start () {
		
	}

	public Vector2 getCoordinates(){
		return coordinates;
	}

	public void setCoordinates(Vector2 coordVec ){
		coordinates = coordVec;
	}

	public void setHValue(int newH ){
		hValue = newH;
		transform.Find("hValueText").GetComponent<TextMesh>().text = hValue.ToString();
	}

	public int getHValue() {
		return hValue;
	}

	public void setParentNumber(int newParentNumber ){
		parentNumber = newParentNumber;
		transform.Find("parentText").GetComponent<TextMesh>().text = parentNumber.ToString();
		
	}
	
	public int getParentNumber() {
		return parentNumber;
	}

	public void setGValue(int newG ){
		gValue = newG;
		transform.Find("gValueText").GetComponent<TextMesh>().text = gValue.ToString();
	}
	
	public int getGValue() {
		return gValue;
	}

	public void setFValue(int newF ){
		fValue = newF;
		transform.Find("fValueText").GetComponent<TextMesh>().text = fValue.ToString();
	}
	
	public int getFValue() {
		return fValue;
	}

	public Vector4[] getAdjacentTilesNumber() {
		return new Vector4[2]{new Vector4(northNeighbour,eastNeighbour,southNeighbour,westNeighbour), 
		                      new Vector4(northEastNeighbour, southEastNeighbour, southWestNeighbour, northWestNeighbour)};
	}

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

}
