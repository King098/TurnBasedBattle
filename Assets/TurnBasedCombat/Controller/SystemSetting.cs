using System.Collections.Generic;

namespace King.TurnBasedCombat
{
    public class SystemSetting
    {
		/// <summary>
		/// 使用Resources路径或者StreamingAssets路径
		/// </summary>
		public static bool UseResourcesPathOrStreamingAssetsPath = true;
		/// <summary>
		///	使用StreamingAssets路径需要打包AB包，AB包名字设置
		/// </summary>
		public static string AssetBundleName = "turnbasedcombat.unity3d";
		/// <summary>
		/// 英雄生命值过低数值
		/// </summary>
		public static long HeroLowLifeValue = 30;
		/// <summary>
		/// 英雄生命值过低百分比
		/// </summary>
		public static float HeroLowLifePercent = 0.2f;
		/// <summary>
		/// 英雄魔力值过低数值
		/// </summary>
		public static long HeroLowMagicValue = 30;
		/// <summary>
		/// 英雄魔力值过低百分比
		/// </summary>
		public static float HeroLowMagicPercent = 0.3f;
		/// <summary>
		/// 英雄速度乱敏数值
		/// </summary>
		public static float HeroSpeedMix = 0.7f;
		/// <summary>
		/// 战斗每回合开始停顿间隔
		/// </summary>
		public static float BattlePerTurnStartGapTime = 2f;
		/// <summary>
		/// 战斗每回合结束停顿间隔
		/// </summary>
		public static float BattlePerTurnEndGapTime = 2f;
		/// <summary>
		/// 回合开始前发现没有技能可以使用，则会直接进入下一回合，在进入下一回合的时候延迟
		/// </summary>
		public static float BatttleNoSkillChangeAction = 1f;
		/// <summary>
		/// 设置英雄动作对应的元素名称
		/// </summary>
		public static Dictionary<HeroAnimation,string> HeroAnimationParameters = new Dictionary<HeroAnimation, string>()
		{
			{HeroAnimation.None,"None"},
			{HeroAnimation.Idle,"Idle"},
			{HeroAnimation.Walk,"Walk"},
			{HeroAnimation.Dead,"Dead"},
			{HeroAnimation.Attack1,"Attack1"},
			{HeroAnimation.Attack2,"Attack2"},
			{HeroAnimation.Attack3,"Attack3"},
			{HeroAnimation.Attack4,"Attack4"},
			{HeroAnimation.Defense1,"Defense1"},
			{HeroAnimation.Defense2,"Defense2"},
			{HeroAnimation.Defense3,"Defense3"},
			{HeroAnimation.Defense4,"Defense4"},
			{HeroAnimation.MagicAttack1,"MagicAttack1"},
			{HeroAnimation.MagicAttack2,"MagicAttack2"},
			{HeroAnimation.MagicAttack3,"MagicAttack3"},
			{HeroAnimation.MagicAttack4,"MagicAttack4"},
			{HeroAnimation.MagicDefense1,"MagicDefense1"},
			{HeroAnimation.MagicDefense2,"MagicDefense2"},
			{HeroAnimation.MagicDefense3,"MagicDefense3"},
			{HeroAnimation.MagicDefense4,"MagicDefense4"},
		};
    }
}