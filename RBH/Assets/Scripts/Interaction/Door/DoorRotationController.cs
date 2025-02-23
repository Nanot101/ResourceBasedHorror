using System;
using System.Collections;
using UnityEngine;

public class DoorRotationController : MonoBehaviour
{
    [SerializeField] private DoorAudio doorAudio;

    [SerializeField] private Animator doorAnimator;

    [SerializeField] private string doorShakeAnimaParamName = "Shake";

    [SerializeField] [Tooltip("In seconds")]
    private float openDelay = 3.0f;

    public bool IsLocked { get; private set; } = false;

    public bool IsOpen => currentRotation && currentRotation.IsOpen;

    public bool CanToggleState => !IsLocked && (!currentRotation || !currentRotation.IsRotating);

    public DoorRotation TopLeftRotation { get; set; }

    public DoorRotation BottomLeftRotation { get; set; }

    private DoorRotation currentRotation;

    private bool openDelayInProgress = false;

    private void Start()
    {
        Debug.Assert(doorAudio, $"{nameof(doorAudio)} is required for {nameof(DoorRotationController)}",
            this);

        Debug.Assert(doorAnimator, $"{nameof(doorAnimator)} is required for {nameof(DoorRotationController)}",
            this);
    }

    public void ToggleState(Vector3 callerPosition, bool withOpenDelay = false)
    {
        if (!CanToggleState)
        {
            return;
        }

        if (currentRotation && currentRotation.IsOpen)
        {
            currentRotation.CloseDoor();
            doorAudio.PlayCloseAudio();
            return;
        }

        SetCurrentRotation(callerPosition);

        if (!currentRotation)
        {
            Debug.LogWarning("You should never be able to read me!");
            return;
        }

        if (withOpenDelay)
        {
            StartOpenDelay();
            return;
        }

        StopOpenDelay();

        currentRotation.OpenDoor();
        doorAudio.PlayOpenAudio();
    }

    private void SetCurrentRotation(Vector3 callerPosition)
    {
        var directionToCaller = callerPosition - transform.position;
        directionToCaller.Normalize();

        var doorUp = transform.up;

        var doorUpDirectionDot = Vector3.Dot(doorUp, directionToCaller);

        if (doorUpDirectionDot < 0)
        {
            currentRotation = TopLeftRotation;
            return;
        }

        currentRotation = BottomLeftRotation;
    }

    private void StartOpenDelay()
    {
        if (openDelayInProgress)
        {
            return;
        }

        openDelayInProgress = true;

        doorAnimator.SetBool(doorShakeAnimaParamName, true);
        doorAudio.StartShakeAudio();

        StartCoroutine(OpenDelayCoroutine());
    }

    private void StopOpenDelay()
    {
        StopAllCoroutines();

        openDelayInProgress = false;

        doorAnimator.SetBool(doorShakeAnimaParamName, false);
        doorAudio.StopShakeAudio();
    }

    private IEnumerator OpenDelayCoroutine()
    {
        yield return new WaitForSeconds(openDelay);

        FinishOpenDelay();
    }

    private void FinishOpenDelay()
    {
        openDelayInProgress = false;

        doorAnimator.SetBool(doorShakeAnimaParamName, false);
        doorAudio.StopShakeAudio();

        currentRotation.OpenDoor();
        doorAudio.PlayOpenAudio();
    }
}