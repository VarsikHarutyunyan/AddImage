using System;
using System.Collections.Generic;
using System.Text;

namespace AddInfoAndImage
{
    public class DetectByRiskManager
    {
        public string Description { get; internal set; }
        public int MatchId { get; internal set; }
        public int Category { get; internal set; }
        public string RiskManagerName { get; internal set; }
        public string ImagesPath { get; internal set; }
    }
}
