using UnityEngine;

public class BombPlanting : MonoBehaviour
{
    public Transform BombTransform;
    public Transform clockTransform;
    private bool isNearClockTower = false;

    void Update()
    {
        // if (isNearClockTower && Input.GetKeyDown(KeyCode.E))
        // {
        //     PlantBomb();
        // }
    }

    void PlantBomb()
    {
        BombTransform.SetParent(clockTransform);
        Debug.Log("Bomb planted! Find the exit!");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ClockMechanic clockMechanic))
        {
           // isNearClockTower = true;
            clockTransform = clockMechanic.transform;
            PlantBomb();
        }
        
        if (collision.TryGetComponent(out Bomb bomb))
        {
            BombTransform = bomb.gameObject.transform;
            bomb.transform.SetParent(this.transform);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ClockMechanic>(out ClockMechanic clockMechanic))
        {
            isNearClockTower = false;
        }
    }
}