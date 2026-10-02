using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace paa_frontend.Client.Services
{
    public class GlobalHttpErrorHandler : DelegatingHandler
    {
        private readonly NotificationService _notificationService;
        private readonly UiActivityState _activityState;

        public GlobalHttpErrorHandler(NotificationService notificationService, UiActivityState activityState)
        {
            _notificationService = notificationService;
            _activityState = activityState;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var activity = _activityState.Begin(request.Method, request.RequestUri?.AbsolutePath);
            try
            {
                var response = await SendCoreAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode) activity.MarkFailed();
                return response;
            }
            catch
            {
                activity.MarkFailed();
                throw;
            }
            finally
            {
                activity.Dispose();
            }
        }

        private async Task<HttpResponseMessage> SendCoreAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            const int maxRetries = 5;
            var delay = TimeSpan.FromSeconds(3);
            HttpResponseMessage? response = null;
            Exception? lastException = null;

            // Pre-clone and buffer request content bytes before the first attempt to prevent stream disposal issues on retries
            byte[]? contentBytes = null;
            HttpContentHeaders? contentHeaders = null;
            if (request.Content != null)
            {
                var ms = new MemoryStream();
                await request.Content.CopyToAsync(ms);
                contentBytes = ms.ToArray();
                contentHeaders = request.Content.Headers;
            }

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Create the request message for this attempt (since a request cannot be sent twice)
                    HttpRequestMessage currentRequest;
                    if (attempt == 1)
                    {
                        currentRequest = request;
                    }
                    else
                    {
                        currentRequest = new HttpRequestMessage(request.Method, request.RequestUri);
                        foreach (var header in request.Headers)
                        {
                            currentRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        }
#pragma warning disable CS0618
                        foreach (var prop in request.Properties)
                        {
                            currentRequest.Properties.Add(prop.Key, prop.Value);
                        }
#pragma warning restore CS0618

                        if (contentBytes != null)
                        {
                            var ms = new MemoryStream(contentBytes);
                            currentRequest.Content = new StreamContent(ms);
                            if (contentHeaders != null)
                            {
                                foreach (var header in contentHeaders)
                                {
                                    currentRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                                }
                            }
                        }
                    }

                    response = await base.SendAsync(currentRequest, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }

                    // Only retry on server-side errors (5xx status codes)
                    if ((int)response.StatusCode < 500)
                    {
                        // Fail immediately for client errors (4xx)
                        return response;
                    }
                }
                catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                {
                    lastException = ex;
                    response = null;
                }

                if (attempt == maxRetries)
                {
                    break;
                }

                // Wait before retrying
                await Task.Delay(delay, cancellationToken);
            }

            // All retry attempts failed
            if (response != null)
            {
                return response;
            }

            if (lastException != null)
            {
                throw lastException;
            }

            return response ?? new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
        }

        private async Task HandleErrorResponseAsync(HttpResponseMessage response)
        {
            // Standard mapping by HTTP Status Code
            string friendlyMessage = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => "The request could not be completed. Please check the information entered and try again.",
                HttpStatusCode.Unauthorized => "Session expired. Please log in again.",
                HttpStatusCode.Forbidden => "Access denied. You do not have permission to perform this action.",
                HttpStatusCode.NotFound => "The requested resource could not be found.",
                HttpStatusCode.Conflict => "This change conflicts with existing school data. Please review the record and try again.",
                HttpStatusCode.TooManyRequests => "There are too many requests at the moment. Please wait a moment and try again.",
                HttpStatusCode.RequestTimeout => "The request timed out. Please check your network connection.",
                HttpStatusCode.InternalServerError => "The server could not complete that action. Please try again shortly.",
                HttpStatusCode.BadGateway => "The service is temporarily unavailable. Please try again shortly.",
                HttpStatusCode.ServiceUnavailable => "The service is temporarily offline. Please try again shortly.",
                HttpStatusCode.GatewayTimeout => "The server took too long to respond. Please try again.",
                _ => string.Empty
            };

            // If we already have a status code mapping, display it and return
            if (!string.IsNullOrEmpty(friendlyMessage))
            {
                _notificationService.ShowError(friendlyMessage);
                return;
            }

            // Otherwise, read the content body to determine if we can extract a friendly message
            try
            {
                var content = await response.Content.ReadAsStringAsync();
                friendlyMessage = CleanAndExtractMessage(content, response.StatusCode);
            }
            catch
            {
                // Fallback if reading stream fails
                friendlyMessage = GetFallbackMessage(response.StatusCode);
            }

            _notificationService.ShowError(friendlyMessage);
        }

        private string CleanAndExtractMessage(string content, HttpStatusCode statusCode)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return GetFallbackMessage(statusCode);
            }

            content = content.Trim();

            // Some endpoints return a Boolean response even when the operation was not completed.
            // Never surface that implementation detail as a notification.
            if (string.Equals(content, "false", StringComparison.OrdinalIgnoreCase)
                || string.Equals(content, "true", StringComparison.OrdinalIgnoreCase))
            {
                return GetFallbackMessage(statusCode);
            }

            // 1. Detect HTML response (Nginx/IIS crash pages or raw stack traces)
            if (content.StartsWith("<") || content.Contains("<html>") || content.Contains("<!DOCTYPE"))
            {
                return GetFallbackMessage(statusCode);
            }

            // 2. Detect overly long text (usually raw stack traces or long IIS dumps)
            if (content.Length > 300)
            {
                return GetFallbackMessage(statusCode);
            }

            // 3. Try parsing as JSON to find standardized error keys
            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                string? extracted = null;

                // Look for common JSON error properties
                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("message", out var msgProp) || root.TryGetProperty("Message", out msgProp))
                    {
                        extracted = msgProp.GetString();
                    }
                    else if (root.TryGetProperty("error", out var errProp) || root.TryGetProperty("Error", out errProp))
                    {
                        extracted = errProp.GetString();
                    }
                    else if (root.TryGetProperty("title", out var titleProp) || root.TryGetProperty("Title", out titleProp))
                    {
                        extracted = titleProp.GetString();
                    }
                }

                if (!string.IsNullOrWhiteSpace(extracted))
                {
                    var sanitized = SanitizeMessage(extracted.Trim());
                    if (!string.IsNullOrWhiteSpace(sanitized))
                    {
                        return sanitized;
                    }
                }
            }
            catch
            {
                // Not JSON, treat as plain text
            }

            // 4. Sanitized plain text message
            var cleanText = SanitizeMessage(content);
            if (!string.IsNullOrWhiteSpace(cleanText))
            {
                return cleanText;
            }

            return GetFallbackMessage(statusCode);
        }

        private string SanitizeMessage(string message)
        {
            var lower = message.ToLowerInvariant();

            if (lower is "false" or "true" || lower.Contains("internal error") || lower.Contains("internal server error"))
            {
                return "The server could not complete that action. Please try again shortly.";
            }

            // Detect database/SQL keywords to prevent leaking schemas/stack traces
            if (lower.Contains("sql") || lower.Contains("dbo.") || lower.Contains("database") || lower.Contains("table '"))
            {
                if (lower.Contains("duplicate") || lower.Contains("unique constraint"))
                {
                    return "A database conflict occurred. This record or unique identifier already exists.";
                }
                if (lower.Contains("foreign key") || lower.Contains("reference constraint"))
                {
                    return "Unable to complete action due to dependency constraints on this record.";
                }
                return "A database transaction error occurred. Please ensure your inputs are valid.";
            }

            // Detect code execution / server error keywords
            if (lower.Contains("exception:") || lower.Contains("stack trace") || lower.Contains("nullreferenceexception") || lower.Contains("object reference not set"))
            {
                return "An internal server error occurred while processing your request.";
            }

            // Detect network / connection error keywords
            if (lower.Contains("cannot connect") || lower.Contains("connection refused") || lower.Contains("network unreachable"))
            {
                return "Unable to connect to the backend server. Please check your network.";
            }

            // If the message is clean and short, return it as user-friendly
            if (message.Length < 180)
            {
                return message;
            }

            return string.Empty;
        }

        private string GetFallbackMessage(HttpStatusCode statusCode)
        {
            int code = (int)statusCode;
            if (code >= 500)
            {
                return "Something went wrong on our servers. Please try again later.";
            }
            if (code == 400)
            {
                return "The request was invalid. Please check your inputs.";
            }
            if (code == 429)
            {
                return "There are too many requests at the moment. Please wait a moment and try again.";
            }
            return "We could not complete that action right now. Please try again shortly.";
        }

    }
}
