﻿using System.Data.SqlTypes;
using System.Security.Authentication;
using Dapper;
using infrastructure.Models;
using MySqlConnector;

namespace infrastructure;

public class DeviceRepository
{
    private readonly string _connectionString;

    public DeviceRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DeviceWithIdDto Create(DeviceDto deviceDto)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string createDeviceQuery = @"
                INSERT INTO Device (DeviceName, RoomId) 
                VALUES (@DeviceName, @RoomId)
                RETURNING *;";
            
            var createdDevice = connection.QueryFirst<DeviceWithIdDto>(createDeviceQuery, new { DeviceName = deviceDto.DeviceName, RoomId = deviceDto.RoomId });
            
            return new DeviceWithIdDto
            {
                DeviceName = createdDevice.DeviceName,
                RoomId = createdDevice.RoomId,
                Id = createdDevice.Id
            };
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Could not create device", ex);
        }
    }

    public bool DeleteDevice(int Id)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string deleteDeviceQuery = @"
                DELETE FROM Device
                WHERE Id = @Id;";

            return connection.Execute(deleteDeviceQuery, new { Id = Id }) == 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new SqlTypeException("Could not delete device", ex);
        }
    }
    
    
    /**
     * Gets all devices from a specific room.
     * Returns a list of devices - can be null if no devices in the room.
     */
    public IEnumerable<DeviceByRoomIdDto> GetDevicesByRoomId(int roomId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string getAllQuery = @"
                SELECT Id, DeviceName
                FROM Device 
                WHERE RoomId = @RoomId;";
            return connection.Query<DeviceByRoomIdDto>(getAllQuery, new {RoomId = roomId});
        }
        catch (Exception e)
        {
            // Handle exceptions, maybe log them
            throw new SqlTypeException("Failed to retrieve device(s) from room "+roomId, e);
        }
    }
    
    /**
     * Gets all devices for logged in user.
     * Returns a list of devices - can be null if user has no devices.
     */
    public IEnumerable<DeviceWithIdDto> GetDevicesByUserId(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string getAllQuery = @"
                SELECT Device.Id, DeviceName, Device.RoomId
                FROM Device 
                JOIN Room ON Device.RoomId = Room.Id
                WHERE Room.UserId = @UserId;";
            return connection.Query<DeviceWithIdDto>(getAllQuery, new {UserId = userId});
        }
        catch (Exception e)
        {
            // Handle exceptions, maybe log them
            throw new SqlTypeException("Failed to retrieve device(s) for user with id "+userId, e);
        }
    }
    
    /**
     * Gets device from device id.
     * Returns a device.
     */
    public DeviceWithIdDto GetDeviceById(int deviceId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string getDeviceByIdQuery = @"
                SELECT *
                FROM Device 
                WHERE Id = @DeviceId;";
            return connection.QueryFirstOrDefault<DeviceWithIdDto>(getDeviceByIdQuery, new {DeviceId = deviceId}) ?? throw new InvalidOperationException();
        }
        catch (Exception e)
        {
            // Handle exceptions, maybe log them
            throw new SqlTypeException("Failed to retrieve device with id "+deviceId, e);
        }
    }

    public bool IsItUsersRoom(int roomId, int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string getAllQuery = @"
                SELECT Id
                FROM Room 
                WHERE Id = @RoomId
                AND UserId = @UserId;";
            
            return connection.QuerySingleOrDefault(getAllQuery, new {UserId = userId, RoomId = roomId}) != null;
        }
        catch (Exception e)
        {
            // Handle exceptions, maybe log them
            throw new Exception(e.Message, e);
        }
    }
    
    public bool IsItUsersDevice(int deviceId, int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string getAllQuery = @"
                SELECT d.Id
                FROM Device AS d
                INNER JOIN Room AS r ON d.RoomId = r.Id
                WHERE d.Id = @DeviceId
                AND r.UserId = @UserId;";
            
            return connection.QuerySingleOrDefault(getAllQuery, new {UserId = userId, DeviceId = deviceId}) != null;
        }
        catch (Exception e)
        {
            // Handle exceptions, maybe log them
            throw new Exception(e.Message, e);
        }
    }

    public bool EditDevice(int dtoId, string deviceDtoDeviceName)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string editDeviceQuery = @"
            UPDATE Device
            SET DeviceName = @DeviceName
            WHERE Id = @DeviceId;";

            return connection.Execute(editDeviceQuery, new { DeviceName = deviceDtoDeviceName, DeviceId = dtoId }) > 0;
        }
        catch (Exception e)
        {
            // Handle exceptions, maybe log them
            throw new Exception(e.Message, e);
        }
    }
}