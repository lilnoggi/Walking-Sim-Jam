using UnityEngine;

public class RigSyncer : MonoBehaviour
{
    [Header("Rig Roots")]
    [SerializeField] private Transform _physicalRigRoot;
    [SerializeField] private Transform _animatedRigRoot;

    private ConfigurableJoint[] _joints;
    private Transform[] _animatedBones;
    private Quaternion[] _startRotations;

    // -------------------------------------------------

    private void Start()
    {
        // Find all physical joints
        _joints = _physicalRigRoot.GetComponentsInChildren<ConfigurableJoint>();
        _animatedBones = new Transform[_joints.Length];
        _startRotations = new Quaternion[_joints.Length];

        // Match them to the bones on the hidden rig
        for (int i = 0; i < _joints.Length; i++)
        {
            _animatedBones[i] = FindBoneRecursive(_animatedRigRoot, _joints[i].gameObject.name);
            _startRotations[i] = _joints[i].transform.localRotation;
        }
    }

    private void FixedUpdate()
    {
        // Continously feed the Ghost Rig's IK angles into the physical rigs joints
        for (int i = 0; i < _joints.Length; i++)
        {
            if (_animatedBones[i] != null)
            {
                _joints[i].targetRotation = Quaternion.Inverse(_animatedBones[i].localRotation) * _startRotations[i];
            }
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
}
