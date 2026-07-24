using System;
using System.Text.Json.Serialization;

namespace DTOs
{
    public record ChangeNameUsersDTO
    {
        [JsonPropertyName("idUser")]
        public int IdUser { get; set; }

        [JsonPropertyName("newUserName")]
        public string? NewUserName { get; set; }

        [JsonPropertyName("newLastName")]
        public string? NewLastName { get; set; }
        [JsonPropertyName("email")]
        public string? NewEmail { get; set; }
        public string? NewPhone { get; set; }
    }
}