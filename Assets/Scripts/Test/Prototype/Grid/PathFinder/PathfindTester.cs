using Prototype.Grid;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class PathfindTester : MonoBehaviour
{
    public HexTile startTile;
    public HexTile goalTile;

    public List<HexTile> path = new();

    [ContextMenu("경로 시각화")]
    public void ShowPath()
    {
        if (GridManager.Instance.pathfinder.TryGetPath(startTile, goalTile, path))
            Debug.Log("경로 탐색 완료");
        else
            Debug.LogWarning("경로 탐색 실패");
    }

    private void OnDrawGizmos()
    {
        if(path != null && path.Count > 0)
        {
            for(int i = 1; i < path.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(path[i - 1].transform.position, path[i].transform.position);
            }
        }
    }
}
