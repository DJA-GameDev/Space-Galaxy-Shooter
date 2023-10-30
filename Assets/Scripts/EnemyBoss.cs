using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class EnemyBoss : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1.0f;
    [SerializeField]
    private float _moveDirection = 1.0f;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _multiShotPrefab;

    [SerializeField]
    private GameObject _directShotPrefab;
    private int _shotCount = 5;
    [SerializeField]
    private float _burstFireRate = 0.1f;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;

    [SerializeField]
    private float _enemyFireRate = 3.0f;
    [SerializeField]
    private float _enemyCanFire = -1.0f;

    [SerializeField]
    private BossLifeBar _bossLifebar;
    [SerializeField]
    private int _currentLives = 30;

    [SerializeField]
    private float _savedTime = 0.0f;
    [SerializeField]
    private float _superLaserDamageTime = 1.0f;


    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private SpawnManager _spawnManager;

    private bool _canTakeDamage = false;
    private bool _isEnemyAlive = true;
    private bool _isSuperLaserActive = false;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _bossLifebar = GetComponentInChildren<BossLifeBar>();

        _bossLifebar.gameObject.SetActive(false);

        if (_bossLifebar == null)
        {
            Debug.LogError("Boss Life Bar is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isEnemyAlive == true)
        {
            CalculateMovement();
        }

    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= 4.0f)
        {
            _canTakeDamage = true;
            _bossLifebar.gameObject.SetActive(true);
            _speed = 0.0f;
            _isSuperLaserActive = false;
            SideMovement();
        }

    }

    private void SideMovement()
    {
        if (_isEnemyAlive == true)
        {
            if (transform.position.x <= -2)
            {
                _moveDirection = 1.0f;
            }
            else if (transform.position.x >= 2)
            {
                _moveDirection = -1.0f;
            }
            transform.Translate(Vector3.right * _moveDirection * Time.deltaTime);

            BossAttack();
        }
    }

    private void BossAttack()
    {
        if (Time.time > _enemyCanFire)
        {
            int _attackType = Random.Range(0, 3);
            
            if (_attackType == 0)
            {
                BossMultiShot();
            }
            if (_attackType == 1)
            {
                StartCoroutine(BossBurstShot());
            }
            else if( _attackType == 2)
            {
               BossSingleShot();
            }

        }
    }

    private void BossSingleShot()
    {
        _enemyFireRate = Random.Range(2.0f, 4.0f);
        _enemyCanFire = Time.time + _enemyFireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    private void BossMultiShot()
    {
        _enemyFireRate = Random.Range(3.0f, 5.0f);
        _enemyCanFire = Time.time + _enemyFireRate;
        GameObject enemyLaser = Instantiate(_multiShotPrefab, transform.position + new Vector3(-3.5f, -1.4f, 0), Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    IEnumerator BossBurstShot()
    {
        for (int s = 0; s < _shotCount; s++)
        {
            _burstFireRate = Random.Range(1.0f, 1.5f);
            _enemyCanFire = Time.time + _burstFireRate;
            GameObject enemyLaser = Instantiate(_directShotPrefab, transform.position + new Vector3(0, -1.3f, 0), Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void BossDamage()
    {
        if (_canTakeDamage == false) 
        {
            return;
        }

        _currentLives--;
        _bossLifebar.UpdateBossLifeBar(_currentLives);

        if (_currentLives == 0 )
        {
            _isEnemyAlive = false;
            _bossLifebar.gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
        }

        if (other.tag == "Laser")
        {
            Laser laser = other.transform.GetComponent<Laser>();

            if (laser.IsEnemyLaser() == false)
            {
                BossDamage();

                if (other.tag == "Laser")
                {
                    Destroy(other.gameObject);
                }

                if (_player != null && _currentLives == 0)
                {
                    _player.AddScore(100);
                    _player.EnemyKillCount();
                    _anim.SetTrigger("OnEnemyDeath");
                    _moveDirection= 0;
                    _audioSource.Play();
                    Destroy(GetComponent<Collider2D>());
                    Destroy(this.gameObject, 2.5f);
                    _spawnManager.OnBossDeath();
                    _uiManager.GameOverWinner();
                }

            }
        }

        if (other.tag == "Missile")
        {
            HomingMissile missile = other.transform.GetComponent<HomingMissile>();

            BossDamage();

            if (other.tag == "Missile")
            {
                Destroy(other.gameObject);
            }

            if (_player != null && _currentLives == 0)
            {
                _player.AddScore(100);
                _player.EnemyKillCount();
                _anim.SetTrigger("OnEnemyDeath");
                _moveDirection = 0;
                _audioSource.Play();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.5f);
                _spawnManager.OnBossDeath();
                _uiManager.GameOverWinner();
            }
        }

        if(other.tag == "SuperLaser")
        {
            _isSuperLaserActive = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "SuperLaser")
        {
            if ((Time.time - _savedTime) > _superLaserDamageTime)
            { 
                _savedTime = Time.time;

                Laser laser = other.transform.GetComponent<Laser>();

                BossDamage();
                
                if (_player != null && _currentLives == 0)
                {
                    _player.AddScore(100);
                    _player.EnemyKillCount();
                    _anim.SetTrigger("OnEnemyDeath");
                    _moveDirection = 0;
                    _audioSource.Play();
                    Destroy(GetComponent<Collider2D>());
                    Destroy(this.gameObject, 2.5f);
                    _spawnManager.OnBossDeath();
                    _uiManager.GameOverWinner();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "SuperLaser")
        {
            _isSuperLaserActive = false;
        }
    }
}