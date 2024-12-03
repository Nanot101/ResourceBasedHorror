using UnityEngine;

public class StaticWorldRotationSetter : MonoBehaviour
{
    [SerializeField] private Vector3 worldRotation;

    private void LateUpdate() => transform.rotation = Quaternion.Euler(worldRotation);
}