using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActiveRagdollIK : MonoBehaviour
{
    [Header("Physics & Stability")]
    [SerializeField] private Rigidbody _torsoRigidbody;
    [SerializeField] private float _upwardForce = 120f;

    [Header("Rig Syncing")]
    [SerializeField] private Transform _physicalRigRoot;
    [SerializeField] private Transform _animatedRigRoot;

    [Header("IK Targets")]
    [SerializeField] private Transform _leftFootTarget;
    [SerializeField] private Transform _rightFootTarget;
    [SerializeField] private Transform _rightArmTarget;

    [Header("Movement Settings")]
    [SerializeField] private float _legMoveSpeed = 0.05f;
    [SerializeField] private float _armMoveSpeed = 0.05f;
    [SerializeField] private float _legLiftHeight = 0.5f;

    private ConfigurableJoint[] _joints;
    private Transform[] _animatedBones;
    private Quaternion[] _startRotations;

    private InputSystem_Actions _actions;

    // -----------------------------------------------------

    private void Start()
    {
        // Reference to the InputManager
        _actions = InputManager.Instance.Actions;
        InitialiseJointSync();
    }

    private void FixedUpdate()
    {
        // Apply a continous force directly to the torso rig
        if (_torsoRigidbody != null)
        {
            _torsoRigidbody.AddForce(Vector3.up * _upwardForce, ForceMode.Force);
        }

        // Copy IK rotations safely into the physical joints' Slerp Drives
        for (int i = 0; i < _joints.Length; i++)
        {
            _joints[i].targetRotation = Quaternion.Inverse(_animatedBones[i].localRotation) * _startRotations[i];
        }
    }

    private void Update()
    {
        // Handle movement based on which Action Map is currently active
        if (_actions.Legs.enabled)
        {
            HandleLegMovement();
        }
        else if (_actions.Arms.enabled)
        {
            HandleArmMovement();
        }
    }

    // --- JOINT SYNC ---
    private void InitialiseJointSync()
    {
        // Auto-map the physical joints to the animated rig 
        _joints = _physicalRigRoot.GetComponentsInChildren<ConfigurableJoint>();
        _animatedBones = new Transform[_joints.Length];
        _startRotations = new Quaternion[_joints.Length];
        
        for (int i = 0; i < _joints.Length; i++)
        {
            _animatedBones[i] = FindBoneRecursive(_animatedRigRoot, _joints[i].gameObject.name);
            _startRotations[i] = _joints[i].transform.localRotation;
        }
    }

    private Transform FindBoneRecursive(Transform parent, string targetName)
    {
        if (parent.name == targetName)
        {
            return parent;
        }

        foreach (Transform child in parent)
        {
            Transform result = FindBoneRecursive(child, targetName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    // --- Joint Movement ---

    private void HandleLegMovement()
    {
        Vector2 mouseDelta = _actions.Legs.MouseDelta.ReadValue<Vector2>();

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
        // Map the Y-axis of the MouseDelta to the Z-axis (forward) of the FootTarget
        Vector3 movement = new Vector3(0, 0, mouseDelta.y * _legMoveSpeed);

        // Add a slight upwards arc to the Y-axis (Vertical) so the foot lifts
        movement.y = _legLiftHeight * (Mathf.Abs(mouseDelta.y) > 0.1 ? 1 : 0);

        footTarget.Translate(movement * Time.deltaTime, Space.World);
    }

    private void HandleArmMovement()
    {
        Vector2 mouseDelta = _actions.Arms.MouseDelta.ReadValue<Vector2>();
        Vector3 armMovement = Vector3.zero;

        // If VerticalModifier is held, map the delta's Y-axis to the IK target's vertical (Y) axis
        if (_actions.Arms.VerticalModifier.IsPressed())
        {
            armMovement.y = mouseDelta.y * _armMoveSpeed;
        }
        // If released, map it to the horizontal (X and Z) axis
        else
        {
            armMovement.x = mouseDelta.x * _armMoveSpeed;
            armMovement.z = mouseDelta.y * _armMoveSpeed;   
        }

        _rightArmTarget.Translate(armMovement * Time.deltaTime, Space.World);
    }
}
