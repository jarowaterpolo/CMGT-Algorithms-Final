using Unity.AI.Navigation;
using UnityEngine;

public class BakeNavMesh : Generator
{
    private AddDungeonAssets DungeonAssets;

    [SerializeField]
    private NavMeshSurface navMesh;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DungeonAssets = GetComponent<AddDungeonAssets>();

        DungeonAssets.OnEndGeneration += DungeonAssets_OnEndGeneration;
    }

    private void DungeonAssets_OnEndGeneration()
    {
        BuildMesh();
    }

    private void BuildMesh()
    {
        navMesh.BuildNavMesh();
    }
}
