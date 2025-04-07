using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager Instance { get; private set; }

    [SerializeField] PlayerController playerController;
    [SerializeField] DrillShip drillShip;
 
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void TriggerGameEndSequence()
    {
        playerController.GameOver();
        drillShip.GameOver();
    }
}
