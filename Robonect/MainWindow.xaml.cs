﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO.Ports;
using System.Management;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;

namespace Robonect
{
    public partial class MainWindow : Window
    {
        private readonly KinectSensorChooser sensorChooser;

        SerialPort conexionArduino = new SerialPort();

        public MainWindow()
        {

            //-------ARDUINO--------
            conexionArduino.BaudRate = 9600;
            string nombrePuerto = "";
            MessageBoxButton botones = MessageBoxButton.OKCancel;

            nombrePuerto = puertoArduino();

            while (nombrePuerto == null)
            {
                MessageBox.Show("Dispositivo no Enlazado", "Error", botones);
                nombrePuerto = puertoArduino();
            }

            conexionArduino.PortName = nombrePuerto;

            InitializeComponent();

            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            // initialize the sensor chooser and UI
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();

            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kinectRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.sensorChooser.Stop();

            if  ( conexionArduino.IsOpen )
                conexionArduino.Close();
        }


        private static void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }

        //Mover Motores

        private void AbrirPinza(object sender, RoutedEventArgs e)
        {
            try
            {
                conexionArduino.Open();

                conexionArduino.WriteLine("A");

                conexionArduino.Close();

            }
            catch (System.IO.IOException error)
            {
                MessageBox.Show(error.ToString());
            }
        }

        private void CerrarPinza(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cerrar Pinza");

        }

        private void IzquierdaMotor1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("IzquierdaMotor1");
        }

        private void DerechaMotor1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DerechaMotor1");

        }

        private void IzquierdaMotor2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("IzquierdaMotor2");
        }

        private void DerechaMotor2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DerechaMotor2");

        }

        private void IzquierdaMotor3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("IzquierdaMotor3");
        }

        private void DerechaMotor3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DerechaMotor3");

        }

        private void IzquierdaBase(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("IzquierdaBase");
        }

        private void DerechaBase(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("DerechaBase");

        }

        private static string puertoArduino()
        {
            ManagementScope connectionScope = new ManagementScope();
            SelectQuery serialQuery = new SelectQuery("SELECT * FROM Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(connectionScope, serialQuery);

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string desc = item["Description"].ToString();
                    string deviceId = item["DeviceID"].ToString();

                    if (desc.Contains("Arduino"))
                    {
                        return deviceId;
                    }
                }
            }
            catch (ManagementException e)
            {
                MessageBox.Show(e.Message.ToString());
            }

            return null;
        }
    }
}
