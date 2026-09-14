using Unity.VisualScripting;
using UnityEngine;

public class TorsoStabliser : MonoBehaviour
{
    [SerializeField] private Rigidbody _torsoRigidbody;
    [SerializeField] private float _upwardForce = 150f;

    private void FixedUpdate()
    {
        if (_torsoRigidbody != null)
        {
            // Apply a continous stablising force directly on the torso
            _torsoRigidbody.AddForce(Vector3.up * _upwardForce, ForceMode.Force);
        }
    }
}
