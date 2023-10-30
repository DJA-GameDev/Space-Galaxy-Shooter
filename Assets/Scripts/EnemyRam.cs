using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyRam : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;

    [SerializeField]
    private float _ramDistance;
    private float _attackRange = 4.0f;
    private float _ramMultiplier = 2.0f;

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
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -7.0f)
        {
            transform.position = new Vector3(Random.Range(-8.5f, 8.5f), 7.0f, 0);
        }

        _ramDistance = Vector3.Distance(_player.transform.position, this.transform.position);

        if (_ramDistance <= _attackRange) 
        {
            Vector3 direction = this.transform.position - _player.transform.position;
            direction = direction.normalized;
            this.transform.position -= direction * Time.deltaTime * (_speed * _ramMultiplier);
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
                _player.AddScore(-15);
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
