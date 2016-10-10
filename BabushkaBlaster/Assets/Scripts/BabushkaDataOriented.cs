using UnityEngine;
using System.Collections;

public class BabushkaDataOriented : MonoBehaviour {
  struct MyGrid {
    public int sizeX;
    public int sizeY;
    public Vector2[] tilePosition;
    public bool[] isAccessibleTile;
  }

  public int numColumns = 7;
  public int numRows = 11;
  public Vector2 startCoordinates;
  public Vector2 goalCoordinates;
  public float tileWidth = 1;
  public float tileHeight = 1;

  MyGrid grid;


  void Start () {
    grid.sizeX = numColumns;
    grid.sizeY = numRows;
    grid.tilePosition = new Vector2[numColumns * numRows];
    grid.isAccessibleTile = new bool[numColumns * numRows];

    float offsetX = -tileWidth * (numColumns-1)/2.0f;
    float offsetY = tileHeight * (numRows-1) /2.0f;
    for (int row = 0; row < numRows; row++) {
      for (int column = 0; column < numColumns; column++) {
        int index = row * numColumns + column;
//        print(index + "\n");
        grid.tilePosition[index] = new Vector2(column * tileWidth + offsetX, -row * tileHeight + offsetY );
//        print(grid.tilePosition[index] + "\n");
      }
    }

  }
}