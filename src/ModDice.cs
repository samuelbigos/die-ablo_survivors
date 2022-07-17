using Godot;
using System;
using System.Collections.Generic;

public class ModDice : Node
{
    public ModManager.ModTypes Type;
    public int Face;

    public ModDice(ModManager.ModTypes type, int face)
    {
        Type = type;
        Face = face;
    }

    public void Activate(Vector2 pos, Vector2 forward)
    {
        switch (Type)
        {
            case ModManager.ModTypes.Number:
                break;
            case ModManager.ModTypes.Bullet:
                Activate_Bullet(pos, forward);
                break;
            case ModManager.ModTypes.Heal:
                Activate_Heal(pos, forward);
                break;
            case ModManager.ModTypes.Lightning:
                Activate_Lightning(pos, forward);
                break;
            // case ModManager.ModTypes.Coin:
            //     break;
            // case ModManager.ModTypes.Freeze:
            //     break;
            // case ModManager.ModTypes.Explode:
            //     break;
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
                    enemy.OnHit(BulletDamage(), ModManager.ModTypes.Bullet);
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
        mat.AlbedoColor = ModManager.Instance.ModColoursSecondary[Type];
        
        bulletParticles.Emitting = true;
    }

    private void Activate_Heal(Vector2 pos, Vector2 forward)
    {
        Dice.Instance.Heal(HealAmount());
        
        // TODO: if 6, do some aoe damage.
    }
    
    private void Activate_Lightning(Vector2 gridPos, Vector2 forward)
    {
        List<Enemy> enemies = Game.Instance.Enemies;
        float dist = 0;
        bool chain = false;
        int damage = 1;
        switch (Face)
        {
            case 0: dist = 2.5f;
                damage = 1;
                break;
            case 1: dist = 3.0f;
                damage = 1;
                break;
            case 2: dist = 3.5f;
                damage = 2;
                break;
            case 3: dist = 3.5f;
                damage = 2;
                break;
            case 4: dist = 4.0f;
                damage = 3;
                break;
            case 5: dist = 4.0f;
                chain = true;
                damage = 3;
                break;
        }

        List<Enemy> hit = new List<Enemy>();
        List<(Enemy, Enemy)> chainHits = new List<(Enemy, Enemy)>();
        foreach (Enemy e1 in enemies)
        {
            if ((e1.GridPos - gridPos).Length() < dist)
            {
                hit.Add(e1);
                if (chain)
                {
                    foreach (Enemy e2 in enemies)
                    {
                        if (e1 == e2) continue;
                        if ((e1.GridPos - e2.GridPos).Length() < dist)
                        {
                            chainHits.Add((e1, e2));
                        }
                    }
                }
            }
        }

        foreach (Enemy enemy in hit)
        {
            enemy.OnHit(damage, ModManager.ModTypes.Lightning);
            LightningVFX vfx = Resources.Instance.LightningVFX.Instance<LightningVFX>();
            Game.Instance.AddChild(vfx);
            vfx.P1 = Dice.Instance;
            vfx.P2 = enemy;
        }

        if (chain)
        {
            foreach ((Enemy, Enemy) e1e2 in chainHits)
            {
                e1e2.Item2.OnHit(damage, ModManager.ModTypes.Lightning);
                LightningVFX vfx = Resources.Instance.LightningVFX.Instance<LightningVFX>();
                Game.Instance.AddChild(vfx);
                vfx.P1 = e1e2.Item1;
                vfx.P2 = e1e2.Item2;
            }
        }
    }

    private int BulletDamage()
    {
        return Face + 3;
    }
    
    private int HealAmount()
    {
        return (Face + 2) / 2;
    }
}
