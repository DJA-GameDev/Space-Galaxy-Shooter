using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private Transform _target = null;
    private GameObject[] targets;

    [SerializeField]
    private Rigidbody2D _rbMissile;
    private float _distance;
    private float _closestTarget = Mathf.Infinity;
    private float _missileSpeed = 300.0f;
    private float _missileTurnSpeed = 700.0f;


    // Start is called before the first frame update
    void Start()
    {
        _rbMissile = GetComponent<Rigidbody2D>();

        if (_rbMissile == null)
        {
            Debug.Log("The RigidBody2D is NULL");
        }
        FindClosestEnemy();
    }

    void FixedUpdate()
    {
        FireMissile();
    }

    private void FindClosestEnemy()
    {
        targets = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in targets)
        {
            _distance = (enemy.transform.position - this.transform.position).sqrMagnitude;

            if (_distance < _closestTarget)
            {
                _closestTarget = _distance;
                _target = enemy.transform;
            }
        }
    }

    public void FireMissile()
    {
        _rbMissile.velocity = transform.up * _missileSpeed * Time.deltaTime;

        if (_target != null)
        {
            Vector2 direction = (Vector2)_target.position - _rbMissile.position;
            direction.Normalize();

            float rotationValue = Vector3.Cross(direction, transform.up).z;
            _rbMissile.angularVelocity = -rotationValue * _missileTurnSpeed;
            _rbMissile.velocity = transform.up * _missileSpeed * Time.deltaTime;
        }
        MissileBoundaries();
    }

    private void MissileBoundaries()
    {
        if (transform.position.y < -8.0f)
        {
            Destroy(this.gameObject);
        }

        if (transform.position.y > 8.0f)
        {
            Destroy(this.gameObject);
        }

        if (transform.position.x < -11.5f)
        {
            Destroy(this.gameObject);
        }

        if (transform.position.x > 11.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Enemy enemy = other.transform.GetComponent<Enemy>();

            if (enemy != null)
            {
                Destroy(other.gameObject);

                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);
            }

        }

    }
}
