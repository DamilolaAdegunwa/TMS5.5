using Egoal.Annotations;
using Egoal.Application.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Egoal.Scenics.Dto
{
    public class ScenicDto : EntityDto
    {
        public ScenicDto()
        {
            PhotoList = new List<PhotoDto>();
        }

        [Display(Name = "名称")]
        [MustFillIn]
        [MaximumLength(50)]
        public string ScenicName { get; set; }

        [Display(Name = "开放时间")]
        [MaximumLength(5)]
        public string OpenTime { get; set; }

        [Display(Name = "至")]
        [MaximumLength(5)]
        public string CloseTime { get; set; }

        public List<PhotoDto> PhotoList { get; set; }

        [Display(Name = "简介")]
        [MustFillIn]
        public string ScenicIntro { get; set; }
        public string ScenicFeature { get; set; }

        [Display(Name = "公告标题")]
        [MaximumLength(50)]
        public string NoticeTitle { get; set; }
        public string NoticeContent { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }

        [Display(Name = "景区地址")]
        [MaximumLength(200)]
        public string Address { get; set; }

        public string WxSubscribeUrl { get; set; }

        public string PageLabelMainText { get; set; }
    }

    public class PhotoDto
    {
        public string Name { get; set; }
        public long Uid { get; set; }
        public string Url { get; set; }
    }
}
