﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        GameOver
    }
    public GameState gState;
    public GameObject gameLabel;
    Text gameText;
    PlayerMove player;
    // Start is called before the first frame update
    void Start()
    {
        gState = GameState.Ready;
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
            gameLabel.SetActive(true);
            gameText.text = "Game Over";
            gameText.color = new Color32(255, 0, 0, 255);
            gState = GameState.GameOver;
        }
    }
}
