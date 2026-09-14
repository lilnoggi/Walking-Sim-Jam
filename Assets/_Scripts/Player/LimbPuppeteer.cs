using UnityEngine;
using UnityEngine.InputSystem;

public class LimbPuppeteer : MonoBehaviour
{
    [Header("IK Targets")]
    [SerializeField] private Transform _leftFootTarget;
    [SerializeField] private Transform _rightFootTarget;
    [SerializeField] private Transform _pelvisReference;

    [Header("Movement Settings")]
    [SerializeField] private float _legMoveSpeed = 0.05f;
    [SerializeField] private float _legLiftHeight = 0.5f;
    [SerializeField] private float _maxStepDistance = 1.2f;

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

        // Call both right and left inputs every frame so the unpressed foot correctly drops to the floor
        MoveFoot(_leftFootTarget, mouseDelta, _actions.Legs.LeftLegDrag.IsPressed());
        MoveFoot(_rightFootTarget, mouseDelta, _actions.Legs.RightLegDrag.IsPressed());
    }

    private void MoveFoot(Transform footTarget, Vector2 mouseDelta, bool isPressed)
    {
        Vector3 newPos = footTarget.position;

        if (isPressed)
        {
            // Lift the leg up
            newPos.y = 0.2f + _legLiftHeight;
            newPos.x += mouseDelta.x * _legMoveSpeed;
            newPos.z += mouseDelta.y * _legMoveSpeed;

            // Clamp horizontal distance relative to the pelvis
            Vector3 hipPos = _pelvisReference.position;
            hipPos.y = 0.2f; // Flatten to ground level for 2D radius calculation

            Vector3 offset = newPos - hipPos;
            offset.y = 0; 

            if (offset.magnitude > _maxStepDistance)
            {
                offset = Vector3.ClampMagnitude(offset, _maxStepDistance);
                newPos.x = hipPos.x + offset.x;
                newPos.z = hipPos.z + offset.z;
            }
        }
        else
        {
            // Drop to the floor
            newPos.y = 0.2f; 
        }

        footTarget.position = newPos;
    }
}
