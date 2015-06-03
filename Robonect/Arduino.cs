using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Serial Port
using System.IO.Ports;
using System.Management;
using System.Windows;

namespace Robonect
{
    class Arduino
    {
        private int baudrate = 9600;
        private string port;
        private SerialPort connection;

        public Arduino()
        {
            connection = new SerialPort();
            connection.BaudRate = this.baudrate;

            port = getPort();

            while (port == null)
            {
                MessageBox.Show("Device not found", "Error", MessageBoxButton.OKCancel);
                port = getPort();
            }

            connection.PortName = port;
        }

        private static string getPort()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery query = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher search = new ManagementObjectSearcher(connectionScope, query);

            try
            {
                foreach (ManagementObject item in search.Get())
                {
                    string description = item["Description"].ToString();
                    string device = item["DeviceID"].ToString();

                    if (description.Contains("Arduino"))
                    {
                        return device;
                    }
                }
            }
            catch (ManagementException e)
            {
                //
            }

            return null;
        }

        public void send(string data)
        {
            try
            {
                if ( connection.IsOpen ) {

                    connection.WriteLine(data);
                }
                else {

                    connection.Open();
                    connection.WriteLine(data);
                }
                connection.Close();
            }
            catch (System.IO.IOException error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        public void open()
        {
            connection.Open();
        }

        public void close()
        {
            connection.Close();
        }

        public bool isOpen()
        {
            return connection.IsOpen;
        }

        public bool isClosed()
        {
            return !connection.IsOpen;
        }
    }
}
