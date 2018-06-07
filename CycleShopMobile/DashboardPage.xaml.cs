using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Xamarin.Forms;
using System.Linq;


namespace CycleShopMobile
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();

            CrossGeolocator.Current.PositionChanged += PositionChanged;
            CrossGeolocator.Current.PositionError += PositionError;

            StartListening();
        }


        async Task StartListening()
        {
            if (CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StartListeningAsync(TimeSpan.FromSeconds(5), 10, true);


        }

        private async void PositionChanged(object sender, PositionEventArgs e)
        {

            //If updating the UI, ensure you invoke on main thread
            var position = e.Position;
            //var output = "Full: Lat: " + position.Latitude + " Long: " + position.Longitude;
            //output += "\n" + $"Time: {position.Timestamp}";
            //output += "\n" + $"Heading: {position.Heading}";
            //output += "\n" + $"Speed: {position.Speed}";
            //output += "\n" + $"Accuracy: {position.Accuracy}";
            //output += "\n" + $"Altitude: {position.Altitude}";
            //output += "\n" + $"Altitude Accuracy: {position.AltitudeAccuracy}";
            //Debug.WriteLine(output);
            try
            {
                string mapKey = null; //only needed on UWP
                var addresses = await CrossGeolocator.Current.GetAddressesForPositionAsync(position, mapKey);
                var address = addresses.FirstOrDefault();

                LocationLabel.Text = $"{address.Locality} {address.PostalCode}";
                RegionLabel.IsVisible = true;
                SalesLabel.IsVisible = true;
                EmployeesLabel.IsVisible = true;

            }
            catch (Exception ex)
            {
                //Debug.WriteLine("Unable to get address: " + ex);
            }
        }

        private void PositionError(object sender, PositionErrorEventArgs e)
        {
            //Debug.WriteLine(e.Error);
            //Handle event here for errors
        }

        async Task StopListening()
        {
            if (!CrossGeolocator.Current.IsListening)
                return;

            await CrossGeolocator.Current.StopListeningAsync();

            CrossGeolocator.Current.PositionChanged -= PositionChanged;
            CrossGeolocator.Current.PositionError -= PositionError;
        }
    }
}
