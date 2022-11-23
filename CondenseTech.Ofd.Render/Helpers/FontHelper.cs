using System.Collections.Generic;
using SkiaSharp;

namespace CondenseTech.Ofd.Render.Helpers
{
    internal static class FontHelper
    {
        private static readonly Dictionary<string, string> fontNameDictionary = new Dictionary<string, string>
        {
            {"宋体", "SimSun"},
            {"黑体", "SimHei"},
            {"仿宋", "FangSong"},
            {"楷体", "KaiTi"},
            {"隶书", "LiSu"},
            {"幼圆", "YouYuan"},
            {"新宋体", "NSimSun"},
            {"微软雅黑", "Microsoft YaHei"},
            {"华文楷体","STKaiti"},
            {"华文细黑","STXihei"},
            {"华文宋体","STSong"},
            {"华文中宋","STZhongsong"},
            {"华文仿宋","STFangsong"},
            {"方正舒体","FZShuTi"},
            {"方正姚体","FZYaoti"},
            {"华文彩云","STCaiyun"},
            {"华文琥珀","STHupo"},
            {"华文隶书","STLiti"},
            {"华文行楷","STXingkai"},
            {"华文新魏","STXinwei"}
        };

        public static SKFont InstanceFont(string fontName, float fontSize)
        {
            string name = fontNameDictionary.ContainsKey(fontName) ? fontNameDictionary[fontName] : fontName;
            SKTypeface typeFace = SKTypeface.FromFamilyName(name);
            return typeFace.ToFont(fontSize);
        }
    }
}