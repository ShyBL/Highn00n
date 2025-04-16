using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator _animator;
    private CharacterMovement _characterMovement;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _characterMovement = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        if (_characterMovement.dir.magnitude > 0)
        {
            _animator.SetBool("IsMoving", true);

            _animator.SetFloat("LastInputX", _characterMovement.dir.x);
            _animator.SetFloat("LastInputY", _characterMovement.dir.y);
        }
        else
        {
            _animator.SetBool("IsMoving", false);
            _animator.SetFloat("LastInputX", _characterMovement.dir.x);
            _animator.SetFloat("LastInputY", _characterMovement.dir.y);
        }
    }
    
    
    public int GetLastInputX()
    {
        return _animator.GetInteger("LastInputX");
    }
    
    public int GetLastInputY()
    {
        return _animator.GetInteger("LastInputY");
    }
}