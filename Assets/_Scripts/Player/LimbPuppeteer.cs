using UnityEngine;
using UnityEngine.InputSystem;

public class LimbPuppeteer : MonoBehaviour
{
    [Header("IK Targets")]
    [SerializeField] private Transform _leftFootTarget;
    [SerializeField] private Transform _rightFootTarget;

    [Header("Movement Settings")]
    [SerializeField] private float _legMoveSpeed = 0.05f;
    [SerializeField] private float _legLiftHeight = 0.5f;

    private InputSystem_Actions _actions;

    // ------------------------------------------------------

    private void Start()
    {
        _actions = InputManager.Instance.Actions;
    }

    private void Update()
    {
        // Read MouseDelta action
        Vector2 mouseDelta = _actions.Legs.MouseDelta.ReadValue<Vector2>();

        // Check if LeftLegDrag or RightLegDrag interactions are pressed
        if (_actions.Legs.LeftLegDrag.IsPressed())
        {
            MoveFoot(_leftFootTarget, mouseDelta);
        }
        else if (_actions.Legs.RightLegDrag.IsPressed())
        {
            MoveFoot(_rightFootTarget, mouseDelta);
        }
    }

    private void MoveFoot(Transform footTarget, Vector2 mouseDelta)
    {
        // Map Y-axis of MouseDelta to the Z-axis (forward) of the FootTarget
        Vector3 movement = new Vector3(0, 0, mouseDelta.y * _legMoveSpeed);

        // Add an upward arc to the Y-axis so the foot lifts off the ground collider
        movement.y = _legLiftHeight * (Mathf.Abs(mouseDelta.y) > 0.1f ? 1f : 0f);

        footTarget.Translate(movement * Time.deltaTime, Space.World);
    }
}
