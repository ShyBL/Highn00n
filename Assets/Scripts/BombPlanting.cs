using UnityEngine;

public class BombPlanting : MonoBehaviour
{
    public GameObject tntPrefab;

    private bool isNearClockTower = false;

    void Update()
    {
        if (isNearClockTower && Input.GetKeyDown(KeyCode.E))
        {
            PlantBomb();
        }
    }

    void PlantBomb()
    {
        Instantiate(tntPrefab, transform.position, Quaternion.identity);
        Debug.Log("Bomb planted! Find the exit!");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ClockTower"))
        {
            isNearClockTower = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ClockTower"))
        {
            isNearClockTower = false;
        }
    }
}