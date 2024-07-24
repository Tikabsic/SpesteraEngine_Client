using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMotionAnimationTriggers : MonoBehaviour
{

    private Animator _animator;
    private Net_PlayerController _controller;

    [SerializeField] private bool _isRunning;
    [SerializeField] private bool _isPlaying;
    [SerializeField] private bool _isIdle;


    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _controller = GetComponent<Net_PlayerController>();

        _isPlaying = false;
        _isIdle = true;
    }

    void FixedUpdate()
    {
        _isRunning = _controller._isRunning;
        UpdateMotionAnimations();
    }

    private void UpdateMotionAnimations()
    {
        if (_isRunning)
        {
            if (!_isPlaying)
            {
                _animator.SetBool("_isRunning", true);
                _isPlaying = true;
                _isIdle = false;
            }

        }

        if (!_isRunning)
        {
            if (!_isIdle)
            {
                _animator.SetBool("_isRunning", false);
                _isIdle = true;
                _isPlaying = false;
                Debug.Log("_isIdle");
            }
        }

    }
}
