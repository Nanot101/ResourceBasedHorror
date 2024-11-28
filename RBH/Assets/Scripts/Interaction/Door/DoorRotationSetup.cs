using UnityEngine;

public class DoorRotationSetup : MonoBehaviour
{
    private static readonly Vector3 TopLeftPivotPosition = new(-0.5f, 0.5f, 0.0f);
    private static readonly Vector3 BottomLeftPivotPosition = new(-0.5f, -0.5f, 0.0f);

    [SerializeField] private DoorRotationController doorRotationController;

    [SerializeField] private BoxCollider2D doorCollider;

    [SerializeField] private float doorOpenRotation = 90.0f;

    [SerializeField] private float doorOpenSpeed = 180.0f;

    private void Start()
    {
        AsserDesignerFields();

        CreateTopLeftPivot();
        CreateBottomLeftPivot();

        Destroy(this); // We no longer need this component.
    }

    private void CreateTopLeftPivot()
    {
        var topLeftPivot = new GameObject("TopLeftPivot")
        {
            transform =
            {
                parent = doorRotationController.transform,
                localPosition = TopLeftPivotPosition * doorCollider.size
            }
        };

        var topLeftPivotRotation = topLeftPivot.AddComponent<DoorRotation>();
        topLeftPivotRotation.Door = doorCollider.transform;
        topLeftPivotRotation.DoorOpenRotation = doorOpenRotation;
        topLeftPivotRotation.DoorOpenSpeed = doorOpenSpeed;

        doorRotationController.TopLeftRotation = topLeftPivotRotation;
    }

    private void CreateBottomLeftPivot()
    {
        var bottomLeftPivot = new GameObject("BottomLeftPivot")
        {
            transform =
            {
                parent = doorRotationController.transform,
                localPosition = BottomLeftPivotPosition * doorCollider.size
            }
        };

        var bottomLeftPivotRotation = bottomLeftPivot.AddComponent<DoorRotation>();
        bottomLeftPivotRotation.Door = doorCollider.transform;
        bottomLeftPivotRotation.DoorOpenRotation = -doorOpenRotation;
        bottomLeftPivotRotation.DoorOpenSpeed = doorOpenSpeed;

        doorRotationController.BottomLeftRotation = bottomLeftPivotRotation;
    }

    private void AsserDesignerFields()
    {
        Debug.Assert(doorRotationController,
            $"{nameof(doorRotationController)} is required for {gameObject.name} {nameof(DoorRotationSetup)}", this);

        Debug.Assert(doorCollider,
            $"{nameof(doorCollider)} is required for {gameObject.name} {nameof(DoorRotationSetup)}", this);

        Debug.Assert(doorOpenSpeed > 0.0f, $"{nameof(doorOpenSpeed)} must be greater than 0.", this);
    }
}