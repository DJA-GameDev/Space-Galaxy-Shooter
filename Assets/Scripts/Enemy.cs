using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private GameObject _laserPrefab;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;

    [SerializeField]
    private float _frequency = 1.0f;
    [SerializeField]
    private float _amplitude = 2.0f;
    [SerializeField]
    private float _cycleSpeed = 1.0f;

    private Vector3 enemyPos;
    private Vector3 axis;

    [SerializeField]
    private float _enemyFireRate = 3.0f;
    [SerializeField]
    private float _enemyCanFire = -1.0f;

    private bool _isEnemyAlive = true;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        enemyPos = transform.position;
        axis = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isEnemyAlive == true) 
        {
            CalculateMovement();
        }
        
        if (Time.time > _enemyCanFire && _isEnemyAlive == true)
        {
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            _enemyFireRate = Random.Range(4.0f, 7.0f);
            _enemyCanFire = Time.time + _enemyFireRate;

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }

        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        
        ZigZagMovement();

        if (transform.position.y < -7.0f)
        {
            float randomX = (Random.Range(-8.0f, 8.0f));
            transform.position = new Vector3(randomX, 7.0f, 0);
            enemyPos = new Vector3(randomX, 7.0f, 0);
        }
    }

    void ZigZagMovement()
    {
        enemyPos += Vector3.down * Time.deltaTime * _cycleSpeed;
        transform.position = enemyPos + axis * Mathf.Sin(Time.time * _frequency) * _amplitude;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            _isEnemyAlive = false;

            if (player != null) 
            {
                player.Damage();
            }

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.5f);
        }

        if (other.tag == "Laser" || other.tag == "SuperLaser")
        {
            Laser laser = other.transform.GetComponent<Laser>();

            if (laser.IsEnemyLaser() == false)
            {
                _isEnemyAlive = false;

                if (other.tag == "Laser")
                { 
                    Destroy(other.gameObject);
                }

                if (_player != null)
                {
                    _player.AddScore(10);
                    _player.EnemyKillCount();
                }

                _anim.SetTrigger("OnEnemyDeath");
                _speed = 0;
                _audioSource.Play();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.5f);
            }
        }

        if (other.tag == "Missile")
        {
            HomingMissile missile = other.transform.GetComponent<HomingMissile>();

            _isEnemyAlive = false;

            if (other.tag == "Missile")
            {
                Destroy(other.gameObject);
            }

            if (_player != null)
            {
                _player.AddScore(10);
                _player.EnemyKillCount();
            }

            _speed = 0;
            _audioSource.Play();
            _anim.SetTrigger("OnEnemyDeath");
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.5f);
        }
    }
}