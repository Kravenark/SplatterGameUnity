using UnityEngine;
using System.Collections.Generic;

public class CityBlockManager : MonoBehaviour
{
    private GameObject[] cityBlocks; // Array to store all city blocks in the scene
    public bool cityBlockInitialized = false; // Flag to indicate if city blocks are initialized

    private void Awake()
    {
        // Find all GameObjects with the tag "CityBlockNone"
        cityBlocks = GameObject.FindGameObjectsWithTag("CityBlockNone");
        Debug.Log($"CityBlockManager: Found {cityBlocks.Length} city blocks in the scene.");
    }

    private void Start()
    {
        AssignRandomColors();
        SetRemainingBlocksToGrey();

        // Signal that the city blocks are initialized
        cityBlockInitialized = true;
        Debug.Log("CityBlockManager: City blocks are initialized.");
    }

    // Assign random colors to three blocks
    public void AssignRandomColors()
    {
        if (cityBlocks.Length < 3)
        {
            Debug.LogError("CityBlockManager: Not enough blocks to assign colors.");
            return;
        }

        AssignColorToRandomBlock("CityBlockRed");
        AssignColorToRandomBlock("CityBlockGreen");
        AssignColorToRandomBlock("CityBlockBlue");
    }

    // Set all remaining blocks to grey
    private void SetRemainingBlocksToGrey()
    {
        foreach (GameObject block in cityBlocks)
        {
            if (block.CompareTag("CityBlockNone"))
            {
                ChangeBlockTagAndMaterial(block, "CityBlockGrey");
            }
        }
    }

    // Helper to assign a specific color to a random block
    private void AssignColorToRandomBlock(string colorTag)
    {
        GameObject randomBlock = GetRandomBlockWithTag("CityBlockNone");
        if (randomBlock != null)
        {
            ChangeBlockTagAndMaterial(randomBlock, colorTag);
        }
    }

    // Get a random block with a specific tag
    private GameObject GetRandomBlockWithTag(string tag)
    {
        GameObject[] filteredBlocks = System.Array.FindAll(cityBlocks, block => block.CompareTag(tag));
        if (filteredBlocks.Length == 0) return null;

        int randomIndex = Random.Range(0, filteredBlocks.Length);
        return filteredBlocks[randomIndex];
    }

    // Change the tag and material of a block
    private void ChangeBlockTagAndMaterial(GameObject block, string newTag)
    {
        block.tag = newTag;

        CityBlockTextureChange textureChange = block.GetComponent<CityBlockTextureChange>();
        if (textureChange != null)
        {
            textureChange.UpdateBlockTexture();
        }
    }

    // Get all blocks with a specific tag
    public GameObject[] GetCityBlocksByTag(string tag)
    {
        return System.Array.FindAll(cityBlocks, block => block.CompareTag(tag));
    }

    // Get all blocks excluding a specific tag
    public GameObject[] GetCityBlocksExcludingTag(string excludedTag)
    {
        return System.Array.FindAll(cityBlocks, block => !block.CompareTag(excludedTag));
    }

    // Find the closest grey block to a given position
    public GameObject FindClosestGreyBlock(Vector3 position)
    {
        GameObject closestGreyBlock = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject block in cityBlocks)
        {
            if (block.CompareTag("CityBlockGrey"))
            {
                float distance = Vector3.Distance(position, block.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestGreyBlock = block;
                }
            }
        }

        return closestGreyBlock;
    }

    // Find a random block excluding a specific tag
    public GameObject FindRandomBlockExcludingTag(string excludedTag)
    {
        GameObject[] filteredBlocks = GetCityBlocksExcludingTag(excludedTag);

        if (filteredBlocks.Length == 0)
        {
            Debug.LogWarning("CityBlockManager: No blocks available excluding the specified tag.");
            return null;
        }

        int randomIndex = Random.Range(0, filteredBlocks.Length);
        return filteredBlocks[randomIndex];
    }
}
