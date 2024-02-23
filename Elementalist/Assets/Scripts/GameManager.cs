using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private int score;

    public List<Element> _elements = new List<Element>();

    public Element selectedElement;

    public Exploration_HUD hud;

    private GameObject map;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
    }

    private void Start()
    {
        selectedElement = _elements[0];
    }

    public void LoadMainMenu()
    {
        score = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("StartScreen");
    }

    public void LoadLevel()
    {
        score = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("Level1");
        StartCoroutine(StartLevel());
    }

    private IEnumerator StartLevel()
    {
        yield return new WaitForNextFrameUnit();
        map = GameObject.FindGameObjectWithTag("Map");
        map.GetComponent<ProcGenV3>().OnLevelLoad();
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = false;
        GameObject.FindGameObjectWithTag("Player").transform.position = map.GetComponent<ProcGenV3>().GetStartPos();
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = true;
        
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<Exploration_HUD>();
        StartCoroutine(RemoveLoadingScreen());
    }

    private IEnumerator RemoveLoadingScreen()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        hud.loadingScreen.gameObject.SetActive(false);
        hud.inGameHUD.gameObject.SetActive(true);
    }
    
    public void StartCombat()
    {
        map = GameObject.FindGameObjectWithTag("Map");
            
        GameData levelData = new GameData
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position,
            levelSize = map.GetComponent<ProcGenV3>().GetIds().Length,
            ids = map.GetComponent<ProcGenV3>().GetIds(),
            roomsCompleted = map.GetComponent<ProcGenV3>().GetRoomsCompleted(),
            roomTypes = map.GetComponent<ProcGenV3>().GetRoomTypes()
        };
        DataManager.instance.SaveLevelData(levelData);

        SceneManager.LoadScene("CombatScene");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public IEnumerator ReturnToLevel()
    {
        yield return new WaitForSeconds(2f);
        
        if (score == 4)
        {
            SceneManager.LoadScene("WinScreen");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SceneManager.LoadScene("Level1");

            StartCoroutine(ReloadLevel());
            
            score++;
        }
    }

    private IEnumerator ReloadLevel()
    {
        yield return new WaitForNextFrameUnit();
        GameData levelData = DataManager.instance.LoadLevelData();
        map = GameObject.FindGameObjectWithTag("Map");
        
        // Reinitialize arrays in map with saved arrays
        map.GetComponent<ProcGenV3>().SetIds(levelData.ids);
        map.GetComponent<ProcGenV3>().SetRoomsCompleted(levelData.roomsCompleted);
        map.GetComponent<ProcGenV3>().SetRoomTypes(levelData.roomTypes);
        
        map.GetComponent<ProcGenV3>().GenerateMap(levelData.ids, levelData.roomsCompleted, levelData.roomTypes);

        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<Exploration_HUD>();
        hud.SetScoreText("Enemies Defeated: " + score);
        
        Debug.Log(levelData.playerPos);
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = false;
        GameObject.FindGameObjectWithTag("Player").transform.position = levelData.playerPos;
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>().enabled = true;
        
        StartCoroutine(RemoveLoadingScreen());
    }

    public IEnumerator LoseGame()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("DeathScreen");
    }

    public void AddElement(Element element)
    {
        _elements.Add(element);
    }
    
    public Element GetElement(int i)
    {
        return _elements[i];
    }

    public List<Element> GetElements()
    {
        return _elements;
    }
}
