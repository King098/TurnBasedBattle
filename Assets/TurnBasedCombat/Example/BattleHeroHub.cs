using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace King.TurnBasedCombat
{
    public class BattleHeroHub : BaseBattleHeroHub
    {
        /// <summary>
        /// 用于显示Hub的Text
        /// </summary>
        private Text TextHub;

        void Awake()
        {
            TextHub = this.GetComponent<Text>();
        }

        /// <summary>
        /// 根据Hub类型获取Hub属性
        /// </summary>
        /// <param name="type">Hub类型</param>
        /// <returns>返回此Hub的信息</returns>
        protected override HeroHubInfo GetHubInfoByType(HubType type)
        {
            for (int i = 0; i < Hubs.Count; i++)
            {
                if (Hubs[i].type == type)
                {
                    return Hubs[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 显示一个Hub
        /// </summary>
        /// <param name="hero">为哪个英雄显示</param>
        /// <param name="type">Hub类型</param>
        /// <param name="life">生命值变化</param>
        /// <param name="magic">魔力值变化</param>
        public override void ShowHubInfo(HeroMono hero, HubType type, ValueUnit value = null)
        {
            HeroHubInfo info = GetHubInfoByType(type);
            if (info == null)
                return;
            TextHub.color = info.TextColor;
            TextHub.text = info.Name.Replace("[CRITICAL]",value.RealTempValue(0).ToString());
            TextHub.text = TextHub.text.Replace("[LIFE]",value.RealTempValue(0).ToString());
            TextHub.text = TextHub.text.Replace("[MAGIC]",value.RealTempValue(0).ToString());
            TextHub.text = TextHub.text.Replace("[ATTACK]",value.RealTempValue(0).ToString());
            TextHub.text = TextHub.text.Replace("[DEFENSE]",value.RealTempValue(0).ToString());
            TextHub.text = TextHub.text.Replace("[MAGIC ATTACK]",value.RealTempValue(0).ToString());
            TextHub.text = TextHub.text.Replace("[MAGIC DEFENSE]",value.RealTempValue(0).ToString());
            TextHub.text = TextHub.text.Replace("[MAX LIFE]",value.RealTempValue(0).ToString());
            TextHub.text = TextHub.text.Replace("[MAX MAGIC]",value.RealTempValue(0).ToString());
            TextHub.text = TextHub.text.Replace("[SPEED]",value.RealTempValue(0).ToString());
            this.transform.position = hero.HeroPosition;
            this.gameObject.SetActive(true);
            StartCoroutine(Show(this.transform.position));
        }

        /// <summary>
        /// 显示动画
        /// </summary>
        /// <param name="startPos">动画起始位置</param>
        /// <returns></returns>
        IEnumerator Show(Vector3 startPos)
        {
            float time = 0f;
            Vector3 endPos = startPos + Vector3.up * 40;
            Color s = TextHub.color;
            Color e = new Color(TextHub.color.r,TextHub.color.g,TextHub.color.b,0f);
            while (time <= 1f)
            {
                time += Time.deltaTime;
                this.transform.position = Vector3.Lerp(startPos, endPos, time);
                TextHub.color = Color.Lerp(s, e, time);
                yield return new WaitForEndOfFrame();
            }
            this.gameObject.SetActive(false);
        }
    }
}