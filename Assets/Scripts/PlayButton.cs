using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    Button playBtn;
    GamePlay game;

    void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GamePlay>();
        // disables gameplay script
        game.enabled = false;

        playBtn = GetComponent<Button>();
        playBtn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        // get selected level data
        List<string> levelData = transform.parent.GetComponentInChildren<LevelSelect>().GetSelectedLevelData();

        // start game
        game.enabled = true;
        game.StartGame(levelData);

        // hide main menu
        transform.parent.gameObject.SetActive(false);
    }
}
