﻿using AppX.Services;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using AppX.Models;
using Newtonsoft.Json;
using System.Text;

public class SubscribeForVehiclesWorker : BackgroundService
{
    private readonly ConfigData _configData;
    private readonly IMessagesService _messagesService;
    private IModel? _subscribeChannel;
    private IModel? _publishChannel;

    public SubscribeForVehiclesWorker(IMessagesService messagesService, ConfigData configData)
    {
        _messagesService = messagesService;
        _configData = configData;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Use using for disposing the channels at the end
        using (var subscribeChannel = _messagesService.GetNewChannel())
        using (var publishChannel = _messagesService.GetNewChannel())
        {
            // Create channels
            _subscribeChannel = subscribeChannel;
            _publishChannel = publishChannel;

            // Subscribe
            _messagesService.SubscribeForStream(_subscribeChannel, _configData.ConsumeQueueName, OnVehicleArrived);

            // keep the worker alive
            await Task.Delay(-1, stoppingToken);
        }
    }

    void OnVehicleArrived(object? se, BasicDeliverEventArgs ea)
    {
        try
        {
            // Deserialize the message and print it
            var motorcycleBytes = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(motorcycleBytes);
            Vehicle motorcycle = JsonConvert.DeserializeObject<Motorcycle>(message);

            Console.WriteLine($"{motorcycle?.Timestamp} - Motorcycle with plate number {motorcycle?.PlateNumber} received from appY.");

            _messagesService.PublishToStream(_publishChannel, _configData.PublishUiQueueName, motorcycleBytes);
        }
        catch (Exception e)
        {
            Console.WriteLine("Got error while trying to get the vehicle data.", e);
        }
    }
}