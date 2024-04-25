﻿using FashionSense.Framework.Interfaces.API;
using FashionSense.Framework.Models.Appearances.Generic;

namespace FashionSense.Framework.Models.Appearances.Body
{
    public class BodyModel : AppearanceModel
    {
        public int EyePosition { get; set; }
        public bool HideEyes { get; set; }

        public int HeightOffset { get; set; }
        public int HeadOffset { get; set; }
        public int LegOffset { get; set; }
        public int BodyOffset { get; set; }
        public int ArmsOffset { get; set; }
        public Size BodySize { get; set; }

        internal int GetFeatureOffset(IApi.Type type)
        {
            switch (type)
            {
                case IApi.Type.Hat:
                case IApi.Type.Hair:
                    return HeadOffset;
                case IApi.Type.Pants:
                case IApi.Type.Shoes:
                    return LegOffset;
                case IApi.Type.Shirt:
                    return BodyOffset;
                case IApi.Type.Sleeves:
                    return ArmsOffset;
                default:
                    return 0;
            }
        }
    }
}
