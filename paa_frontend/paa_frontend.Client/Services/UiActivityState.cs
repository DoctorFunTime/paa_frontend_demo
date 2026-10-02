using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace paa_frontend.Client.Services
{
    /// <summary>Tracks frontend reads and database mutations for portal feedback.</summary>
    public sealed class UiActivityState
    {
        private int activeReads;
        private int activeWrites;
        private int writeFailed;
        private int saveCompleted;
        private long saveVersion;
        private string currentReadLabel = "data";

        public event Action? Changed;
        public bool IsLoading => Volatile.Read(ref activeReads) > 0;
        public bool IsSaving => Volatile.Read(ref activeWrites) > 0;
        public bool IsSaveComplete => Volatile.Read(ref saveCompleted) == 1;
        public string StatusMessage => IsSaving ? "Saving changes… Please wait" : $"Loading {currentReadLabel}…";

        public ActivityLease Begin(HttpMethod method, string? path = null)
        {
            bool isRead = method == HttpMethod.Get || method == HttpMethod.Head || method == HttpMethod.Options;
            if (isRead)
            {
                currentReadLabel = GetLabel(path);
                Interlocked.Increment(ref activeReads);
            }
            else if (Interlocked.Increment(ref activeWrites) == 1) Volatile.Write(ref writeFailed, 0);
            if (!isRead)
            {
                Interlocked.Increment(ref saveVersion);
                Volatile.Write(ref saveCompleted, 0);
            }
            Changed?.Invoke();
            return new ActivityLease(this, isRead);
        }

        private static string GetLabel(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "data";
            if (path.Contains("student", StringComparison.OrdinalIgnoreCase)) return "student records";
            if (path.Contains("attendance", StringComparison.OrdinalIgnoreCase)) return "attendance";
            if (path.Contains("grade", StringComparison.OrdinalIgnoreCase) || path.Contains("mark", StringComparison.OrdinalIgnoreCase)) return "academic records";
            if (path.Contains("finance", StringComparison.OrdinalIgnoreCase) || path.Contains("invoice", StringComparison.OrdinalIgnoreCase)) return "finance data";
            if (path.Contains("admin", StringComparison.OrdinalIgnoreCase) || path.Contains("teacher", StringComparison.OrdinalIgnoreCase) || path.Contains("burser", StringComparison.OrdinalIgnoreCase)) return "staff records";
            return "data";
        }

        private void End(bool isRead)
        {
            if (isRead)
            {
                Interlocked.Decrement(ref activeReads);
            }
            else
            {
                Interlocked.Decrement(ref activeWrites);
                if (Volatile.Read(ref activeWrites) == 0 && Volatile.Read(ref writeFailed) == 0)
                {
                    var completedVersion = Volatile.Read(ref saveVersion);
                    Volatile.Write(ref saveCompleted, 1);
                    Changed?.Invoke();
                    _ = ClearSaveConfirmationAsync(completedVersion);
                }
            }
            Changed?.Invoke();
        }

        private void MarkWriteFailed()
        {
            Volatile.Write(ref writeFailed, 1);
        }

        private async Task ClearSaveConfirmationAsync(long completedVersion)
        {
            await Task.Delay(1400);
            if (Volatile.Read(ref saveVersion) == completedVersion && Volatile.Read(ref activeWrites) == 0)
            {
                Volatile.Write(ref saveCompleted, 0);
                Changed?.Invoke();
            }
        }

        public sealed class ActivityLease : IDisposable
        {
            private readonly UiActivityState owner;
            private readonly bool isRead;
            private int disposed;

            public ActivityLease(UiActivityState owner, bool isRead)
            {
                this.owner = owner;
                this.isRead = isRead;
            }

            public void MarkFailed()
            {
                if (!isRead) owner.MarkWriteFailed();
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref disposed, 1) == 0) owner.End(isRead);
            }
        }
    }
}
