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
    }
    public void Update(Shell shell)
    {
        Transform transform = shell.transform;
        shell.speedVec += OnPlanetDataManager.instance.gravity * Time.deltaTime;  
        transform.position += shell.speedVec * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(shell.speedVec.normalized);
    }
    public void Exit(Shell shell)
    {
        
    }
}

public class HitState : IAmmoState
{
    public void Enter(Shell shell)
    {
        shell.speed = shell.speedVec.magnitude;
        // Add explosion effects here
    }
    public void Update(Shell shell)
    {
        if (shell.hitTarget == null)
        {
            shell.ShellHit();
            return;
        }
        Vector3 dir = shell.hitTarget.transform.position - shell.transform.position;
        if (dir.magnitude < 0.2f)
        {
            shell.ShellHit();
            return;
        }
        shell.transform.position += dir.normalized * shell.speed * Time.deltaTime;
        shell.transform.rotation = Quaternion.LookRotation(dir.normalized);
    }
    public void Exit(Shell shell)
    {
    }
}


