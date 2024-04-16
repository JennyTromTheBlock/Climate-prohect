﻿using System.Security.Authentication;
using System.Text.Json;
using Fleck;
using api.helpers;
using api.security;
using api.serverEventModels;
using api.WebSocket;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;


public class ClientWantsToSignInDto : BaseDto
{
    public string email { get; set; }
    public string password { get; set; }
}

public class ClientWantsToAuthenticate : BaseEventHandler<ClientWantsToSignInDto>
{
    private readonly AuthService _authService;
    private readonly TokenService _tokenService;

    public ClientWantsToAuthenticate(
        AuthService authService,
        TokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    public override Task Handle(ClientWantsToSignInDto request, IWebSocketConnection socket)
    {
        //gets user information from db and checks for ban status
        var user = _authService.GetUser(request.email);
        //if (user.Isbanned) throw new AuthenticationException("User is banned");
        
        //checks password hash
        bool validated = _authService.ValidateHash(request.password!, user.PasswordInfo!);
        if (!validated) throw new AuthenticationException("Wrong credentials!");

        //authenticates and sets user information in state service for later use
        StateService.GetClient(socket.ConnectionInfo.Id).IsAuthenticated = true;
        StateService.GetClient(socket.ConnectionInfo.Id).User = user;
            
        //sends the JWT token to the client
        socket.SendDto(new ServerAuthenticatesUser { Jwt = _tokenService.IssueJwt(user.Id) });
        return Task.CompletedTask;
    }
}
