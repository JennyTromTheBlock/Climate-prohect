﻿namespace infrastructure.Models;

public class RoomModels
{
    
}

public class RoomWithId
{
    public required int Id { get; set; }
    public required string RoomName { get; set; }
}

public class RoomWithNoId
{
    public required string RoomName { get; set; }
}

public class RoomDto
{
    public required int Id { get; set; }

    public required string RoomName { get; set; }
}