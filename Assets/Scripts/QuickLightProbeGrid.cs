using UnityEngine;

[RequireComponent(typeof(LightProbeGroup))]
public class QuickLightProbeGrid : MonoBehaviour
{
    public Vector3Int gridDimensions = new Vector3Int(5, 3, 5);
    public Vector3 spacing = new Vector3(2f, 2f, 2f);

    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        LightProbeGroup probeGroup = GetComponent<LightProbeGroup>();
        Vector3[] probePositions = new Vector3[gridDimensions.x * gridDimensions.y * gridDimensions.z];

        int index = 0;
        Vector3 offset = new Vector3(
            (gridDimensions.x - 1) * spacing.x / 2f,
            0,
            (gridDimensions.z - 1) * spacing.z / 2f
        );

        for (int x = 0; x < gridDimensions.x; x++)
        {
            for (int y = 0; y < gridDimensions.y; y++)
            {
                for (int z = 0; z < gridDimensions.z; z++)
                {
                    probePositions[index++] = new Vector3(x * spacing.x, y * spacing.y, z * spacing.z) - offset;
                }
            }
        }

        probeGroup.probePositions = probePositions;
    }
}