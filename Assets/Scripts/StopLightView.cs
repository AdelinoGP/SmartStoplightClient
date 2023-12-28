using UnityEngine;

public enum StopLightState
{
    Green,
    Yellow,
    Red
}

public class StopLightView : MonoBehaviour
{
    [SerializeField] private MeshRenderer greenLight;
    [SerializeField] private MeshRenderer yellowLight;
    [SerializeField] private MeshRenderer redLight;
    [Header("Colored Materials")]
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material redMaterial;

    public void ChangeState(StopLightState state)
    {
        greenLight.material = blackMaterial;
        yellowLight.material = blackMaterial;
        redLight.material = blackMaterial;
        switch (state)
        {
            case StopLightState.Green:
                greenLight.material = greenMaterial;
                break;
            case StopLightState.Yellow:
                yellowLight.material = yellowMaterial;
                break;
            case StopLightState.Red:
                redLight.material = redMaterial;
                break;
        }
    }
}
