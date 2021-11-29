using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;

namespace Kasumi.Telemetry;

public class TelemetryClient
{

    private string _eventUrl;
    private static string _metricUrl = "https://metric-api.eu.newrelic.com/metric/v1";

    private HttpClient _httpClient = new();

    private string _accountId;
    private string _licenseKey;

    public TelemetryClient(string accountId, string licenseKey)
    {

        this._accountId = accountId;
        this._licenseKey = licenseKey;

        this._eventUrl = $"https://insights-collector.eu01.nr-data.net/v1/accounts/{accountId}/events";

    }

    public async Task SendMetrics(MetricPayload[] payloads)
    {
        List<string> payloadJson = new();
        long currentUnixTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        string hostname = System.Net.Dns.GetHostName();

        foreach (var payload in payloads)
        {
            var payload2 = payload;

            payload2.Timestamp = currentUnixTime;
            payload2.Attributes.Add("host.name", hostname);
            
            string json = JsonConvert.SerializeObject(payload2);
            payloadJson.Add(json);
        }

        string finalJson = String.Join(',', payloadJson);

        var apiResponse = await _metricUrl.WithHeader("Content-Type", "application/json")
            .WithHeader("Api-Key", _licenseKey)
            .SendJsonAsync(HttpMethod.Post, finalJson);

        if (!apiResponse.StatusCode.Equals(202))
            throw new HttpRequestException(
                $"Request rejected by New Relic, response code was {apiResponse.StatusCode}");
    }

    public async Task SendEvent(Dictionary<string, object> payload)
    {
        if (!payload.First().Key.Equals("eventType")
            && !(payload.First().Value is string))
            throw new ArgumentException("First key in event payload should be eventType, and value should be a string");

        string json = JsonConvert.SerializeObject(payload);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        byte[] compressedBytes;

        await using (var outputStream = new MemoryStream())
        {
            await using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress))
                gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
            
            compressedBytes = outputStream.ToArray();

        }
        
        var apiResponse = await _eventUrl.WithHeader("Content-Type", "application/json")
            .WithHeader("Api-Key", _licenseKey)
            .WithHeader("Content-Encoding", "gzip")
            .SendAsync(HttpMethod.Post, new ByteArrayContent(compressedBytes));
        
        if (!apiResponse.StatusCode.Equals(200))
            throw new HttpRequestException(
                $"Request rejected by New Relic, response code was {apiResponse.StatusCode}");
    }

}