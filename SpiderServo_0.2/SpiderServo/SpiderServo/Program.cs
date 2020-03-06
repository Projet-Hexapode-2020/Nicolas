﻿using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;


using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerApp1
{


    public partial class Program
    {
        // This method is run when the mainboard is powered up or reset.   
        
        GT.Interfaces.Serial serie;
        uint step = 2;

        void ProgramStarted()
        {

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            displayText("Debug: ", GT.Color.Red);
            serie = new GT.Interfaces.Serial(GT.Socket.GetSocket(11, true, null, string.Empty), 200000, GT.Interfaces.Serial.SerialParity.None, GT.Interfaces.Serial.SerialStopBits.One, 8, GT.Interfaces.Serial.HardwareFlowControl.NotRequired, null);
            serie.Open();

            xBee.Configure(115200, GT.Interfaces.Serial.SerialParity.None, GT.Interfaces.Serial.SerialStopBits.One, 8);
            xBee.SerialLine.Open();
            xBee.SerialLine.DataReceived += new GT.Interfaces.Serial.DataReceivedEventHandler(SerialLine_DataReceived);

            button.ButtonPressed += new Button.ButtonEventHandler(button_ButtonPressed);
            button.ButtonReleased += new Button.ButtonEventHandler(button_ButtonReleased);

            //Output to see if the controller has received the frame
            xBee.SerialLine.WriteLine("Connected");
            
        }

        void SerialLine_DataReceived(GT.Interfaces.Serial sender, System.IO.Ports.SerialData data)
        {
            var font = Resources.GetFont(Resources.FontResources.NinaB);
            string msg = receiveBuffXbee();

            string rotate = "tourner";
            string speed = "vitesse";
            string avancer = "avancer";
            bool command_rotate = true;
            bool command_speed = true;
            bool command_avancer = true;

            for (int i = 0; i < rotate.Length; i++)
            {
                if ((msg[i] != rotate[i]) || (msg.Length > 10))
                {
                    command_rotate = false;
                    break;
                }        
 
            }

            for (int i = 0; i < speed.Length; i++)
            {
                if ((msg[i] != speed[i]) || (msg.Length > 10))
                {
                    command_speed = false;
                    break;
                }
            }

            for (int i = 0; i < avancer.Length;i++)
            {
                if ((msg[i] != avancer[i] || (msg.Length > 10)))
                {
                    command_avancer = false;
                    break;
                }
                    
            }
           
            if ((command_avancer == true) && (command_speed == false) && (command_rotate == false))
            {
                int value = System.Convert.ToInt16(msg.Substring(7));

                displayText("Avancer: " + value ,GT.Color.White);
                Thread.Sleep(200);
            }

            if ((command_rotate == true) && (command_speed == false))
            {
                int value = System.Convert.ToInt16(msg.Substring(7));

                displayText("Rotation: " + value + " degres", GT.Color.White);


                rotateEngine(value, 0x01);
                Thread.Sleep(10);
                rotateEngine(value, 0x02);
            
                Thread.Sleep(200);
            }

            if ((command_speed == true) && (command_rotate == false))
            {
                int value = System.Convert.ToInt16(msg.Substring(7));
                displayText("Speed: " + value + " ms", GT.Color.White);

                setSpeed(value,0x01);
                Thread.Sleep(10);
                setSpeed(value,0x02);
                Thread.Sleep(200);
            }
            

        }

        void reinitServo()
        {
            byte[] buffer = new byte[] { 0xFF, 0xFF, 0X01, 0x05, 0x03, 0x1E, 0x00, 0x00, 0xD8 };
            serie.Write(buffer);
        }

        void button_ButtonReleased(Button sender, Button.ButtonState state)
        {
            if (step > 229)
            {
                display_T35.SimpleGraphics.Clear();
                step = 2;
                displayText("Debug: ", GT.Color.Red);

            }

           byte[] buffer = new byte[] { 0xFF, 0xFF, 0x01, 0x04, 0x03, 0x19, 0x00, 0xDE };
           serie.Write(buffer);
           rotateEngine(0, 0x01);
           Thread.Sleep(1000);
           byte[] buffer2 = new byte[] { 0xFF, 0xFF, 0x02, 0x04, 0x03, 0x19, 0x00, 0xDD };
           serie.Write(buffer2);
           rotateEngine(0, 0x02);
           displayText("Led: OFF", GT.Color.White);
           Thread.Sleep(200);
           
            

        }

        void button_ButtonPressed(Button sender, Button.ButtonState state)
        {
            byte[] buffer = new byte[] { 0xFF, 0xFF, 0x01, 0x04, 0x03, 0x19, 0x01, 0xDD };
            serie.Write(buffer);
            Thread.Sleep(1000);
            byte[] buffer2 = new byte[] { 0xFF, 0xFF, 0x02, 0x04, 0x03, 0x19, 0x01, 0xDC };
            serie.Write(buffer2);
            displayText("Led: ON", GT.Color.White);
            Thread.Sleep(200);

        }

        public void displayText(string msg, GT.Color color)
        {
            var font = Resources.GetFont(Resources.FontResources.small);          
            display_T35.SimpleGraphics.DisplayText(msg, font, color, 4, step);
            step += 12;

        }

       

        public void rotateEngine(int degre, byte id)
        {
            int valhex = degre * 0x3ff / 300;
            byte[] buffer = new byte[] { 0xFF, 0xFF, id , 0x05, 0x03, 0x1E, 0, 0, 0 };
           
            buffer[6] = (byte)(valhex & 0xff);
            buffer[7] = (byte)(valhex >> 8);

            calculateCheckSum(3, buffer);
            serie.Write(buffer);
            
        }

        public void setSpeed(int speed,byte id)//0-114)
        {

            int valhex = speed * 0x3ff / 114;
            byte[] buffer = new byte[] { 0xFF, 0xFF, id, 0x05, 0x03, 0x20, 0, 0, 0 };
            buffer[6] = (byte)(valhex & 0xFF);
            buffer[7] = (byte)(valhex >> 8);


            calculateCheckSum(3,buffer);
            serie.Write(buffer);
            

        }

        public string receiveBuffXbee()
        {
            int buff = xBee.SerialLine.BytesToRead;

            byte[] buffer = new byte[buff];
            xBee.SerialLine.Read(buffer, 0, buff);

            string msg = new string(System.Text.Encoding.UTF8.GetChars(buffer));
            return msg;
        }

       void calculateCheckSum(int nbParameters, byte[] buff)
        {
            int checkSum = 0;

            for (int i = 2; i < 5 + nbParameters ; i++)
            {
                checkSum += buff[i];
            }

            buff[5 + nbParameters] = (byte)(~checkSum & 0xff);
            
        }

       void getStatusPacket(/*byte id*/)
       {    

           byte[] statusPacket = new byte[] { 0xFF, 0xFF, 0x01, 0x02, 0x01, 0xFB };
           serie.Write(statusPacket); //On ping le servomoteur afin qu'il nous retourne une trame d'erreur             
       }
    }
}
