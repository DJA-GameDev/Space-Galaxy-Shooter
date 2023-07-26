using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.3f;
    [SerializeField]
    private float _canFire = -1.0f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;

    [SerializeField]
    private int _score;

    private bool _isTripleShotActive = false;
    private bool _isShieldActive = false;

    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if ( _spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }

        if ( _uiManager == null) 
        {
            Debug.LogError("The UI Manager is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source on the player is NULL");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }
    }

    // Update is called once per frame

    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        
        transform.Translate(_speed * Time.deltaTime * direction);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    public void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        { 
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();
    }
    
    public void Damage()
    {
       
        if (_isShieldActive == true) 
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }
        
        _lives --;

        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1 ) 
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDown());
    }

    IEnumerator TripleShotPowerDown()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDown());
    }

    IEnumerator SpeedBoostPowerDown()
    {
        yield return new WaitForSeconds(5f);
        _speed /= _speedMultiplier;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
