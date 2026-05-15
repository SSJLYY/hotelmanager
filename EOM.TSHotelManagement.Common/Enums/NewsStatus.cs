using System.ComponentModel;

namespace EOM.TSHotelManagement.Common
{
    public enum NewsStatus
    {
        [Description("已发布")]
        Publish = 0,
        [Description("未发布")]
        Unpublished = 1,
        [Description("已删除")]
        Deleted = 2,
        [Description("已归档")]
        Archived = 3,
        [Description("草稿")]
        Draft = 4,
        [Description("待审核")]
        PendingReview = 5,
        [Description("已审核")]
        Reviewed = 6,
        [Description("已过期")]
        Expired = 7,
        [Description("已撤回")]
        Withdrawn = 8,
        [Description("已置顶")]
        Pinned = 9,
    }
}
