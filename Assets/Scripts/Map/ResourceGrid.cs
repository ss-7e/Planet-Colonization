using UnityEngine;

public class ResourceGrid : Grid
{
    public string resourceName;
    public int richness;
    protected NaturalResource gridResource;
    public ResourceGrid(Vector3 gridPos, NaturalResource resource) : base(gridPos)
    {
        gridResource = resource;
    }

    public NaturalResource GetMineOnGrid()
    {
        if (gridResource == null)
        {
            Debug.LogError("NaturalResource is not set on ResourceGrid!");
            return null;
        }
        NaturalResource resourceCopy = Object.Instantiate(gridResource);
        resourceCopy.currentCount = richness;
        return resourceCopy;
    }
}

public class RenewableResourceGrid : ResourceGrid
{
    public float regenerationRate;
    public float lastRegenerationTime;
    public float amount;
    public float maxAmount;
    public float regenrateAmount;

    
    public RenewableResourceGrid(Vector3 gridPos, NaturalResource resource, float regenerationRate, float maxAmount, float regenrateAmount) 
        : base(gridPos, resource)
    {
        this.regenerationRate = regenerationRate;
        this.maxAmount = maxAmount;
        this.regenrateAmount = regenrateAmount;
        this.amount = maxAmount; // Start with full amount
    }
    public void Regenerate()
    {
        if(lastRegenerationTime < regenerationRate)
        {
            lastRegenerationTime += Time.deltaTime;
            return;
        }
        lastRegenerationTime = 0f;
        if (amount < maxAmount)
        {
            amount += regenrateAmount;
            if (amount > maxAmount)
            {
                amount = maxAmount;
            }
        }
    }
}

