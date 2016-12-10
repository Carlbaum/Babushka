using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum gameState { Running, Paused, GameOver, Victorious };
public enum enemyTypes { Chicken, Protector };
public enum towerTypes { Standard, AreaDamage };

public class GameController : MonoBehaviour {
  //PRIVATE VARIABLES

  public bool buildMode = false; // TODO: enumerator? GameState?


  private Camera camera;

  private List<Enemy> enemies;

  Vector3 spawnPoint  = new Vector3(-8, 0, 0);
  Vector3 targetPoint = new Vector3( 8, 0, 0);

  private static MyGui gui;

  // PUBLIC VARIABLES
  
  public EnemyHandler enemyHandler;
  public gameState state;

  public int playerHealth = 25;
  public static int killScore = 0;
  public int enemiesOnTheBoard = 0;
  public int cash = 1337;

//  public Transform grid;
//  public LayerMask gridLayer;

  public float cameraSpeed = 10.0f;
  public float cameraRotSpeed = 50.0f;

  public Enemy[] EnemyType;

  public GridHandlerNew gridHandler;


  // ---------FUNCTIONS----------

  void Start() {
    camera = FindObjectOfType<Camera>();
    gui = FindObjectOfType<MyGui>();
    gridHandler = FindObjectOfType<GridHandlerNew>();
    state = gameState.Running;
    enemies = new List<Enemy>();
    gui.setMoney(cash);
    gui.setScore(killScore);
    gui.setPlayerHealth(playerHealth);
    gui.setEnemiesLeft(enemiesOnTheBoard);
    gui.setGridVisibility(false);
  }

  void Update() {
//    print("current state: " + state);
    switch (state) {
      case gameState.Running:
        if (playerHealth <= 0) {
          GameOver();
        }
        runningStateKeyEvents();
        if (enemiesOnTheBoard > 0) {
          enemyHandler.handleBoids(ref enemies);
        }
        if (Input.GetKeyDown ("space")) {
          if (gridHandler.isPathFound) {
            gridHandler.isPathFound = false;
            gridHandler.ResetGrid();
          }
          gridHandler.FindPath();
        }
        break;
      case gameState.Paused:
        if (Input.GetKeyDown("escape")) {
          print ("Resuming game...");
          Time.timeScale = 1;
          changeState(gameState.Running);
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

  public void EnemyKilled(GameObject killedEnemy) {
    killScore++;
    cash += 10;
    gui.setMoney(cash);
    gui.setScore(killScore);
    gui.setEnemiesLeft(--enemiesOnTheBoard);
    enemies.Remove(killedEnemy.GetComponentInParent<Enemy>());
    Destroy(killedEnemy); 
  }

  public void EnemyReachedTarget(GameObject missedEnemy) {
    gui.setEnemiesLeft(--enemiesOnTheBoard);
    enemies.Remove(missedEnemy.GetComponentInParent<Enemy>());
    Destroy(missedEnemy); 
  }

  public void addToPlayerHealth(int hp) {
    playerHealth += hp;
    gui.setPlayerHealth(playerHealth);
  } 

  public void addToEnemiesOnTheBoard(int term) {
    enemiesOnTheBoard += term;
    gui.setEnemiesLeft(enemiesOnTheBoard);
  }

  public void GameOver() {
    print("You Lose!");
    changeState(gameState.GameOver);
  }

  private void runningStateKeyEvents() {
    if (Input.GetKeyUp(KeyCode.K)) {
      print("KILL BOID");
      print("enemies length pre: " + enemies.Count);
      enemies[0].EnemyDeath();
      print("enemies length post: " + enemies.Count);
    }
    if (Input.GetKeyUp(KeyCode.J)) {
      print("Add BOID");
      print("enemies length pre: " + enemies.Count);
      enemyHandler.spawnEnemies(1, enemyTypes.Chicken, ref enemies);
      enemiesOnTheBoard += 1;
      gui.setEnemiesLeft(enemiesOnTheBoard);
      print("enemies length post: " + enemies.Count);
    }
    if (Input.GetKeyUp(KeyCode.B)) {
      print("PLACE TOWER");
      changeBuildMode();
    }
    if (Input.GetKeyDown("escape")) {
      print("PAUSE GAME");
      print("");
      Time.timeScale = 0;
      changeState(gameState.Paused);
    }
    if (Input.GetKeyDown("return")) {
      enemyHandler.spawnEnemies(5, enemyTypes.Protector, ref enemies);
      enemyHandler.spawnEnemies(1, enemyTypes.Chicken, ref enemies);
      enemiesOnTheBoard += 6;
      gui.setEnemiesLeft(enemiesOnTheBoard);
      //          spawnEnemies(7, enemyTypes.Chicken);
      //          gui.setPlayerHealth();
    }
    if (Input.GetKey(KeyCode.RightArrow)) {
      if (Input.GetKey(KeyCode.LeftAlt)) {
        camera.transform.Rotate(Vector3.up * cameraRotSpeed * Time.deltaTime, Space.World);
      } else {
        camera.transform.Translate(Vector3.right * cameraSpeed * Time.deltaTime, Space.World);
      }
    }
    if (Input.GetKey(KeyCode.LeftArrow)) {
      if (Input.GetKey(KeyCode.LeftAlt)) {
        camera.transform.Rotate(Vector3.down * cameraRotSpeed * Time.deltaTime, Space.World);
      } else {
        camera.transform.Translate(Vector3.left * cameraSpeed * Time.deltaTime, Space.World);
      }
    }
    if (Input.GetKey(KeyCode.DownArrow)) {
      if (Input.GetKey(KeyCode.LeftAlt)) {
        camera.transform.Translate(Vector3.back * cameraSpeed * Time.deltaTime);
      } else {
        camera.transform.Translate(Vector3.back * cameraSpeed * Time.deltaTime, Space.World);
      }
    }
    if (Input.GetKey(KeyCode.UpArrow)) {
      if (Input.GetKey(KeyCode.LeftAlt)) {
        camera.transform.Translate(Vector3.forward * cameraSpeed * Time.deltaTime);
      } else {
        camera.transform.Translate(Vector3.forward * cameraSpeed * Time.deltaTime, Space.World);
      }
    }
  }

  public void changeBuildMode() {
    buildMode = buildMode ? false : true;
    gui.buildMode = buildMode;
    gui.setGridVisibility(buildMode);
  }

  public void changeState(gameState newState) {
    state = newState;
    gui.state = state;
  }
}