namespace Pulsar.Client.Api

open Pulsar.Client.Common

type ControlledClusterFailoverBuilder private (config: ControlledClusterFailoverConfiguration) =
    
      new() = ControlledClusterFailoverBuilder(ControlledClusterFailoverConfiguration.Default)
      
       member this.ServiceUrl (url: string) =
            match url |> ServiceUri.parse with
            | Result.Ok serviceUri ->
                ControlledClusterFailoverBuilder { config with ServiceUri = serviceUri.Addresses.Head }
            | Result.Error message -> invalidArg null message
       
       member this.UrlProvider urlProvider =
            ControlledClusterFailoverBuilder { config with UrlProvider = urlProvider }
       
       member this.UrlProviderHeaders urlProviderHeaders =
            ControlledClusterFailoverBuilder { config with UrlProviderHeaders = urlProviderHeaders }
       
       member this.CheckInterval interval =
            ControlledClusterFailoverBuilder { config with CheckInterval = interval }
            
type AutoClusterFailoverBuilder private (config: AutoClusterFailoverConfiguration) =
    
      new() = AutoClusterFailoverBuilder(AutoClusterFailoverConfiguration.Default)
      
       member this.PrimaryUrl (url: string) =
            match url |> ServiceUri.parse with
            | Result.Ok serviceUri ->
                AutoClusterFailoverBuilder { config with Primary = serviceUri.Addresses.Head }
            | Result.Error message -> invalidArg null message
       
        member this.Secondary (url: string) =
            match url |> ServiceUri.parse with
            | Result.Ok serviceUri ->
                AutoClusterFailoverBuilder { config with Secondary = serviceUri.Addresses }
            | Result.Error message -> invalidArg null message
            
       member this.UrlProvider secondaryAuthentication =
            AutoClusterFailoverBuilder { config with SecondaryAuthentication = secondaryAuthentication }
       
       member this.UrlProviderHeaders secondaryTlsTrustCertsFilePath =
            AutoClusterFailoverBuilder { config with SecondaryTlsTrustCertsFilePath = secondaryTlsTrustCertsFilePath }
       
       member this.SecondaryTlsTrustStorePath secondaryTlsTrustStorePath =
            AutoClusterFailoverBuilder { config with SecondaryTlsTrustStorePath = secondaryTlsTrustStorePath }
            
       member this.SecondaryTlsTrustStorePassword secondaryTlsTrustStorePassword =
            AutoClusterFailoverBuilder { config with SecondaryTlsTrustStorePassword = secondaryTlsTrustStorePassword }
       member this.CheckInterval interval =
            AutoClusterFailoverBuilder { config with CheckInterval = interval }
        member this.FailoverDelay delay =
            AutoClusterFailoverBuilder { config with FailoverDelay = delay }
        member this.SwitchBackDelay delay =
            AutoClusterFailoverBuilder { config with SwitchBackDelay = delay }