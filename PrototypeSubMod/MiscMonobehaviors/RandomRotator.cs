using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

internal class RandomRotator : MonoBehaviour
{
    [SerializeField] private float xSpeed;
    [SerializeField] private float ySpeed;
    [SerializeField] private float zSpeed;

    private void Update()
    {
        float x = (Time.time * xSpeed) % 360;
        float y = (Time.time * ySpeed) % 360;
        float z = (Time.time * zSpeed) % 360;
        transform.localRotation = Quaternion.Euler(x, y, z);
    }
}
