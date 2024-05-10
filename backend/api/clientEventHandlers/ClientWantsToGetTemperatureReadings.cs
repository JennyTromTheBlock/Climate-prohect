﻿using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToGetTemperatureReadingsDto : BaseDto
{
    [Required(ErrorMessage = "Device Id is required")]
    public int DeviceId { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToGetTemperatureReadings : BaseEventHandler<ClientWantsToGetTemperatureReadingsDto>
{
    private readonly DeviceReadingsService _deviceReadingsService;
    public ClientWantsToGetTemperatureReadings(DeviceReadingsService deviceReadingsService)
    {
        _deviceReadingsService = deviceReadingsService;
    }

    public override Task Handle(ClientWantsToGetTemperatureReadingsDto dto, IWebSocketConnection socket)
    {
        var guid = socket.ConnectionInfo.Id;

        if (!StateService.ConnectionHasDevice(guid, dto.DeviceId))
        {
            throw new AuthenticationException("Only the owner of device #"+dto.DeviceId+" has access to this information");
        }
        
        var readings =
            _deviceReadingsService.GetTemperatureReadingsFromDevice(dto.DeviceId);
        
        socket.SendDto(new ServerSendsTemperatureReadings
        {
            DeviceId = dto.DeviceId,
            TemperatureReadings = readings
        });

        return Task.CompletedTask;
    }
}