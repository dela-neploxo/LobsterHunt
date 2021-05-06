﻿using Lobster.Entities;
using Lobster.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Lobster
{
    public class GameManager : MonoBehaviour
    {
        private GameManager() {}
        [SerializeField] private static GameManager instance;
        [SerializeField] public static GameManager Instance
        {
            get { return instance;}
        }
        
        [SerializeField]
        private GameObject[] fishCount;
// костыль(то что паблик, решиит валера. Сделать ShopAlert синглтон?) -> 25 строчка в weapon 
        [SerializeField] public ShopAlert shop;
        private bool shopIsOpen;
        private int initialScore;
        public bool GameIsPaused;
        public Player Player;
        public GameObject PauseMenu;
        public GameObject GameOverScreen;
        [FormerlySerializedAs("NextLevelScreen")] public GameObject EndLevelScreen;
        
        private void Awake()
        {
            initialScore = Score.Instance.Amount;
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
            UpdateScore();
        }

        public void Update()
        {
            if (!shop.ShopIsOpen)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (GameIsPaused)
                    {
                        Resume();
                    }
                    else
                    {
                        Pause();
                    }
                }
            }

            if (shop.alertShop)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (!shop.ShopIsOpen)
                    {
                        shop.OpenShop();
                    }
                    else
                    {
                        shop.CloseShop();
                    }
                }
            }

            if (!Player.IsBreathing || Player.IsDead)
            {
                ShowGameOverScreen();
            }

            fishCount = GameObject.FindGameObjectsWithTag("Fish");


            if (SceneManager.GetActiveScene().name == "LastLevel" && fishCount.Length == 0)
            {
                ShowWinScreen();
                return;
            }

            if (fishCount.Length == 0)
            {
                ShowNextLevelScreen();
            }
        }

        public void OnDestroy()
        {
            Time.timeScale = 1f;
            GameIsPaused = false;
        }

        public void Resume()
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Pause()
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void LoadMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void LoadNextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void ShowGameOverScreen()
        {
            PauseMenu.SetActive(false);
            GameOverScreen.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            GameIsPaused = true;
        }

        public void RestartGame()
        {
            Score.Instance.Amount = initialScore;
            Score.Instance.Deaths += 1;
            GameOverScreen.SetActive(false);
            GameIsPaused = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ShowNextLevelScreen()
        {
            if (Time.timeScale != 0)
            {
                Score.Instance.TimeSpent += Time.timeSinceLevelLoad;
            }
            PauseMenu.SetActive(false);
            EndLevelScreen.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            GameIsPaused = true;
        }

        public void ShowWinScreen()
        {
            if (Time.timeScale != 0)
            {
                Score.Instance.TimeSpent += Time.timeSinceLevelLoad;
            }
            PauseMenu.SetActive(false);
            EndLevelScreen.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            GameIsPaused = true;
            ShowStats();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void ShowStats()
        {
            var deaths = GameObject.Find("Deaths");
            deaths.GetComponent<Text>().text = "Deaths: " + Score.Instance.Deaths;
            var timeSpent = GameObject.Find("TimeSpent");
            timeSpent.GetComponent<Text>().text = "Time spent: " + Score.Instance.TimeSpent;
            var fishKilled = GameObject.Find("FishKilled");
            fishKilled.GetComponent<Text>().text = "Fish killed: " + Score.Instance.Fishkilled;
        }
        public static void UpdateScore()
        {
            var txt = GameObject.Find("ScoreText");
            txt.GetComponent<TextMeshProUGUI>().text = "Score: " + Score.Instance.Amount;
        }
    }
}