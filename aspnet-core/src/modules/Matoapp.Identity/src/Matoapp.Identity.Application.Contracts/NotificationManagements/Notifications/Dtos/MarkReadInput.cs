using System;

namespace Matoapp.Identity.NotificationManagements.Notifications.Dtos
{
    /// <summary>
    /// ����Ѷ�
    /// </summary>
    public class MarkReadInput
    {
        /// <summary>
        /// ��ϢId
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ������Id
        /// </summary>
        public Guid ReceiveId { get; set; }
    }
}