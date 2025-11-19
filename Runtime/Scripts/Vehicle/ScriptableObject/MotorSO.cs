using UnityEngine;

[CreateAssetMenu(fileName = "MotorSO", menuName = "ScriptableObjects/MUPController/MotorSO")]
public class MotorSO : ScriptableObject
{
    [Min(0f)]
    public float carTopSpeed = 15;
    [Min(0f)]
    public float motorTorque = 2000f;
    [Min(0f)]
    public float brakeTorque = 2000f;
    public AnimationCurve motorCurve;
}
