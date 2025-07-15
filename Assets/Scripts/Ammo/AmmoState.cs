using Game.Ammo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmmoState
{
    void Enter(Shell shell);   
    void Update(Shell shell);  
    void Exit(Shell shell);
}
public class StoredState : IAmmoState
{
    public void Enter(Shell shell)
    {
        Debug.Log("Shell stored.");
        
    }

    public void Update(Shell shell)
    {
        
    }

    public void Exit(Shell shell)
    {
        
    }
}

public class FiredState : IAmmoState
{
    public void Enter(Shell shell)
    {
        Debug.Log("Shell fired.");
    }
    public void Update(Shell shell)
    {
        Transform transform = shell.transform;
        shell.speedVec += PlanetDataManager.instance.gravity * Time.deltaTime * 0.2f;  
        transform.position += shell.speedVec * Time.deltaTime;
    }
    public void Exit(Shell shell)
    {
        Debug.Log("Shell exited fired state.");
    }
}

public class HitState : IAmmoState
{
    public void Enter(Shell shell)
    {
        Debug.Log("Shell exploded.");
        // Add explosion effects here
    }
    public void Update(Shell shell)
    {
        // Handle any post-explosion logic if needed
    }
    public void Exit(Shell shell)
    {
        Debug.Log("Shell exited exploded state.");
    }
}


