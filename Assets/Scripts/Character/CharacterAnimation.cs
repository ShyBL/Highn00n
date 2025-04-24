using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private static readonly int Direction = Animator.StringToHash("Direction");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private Animator _animator;
    private FloatingGunAim _floatingGunAim;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _floatingGunAim = GetComponent<FloatingGunAim>();
    }

    private void Update()
    {
        if (_floatingGunAim.AimDirection.magnitude > 0)
        {
            _animator.SetBool(IsMoving, true);

            switch (_floatingGunAim.AimDirection.y)
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