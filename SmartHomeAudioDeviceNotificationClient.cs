using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeAudioDevice
{
    internal class SmartHomeAudioDeviceNotificationClient : NAudio.CoreAudioApi.Interfaces.IMMNotificationClient
    {

        private readonly HttpClient httpClient;
        private readonly string ioBrokerApiUrl;
        private readonly string stateId;
        private readonly string[] audioDevices;
        private bool? lastValue;

        public SmartHomeAudioDeviceNotificationClient(string ioBrokerApiUrl, string stateId, string[] audioDevices)
        {
            if (System.Environment.OSVersion.Version.Major < 6)
            {
                throw new NotSupportedException("This functionality is only supported on Windows Vista or newer.");
            }
            this.httpClient = new HttpClient();
            this.ioBrokerApiUrl = ioBrokerApiUrl;
            this.stateId = stateId;
            this.audioDevices = audioDevices;
            this.lastValue = null;
        }

        private void SendUpdateToIoBroker(bool value)
        {
            if (lastValue != null && lastValue == value) {
                Console.WriteLine("No need to update ioBroker");
                return; 
            }
            httpClient.GetStringAsync($"{ioBrokerApiUrl}/set/{stateId}?value={(value ? "true" : "false")}&ack=true");
            Console.WriteLine("Send update to ioBroker!");
            lastValue = value;
        }

        public void OnDefaultDeviceChanged(DataFlow dataFlow, Role deviceRole, string defaultDeviceId)
        {
            if (dataFlow != DataFlow.Render)
            {
                return;
            }
            Console.WriteLine($"Default Audio Output Changed --> new default device is {defaultDeviceId}");
            SendUpdateToIoBroker(audioDevices.Contains(defaultDeviceId));
        }

        public void OnDeviceAdded(string deviceId)
        {
            // Nothing to do
        }

        public void OnDeviceRemoved(string deviceId)
        {

            // Nothing to do
        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            // Nothing to do
        }



        public void OnPropertyValueChanged(string deviceId, PropertyKey propertyKey)
        {
            // Nothing to do

        }

    }
}
