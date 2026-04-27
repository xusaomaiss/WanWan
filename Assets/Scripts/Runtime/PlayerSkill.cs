namespace Wanwan.Runtime
{
    public enum PlayerSkill
    {
        None,
        ExtraShield,
        BetterGraze,
        Level2Weapon
    }

    public static class PlayerSkillExtensions
    {
        public static string GetDisplayName(this PlayerSkill skill)
        {
            switch (skill)
            {
                case PlayerSkill.ExtraShield:
                    return "初始护盾";
                case PlayerSkill.BetterGraze:
                    return "擦弹强化";
                case PlayerSkill.Level2Weapon:
                    return "火力预热";
                default:
                    return string.Empty;
            }
        }
    }
}
