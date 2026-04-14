using System;
using Unity.AI.Navigation;
using UnityEngine;

public class BakeNavMesh : Generator
{
    private NewDungeonGenerator dungeonGen;
    private AddDungeonAssets dungeonAssets;

    [SerializeField]
    private NavMeshSurface navMesh;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
        dungeonAssets = GetComponent<AddDungeonAssets>();

        dungeonGen.OnStartGeneration += dungeonGen_OnStartGeneration;
        dungeonAssets.OnEndGeneration += DungeonAssets_OnEndGeneration;
    }

    private void dungeonGen_OnStartGeneration()
    {
        navMesh.RemoveData();
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
