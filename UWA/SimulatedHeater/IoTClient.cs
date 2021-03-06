﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Samples;

namespace SimHeater
{
    class IoTClient
    {
        private static int MESSAGE_COUNT = 5;

        // String containing Hostname, Device Id & Device Key in one of the following formats:
        private const string DeviceConnectionString = "HostName=<>hostname;DeviceId=<device>;SharedAccessKey=<key>="; //Update
        private static MainPage page;
        private static DeviceClient deviceClient;
        public async static Task Start(MainPage p)
        {
            page = p;
            try
            {
                deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString, TransportType.Http1);

                //await SendEvent(deviceClient);
                await ReceiveCommands();

                Debug.WriteLine("Exited!\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        internal static async Task SendEvent(String json)
        {
   
            Debug.WriteLine("Sending: " + json);
            Message eventMessage = new Message(Encoding.UTF8.GetBytes(json));
            deviceClient.SendEventAsync(eventMessage);
        }

        static async Task ReceiveCommands()
        {
            Debug.WriteLine("\nDevice waiting for commands from IoTHub...\n");
            Message receivedMessage;
            string messageData;

            while (true)
            {
                receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    Debug.WriteLine("\t{0}> Received message: {1}", DateTime.Now.ToLocalTime(), messageData);
                    page.showReceived(messageData);
                    await deviceClient.CompleteAsync(receivedMessage);
                }

                //  Note: In this sample, the polling interval is set to 
                //  10 seconds to enable you to see messages as they are sent.
                //  To enable an IoT solution to scale, you should extend this //  interval. For example, to scale to 1 million devices, set 
                //  the polling interval to 25 minutes.
                //  For further information, see
                //  https://azure.microsoft.com/documentation/articles/iot-hub-devguide/#messaging
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }
    }
}