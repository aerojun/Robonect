namespace Robonect
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Threading;

    using Microsoft.Kinect.Toolkit.Controls;

    using System.IO.Ports;
    using System.Management;

    /// <summary>
    /// A button that continually triggers a click when the mouse or hand pointer hovers over it
    /// </summary>
    internal class KinectHoverButton : KinectButtonBase
    {
        SerialPort conexionArduino = new SerialPort();

        public static readonly DependencyProperty IsHandPointerOverProperty = DependencyProperty.Register(
            "IsHandPointerOver", typeof(bool), typeof(KinectHoverButton), new PropertyMetadata(false));

        private const int ButtonRepeatIntervalMilliseconds = 1000 / 60;

        private static readonly bool IsInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());

        private readonly DispatcherTimer repeatTimer;

        private HandPointer activeHandpointer;

        public KinectHoverButton()
        {
            if (!IsInDesignMode)
            {
                this.InitializeKinectHoverButton();
                this.repeatTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(ButtonRepeatIntervalMilliseconds) };
                this.repeatTimer.Tick += this.RepeatTimerTick;
            }

            conexionArduino.BaudRate = 9600;
            conexionArduino.PortName = "COM4";
        }

        /// <summary>
        /// Boolean value that returns true if a mouse or hand pointer is over this button
        /// </summary>
        public bool IsHandPointerOver
        {
            get
            {
                return (bool)this.GetValue(IsHandPointerOverProperty);
            }

            set
            {
                this.SetValue(IsHandPointerOverProperty, value);
            }
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsHandPointerOver = true;
            this.repeatTimer.Start();
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsHandPointerOver = false;
            this.repeatTimer.Stop();

            conexionArduino.Open();

            conexionArduino.WriteLine("a");

            conexionArduino.Close();
        }

        private void InitializeKinectHoverButton()
        {
            KinectRegion.AddHandPointerEnterHandler(this, this.OnHandPointerEnter);
            KinectRegion.AddHandPointerLeaveHandler(this, this.OnHandPointerLeave);
        }

        private void RepeatTimerTick(object sender, EventArgs e)
        {
            this.OnClick();
        }

        private void OnHandPointerEnter(object sender, HandPointerEventArgs e)
        {
            if (!e.HandPointer.IsPrimaryHandOfUser || !e.HandPointer.IsPrimaryUser)
            {
                return;
            }

            this.activeHandpointer = e.HandPointer;
            this.IsHandPointerOver = true;
            this.repeatTimer.Start();
        }

        private void OnHandPointerLeave(object sender, HandPointerEventArgs e)
        {
            if (this.activeHandpointer != e.HandPointer)
            {
                return;
            }

            this.activeHandpointer = null;
            this.IsHandPointerOver = false;
            this.repeatTimer.Stop();

            
        }
    }
}
