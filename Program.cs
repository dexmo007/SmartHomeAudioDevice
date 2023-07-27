using NAudio;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using SmartHomeAudioDevice;
using System.Runtime.InteropServices;

internal class Program
{

    private readonly MMDeviceEnumerator deviceEnum = new();
    private SmartHomeAudioDeviceNotificationClient? notificationClient;

    /// <summary>
    /// Registers a call back for Device Events
    /// </summary>
    /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface</param>
    /// <returns></returns>
    public int RegisterEndpointNotificationCallback([In][MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
    {
        //DeviceEnum declared below
        return deviceEnum.RegisterEndpointNotificationCallback(client);
    }

    /// <summary>
    /// UnRegisters a call back for Device Events
    /// </summary>
    /// <param name="client">Object implementing IMMNotificationClient type casted as IMMNotificationClient interface </param>
    /// <returns></returns>
    public int UnRegisterEndpointNotificationCallback([In][MarshalAs(UnmanagedType.Interface)] IMMNotificationClient client)
    {
        //DeviceEnum declared below
        return deviceEnum.UnregisterEndpointNotificationCallback(client);
    }

    private void Run(bool onlyShow)
    {
        foreach (var device in deviceEnum.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
        {
            Console.WriteLine($"{device.DeviceFriendlyName} ({device.ID})");
        }
        if (onlyShow) { return; }
        string ioBrokerApiUrl = "http://192.168.178.225:8087";
        string stateId = "0_userdata.0.computer.audio_to_amplifier";
        string[] audioDevices = { "{0.0.0.00000000}.{e9184462-38c7-41f9-96e5-41af37e7c02f}" };
        notificationClient = new SmartHomeAudioDeviceNotificationClient(ioBrokerApiUrl, stateId, audioDevices);
        deviceEnum.RegisterEndpointNotificationCallback(notificationClient);


    }

    private static void Main(string[] args)
    {
        new Program().Run(false);


        Console.WriteLine("Listening...");
        Console.ReadLine();
    }
}