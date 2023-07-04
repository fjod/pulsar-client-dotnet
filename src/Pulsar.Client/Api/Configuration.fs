namespace Pulsar.Client.Api

open System.Collections.Generic
open FSharp.UMX
open Pulsar.Client.Common
open Pulsar.Client.Internal
open System
open System.Security.Authentication
open System.Security.Cryptography.X509Certificates

type PulsarClientConfiguration =
    {
        ServiceAddresses: Uri list
        OperationTimeout: TimeSpan
        StatsInterval: TimeSpan
        MaxNumberOfRejectedRequestPerConnection: int
        UseTls: bool
        TlsHostnameVerificationEnable: bool
        TlsAllowInsecureConnection: bool
        TlsTrustCertificate: X509Certificate2
        Authentication: Authentication
        TlsProtocols: SslProtocols
        ListenerName: string
        MaxLookupRedirects: int
        EnableTransaction: bool
        InitialBackoffInterval: TimeSpan
        MaxBackoffInterval: TimeSpan
        KeepAliveInterval: TimeSpan
    }
    static member Default =
        {
            ServiceAddresses = List.empty
            OperationTimeout = TimeSpan.FromMilliseconds(30000.0)
            StatsInterval = TimeSpan.Zero
            MaxNumberOfRejectedRequestPerConnection = 50
            UseTls = false
            TlsHostnameVerificationEnable = false
            TlsAllowInsecureConnection = false
            TlsTrustCertificate = null
            Authentication = Authentication.AuthenticationDisabled
            TlsProtocols = SslProtocols.None
            ListenerName = ""
            MaxLookupRedirects = 20
            EnableTransaction = false
            InitialBackoffInterval = TimeSpan.FromMilliseconds(100.0)
            MaxBackoffInterval = TimeSpan.FromSeconds(60.0)
            KeepAliveInterval = TimeSpan.FromSeconds(30.0)
        }

type ConsumerConfiguration<'T> =
    {
        Topics: TopicName seq
        TopicsPattern: string
        ConsumerName: string
        SubscriptionName: SubscriptionName
        SubscriptionType: SubscriptionType
        SubscriptionMode: SubscriptionMode
        ReceiverQueueSize: int
        MaxTotalReceiverQueueSizeAcrossPartitions: int
        SubscriptionInitialPosition: SubscriptionInitialPosition
        AckTimeout: TimeSpan
        AckTimeoutTickTime: TimeSpan
        AcknowledgementsGroupTime: TimeSpan
        AutoUpdatePartitions: bool
        AutoUpdatePartitionsInterval: TimeSpan
        PatternAutoDiscoveryPeriod: TimeSpan
        ReadCompacted: bool
        NegativeAckRedeliveryDelay: TimeSpan
        ResetIncludeHead: bool
        DeadLetterProcessor : TopicName -> IDeadLetterProcessor<'T>
        DeadLetterPolicy: DeadLetterPolicy option
        KeySharedPolicy: KeySharedPolicy option
        BatchReceivePolicy: BatchReceivePolicy
        PriorityLevel: PriorityLevel
        MessageDecryptor: IMessageDecryptor option
        ConsumerCryptoFailureAction: ConsumerCryptoFailureAction
        RetryEnable: bool
        BatchIndexAcknowledgmentEnabled: bool
        MaxPendingChunkedMessage: int
        AutoAckOldestChunkedMessageOnQueueFull: bool
        ExpireTimeOfIncompleteChunkedMessage: TimeSpan
    }
    member this.SingleTopic with get() = this.Topics |> Seq.head
    static member Default =
        {
            Topics = []
            TopicsPattern = ""
            ConsumerName = ""
            SubscriptionName = %""
            SubscriptionType = SubscriptionType.Exclusive
            SubscriptionMode = SubscriptionMode.Durable
            ReceiverQueueSize = 1000
            MaxTotalReceiverQueueSizeAcrossPartitions = 50000
            SubscriptionInitialPosition = SubscriptionInitialPosition.Latest
            AckTimeout = TimeSpan.Zero
            AckTimeoutTickTime = TimeSpan.FromMilliseconds(1000.0)
            AcknowledgementsGroupTime = TimeSpan.FromMilliseconds(100.0)
            AutoUpdatePartitions = true
            AutoUpdatePartitionsInterval = TimeSpan.FromSeconds(60.0)
            PatternAutoDiscoveryPeriod = TimeSpan.FromMinutes(1.0)
            ReadCompacted = false
            NegativeAckRedeliveryDelay = TimeSpan.FromMinutes(1.0)
            ResetIncludeHead = false
            DeadLetterProcessor = fun _ -> DeadLetterProcessor<'T>.Disabled
            DeadLetterPolicy = None
            KeySharedPolicy = None
            BatchReceivePolicy = BatchReceivePolicy()
            PriorityLevel = %0
            MessageDecryptor = None
            ConsumerCryptoFailureAction = ConsumerCryptoFailureAction.FAIL
            RetryEnable = false
            BatchIndexAcknowledgmentEnabled = false
            MaxPendingChunkedMessage = 10
            AutoAckOldestChunkedMessageOnQueueFull = false
            ExpireTimeOfIncompleteChunkedMessage = TimeSpan.FromSeconds(60.0)
        }

type ProducerConfiguration =
    {
        Topic: TopicName
        ProducerName: string
        MaxPendingMessagesAcrossPartitions: int
        MaxPendingMessages: int
        BatchingEnabled: bool
        ChunkingEnabled: bool
        BatchingMaxMessages: int
        BatchingMaxBytes: int
        BatchingMaxPublishDelay: TimeSpan
        BatchingPartitionSwitchFrequencyByPublishDelay: int
        BatchBuilder: BatchBuilder
        SendTimeout: TimeSpan
        CompressionType: CompressionType
        MessageRoutingMode: MessageRoutingMode
        CustomMessageRouter: IMessageRouter option
        AutoUpdatePartitions: bool
        AutoUpdatePartitionsInterval: TimeSpan
        HashingScheme: HashingScheme
        InitialSequenceId : SequenceId option
        BlockIfQueueFull: bool
        MessageEncryptor: IMessageEncryptor option
        ProducerCryptoFailureAction: ProducerCryptoFailureAction
    }
    member this.BatchingPartitionSwitchFrequencyIntervalMs =
        this.BatchingPartitionSwitchFrequencyByPublishDelay * (int this.BatchingMaxPublishDelay.TotalMilliseconds)
    static member Default =
        {
            Topic = Unchecked.defaultof<TopicName>
            ProducerName = ""
            MaxPendingMessages = 1000
            MaxPendingMessagesAcrossPartitions = 50000
            BatchingEnabled = true
            ChunkingEnabled = false
            BatchingMaxMessages = 1000
            BatchingMaxBytes = 128 * 1024 // 128KB
            BatchingMaxPublishDelay = TimeSpan.FromMilliseconds(1.0)
            BatchingPartitionSwitchFrequencyByPublishDelay = 10
            BatchBuilder = BatchBuilder.Default
            SendTimeout = TimeSpan.FromMilliseconds(30000.0)
            CompressionType = CompressionType.None
            MessageRoutingMode = MessageRoutingMode.RoundRobinPartition
            CustomMessageRouter = None
            AutoUpdatePartitions = true
            AutoUpdatePartitionsInterval = TimeSpan.FromSeconds(60.0)
            HashingScheme = HashingScheme.DotnetStringHash
            InitialSequenceId = Option.None
            BlockIfQueueFull = false
            MessageEncryptor = None
            ProducerCryptoFailureAction = ProducerCryptoFailureAction.FAIL
        }

type ReaderConfiguration =
    {
        Topic: TopicName
        StartMessageId: MessageId option
        SubscriptionRolePrefix: string
        ReceiverQueueSize: int
        ReadCompacted: bool
        ReaderName: string
        ResetIncludeHead: bool
        StartMessageFromRollbackDuration: TimeSpan
        MessageDecryptor: IMessageDecryptor option
        KeySharedPolicy: KeySharedPolicy option
        SubscriptionName: string
        AutoUpdatePartitions: bool
        AutoUpdatePartitionsInterval: TimeSpan
    }
    static member Default =
        {
            Topic = Unchecked.defaultof<TopicName>
            StartMessageId = None
            SubscriptionRolePrefix = ""
            ReceiverQueueSize = 1000
            ReadCompacted = false
            ReaderName = ""
            ResetIncludeHead = false
            StartMessageFromRollbackDuration = TimeSpan.Zero
            MessageDecryptor = None
            KeySharedPolicy = None
            SubscriptionName = ""
            AutoUpdatePartitions = true
            AutoUpdatePartitionsInterval = TimeSpan.FromSeconds(60.0)
        }

type TransactionConfiguration =
    {
        TxnTimeout: TimeSpan
    }
    static member Default =
        {
            TxnTimeout = TimeSpan.FromMinutes(1.0)
        }

type TableViewConfiguration =
    {
        Topic: TopicName
        AutoUpdatePartitions: bool
        AutoUpdatePartitionsInterval: TimeSpan
    }
    static member Default =
        {
            Topic = Unchecked.defaultof<TopicName>
            AutoUpdatePartitions = true
            AutoUpdatePartitionsInterval = TimeSpan.FromSeconds(60.0)
        }

type ControlledClusterFailoverConfiguration =
    {
        /// Default service url.
        ServiceUri : Uri
        /// Service url provider. ServiceUrlProvider will fetch serviceUrl from urlProvider periodically.
        UrlProvider : string
        /// Service url provider header to authenticate provider service.
        UrlProviderHeaders: Dictionary<string, string>
        /// Probe check interval.
        CheckInterval : TimeSpan
    }
    static member Default =
        {
            ServiceUri = Unchecked.defaultof<Uri>
            UrlProvider = String.Empty
            UrlProviderHeaders = Unchecked.defaultof<Dictionary<string, string>>
            CheckInterval = TimeSpan.FromSeconds(1)
        }

type FailoverPolicy =
    | ORDER

type AutoClusterFailoverConfiguration =
    {
        // Primary service url.
        Primary : Uri
        // Secondary service urls.
        Secondary : Uri list
        // Secondary choose policy. The default secondary choose policy is `ORDER`
        FailoverPolicy : FailoverPolicy
        SecondaryAuthentication : Dictionary<string, Authentication>
        SecondaryTlsTrustCertsFilePath : Dictionary<string, string>
        SecondaryTlsTrustStorePath : Dictionary<string, string>
        SecondaryTlsTrustStorePassword : Dictionary<string, string>
        // Switch failoverDelay. When one cluster failed longer than failoverDelay, it will trigger cluster switch.
        FailoverDelay : TimeSpan
        // SwitchBackDelay. When switched to the secondary cluster, and after the primary cluster comes back, it will wait for switchBackDelay to switch back to the primary cluster.
        SwitchBackDelay : TimeSpan
        // CheckInterval for probe.
        CheckInterval : TimeSpan
    }
    static member Default =
        {
            Primary = Unchecked.defaultof<Uri>
            Secondary = List.Empty
            FailoverPolicy = ORDER
            SecondaryAuthentication = Unchecked.defaultof<Dictionary<string, Authentication>>
            SecondaryTlsTrustCertsFilePath = Unchecked.defaultof<Dictionary<string, string>>
            SecondaryTlsTrustStorePath = Unchecked.defaultof<Dictionary<string, string>>
            SecondaryTlsTrustStorePassword = Unchecked.defaultof<Dictionary<string, string>>
            FailoverDelay = TimeSpan.FromSeconds(1)
            SwitchBackDelay = TimeSpan.FromSeconds(1)
            CheckInterval = TimeSpan.FromSeconds(1)
        }