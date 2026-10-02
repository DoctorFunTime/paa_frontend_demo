using Microsoft.AspNetCore.Components;

namespace paa_frontend.Client.Services
{
    public class NotificationService
    {
        public event Action<Notification>? OnNotification;

        public void ShowSuccess(string message)
        {
            Console.WriteLine($"[NotificationService] ShowSuccess: {message}");
            OnNotification?.Invoke(new Notification
            {
                Message = message,
                Type = NotificationType.Success
            });
        }

        public void ShowError(string message)
        {
            Console.WriteLine($"[NotificationService] ShowError: {message}");
            OnNotification?.Invoke(new Notification
            {
                Message = message,
                Type = NotificationType.Error
            });
        }

        public void ShowInfo(string message)
        {
            Console.WriteLine($"[NotificationService] ShowInfo: {message}");
            OnNotification?.Invoke(new Notification
            {
                Message = message,
                Type = NotificationType.Info
            });
        }

        public void ShowWarning(string message)
        {
            Console.WriteLine($"[NotificationService] ShowWarning: {message}");
            OnNotification?.Invoke(new Notification
            {
                Message = message,
                Type = NotificationType.Warning
            });
        }
    }

    public class Notification
    {
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
    }

    public enum NotificationType
    {
        Success,
        Error,
        Info,
        Warning
    }
}
