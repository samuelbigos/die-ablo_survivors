using Godot;
using System;
using System.Collections.Generic;

public class ModDice : Node
{
    public ModManager.ModTypes Type;

    public ModDice(ModManager.ModTypes type)
    {
        Type = type;
    }

    public void Activate(Vector2 pos, Vector2 forward, int value)
    {
        switch (Type)
        {
            case ModManager.ModTypes.Number:
                break;
            case ModManager.ModTypes.Bullet:
                Activate_Bullet(pos, forward, value);
                break;
            case ModManager.ModTypes.Heal:
                Activate_Heal(pos, forward, value);
                break;
            case ModManager.ModTypes.Lightning:
                Activate_Lightning(pos, forward, value);
                break;
            case ModManager.ModTypes.Coin:
                Activate_Coin(pos, forward, value);
                break;
            // case ModManager.ModTypes.Freeze:
            //     break;
            // case ModManager.ModTypes.Explode:
            //     break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Activate_Bullet(Vector2 pos, Vector2 forward, int value)
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
                    enemy.OnHit(BulletDamage(value), ModManager.ModTypes.Bullet);
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

    private void Activate_Heal(Vector2 pos, Vector2 forward, int value)
    {
        Dice.Instance.Heal(HealAmount(value));
        
        // TODO: if 6, do some aoe damage.
    }
    
    private void Activate_Lightning(Vector2 gridPos, Vector2 forward, int value)
    {
        List<Enemy> enemies = Game.Instance.Enemies;
        float dist = 0;
        bool chain = false;
        int damage = 1;
        switch (value)
        {
            case 1: dist = 2.5f;
                damage = 1;
                break;
            case 2: dist = 3.0f;
                damage = 1;
                break;
            case 3: dist = 3.5f;
                damage = 2;
                break;
            case 4: dist = 3.5f;
                damage = 2;
                break;
            case 5: dist = 4.0f;
                damage = 3;
                break;
            case 6: dist = 4.0f;
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

    private void Activate_Coin(Vector2 gridPos, Vector2 forward, int value)
    {
        Game.Instance.OnCoinCollect(CoinAmount(value));
    }

    private int BulletDamage(int value)
    {
        return value + 2;
    }
    
    private int HealAmount(int value)
    {
        return (value + 1) / 2;
    }
    
    private int CoinAmount(int value)
    {
        if (value == 1 || value == 2) return 1;
        if (value == 3 || value == 4) return 2;
        if (value == 5) return 3;
        if (value == 6) return 6;
        return 0;
    }
}
