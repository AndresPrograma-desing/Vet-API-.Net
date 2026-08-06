using System;
using DTOs;

namespace vet_api_Net.Exceptions;

public class LoginSecurityException(LoginSecurityDTO security) : Exception(security.Message)
{
    public LoginSecurityDTO Security { get; } = security;
}