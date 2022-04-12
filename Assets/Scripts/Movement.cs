using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float speed;
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float boostSpeed = 1.5f;

    [SerializeField] private CinemachineImpulseSource _impulseSource;

    private Camera _mainCamera;
    private Rigidbody _rb;
    private Controls _controls;
    private Animator _animator;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int IsPunching = Animator.StringToHash("isPunching");

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _controls = new Controls();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _controls.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        _controls.Disable();
    }

    private void Start()
    {
        speed = baseSpeed;
        _mainCamera = Camera.main;
        _rb = gameObject.GetComponent<Rigidbody>();
        _animator = gameObject.GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!_controls.Player.Move.IsPressed()) return;
        Vector2 input = _controls.Player.Move.ReadValue<Vector2>();
        Vector3 target = HandleInput(input);
        RotateCharacter(target);
    }


    private void FixedUpdate()
    {
        if (_controls.Player.Jump.IsPressed())
        {
            if (!_animator.GetBool(IsJumping))
            {
                _animator.SetBool(IsJumping, true);
            }
        }
        
        if (_controls.Player.Punch.IsPressed())
        {
            if (!_animator.GetBool(IsPunching))
            {
                _animator.SetBool(IsPunching, true);
            }
        }
        else{
            _animator.SetBool(IsPunching, false);
        }

        if (_controls.Player.Run.IsPressed())
        {
            _animator.SetBool(IsRunning, true);
            _impulseSource.GenerateImpulse();
            speed = baseSpeed * boostSpeed;
        }
        else
        {
            _animator.SetBool(IsRunning, false);
            speed = baseSpeed;
        }
        if (_controls.Player.Move.IsPressed())
        {
            _animator.SetBool(IsWalking, true);
            Vector2 input = _controls.Player.Move.ReadValue<Vector2>();
            Vector3 target = HandleInput(input);
            MovePhysics(target);
        }
        else
        {
            _animator.SetBool(IsWalking, false);
        }
    }

    private Vector3 HandleInput(Vector2 input)
    {
        Vector3 forward = _mainCamera.transform.forward;
        Vector3 right = _mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();

        Vector3 direction = right * input.x + forward * input.y;
        
        return transform.position + direction * speed * Time.deltaTime;
    }

    private void RotateCharacter(Vector3 target)
    {
        transform.rotation = Quaternion.LookRotation(target-transform.position);
    }

    private void MovePhysics(Vector3 target)
    {
        _rb.MovePosition(target); 
    }

    public void Jump()
    {
        _rb.AddForce(Vector3.up * 300);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            _animator.SetBool(IsJumping, false);
        }
    }

}
