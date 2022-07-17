using Godot;
using System;
using System.Collections.Generic;

public static class Waves
{
    public struct Spawn
    {
        public int E;
        public Vector2 P;
    }

    public static List<List<Spawn>> W = new()
    {
        new() // 1
        {
            new() {E = 0, P = new Vector2(10, 4)},
            new() {E = 0, P = new Vector2(10, 16)},
            new() {E = 0, P = new Vector2(4, 10)},
            new() {E = 0, P = new Vector2(16, 10)},
        },
        new() // 2
        {
            new() {E = 0, P = new Vector2(18, 2)},
            new() {E = 0, P = new Vector2(18, 6)},
            new() {E = 0, P = new Vector2(18, 10)},
            new() {E = 0, P = new Vector2(18, 14)},
            new() {E = 0, P = new Vector2(18, 18)},
            new() {E = 0, P = new Vector2(2, 4)},
            new() {E = 0, P = new Vector2(2, 8)},
            new() {E = 0, P = new Vector2(2, 12)},
            new() {E = 0, P = new Vector2(2, 16)},
        },
        new() // 3
        {
            new() {E = 0, P = new Vector2(5, 15)},
            new() {E = 0, P = new Vector2(15, 15)},
            new() {E = 0, P = new Vector2(15, 5)},
            new() {E = 0, P = new Vector2(0, 0)},
            new() {E = 1, P = new Vector2(20, 20)},
            new() {E = 1, P = new Vector2(20, 20)},
            new() {E = 1, P = new Vector2(20, 0)},
            new() {E = 1, P = new Vector2(0, 0)},
        },
        new() // 4
        {
            new() {E = 2, P = new Vector2(2, 2)},
            new() {E = 1, P = new Vector2(4, 4)},
            new() {E = 1, P = new Vector2(2, 4)},
            new() {E = 1, P = new Vector2(4, 2)},
        },
        new() // 5
        {
            new() {E = 1, P = new Vector2(2, 2)},
            new() {E = 1, P = new Vector2(4, 4)},
            new() {E = 1, P = new Vector2(2, 4)},
            new() {E = 1, P = new Vector2(4, 2)},
            new() {E = 1, P = new Vector2(18, 16)},
            new() {E = 1, P = new Vector2(16, 18)},
            new() {E = 1, P = new Vector2(18, 18)},
            new() {E = 1, P = new Vector2(16, 16)},
            new() {E = 0, P = new Vector2(10, 4)},
            new() {E = 0, P = new Vector2(10, 16)},
            new() {E = 0, P = new Vector2(4, 10)},
            new() {E = 0, P = new Vector2(16, 10)},
        },
        new() // 6
        {
            new() {E = 2, P = new Vector2(18, 18)},
            new() {E = 2, P = new Vector2(18, 2)},
            new() {E = 2, P = new Vector2(2, 2)},
            new() {E = 2, P = new Vector2(2, 18)},
            new() {E = 2, P = new Vector2(20, 20)},
            new() {E = 2, P = new Vector2(20, 0)},
            new() {E = 2, P = new Vector2(0, 0)},
            new() {E = 2, P = new Vector2(0, 20)},
        },
        new() // 7
        {
            new() {E = 2, P = new Vector2(18, 18)},
            new() {E = 2, P = new Vector2(18, 2)},
            new() {E = 2, P = new Vector2(2, 2)},
            new() {E = 2, P = new Vector2(2, 18)},
            new() {E = 2, P = new Vector2(20, 20)},
            new() {E = 2, P = new Vector2(20, 0)},
            new() {E = 2, P = new Vector2(0, 0)},
            new() {E = 2, P = new Vector2(0, 20)},
            new() {E = 1, P = new Vector2(2, 2)},
            new() {E = 1, P = new Vector2(4, 4)},
            new() {E = 1, P = new Vector2(2, 4)},
            new() {E = 1, P = new Vector2(4, 2)},
            new() {E = 1, P = new Vector2(18, 16)},
            new() {E = 1, P = new Vector2(16, 18)},
            new() {E = 1, P = new Vector2(18, 18)},
            new() {E = 1, P = new Vector2(16, 16)},
            new() {E = 0, P = new Vector2(10, 4)},
            new() {E = 0, P = new Vector2(10, 16)},
            new() {E = 0, P = new Vector2(4, 10)},
            new() {E = 0, P = new Vector2(16, 10)},
        },
        new() // 8
        {
            new() {E = 0, P = new Vector2(18, 2)},
            new() {E = 0, P = new Vector2(18, 6)},
            new() {E = 0, P = new Vector2(18, 10)},
            new() {E = 0, P = new Vector2(18, 14)},
            new() {E = 0, P = new Vector2(18, 18)},
            new() {E = 0, P = new Vector2(2, 4)},
            new() {E = 0, P = new Vector2(2, 8)},
            new() {E = 0, P = new Vector2(2, 12)},
            new() {E = 0, P = new Vector2(2, 16)},
            new() {E = 1, P = new Vector2(2, 2)},
            new() {E = 1, P = new Vector2(4, 4)},
            new() {E = 1, P = new Vector2(2, 4)},
            new() {E = 1, P = new Vector2(4, 2)},
            new() {E = 1, P = new Vector2(18, 16)},
            new() {E = 1, P = new Vector2(16, 18)},
            new() {E = 1, P = new Vector2(18, 18)},
            new() {E = 1, P = new Vector2(16, 16)},
            new() {E = 0, P = new Vector2(10, 4)},
            new() {E = 0, P = new Vector2(10, 16)},
            new() {E = 0, P = new Vector2(4, 10)},
            new() {E = 0, P = new Vector2(16, 10)},
        },
        new() // 9
        {
            new() {E = 2, P = new Vector2(18, 2)},
            new() {E = 2, P = new Vector2(18, 6)},
            new() {E = 2, P = new Vector2(18, 10)},
            new() {E = 2, P = new Vector2(18, 14)},
            new() {E = 2, P = new Vector2(18, 18)},
            new() {E = 2, P = new Vector2(2, 4)},
            new() {E = 2, P = new Vector2(2, 8)},
            new() {E = 2, P = new Vector2(2, 12)},
            new() {E = 2, P = new Vector2(2, 16)},
            new() {E = 1, P = new Vector2(16, 2)},
            new() {E = 1, P = new Vector2(16, 6)},
            new() {E = 1, P = new Vector2(16, 10)},
            new() {E = 1, P = new Vector2(16, 14)},
            new() {E = 1, P = new Vector2(16, 18)},
            new() {E = 1, P = new Vector2(4, 4)},
            new() {E = 1, P = new Vector2(4, 8)},
            new() {E = 1, P = new Vector2(4, 12)},
            new() {E = 1, P = new Vector2(4, 16)},
        },
        new() // 10
        {
            new() {E = 1, P = new Vector2(18, 2)},
            new() {E = 2, P = new Vector2(18, 6)},
            new() {E = 0, P = new Vector2(18, 10)},
            new() {E = 2, P = new Vector2(18, 14)},
            new() {E = 0, P = new Vector2(18, 18)},
            new() {E = 2, P = new Vector2(2, 4)},
            new() {E = 0, P = new Vector2(2, 8)},
            new() {E = 1, P = new Vector2(2, 12)},
            new() {E = 0, P = new Vector2(2, 16)},
            new() {E = 1, P = new Vector2(2, 2)},
            new() {E = 1, P = new Vector2(4, 4)},
            new() {E = 2, P = new Vector2(2, 4)},
            new() {E = 1, P = new Vector2(4, 2)},
            new() {E = 2, P = new Vector2(18, 16)},
            new() {E = 1, P = new Vector2(16, 18)},
            new() {E = 1, P = new Vector2(18, 18)},
            new() {E = 1, P = new Vector2(16, 16)},
            new() {E = 0, P = new Vector2(10, 4)},
            new() {E = 0, P = new Vector2(10, 16)},
            new() {E = 1, P = new Vector2(4, 10)},
            new() {E = 1, P = new Vector2(16, 10)},
            new() {E = 1, P = new Vector2(9, 4)},
            new() {E = 1, P = new Vector2(9, 16)},
            new() {E = 1, P = new Vector2(4, 9)},
            new() {E = 1, P = new Vector2(16, 9)},
        }
    };
}
