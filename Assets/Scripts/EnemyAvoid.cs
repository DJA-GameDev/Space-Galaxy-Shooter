using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAvoid : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private float _rayDistance = 6.0f;
    [SerializeField]
    private float _rayCastRad = 1.0f;

    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;

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
            _enemyFireRate = Random.Range(2.0f, 5.0f);
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

        AvoidPlayerShot();

        if (transform.position.y < -7.0f)
        {
            transform.position = new Vector3(Random.Range(-8.5f, 8.5f), 7.0f, 0);
        }
    }

    void AvoidPlayerShot()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _rayCastRad, Vector2.down, _rayDistance, LayerMask.GetMask("Laser"));

        float x = Random.Range(-5.0f, 5.0f);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Laser"))
            {
                transform.position = new Vector3(x, transform.position.y, 0);
            }
        }
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

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.5f);
        }
    }
}