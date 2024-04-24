﻿using System.Data.SqlTypes;
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


    public DeviceWithId Create(DeviceDto deviceDto)
    {
        using var connection = new MySqlConnection(_connectionString);
        try
        {
            connection.Open();

            string createDeviceQuery = @"
                INSERT INTO Device (DeviceName, RoomId) 
                VALUES (@DeviceName, @RoomId)
                RETURNING *;";
            
            //Console.WriteLine("før conn.query");

            var createdDevice = connection.QueryFirst<DeviceWithId>(createDeviceQuery, new { DeviceName = deviceDto.DeviceName, RoomId = deviceDto.RoomId });
            
            return new DeviceWithId
            {
                DeviceName = createdDevice.DeviceName,
                RoomId = createdDevice.RoomId,
                Id = createdDevice.Id
            };
            Console.WriteLine(createdDevice.Id);
        }
        catch (Exception ex)
        {
            throw new SqlTypeException("Could not create device", ex);
        }
        
    }



}