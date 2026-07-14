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
    }
}