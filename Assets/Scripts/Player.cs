using System.Collections;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _thrustSpeed = 1.5f;
    [SerializeField]
    private float _powerupSpeed = 5.0f;
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
    private int _shieldLife = 3;
    [SerializeField]
    private SpriteRenderer _shieldRenderer;

    [SerializeField]
    private int _score;
    [SerializeField]
    private int _currentAmmo = 15;

    private bool _isTripleShotActive = false;
    private bool _isShieldActive = false;
    private bool _isDamaged = false;   

    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioClip _noAmmoSound;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _shieldRenderer = this.transform.Find("Shields").GetComponent<SpriteRenderer>();

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
            if ( _currentAmmo == 0 ) 
            {
                AudioSource.PlayClipAtPoint(_noAmmoSound, transform.position);
                return;
            }

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

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _speed = _speed += _thrustSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed = _speed -= _thrustSpeed;
        }

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
        AmmoCount(-1);

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

    public void AmmoCount(int shots)
    {
        if (shots >= _currentAmmo)
        {
             _currentAmmo = 15;
        }
        else
        {
            _currentAmmo += shots;
        }

        _uiManager.UpdateAmmo(_currentAmmo);
    }
    
    public void Damage()
    {
       
        if (_isShieldActive == true && _shieldLife >= 1) 
        {
            _shieldLife--;

            switch (_shieldLife)
            {
                case 0:
                    _isShieldActive = false;
                    _shieldVisualizer.SetActive(false);
                    break;
                case 1:
                    _shieldRenderer.color = Color.red;
                    break;
                case 2:
                    _shieldRenderer.color = Color.blue;
                    break;
            }

            return;
        }

        if(_isDamaged == false)
        {
            _isDamaged = true;
            _lives--;
            _isDamaged = false;
        }

        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives == 0 ) 
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
        _speed += _powerupSpeed;
        StartCoroutine(SpeedBoostPowerDown());
    }

    IEnumerator SpeedBoostPowerDown()
    {
        yield return new WaitForSeconds(5f);
        _speed -= _powerupSpeed;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
        _shieldLife = 3;
        _shieldRenderer.color = Color.white;
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
