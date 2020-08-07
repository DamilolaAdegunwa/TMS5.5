namespace Egoal.Scenics
{
    public class ScenicOptions
    {
        public string ScenicName { get; set; }
        public string ParkOpenTime { get; set; }
        /// <summary>
        /// 闭园时间
        /// </summary>
        public string ParkCloseTime { get; set; } = "23:59";
        public bool IsMultiPark { get; set; }
        public int TradeTimeOffset { get; set; }
        public string PageLabelMainText { get; set; }
        public string Copyright { get; set; }
        public string WxTouristNeedCertTypeFlag { get; set; }
        public string ScenicObject { get; set; }
    }
}
