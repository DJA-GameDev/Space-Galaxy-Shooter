using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemySmart : MonoBehaviour
{ 
    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private float _rayDistance = 8.0f;
    [SerializeField]
    private float _raycastRad = 0.5f;

    [SerializeField]
    private float _enemyFireRate = 3.0f;
    [SerializeField]
    private float _enemyCanFire = -1.0f;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (_isEnemyAlive == true)
        {
            CalculateMovement();
            BackLaserAttack();
            FrontLaserAttack();
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -7.0f)
        {
            transform.position = new Vector3(Random.Range(-8.5f, 8.5f), 7.0f, 0);
        }

    }

    void FireLaserAtPlayer()
    {
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.Euler(transform.rotation.x, transform.rotation.y, 180.0f));
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        _enemyFireRate = Random.Range(3.0f, 5.0f);
        _enemyCanFire = Time.time + _enemyFireRate;

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }

    }

    void FireLaserAtPowerup()
    {
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        _enemyFireRate = Random.Range(3.0f, 5.0f);
        _enemyCanFire = Time.time + _enemyFireRate;

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    void FrontLaserAttack() 
    {

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _raycastRad, Vector2.down, _rayDistance, LayerMask.GetMask("Powerup"));

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Powerup") && Time.time > _enemyCanFire)
            {
                Debug.Log("Powerup detected");
                FireLaserAtPowerup();
            }
        }
    }

    void BackLaserAttack()
    {

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _raycastRad, Vector2.up, _rayDistance, LayerMask.GetMask("Player"));

        if (hit.collider !=  null) 
        {
            if (hit.collider.CompareTag("Player") && Time.time > _enemyCanFire)  
            {
               Debug.Log("Player detected");
               FireLaserAtPlayer();
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
                    _player.AddScore(15);
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
