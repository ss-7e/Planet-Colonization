using UnityEngine;

public class Resource
{
    public string resourceName;
    public int richness;
    public Mesh resourceGridMesh;
}

public class RenewableResource : Resource
{
    public float regenerationRate;
    public float lastRegenerationTime;
    public float amount;
    public float maxAmount;
    public float regenrateAmount;
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

public class ResourceCrystal : Resource
{
    
}

public class ResourceWood : RenewableResource
{

}

public class ResourceLimestone : Resource
{

}