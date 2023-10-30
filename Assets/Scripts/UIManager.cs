using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Image _LivesImg;
    [SerializeField]
    private TMP_Text _gameOverText;
    [SerializeField]
    private TMP_Text _restartText;
    [SerializeField]
    private TMP_Text _ammoText;
    [SerializeField]
    private TMP_Text _gameOverWinText;

    [SerializeField]
    private TMP_Text[] _waves; 

    private int _currentAmmo = 15;
    private int _maxAmmo = 15;
    
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + _currentAmmo + "/" + _maxAmmo;
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _gameOverText.gameObject.SetActive(false);
        _gameOverWinText.gameObject.SetActive(false);
        SetWavePos();

        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateAmmo(int playerAmmo, int _maxAmmo)
    {
        _ammoText.text = "Ammo: " + playerAmmo.ToString() + "/" + _maxAmmo;
    }

    private void SetWavePos()
    {
        for (int i = 0; i < _waves.Length; i++) 
        {
            _waves[i].gameObject.SetActive(false);
        }
    }

    public void WaveOneUI()
    {
        _waves[0].gameObject.SetActive(true);
    }

    public void WaveTwoUI() 
    {
        _waves[1].gameObject.SetActive(true);
        _waves[0].gameObject.SetActive(false);
    }

    public void WaveThreeUI() 
    {
        _waves[2].gameObject.SetActive(true);
        _waves[1].gameObject.SetActive(false);
    }

    public void WaveFourUI() 
    {
        _waves[3].gameObject.SetActive(true);
        _waves[2].gameObject.SetActive(false);
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlicker());
        _restartText.gameObject.SetActive(true);
    }

    public void GameOverWinner()
    {
        _gameManager.GameWinner();
        _gameOverWinText.gameObject.SetActive(true);
        _gameOverText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlicker());
        _restartText.gameObject.SetActive(true);
    }

    IEnumerator GameOverFlicker()
    {
        while (true)
        {
            _gameOverText.enabled = true;
            yield return new WaitForSeconds(0.5f);
            _gameOverText.enabled = false;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
