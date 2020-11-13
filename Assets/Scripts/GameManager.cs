using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

   public static GameManager instance;
   public float turnDelay = 0.1f;
   public float levelStartDelay = 2f;
   public bool doingSetup;

   public BoardManager boardScript;
   public int playerFoodPoints = 100;
   [HideInInspector] public bool playersTurn = true;

   private List<Enemy> enemies = new List<Enemy>();
   private bool enemiesMoving;

   private int level = 0;
   private GameObject leveImage;
   private Text levelText;
  

   private void Awake()
   {
       if(GameManager.instance == null)
       {
           GameManager.instance = this;
       }else if(GameManager.instance != this)
       {
           Destroy(gameObject);
       }

      DontDestroyOnLoad(gameObject);

      boardScript = GetComponent<BoardManager>();
   }

   void InitGame()
   {
       doingSetup = true;
       leveImage = GameObject.Find("LeveImage");
       levelText = GameObject.Find("LeveText").GetComponent<Text>();
       levelText.text = "Dia " + level;
       leveImage.SetActive(true);

       enemies.Clear();
       boardScript.SetupScene(level);


       Invoke("HideLevelImage", levelStartDelay);
   }

   private void HideLevelImage()
   {
       leveImage.SetActive(false);
       doingSetup = false;
   }

   public void GameOver()
   {
       levelText.text = "despues de   " + level + " dias de hambre, te moriste."; 
       leveImage.SetActive(true);
       enabled = false;
   }

   //corrutina de mover los enemigos por turnos
   IEnumerator MoveEnemies()
   {
       enemiesMoving = true;
       yield return new WaitForSeconds(turnDelay);
       if(enemies.Count == 0)
       {
           yield return new WaitForSeconds(turnDelay);
       }
       for(int i=0; i<enemies.Count; i++)
       {
           enemies[i].MoveEnemy();
           yield return new WaitForSeconds(enemies[i].moveTime);
       }
       playersTurn = true;
       enemiesMoving = false;
   }

   private void Update()
   {
       if(playersTurn || enemiesMoving || doingSetup) return;

       StartCoroutine(MoveEnemies());
   }

   public void AddEnemyToList(Enemy enemy)
   {
       enemies.Add(enemy);
   }

   private void OnEnable()
   {
       SceneManager.sceneLoaded += OnLevelFinishedLoading;
   }

   private void OnDisable()
   {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
   }

   private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
   {
       level++;
       InitGame();
   }
}
