using UnityEngine;
using Random = UnityEngine.Random;

public class CrossroadController : MonoBehaviour
{
    public bool test = false;
    public float spawnTime = 5;


    [Header("LeftObjects")]
    [SerializeField] private StopLightView stopLightLeft;
    [SerializeField] private CarSpawner carSpawnerLeft;
    [SerializeField] private Transform stopperLeft;

    [Header("RightObjects")]
    [SerializeField] private StopLightView stopLightRight;
    [SerializeField] private CarSpawner carSpawnerRight;
    [SerializeField] private Transform stopperRight;

    private float currentSpawnTime = 0;

    //Testing Variables
    private const float GreenTime = 20;
    private const float YellowTime = 5;
    private float currentTime = 0;
    private bool leftIsOpen = true;

    private void Start()
    {
        if (test)
        {
            stopLightLeft.ChangeState(leftIsOpen ? StopLightState.Green : StopLightState.Red);
            stopLightRight.ChangeState(!leftIsOpen ? StopLightState.Green : StopLightState.Red);
        }

        OpenStopper(leftIsOpen);
    }

    private void Update()
    {
        currentSpawnTime -= Time.deltaTime;
        if (currentSpawnTime <= 0)
        {
            SpawnRandomCar();
            currentSpawnTime = spawnTime + Random.Range(0, spawnTime / 2f);
        }

        if (test)
            RunTest(Time.deltaTime);
    }

    private void SpawnRandomCar()
    {
        float randomValue = Random.value;
        if (randomValue < 0.65)
            carSpawnerLeft.SpawnCar();
        else
            carSpawnerRight.SpawnCar();
    }

    private void RunTest(float deltaTime)
    {
        currentTime += deltaTime;

        if (currentTime > GreenTime)
        {
            leftIsOpen = !leftIsOpen;
            stopLightLeft.ChangeState(leftIsOpen ? StopLightState.Green : StopLightState.Red);
            stopLightRight.ChangeState(!leftIsOpen ? StopLightState.Green : StopLightState.Red);
            currentTime = 0;

            OpenStopper(leftIsOpen);
        }

        else if (currentTime > GreenTime - YellowTime)
        {
            if (leftIsOpen)
                stopLightLeft.ChangeState(StopLightState.Yellow);
            else
                stopLightRight.ChangeState(StopLightState.Yellow);
            CloseStoppers();
        }

    }

    private void OpenStopper(bool leftIsOpen)
    {
        stopperLeft.position = new(stopperLeft.position.x, leftIsOpen ? 40f : 0f, stopperLeft.position.z);
        stopperRight.position = new(stopperRight.position.x, !leftIsOpen ? 40f : 0f, stopperRight.position.z);
    }
    private void CloseStoppers()
    {
        stopperLeft.position = new(stopperLeft.position.x, 0f, stopperLeft.position.z);
        stopperRight.position = new(stopperRight.position.x, 0f, stopperRight.position.z);
    }
}
