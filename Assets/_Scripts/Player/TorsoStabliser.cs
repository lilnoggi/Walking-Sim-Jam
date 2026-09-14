using Unity.VisualScripting;
using UnityEngine;

public class TorsoStabliser : MonoBehaviour
{
    [SerializeField] private Rigidbody _torsoRigidbody;
    [SerializeField] private float _upwardForce = 500f;

    [Header("Locomotion")]
    [SerializeField] private Transform _leftFootTarget;
    [SerializeField] private Transform _rightFootTarget;
    [SerializeField] private float _walkForce = 400f;

    // ------------------------------------------------------

    private void FixedUpdate()
    {
        if (_torsoRigidbody != null)
        {
            // Keep the character upright
            _torsoRigidbody.AddForce(Vector3.up * _upwardForce, ForceMode.Force);

            // Drag the body toward the feet
            if (_leftFootTarget != null && _rightFootTarget != null)
            {
                // Find the midpoint between where the left and right feet are planted
                Vector3 centerOfFeet = (_leftFootTarget.position + _rightFootTarget.position) /2f;

                // Calculate the direction from the torso to that midpoint
                Vector3 pullDirection = centerOfFeet - _torsoRigidbody.position;

                // Ignore the height differences so it only pulls horizontally
                pullDirection.y = 0;

                // Pull the torso forward to catch up with the thrown feet
                _torsoRigidbody.AddForce(pullDirection * _walkForce, ForceMode.Force);
            }
        }
    }
}
