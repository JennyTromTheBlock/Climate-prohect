﻿using infrastructure.Models;
using lib;

namespace api.serverEventModels;

public class ServerSendsDeviceById : BaseDto
{
    public required DeviceFullDto Device { get; set; }
}