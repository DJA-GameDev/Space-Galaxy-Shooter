using System.Collections;
using System.Runtime.CompilerServices;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
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
    private CameraShake _cameraShake;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;

    [SerializeField]
    private GameObject _superLaserPrefab;
    private bool _isSuperLaserActive = false;

    [SerializeField]
    private GameObject _missilePrefab;
    private HomingMissile _missile;
    private bool _isMissileActive = false;

    [SerializeField]
    private Slider _thrustGauge;
    [SerializeField]
    private float _maxFuel = 100.0f;
    [SerializeField]
    private bool _isThrusting = false;

    [SerializeField]
    private int _shieldLife = 3;
    [SerializeField]
    private SpriteRenderer _shieldRenderer;

    [SerializeField]
    private int _score;
    [SerializeField]
    private int _currentAmmo = 15;
    [SerializeField]
    private int _maxAmmo = 15;

    private bool _isSlowSpeedActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isTripleShotActive = false;
    private bool _isShieldActive = false;
    private bool _isDamaged = false;

    [SerializeField]
    private int _currentKills = 0;

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
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        _thrustGauge = GameObject.Find("Thruster_Slider").GetComponent<Slider>();
        _missile = GameObject.Find("Player_Missile").GetComponent<HomingMissile>();

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

        if (_thrustGauge == null) 
        {
            Debug.LogError("Thrust Gauge Slider is NULL");
        }

    }

    // Update is called once per frame

    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _isSuperLaserActive == false)
        {
            if ( _currentAmmo == 0 ) 
            {
                AudioSource.PlayClipAtPoint(_noAmmoSound, transform.position);
                return;
            }

            FireLaser();
        }

        if (Input.GetKeyDown(KeyCode.F) && _isMissileActive == true)
        {

            Instantiate(_missilePrefab, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            _missile.FireMissile();
        }

        _thrustGauge.value = _maxFuel;
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
            StartThrustBurn();
            StartCoroutine(ThrustActive());
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopThrustBurn();
            StartCoroutine(ThrustRegen());
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
    public void StartThrustBurn()
    {
        _isThrusting = true;
        if (_isSpeedBoostActive == true)
        {
            _speed = 12.0f;
        }
        else if (_isSlowSpeedActive == true)
        {
            _speed = 2.0f;
        }
        else
        {
            _speed = 5.5f;
        }
    }

    public void StopThrustBurn() 
    {
        _isThrusting = false;
        if (_isSpeedBoostActive == true)
        {
            _speed = 10.0f;
        }
        else if (_isSlowSpeedActive == true)
        {
            _speed = 1.0f;
        }
        else
        {
            _speed = 3.5f;
        }
    }

    public void UpdateThrustGauge(float currentFuel)
    {
        if (_maxFuel <= 0)
        {
            StopThrustBurn();
        }

        if (_maxFuel <= 100)
        { 
            _maxFuel += currentFuel;
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
        if (shots >= _maxAmmo)
        {
             _currentAmmo = _maxAmmo;
        }
        else
        {
            _currentAmmo += shots;
        }

        _uiManager.UpdateAmmo(_currentAmmo, _maxAmmo);
    }

    public void RestoreLives()
    {
        if (_lives < 3)
        {
            _lives++;

            if (_lives == 3)
            {
                _leftEngine.SetActive(false);
            }
            else if (_lives == 2)
            {
                _rightEngine.SetActive(false);
            }

            _uiManager.UpdateLives(_lives);
        }
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
            _cameraShake.StartCameraShake();
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

    public void SuperLaserActive()
    {
        _isSuperLaserActive = true; 
        GameObject superLas = Instantiate(_superLaserPrefab, transform.position + new Vector3(0, 6.5f, 0), Quaternion.identity);
        superLas.transform.parent = transform;
        Destroy(superLas, 5.0f);
        StartCoroutine(SuperLaserPowerDown());
    }

    IEnumerator SuperLaserPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isSuperLaserActive = false;    
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed = 10.0f;
        StartCoroutine(SpeedBoostPowerDown());
    }

    IEnumerator SpeedBoostPowerDown()
    {
        yield return new WaitForSeconds(5f);
        _speed = 3.5f;
        _isSpeedBoostActive = false;
    }

    public void SlowSpeedActive()
    {
        _isSlowSpeedActive = true;
        _speed = 1.0f;
        StartCoroutine(SlowSpeedPowerDown());
    }

    IEnumerator SlowSpeedPowerDown()
    {
        yield return new WaitForSeconds(5f);
        _speed = 3.5f;
        _isSlowSpeedActive = false;
    }

    IEnumerator ThrustActive()
    {
        while (_isThrusting == true)
        {
            UpdateThrustGauge(-2.0f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ThrustRegen() 
    {
        while (_isThrusting == false && _maxFuel < 100.0f)
        {
            yield return new WaitForSeconds(0.1f);
            UpdateThrustGauge(1.0f);
        }
    }

    public void MissileReady()
    {
        _isMissileActive = true;
        StartCoroutine(MissilePowerDown());
    }   
    
    IEnumerator MissilePowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isMissileActive = false;
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

    public void EnemyKillCount()
    {
        _currentKills++;
        Debug.Log(_currentKills);

        if(_currentKills == 5)
        {
            _spawnManager.WaveTwo();
            _uiManager.WaveTwoUI();
        }

        if (_currentKills == 10)
        {
            _spawnManager.WaveThree();
            _uiManager.WaveThreeUI();
        }

        if (_currentKills == 15)
        {
            _spawnManager.BossWave();
            _uiManager.WaveFourUI();
        }
    }
}
