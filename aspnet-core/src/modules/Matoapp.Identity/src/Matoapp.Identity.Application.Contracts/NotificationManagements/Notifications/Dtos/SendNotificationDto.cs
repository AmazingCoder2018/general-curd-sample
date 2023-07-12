using Matoapp.Identity.NotificationManagements.Notifications.Enums;

namespace Matoapp.Identity.NotificationManagements.Notifications.Dtos
{
    /// <summary>
    /// ����֪ͨ
    /// </summary>
    public class SendNotificationDto
    {
        /// <summary>
        /// ����
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// ��Ϣ����
        /// </summary>
        public MessageType MessageType { get; set; }

        private SendNotificationDto()
        {

        }

        public SendNotificationDto(string title, string content, MessageType messageType)
        {
            Title = title;
            Content = content;
            MessageType = messageType;
        }
    }
}