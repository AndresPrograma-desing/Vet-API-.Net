using System;
using DTOs;

namespace vet_api_Net.Exceptions;

public class LoginSecurityException : Exception
{
    public LoginSecurityDTO Security { get; }

    public LoginSecurityException(LoginSecurityDTO security) : base(security.Message)
    {
        Security = security;
    }
}