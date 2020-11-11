using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FlyIt.Domain.Models.StationResponse
{
    public partial class StationResponse
    {
        [JsonProperty("results")]
        public long Results { get; set; }

        [JsonProperty("data")]
        public Datum[] Data { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("latitude")]
        public Itude Latitude { get; set; }

        [JsonProperty("longitude")]
        public Itude Longitude { get; set; }

        [JsonProperty("timezone")]
        public Timezone Timezone { get; set; }

        [JsonProperty("elevation")]
        public Elevation Elevation { get; set; }

        [JsonProperty("icao")]
        public string Icao { get; set; }

        [JsonProperty("iata")]
        public string Iata { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public partial class Country
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Elevation
    {
        [JsonProperty("feet")]
        public long Feet { get; set; }

        [JsonProperty("meters")]
        public long Meters { get; set; }
    }

    public partial class Itude
    {
        [JsonProperty("decimal")]
        public double Decimal { get; set; }

        [JsonProperty("degrees")]
        public string Degrees { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Timezone
    {
        [JsonProperty("tzid")]
        public string Tzid { get; set; }

        [JsonProperty("gmt")]
        public long Gmt { get; set; }

        [JsonProperty("zone")]
        public string Zone { get; set; }

        [JsonProperty("dst")]
        public bool Dst { get; set; }
    }
}