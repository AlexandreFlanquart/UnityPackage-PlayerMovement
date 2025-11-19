using UnityEngine;

[CreateAssetMenu(fileName = "WheelSO_", menuName = "ScriptableObjects/MUPController/WheelSO")]
public class WheelSO : ScriptableObject
{
    public float mass;

    [Header("Forward Friction")]
    public float fExtremumSlip = 0.4f;
    public float fExtremumValue = 1f;
    public float fAsymptoteSlip = 0.8f;
    public float fAsymptoteValue = 0.5f;
    public float fStiffness = 1f;
    [Header("Sideways Friction")]
    public float sExtremumSlip = 0.2f;
    public float sExtremumValue = 1f;
    public float sAsymptoteSlip = 0.5f;
    public float sAsymptoteValue = 0.75f;
    public float sStiffness = 1f;
    [Header("Steering")]
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 30f;
}
