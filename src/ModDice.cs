using Godot;
using System;
using System.Collections.Generic;

public class ModDice : Node
{
    public ModManager.ModTypes Type;
    public int Face;

    public void Activate(Vector2 pos, Vector2 forward)
    {
        switch (Type)
        {
            case ModManager.ModTypes.Bullet:
                Activate_Bullet(pos, forward);
                break;
            case ModManager.ModTypes.Laser:
                break;
            case ModManager.ModTypes.Health:
                break;
            case ModManager.ModTypes.Coin:
                break;
            case ModManager.ModTypes.Freeze:
                break;
            case ModManager.ModTypes.Explode:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Activate_Bullet(Vector2 pos, Vector2 forward)
    {
        Vector2 grid = pos + forward;
        bool hit = false;
        while (Game.Instance.InBounds(grid))
        {
            List<Enemy> enemies = Game.Instance.Enemies;
            foreach (Enemy enemy in enemies)
            {
                if (enemy.GridPos == grid)
                {
                    enemy.OnHit(BulletDamage());
                    //hit = true;
                    break;
                }
            }

            if (hit)
                break;

            grid += forward;
        }

        Particles bulletParticles = ModManager.Instance.BulletParticles.Instance<Particles>();
        Game.Instance.AddChild(bulletParticles);
        bulletParticles.GlobalPosition(pos.To3D());
        Vector2 dir = new Vector2(-forward.x, forward.y);
        bulletParticles.GlobalRotate(Vector3.Up, dir.Angle() - Mathf.Pi * 0.5f);
        
        // change colour
        SpatialMaterial mat = bulletParticles.MaterialOverride as SpatialMaterial;
        mat = mat.Duplicate(false) as SpatialMaterial;
        bulletParticles.MaterialOverride = mat;
        mat.AlbedoColor = ModManager.Instance.ModColours[Type];
        
        bulletParticles.Emitting = true;
    }

    private int BulletDamage()
    {
        return Face + 1;
    }
}
