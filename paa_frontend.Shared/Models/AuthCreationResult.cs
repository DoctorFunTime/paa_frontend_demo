using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models;

public class AuthCreationResult
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("student_id")]
    public int StudentId { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }

    [JsonPropertyName("is_created")]
    public bool IsCreated { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}
