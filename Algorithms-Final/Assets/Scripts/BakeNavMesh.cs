using System;
using Unity.AI.Navigation;
using UnityEngine;

public class BakeNavMesh : Generator
{
    private NewDungeonGenerator dungeonGen;
    private AddDungeonAssets dungeonAssets;
    private AddFloors addFloors;

    [SerializeField]
    private NavMeshSurface navMesh;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dungeonGen = GetComponent<NewDungeonGenerator>();
        dungeonAssets = GetComponent<AddDungeonAssets>();
        addFloors = GetComponent<AddFloors>();

        dungeonGen.OnStartGeneration += DungeonGen_OnStartGeneration;
        dungeonAssets.OnEndGeneration += DungeonAssets_OnEndGeneration;
        addFloors.OnEndGeneration += AddFloors_OnEndGeneration;
    }

    private void DungeonGen_OnStartGeneration()
    {
        navMesh.RemoveData();
    }

    private void DungeonAssets_OnEndGeneration()
    {
        BuildMesh();
    }

    private void AddFloors_OnEndGeneration()
    {
        BuildMesh();
    }

    private void BuildMesh()
    {
        navMesh.BuildNavMesh();
    }
}
