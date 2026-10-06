using UnityEngine;
using System.Collections;

public class CameraMoveAndZoom : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 3f;
    public float zoomSpeed = 3f;

    private Camera cam;
    private Coroutine moveRoutine;

    void Awake()
    {
        cam = Camera.main;
    }

    // ✅ Unity Button compatible
    public void MoveCameraTo(Transform target)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveAndZoom(target));
    }

    IEnumerator MoveAndZoom(Transform target)
    {
        Vector3 startPos = cam.transform.position;
        float startZoom = cam.orthographicSize;

        Vector3 targetPos = new Vector3(
            target.position.x,
            target.position.y,
            startPos.z
        );

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;

            cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            cam.orthographicSize = Mathf.Lerp(startZoom, zoomSpeed, t);

            yield return null;
        }

        cam.transform.position = targetPos;
        cam.orthographicSize = zoomSpeed;
    }
}
