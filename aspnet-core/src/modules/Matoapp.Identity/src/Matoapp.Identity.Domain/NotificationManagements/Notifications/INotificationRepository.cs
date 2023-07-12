using Matoapp.Identity.NotificationManagements.Notifications.Aggregates;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Matoapp.Identity.NotificationManagements.Notifications
{
    /// <summary>
    /// 消息通知 仓储接口
    /// </summary>
    public partial interface INotificationRepository : IBasicRepository<Notification, Guid>
    {
        /// <summary>
        /// 查找用户消息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Notification> FindByIdAsync(Guid id);

    }
}