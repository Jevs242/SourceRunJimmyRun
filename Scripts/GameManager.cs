using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Component Reference")]
    [SerializeField] private GameObject _player;

    [Header("UI")]
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameObject _menuScreen;
    [SerializeField] private GameObject _gameScreen;
    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private Slider _resistenceSlider;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _ScoreText;

    [Header("Sound")]
    [SerializeField] private AudioSource _menuSong;
    [SerializeField] private AudioSource _gameSound;
    [SerializeField] private AudioClip _endSong;
    [SerializeField] private AudioClip _actionSound;
    [SerializeField] private AudioClip _deadSound;

    [Header("Actions")]
    private bool _beginPlay = false;
    private bool _endPlay = false;
    private float _distance = 0;
    private int _lastRandom = -1;
    private float _highRecord = 0;
    private int _currentNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        //_player.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        _menuScreen.gameObject.SetActive(true);
        _highRecord = PlayerPrefs.GetFloat("HighRecord");
    }

    private void Update()
    {
        if (!GetBeginPlay()) return;
        _distance += Time.deltaTime * (int)_player.gameObject.GetComponent<Player>().speed;
        SetUI();

        if ((int)_distance % 200 == 0 && (int)_distance != _currentNumber)
        {
            _currentNumber = (int)_distance;
            print((int)_distance);
            _player.gameObject.GetComponent<Player>().nowSpeed += 1.0f;
            _player.gameObject.GetComponent<Player>().nowSpeed = Math.Clamp(_player.gameObject.GetComponent<Player>().nowSpeed, 8, 10);
        }
    }

    public void BeginPlay()
    {
        ResumeGame();
        _gameScreen.gameObject.SetActive(true);
        _beginPlay = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        _beginPlay = false;
    }

    public void ResumeGame()
    {
        _beginPlay = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }

    public void IsDead()
    {
        if (_endPlay) return;
        _endPlay = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _menuSong.clip = _endSong;
        _menuSong.Play();
        _gameOverScreen.SetActive(true);
        _player.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        _gameScreen.gameObject.SetActive(false);
        Player player = _player.gameObject.GetComponent<Player>();
        player.enabled = false;
        player.speed = 0;
        player.PlayParticule();
        
        if(PlayerPrefs.GetFloat("HighScore") < _distance)
        {
            PlayerPrefs.SetFloat("HighScore", _distance);
        }
        _ScoreText.text = $"Score : {string.Format(String.Format("{0:N0}", (int)_distance))}";
        _highScoreText.text = $"HighScore : {string.Format(String.Format("{0:N0}", (int)PlayerPrefs.GetFloat("HighScore")))}";
    }

    private void SetUI()
    {
        _distanceText.text = $"{string.Format(String.Format("{0:N0}", (int)_distance))}";
        _resistenceSlider.value = _player.gameObject.GetComponent<Player>().GetPercentResistence();
    }

    public void SetPause()
    {
        if(_gameOverScreen.activeSelf || _menuScreen.activeSelf ) { return; }
        _gameScreen.gameObject.SetActive(false);
        PauseGame();
        _pauseScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetLastRandom(int random)
    {
        _lastRandom = random;
    }

    public int GetLastRandom()
    {
        return _lastRandom;
    }

    public bool GetBeginPlay()
    {
        return _beginPlay;
    }

    public void PlayActionSound()
    {
        _gameSound.clip = _actionSound;
        _gameSound.Play();
    }
    public void PlayDeadSound()
    {
        _gameSound.clip = _deadSound;
        _gameSound.Play();
    }
}
