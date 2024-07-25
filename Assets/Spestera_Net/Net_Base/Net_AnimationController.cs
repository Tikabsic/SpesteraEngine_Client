using UnityEngine;

public class Net_AnimationController : MonoBehaviour
{

    [SerializeField] private Animator _animator;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }



    void Update()
    {
        
    }
}
