module Pulsar.Client.Failover

open System
open System.Collections.Generic
open System.Net.Http
open System.Threading.Tasks
open Pulsar.Client.Api
open System.Text.Json
open Pulsar.Client.Common
open Microsoft.Extensions.Logging

type ControlledConfiguration =
    {
        serviceUrl : string
        tlsTrustCertsFilePath : string
        authPluginClassName : string
        authParamsString : string
    }
    
// The ControlledClusterFailover will get the newest service url from the provided http service periodically.
type ControlledClusterFailover(config:ControlledClusterFailoverConfiguration, pulsarClient:PulsarClient) =
    inherit ServiceUrlProvider()    
    
    let DEFAULT_CONNECT_TIMEOUT = TimeSpan.FromSeconds 10
    let DEFAULT_MAX_REDIRECTS = 20
   
    let buildClient() =
        let handler = new HttpClientHandler()
        handler.AllowAutoRedirect <- true
        handler.MaxAutomaticRedirections <- DEFAULT_MAX_REDIRECTS
        let httpClient = new HttpClient(handler)
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Pulsar-dotnet")
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json")
        if (config.UrlProviderHeaders <> null) then
            for entry in config.UrlProviderHeaders do
                 httpClient.DefaultRequestHeaders.Add(entry.Key, entry.Value)
        httpClient.Timeout <- DEFAULT_CONNECT_TIMEOUT
        httpClient
    let httpClient = buildClient()
    
    let mutable currentConfiguration : ControlledConfiguration = {
        serviceUrl = config.ServiceUri.ToString()
        tlsTrustCertsFilePath = String.Empty
        authPluginClassName = String.Empty
        authParamsString = String.Empty
    }
    let mutable keepRequestingForAddress = true
    do (backgroundTask {
        
        while keepRequestingForAddress do            
          let! response = httpClient.GetStringAsync(config.UrlProvider)
          try
            let result = JsonSerializer.Deserialize<ControlledConfiguration>(response)
            if (currentConfiguration.serviceUrl <>  result.serviceUrl) then
                Log.Logger.LogInformation("Switching to new service url: " + result.serviceUrl)
                let oldConf = pulsarClient.GetConf()
                
                 
                currentConfiguration <- result
                
            ()             
          with ex ->
              let (Flatten ex) = ex
              Log.Logger.LogCritical(ex, "Bad response from server, which provides url for pulsar cluster")
          ()
        }
        |> ignore)
        
    override this.Dispose() =
        keepRequestingForAddress <- false
        httpClient.Dispose()
    override this.GetServiceUrl() = failwith "todo"
    override this.init() = failwith "todo"
    
