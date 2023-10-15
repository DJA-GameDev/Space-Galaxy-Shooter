using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyBounce : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private GameObject _laserPrefab;

    private float _randomBounce;
    private float _ping, _pong;
    private int _direction = -1;

    [SerializeField]
    private float _enemyFireRate = 1.5f;
    [SerializeField]
    private float _enemyCanFire = -1.0f;

    [SerializeField]
    private GameObject _enemyShieldVisualizer;
    private bool _isVulnerable = false;

    private bool _isEnemyAlive = true;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        _randomBounce = Random.Range(2.0f, 7.0f);

        _ping = transform.position.x;
        _ping = _ping + _randomBounce;

        _pong = transform.position.x;
        _pong = _pong - _randomBounce;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.x > _ping || transform.position.x >= 8.5f)
        {
            _direction = -1;
        }
        else if (transform.position.x < _pong || transform.position.x <= -8.5f)
        {
            _direction = 1;
        }

        transform.Translate(Vector3.right * _speed * _direction * Time.deltaTime);

        FireLaser();

        if (transform.position.y < -7.0f)
        {
            transform.position = new Vector3(Random.Range(-8.5f, 8.5f), 7.0f, 0);
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -8.5f, 8.5f), transform.position.y, 0);
    }

    void FireLaser()
    {
        if (Time.time > _enemyCanFire && _isEnemyAlive == true)
        {
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            _enemyFireRate = Random.Range(2.0f, 5.0f);
            _enemyCanFire = Time.time + _enemyFireRate;

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            _enemyShieldVisualizer.SetActive(false);
            StartCoroutine(DamageDelay());            

            if (player != null)
            {
                player.Damage();
            }

            if (_isVulnerable == true)
            {
                _isEnemyAlive = false;
                _anim.SetTrigger("OnEnemyDeath");
                _speed = 0;
                _audioSource.Play();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.5f);
            }
        }

        if (other.tag == "Laser" || other.tag == "SuperLaser")
        {
            Laser laser = other.transform.GetComponent<Laser>();
            
            _enemyShieldVisualizer.SetActive(false);
            StartCoroutine(DamageDelay());

            if (laser.IsEnemyLaser() == false && _isVulnerable == true)
            {
                _isEnemyAlive = false;

                if (other.tag == "Laser")
                {
                    Destroy(other.gameObject);
                }

                if (_player != null)
                {
                    _player.AddScore(20);
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

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.5f);
        }
    }

    IEnumerator DamageDelay()
    {
        yield return new WaitForSeconds(0.5f);
        _isVulnerable = true;
    }
}
