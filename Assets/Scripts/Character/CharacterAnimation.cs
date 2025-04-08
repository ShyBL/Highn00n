using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private static readonly int Direction = Animator.StringToHash("Direction");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private Animator animator;
    private CharacterMovement _characterMovement;

    private void Start()
    {
        animator = GetComponent<Animator>();
        _characterMovement = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        if (_characterMovement.dir.magnitude > 0)
        {
            animator.SetBool(IsMoving, true);

            if (_characterMovement.dir.y > 0)
                animator.SetInteger(Direction, 1);
            else if (_characterMovement.dir.y < 0)
                animator.SetInteger(Direction, 0);
            else if (_characterMovement.dir.x > 0)
                animator.SetInteger(Direction, 2);
            else if (_characterMovement.dir.x < 0)
                animator.SetInteger(Direction, 3);
        }
        else
        {
            animator.SetBool(IsMoving, false);
        }
    }
    
    public int GetFacingDirection()
    {
        return animator.GetInteger(Direction);
    }
}