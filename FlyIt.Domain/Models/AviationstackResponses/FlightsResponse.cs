using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FlyIt.Domain.Models.AviationstackResponses
{
    public partial class FlightsResponse
    {
        [JsonProperty("pagination", NullValueHandling = NullValueHandling.Ignore)]
        public Pagination Pagination { get; set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public List<Data> Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("flight_date", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset FlightDate { get; set; }

        [JsonProperty("flight_status", NullValueHandling = NullValueHandling.Ignore)]
        public string FlightStatus { get; set; }

        [JsonProperty("departure", NullValueHandling = NullValueHandling.Ignore)]
        public DepartureDestination Departure { get; set; }

        [JsonProperty("arrival", NullValueHandling = NullValueHandling.Ignore)]
        public DepartureDestination Arrival { get; set; }

        [JsonProperty("airline", NullValueHandling = NullValueHandling.Ignore)]
        public Airline Airline { get; set; }

        [JsonProperty("flight", NullValueHandling = NullValueHandling.Ignore)]
        public Flight Flight { get; set; }
    }

    public partial class Airline
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("iata", NullValueHandling = NullValueHandling.Ignore)]
        public string Iata { get; set; }

        [JsonProperty("icao", NullValueHandling = NullValueHandling.Ignore)]
        public string Icao { get; set; }
    }

    public partial class DepartureDestination
    {
        [JsonProperty("airport", NullValueHandling = NullValueHandling.Ignore)]
        public string Airport { get; set; }

        [JsonProperty("timezone", NullValueHandling = NullValueHandling.Ignore)]
        public string Timezone { get; set; }

        [JsonProperty("iata", NullValueHandling = NullValueHandling.Ignore)]
        public string Iata { get; set; }

        [JsonProperty("icao", NullValueHandling = NullValueHandling.Ignore)]
        public string Icao { get; set; }

        [JsonProperty("terminal")]
        public string Terminal { get; set; }

        [JsonProperty("gate")]
        public string Gate { get; set; }

        [JsonProperty("baggage", NullValueHandling = NullValueHandling.Ignore)]
        public long? Baggage { get; set; }

        [JsonProperty("delay")]
        public long? Delay { get; set; }

        [JsonProperty("scheduled", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Scheduled { get; set; }

        [JsonProperty("estimated", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Estimated { get; set; }

        [JsonProperty("actual", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Actual { get; set; }

        [JsonProperty("estimated_runway", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? EstimatedRunway { get; set; }

        [JsonProperty("actual_runway", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? ActualRunway { get; set; }
    }

    public partial class Flight
    {
        [JsonProperty("number", NullValueHandling = NullValueHandling.Ignore)]
        public long? Number { get; set; }

        [JsonProperty("iata", NullValueHandling = NullValueHandling.Ignore)]
        public string Iata { get; set; }

        [JsonProperty("icao", NullValueHandling = NullValueHandling.Ignore)]
        public string Icao { get; set; }

        [JsonProperty("codeshared")]
        public object Codeshared { get; set; }
    }

    public partial class Pagination
    {
        [JsonProperty("limit", NullValueHandling = NullValueHandling.Ignore)]
        public long? Limit { get; set; }

        [JsonProperty("offset", NullValueHandling = NullValueHandling.Ignore)]
        public long? Offset { get; set; }

        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public long? Count { get; set; }

        [JsonProperty("total", NullValueHandling = NullValueHandling.Ignore)]
        public long? Total { get; set; }
    }
}