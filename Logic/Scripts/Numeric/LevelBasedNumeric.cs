// =======================================================================================
// OpenMMO Groundwork
// =======================================================================================

using System;

[Serializable]
public struct LevelBasedInt
{
    public int baseValue;
    public int bonusPerLevel;
    public int Get(int level) { return baseValue + bonusPerLevel * (level - 1); }
}

[Serializable]
public struct LevelBasedLong
{
    public long baseValue;
    public long bonusPerLevel;
    public long Get(int level) { return baseValue + bonusPerLevel * (level - 1); }
}

[Serializable]
public struct LevelBasedFloat
{
    public float baseValue;
    public float bonusPerLevel;
    public float Get(int level) { return baseValue + bonusPerLevel * (level - 1); }
}