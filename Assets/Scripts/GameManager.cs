using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    private void Awake()
    {
        if(gm == null) gm = this;
    }

    public enum GameState
    {
        Ready,
        Run,
        Pause,
        GameOver
    }
    public GameState gState;
    public GameObject gameLabel;
    Text gameText;
    PlayerMove player;
    public GameObject gameOption;
    // Start is called before the first frame update
    void Start()
    {
        gState = GameState.Ready;
        Cursor.lockState = CursorLockMode.Locked;
        gameText = gameLabel.GetComponent<Text>();
        gameText.text = "Ready...";
        gameText.color = new Color32(255, 185, 0, 255);

        StartCoroutine(ReadyToStart());

        player = GameObject.Find("Player").GetComponent<PlayerMove>();
    }

    IEnumerator ReadyToStart()
    {
        yield return new WaitForSeconds(2f);
        gameText.text = "3";
        yield return new WaitForSeconds(1f);
        gameText.text = "2";
        yield return new WaitForSeconds(1f);
        gameText.text = "1";
        yield return new WaitForSeconds(1f);
        gameText.text = "Go!";
        yield return new WaitForSeconds(0.5f);

        gameLabel.SetActive(false);
        gState = GameState.Run;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.hp <= 0)
        {
            player.GetComponentInChildren<Animator>().SetFloat("MoveMotion", 0f);
            Cursor.lockState = CursorLockMode.None;
            gameLabel.SetActive(true);
            gameText.text = "Game Over";
            gameText.color = new Color32(255, 0, 0, 255);

            Transform buttons = gameText.transform.GetChild(0);
            buttons.gameObject.SetActive(true);

            gState = GameState.GameOver;
        }
        if(Input.GetButtonDown("Cancel"))
        {
            if(gState == GameState.Pause) CloseOptionWindow();
            else OpenOptionWindow();
        }
    }

    public void OpenOptionWindow()
    {
        gameOption.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        if(gState != GameState.Ready) 
        {
            gState = GameState.Pause;
        }
    }

    public void CloseOptionWindow()
    {
        gameOption.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        if(gState != GameState.Ready)
        {
            gState = GameState.Run;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
