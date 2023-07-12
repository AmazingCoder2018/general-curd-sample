using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Auditing;

namespace Matoapp.Identity.NotificationManagements.Hubs
{
    /// <summary>
    /// SignalR��ϢHub
    /// </summary>
    [HubRoute("SignalR/Notification")]
    [Authorize]
    [DisableAuditing]
    public class NotificationHub : AbpHub<INotificationHub>
    {

    }
}