﻿using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.security;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using infrastructure.Models;
using lib;
using service.services;

namespace api.clientEventHandlers;


public class ClientWantsToAuthenticateWithJwtDto : BaseDto
{
    [Required] public string? jwt { get; set; }
}

[ValidateDataAnnotations]
public class ClientWantsToAuthenticateWithJwt  : BaseEventHandler<ClientWantsToAuthenticateWithJwtDto>
{
    
    private readonly AuthService _authService;
    private readonly TokenService _tokenService;

    public ClientWantsToAuthenticateWithJwt(
        AuthService authService,
        TokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }
    public override Task Handle(ClientWantsToAuthenticateWithJwtDto dto, IWebSocketConnection socket)
    {
        //validates the jwt
        var claims = _tokenService.ValidateJwtAndReturnClaims(dto.jwt!);
        //gets the user in db
        EndUser user = _authService.GetUserById(Int32.Parse(claims["userId"]));
       
        
        StateService.GetClient(socket.ConnectionInfo.Id).IsAuthenticated = true;
        StateService.GetClient(socket.ConnectionInfo.Id).User = user;
        
        socket.SendDto(new ServerAuthenticatesUser
        {
            Jwt = dto.jwt
        });
        return Task.CompletedTask;
    }
}