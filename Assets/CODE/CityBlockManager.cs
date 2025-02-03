using UnityEngine;
using System.Collections.Generic;

public class CityBlockManager : MonoBehaviour
{
    private GameObject[] cityBlocks; // Array to store all city blocks in the scene

    private void Awake()
    {
        // Find all GameObjects tagged as "CityBlockNone" at the start
        cityBlocks = GameObject.FindGameObjectsWithTag("CityBlockNone");
        Debug.Log($"CityBlockManager: Found {cityBlocks.Length} city blocks in the scene.");
    }

    private void Start()
    {
        AssignRandomColors();  // Assign initial colors to city blocks
        SetRemainingBlocksToGrey(); // Set all other blocks to grey
    }

    // Assign random colors to city blocks (Red, Green, Blue)
public void AssignRandomColors()
{
    List<string> colors = new List<string> { "CityBlockRed", "CityBlockGreen", "CityBlockBlue" };
    GameObject[] cityBlocks = GameObject.FindGameObjectsWithTag("CityBlockGrey");

    if (cityBlocks.Length < colors.Count)
    {
        Debug.LogError("CityBlockManager: Not enough city blocks to assign unique colors.");
        return;
    }

    foreach (string color in colors)
    {
        AssignColorToRandomBlock(color);
    }
}
    // Set all remaining uncolored blocks to grey
    public void SetRemainingBlocksToGrey()
    {
        foreach (GameObject block in cityBlocks)
        {
            if (block.CompareTag("CityBlockNone"))
            {
                ChangeBlockTagAndMaterial(block, "CityBlockGrey");
            }
        }
    }
// CityBlockManager.cs




// CityBlockManager.cs
public void AssignColorToRandomBlock(string colorTag)
{
    GameObject[] cityBlocks = GameObject.FindGameObjectsWithTag("CityBlockGrey");

    if (cityBlocks.Length == 0)
    {
        Debug.LogWarning("CityBlockManager: No grey city blocks available.");
        return;
    }

    GameObject randomBlock = cityBlocks[Random.Range(0, cityBlocks.Length)];
    randomBlock.tag = colorTag;

    CityBlockTextureChange blockTexture = randomBlock.GetComponent<CityBlockTextureChange>();
    if (blockTexture != null)
    {
        blockTexture.UpdateBlockMaterial();  // Ensure material and buildings update
    }
    else
    {
        Debug.LogWarning($"CityBlockManager: {randomBlock.name} does not have CityBlockTextureChange attached.");
    }

    Debug.Log($"CityBlockManager: Assigned {colorTag} to {randomBlock.name}");
}


    // Change the block's tag and trigger its texture change
    private void ChangeBlockTagAndMaterial(GameObject block, string newTag)
    {
        block.tag = newTag;

        CityBlockTextureChange textureChange = block.GetComponent<CityBlockTextureChange>();
        if (textureChange != null)
        {
            textureChange.UpdateBlockMaterial();  // Trigger the city block to update its material
        }
        else
        {
            Debug.LogWarning($"CityBlockManager: {block.name} does not have a CityBlockTextureChange component.");
        }
    }
public GameObject[] GetCityBlocksExcludingTag(string excludedTag)
{
    return System.Array.FindAll(cityBlocks, block => !block.CompareTag(excludedTag));
}


    // Get a random block with a specific tag
    private GameObject GetRandomBlockWithTag(string tag)
    {
        GameObject[] filteredBlocks = System.Array.FindAll(cityBlocks, block => block.CompareTag(tag));
        if (filteredBlocks.Length == 0) return null;

        int randomIndex = Random.Range(0, filteredBlocks.Length);
        return filteredBlocks[randomIndex];
    }

    // Get all blocks with a specific tag
    public GameObject[] GetCityBlocksByTag(string tag)
    {
        return System.Array.FindAll(cityBlocks, block => block.CompareTag(tag));
    }
}
