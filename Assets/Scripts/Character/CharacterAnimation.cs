using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private static readonly int Direction = Animator.StringToHash("Direction");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
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
            _animator.SetBool(IsMoving, true);

            switch (_characterMovement.dir.y)
            {
                case > 0:
                    _animator.SetInteger(Direction, 1);
                    break;
                case < 0:
                    _animator.SetInteger(Direction, 0);
                    break;
            }
        }
        else
        {
            _animator.SetBool(IsMoving, false);
        }
    }
    
    public int GetFacingDirection()
    {
        return _animator.GetInteger(Direction);
    }
}