﻿using System.ComponentModel.DataAnnotations;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToDeleteDeviceDto : BaseDto
{ 
    [Required(ErrorMessage = "Device Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Device Id is not a valid number")]
    public int Id { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToDeleteDevice : BaseEventHandler<ClientWantsToDeleteDeviceDto>
{
    private readonly DeviceService _deviceService;
    private readonly DeviceReadingsService _deviceReadingsService;

    public ClientWantsToDeleteDevice(DeviceService deviceService, DeviceReadingsService deviceReadingsService)
    {
        _deviceService = deviceService;
        _deviceReadingsService = deviceReadingsService;
    }
    
    
    //todo skal tjekke om du er tilkoblet device først!!
    public override Task Handle(ClientWantsToDeleteDeviceDto dto, IWebSocketConnection socket)
    { 
       _deviceReadingsService.DeleteAllReadings(dto.Id);
       _deviceService.DeleteDevice(dto.Id);
       socket.SendDto(new ServerSendsDeviceDeletionStatus { IsDeleted = true});
       return Task.CompletedTask;
    }
}
