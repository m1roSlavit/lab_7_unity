using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Parameters:")]

    [SerializeField] private float _movingSpeed;
    [SerializeField] private float _sprintMultiplicator;
    [SerializeField] private float _changeSpeedMultiplicator;
    [SerializeField] private float _rotatingSpeed;

    [SerializeField] private float _accelerationSmothness;

    [SerializeField] private float _selectDistance;

    [Space]

    [SerializeField] private float _stamina;
    [SerializeField] private float _health;

    [SerializeField] private float _staminaConsumption;
    [SerializeField] private float _healthConsumption;

    private float _currentStamina;
    private float _currentHealth;

    [Header("Components:")]

    [SerializeField] private Transform _chest;
    [SerializeField] private Transform _legs;

    private Transform _selectedObject;

    [Header("Prefabs:")]
    [SerializeField] private GameObject _bullet;

    [Header("LayerMasks:")]
    [SerializeField] private LayerMask _whatIsInteractive;

    private Rigidbody2D _rigidbody2D;

    private Animator _animator;

    private Camera _mainCamera;

    private Controls _controls;


    public static event Action<float> OnUpdateStamina;
    public static event Action<float> OnUpdateHealth;

    private void Awake()
    {
        _controls = new Controls();

        _controls.Player.Interact.performed += context => Interact();

        _currentStamina = _stamina;
        _currentHealth = _health;
    }

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _mainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        LookAtTheMouse();
        RotateLegs();
        SelectObject();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot(_chest.rotation);
        }
    }

    private void Shoot(Quaternion angle)
    {
        var bullet = Instantiate(_bullet, transform.position, angle);
        bullet.GetComponent<Rigidbody2D>().AddForce(-bullet.transform.right * 10,ForceMode2D.Impulse);

    }

    private void Loot()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _animator.SetTrigger("hasPickedUp");
        }
    }

    private void SelectObject()
    {
        if (Physics2D.Raycast(transform.position, -_chest.right, _selectDistance).transform != _selectedObject)
        {
            _selectedObject = Physics2D.Raycast(transform.position, -_chest.right, _selectDistance).transform;

            _selectedObject?.GetComponent<IInteractable>()?.Highlight();
        }             
    }

    private void Interact()
    {             
        if (((1 << _selectedObject?.gameObject.layer) & _whatIsInteractive) != 0)
        {
            _selectedObject?.GetComponent<IInteractable>()?.Interact();
        }           
    }

    private void Move()
    {
        Vector2 moveDirection = _controls.Player.Move.ReadValue<Vector2>();

        float angleBetweenMoveAndBodyDirections = Vector2.Angle(moveDirection, -_chest.right);

        Vector2 velocity = moveDirection * (_movingSpeed - _changeSpeedMultiplicator * angleBetweenMoveAndBodyDirections / 180);

        if (_controls.Player.Sprint.IsPressed())
        {
            if (_currentStamina > 0)
            {
                velocity *= _sprintMultiplicator;
                _currentStamina -= _staminaConsumption * Time.fixedDeltaTime;
            }
        }
        else
        {
            if(_currentStamina < _stamina)
            {
                _currentStamina += _staminaConsumption / 2 * Time.fixedDeltaTime;
            }
        }

        OnUpdateStamina.Invoke(_currentStamina / _stamina);       

        if (velocity.magnitude > 0)
        {        
            _animator.SetBool("isWalking", true);
        }
        else
        {
            _animator.SetBool("isWalking", false);
        }

        _animator.SetFloat("Speed", _rigidbody2D.velocity.magnitude);

        _rigidbody2D.velocity = Vector2.Lerp(_rigidbody2D.velocity, velocity, Time.fixedDeltaTime * _accelerationSmothness);
    }

    private void RotateLegs()
    {
        _legs.rotation =  Quaternion.Lerp(_legs.rotation, _chest.rotation, Time.deltaTime * _rotatingSpeed);
    }

    private void LookAtTheMouse()
    {
        Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        _chest.rotation = Quaternion.Lerp(_chest.rotation, Quaternion.Euler(Vector3.back * (Mathf.Atan2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y) * Mathf.Rad2Deg + 90)), Time.deltaTime * _rotatingSpeed);
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
}
