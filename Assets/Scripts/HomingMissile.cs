using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private GameObject[] _targets;

    [SerializeField]
    private Rigidbody2D _rbMissile;
    private GameObject _closestEnemy;
    private float _distance;
    private float _closestTarget = Mathf.Infinity;
    private float _missileSpeed = 8.0f;
    private float _missileTurnSpeed = 300.0f;

    private bool _enemyFound = false;


    // Start is called before the first frame update
    void Start()
    {
        _targets = GameObject.FindGameObjectsWithTag("Enemy");
        _rbMissile = GetComponent<Rigidbody2D>();

        _closestTarget = Mathf.Infinity;
        _closestEnemy = null;

        if (_rbMissile == null)
        {
            Debug.Log("The RigidBody2D is NULL");
        }
    }

    private void Update()
    {
        FindClosestEnemy();
        MissileBoundaries();
    }

    void FixedUpdate()
    {
        FireMissile();
    }

    private void FindClosestEnemy()
    {
        _targets = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var target in _targets)
        {
            if (target != null) 
            {
                float distanceToEnemy  = Vector3.Distance(_rbMissile.transform.position, target.transform.position);

                if (distanceToEnemy < _closestTarget)
                {
                    _distance = distanceToEnemy;
                    _closestEnemy = target;
                    _enemyFound = true;
                }
            }
        }
    }

    public void FireMissile()
    {
        if (_closestEnemy != null)
        {
            if (_enemyFound == true)
            {
                Vector3 direction = _closestEnemy.transform.position - (Vector3)_rbMissile.position;
                direction.Normalize();

                float rotationValue = Vector3.Cross(direction, transform.up).z;
                _rbMissile.angularVelocity = -rotationValue * _missileTurnSpeed;
                _rbMissile.velocity = transform.up * _missileSpeed;
            }    
        }
        else 
        {
            _rbMissile.velocity = transform.up * _missileSpeed;
            _rbMissile.angularVelocity = 0f;
        }
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
}
