using System;
using Microsoft.SPOT;
using System.Collections;
using System.Threading;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

using GadgeteerApp1;



 class Movement
 {



     Program selector;
    
    
     void rotateEngine(string bind)
         {
            string msg = selector.receiveBuffXbee(); //On reçoit et lit le buffer envoyé par la Xbee
            string bindPressed = bind ; //On attribue une touche pour faire avancer l'Hexapode
            bool command_rotate=true;

             if (msg != bindPressed || msg.Length > 1)
             {
                 command_rotate = false;
             } 

             if (command_rotate == true)
             {
                 //Ici, faire tourner le servomoteur

                 int value = System.Convert.ToInt16(msg.Substring(1));

                 selector.displayText("Rotation: " + value + " degres", GT.Color.White);
                 //selector.tourne(value,0x02);
                 Thread.Sleep(200);
             }
        }

     void setEngineSpeed()
     {  
         string msg = selector.receiveBuffXbee();
         string speed = "vitesse";
         bool command_speed = true;

         for (int i = 0; i < speed.Length; i++)
            {
                if (speed[i] != msg[i] || msg.Length > 10)
                {
                   command_speed = false;
                   break;
                }
            }
         if (command_speed == true)
            {
                int value = System.Convert.ToInt16(msg.Substring(7));
                selector.displayText("Speed: " + value + " ms", GT.Color.White);

               // selector.vitesseMouv(value);
                Thread.Sleep(200);
            }
     }

     void Forward(int engineID)
     {
         engineID = 0; //On assigne l'ID à 0 par défaut
         byte[] buffer = new byte[] { };
            switch(engineID)
            {
                case 1 :
                    buffer = new byte[] { 0xFF, 0xFF, 0x01, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 2:
                    buffer = new byte[] { 0xFF, 0xFF, 0x02, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;

                case 3:
                    buffer = new byte[] { 0xFF, 0xFF, 0x03, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;

                case 4:
                    buffer = new byte[] { 0xFF, 0xFF, 0x04, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 5:
                    buffer = new byte[] { 0xFF, 0xFF, 0x05, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 6:
                    buffer = new byte[] { 0xFF, 0xFF, 0x06, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 7:
                    buffer = new byte[] { 0xFF, 0xFF, 0x07, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 8:
                    buffer = new byte[] { 0xFF, 0xFF, 0x08, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 9:
                    buffer = new byte[] { 0xFF, 0xFF, 0x09, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 10:
                    buffer = new byte[] { 0xFF, 0xFF, 0x0A, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 11:
                    buffer = new byte[] { 0xFF, 0xFF, 0x0B, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 12:
                    buffer = new byte[] { 0xFF, 0xFF, 0x0C, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 13:
                    buffer = new byte[] { 0xFF, 0xFF, 0x0D, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 14:
                    buffer = new byte[] { 0xFF, 0xFF, 0x0E, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 15:
                    buffer = new byte[] { 0xFF, 0xFF, 0x0F, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 16:
                    buffer = new byte[] { 0xFF, 0xFF, 0x1F, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 17:
                    buffer = new byte[] { 0xFF, 0xFF, 0x2F, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;
                case 18:
                    buffer = new byte[] { 0xFF, 0xFF, 0x3F, 0x05, 0x03, 0x20, 0, 0, 0 };
                    break;


                default :
                    selector.displayText("Default value",GT.Color.White);
                    break;
            }
     }

     void Left()
     {
     }

     void Right()
     { 
     }

     void Backward()
     { 
     }
 }
