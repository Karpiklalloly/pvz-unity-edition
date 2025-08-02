using UnityEngine;

namespace TowerDefense
{
    public static class CameraUtils
    {
        public static Vector3 GetCameraPosition(this Camera camera, Vector2 inCameraPosition, float distanceFromCamera = 1.5f)
        {
            var planeWorldCenter = camera.transform.position + camera.transform.forward * distanceFromCamera;
            var normal = camera.transform.forward;

            var billboardPlane = new Plane(normal, planeWorldCenter);
            var ray = camera.ViewportPointToRay(new Vector3(inCameraPosition.x, inCameraPosition.y, 0));

            if (billboardPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }
            
            return Vector3.zero;
        }
    }
}