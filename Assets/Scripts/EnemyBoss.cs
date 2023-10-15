using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBoss : MonoBehaviour
{
    [SerializeField]
    private float _speed = 1.0f;
    [SerializeField]
    private GameObject _laserPrefab;

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

        if (Time.time > _enemyCanFire && transform.position.y < 4.0f && _isEnemyAlive == true)
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
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < 4.0f)
        {
            _speed = 0.0f;
        }
    }

}