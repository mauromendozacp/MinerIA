using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    #region EXPOSED_FIELDS
    [SerializeField] private float positionDelay = 0f;
    [SerializeField] private float speedDelay = 0f;
    #endregion

    #region PRIVATE_METHODS
    private Action<Vector2Int> onTarget = null;
    private List<Vector2Int> path = null;
    #endregion

    #region PUBLIC_METHODS
    public void StartPathfiding(List<Vector2Int> path, Action<Vector2Int> onTarget)
    {
        this.onTarget = onTarget;

        if (path != null)
        {
            this.path = path;
            transform.position = new Vector3(path[0].x, path[0].y, 0f);

            StartCoroutine(SetPositionDelay());
        }
    }
    #endregion

    #region PRIVATE_METHODS
    private IEnumerator SetPositionDelay()
    {
        for (int i = 1; i < path.Count; i++)
        {
            float timer = 0f;
            Vector3 targetPos = new Vector3(path[i].x, path[i].y, 0f);

            while (timer < speedDelay)
            {
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, targetPos, timer / speedDelay);

                yield return new WaitForEndOfFrame();
            }

            transform.position = targetPos;
            onTarget?.Invoke(path[i]);

            yield return new WaitForSeconds(positionDelay);
        }
    }
    #endregion
}
