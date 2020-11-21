using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FlyIt.Domain.Models.MetarResponse
{
    public partial class MetarResponse
    {
        [JsonProperty("results")]
        public long Results { get; set; }

        [JsonProperty("data")]
        public List<Datum> Data { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [JsonProperty("windChill")]
        public WindChill WindChill { get; set; }

        [JsonProperty("temperature")]
        public Dewpoint Temperature { get; set; }

        [JsonProperty("dewpoint")]
        public Dewpoint Dewpoint { get; set; }

        [JsonProperty("humidity")]
        public Humidity Humidity { get; set; }

        [JsonProperty("barometer")]
        public Barometer Barometer { get; set; }

        [JsonProperty("visibility")]
        public Visibility Visibility { get; set; }

        [JsonProperty("elevation")]
        public Elevation Elevation { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("icao")]
        public string Icao { get; set; }

        [JsonProperty("station")]
        public Station Station { get; set; }

        [JsonProperty("raw_text")]
        public string RawText { get; set; }

        [JsonProperty("observed")]
        public DateTimeOffset Observed { get; set; }

        [JsonProperty("flight_category")]
        public string FlightCategory { get; set; }

        [JsonProperty("clouds")]
        public Cloud[] Clouds { get; set; }

        [JsonProperty("conditions")]
        public object[] Conditions { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }
    }

    public partial class Barometer
    {
        [JsonProperty("hg")]
        public double Hg { get; set; }

        [JsonProperty("hpa")]
        public long Hpa { get; set; }

        [JsonProperty("kpa")]
        public double Kpa { get; set; }

        [JsonProperty("mb")]
        public double Mb { get; set; }
    }

    public partial class WindChill
    {
        [JsonProperty("celsius")]
        public long? Celsius { get; set; }

        [JsonProperty("fahrenheit")]
        public long Fahrenheit { get; set; }
    }

    public partial class Cloud
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("feet")]
        public long Feet { get; set; }

        [JsonProperty("meters")]
        public long Meters { get; set; }

        [JsonProperty("base_feet_agl")]
        public long BaseFeetAgl { get; set; }

        [JsonProperty("base_meters_agl")]
        public long BaseMetersAgl { get; set; }
    }

    public partial class Dewpoint
    {
        [JsonProperty("celsius")]
        public long Celsius { get; set; }

        [JsonProperty("fahrenheit")]
        public long Fahrenheit { get; set; }
    }

    public partial class Elevation
    {
        [JsonProperty("feet")]
        public long Feet { get; set; }

        [JsonProperty("meters")]
        public long Meters { get; set; }
    }

    public partial class Humidity
    {
        [JsonProperty("percent")]
        public long Percent { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Station
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Visibility
    {
        [JsonProperty("miles")]
        public string Miles { get; set; }

        [JsonProperty("miles_float")]
        public long MilesFloat { get; set; }

        [JsonProperty("meters")]
        public string Meters { get; set; }

        [JsonProperty("meters_float")]
        public long MetersFloat { get; set; }
    }

    public partial class Wind
    {
        [JsonProperty("degrees")]
        public long Degrees { get; set; }

        [JsonProperty("speed_kts")]
        public long SpeedKts { get; set; }

        [JsonProperty("speed_mph")]
        public long SpeedMph { get; set; }

        [JsonProperty("speed_mps")]
        public long SpeedMps { get; set; }

        [JsonProperty("speed_kph")]
        public long SpeedKph { get; set; }
    }
}