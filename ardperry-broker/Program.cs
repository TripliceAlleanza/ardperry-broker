using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.IO.Ports;

namespace ardperry_broker {
	class Program {
		static Socket sendSocket;
		static SerialPort serial;

		static void Main(string[] args) {



			var config = GetConfig();

			sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			serial = new SerialPort(config["SerialPort"]);
			serial.NewLine = "\n";
			serial.DataReceived += Serial_DataReceived;
			sendSocket.Connect(new IPEndPoint(IPAddress.Parse(config["ServerIP"]), int.Parse(config["ServerPort"])));
			serial.Open();

			

			while (true) {
				;
			}


			sendSocket.Close();
		}

		static void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e) {
			
			string readString = serial.ReadLine().Replace("\r", "");
			int value = int.Parse(readString);

			int sendValue = (int)Math.Round((float)value / 1024 * 100);

			Console.WriteLine("Sending " + sendValue);
			sendSocket.Send(Encoding.UTF8.GetBytes($"1#{sendValue}"));	
		}

		static Dictionary<string, string> GetConfig() {
			string text = File.ReadAllText("./config.conf").Replace("\r", "");

			return text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
			   .Select(part => part.Split('='))
			   .ToDictionary(split => split[0], split => split[1]);
		}
	}
}
