/*
*  Copyright 2023 MASES s.r.l.
*
*  Licensed under the Apache License, Version 2.0 (the "License");
*  you may not use this file except in compliance with the License.
*  You may obtain a copy of the License at
*
*  http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*  Refer to LICENSE for more information.
*/

#nullable enable

using Java.Lang;
using Java.Util;
using MASES.EntityFrameworkCore.KNet.Serialization.Json;
using MASES.EntityFrameworkCore.KNet.Serialization.Json.Storage;
using MASES.KNet.Common;
using MASES.KNet.Consumer;
using MASES.KNet.Producer;
using MASES.KNet.Streams;
using System.Globalization;

namespace MASES.EntityFrameworkCore.KNet.Infrastructure.Internal;
/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class KafkaOptionsExtension : IDbContextOptionsExtension
{
    private Type _keySerializationType = typeof(DefaultKEFCoreSerDes.Key.Json<>);
    private Type _valueSerializationType = typeof(DefaultKEFCoreSerDes.ValueContainer.Json<>);
    private Type _valueContainerType = typeof(DefaultValueContainer<>);
    private bool _useNameMatching = true;
    private string? _databaseName;
    private string? _applicationId;
    private string? _bootstrapServers;
    private bool _useDeletePolicyForTopic = false;
    private bool _useCompactedReplicator = true;
    private bool _usePersistentStorage = false;
    private int _defaultNumPartitions = 1;
    private int? _defaultConsumerInstances = null;
    private short _defaultReplicationFactor = 1;
    private ConsumerConfigBuilder? _consumerConfigBuilder;
    private ProducerConfigBuilder? _producerConfigBuilder;
    private StreamsConfigBuilder? _streamsConfigBuilder;
    private TopicConfigBuilder? _topicConfigBuilder;
    private Action<IEntityType, bool, object>? _onChangeEvent = null;
    private DbContextOptionsExtensionInfo? _info;

    static Java.Lang.ClassLoader _loader = Java.Lang.ClassLoader.SystemClassLoader;
    static Java.Lang.ClassLoader SystemClassLoader => _loader;
    /// <summary>
    /// Initializer
    /// </summary>
    public KafkaOptionsExtension()
    {
    }
    /// <summary>
    /// Initializer
    /// </summary>
    protected KafkaOptionsExtension(KafkaOptionsExtension copyFrom)
    {
        _keySerializationType = copyFrom._keySerializationType;
        _valueSerializationType = copyFrom._valueSerializationType;
        _valueContainerType = copyFrom._valueContainerType;
        _useNameMatching = copyFrom._useNameMatching;
        _databaseName = copyFrom._databaseName;
        _applicationId = copyFrom._applicationId;
        _bootstrapServers = copyFrom._bootstrapServers;
        _useDeletePolicyForTopic = copyFrom._useDeletePolicyForTopic;
        _useCompactedReplicator = copyFrom._useCompactedReplicator;
        _usePersistentStorage = copyFrom._usePersistentStorage;
        _defaultNumPartitions = copyFrom._defaultNumPartitions;
        _defaultConsumerInstances = copyFrom._defaultConsumerInstances;
        _defaultReplicationFactor = copyFrom._defaultReplicationFactor;
        _consumerConfigBuilder = ConsumerConfigBuilder.CreateFrom(copyFrom._consumerConfigBuilder);
        _producerConfigBuilder = ProducerConfigBuilder.CreateFrom(copyFrom._producerConfigBuilder);
        _streamsConfigBuilder = StreamsConfigBuilder.CreateFrom(copyFrom._streamsConfigBuilder);
        _topicConfigBuilder = TopicConfigBuilder.CreateFrom(copyFrom._topicConfigBuilder);
        _onChangeEvent = copyFrom._onChangeEvent;
    }
    /// <inheritdoc/>
    public virtual DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);
    /// <inheritdoc/>
    protected virtual KafkaOptionsExtension Clone() => new(this);
    /// <summary>
    /// Internal property
    /// </summary>
    public virtual string ClusterId => _bootstrapServers!;
    /// <inheritdoc cref="KafkaDbContext.KeySerializationType"/>
    public virtual Type KeySerializationType => _keySerializationType;
    /// <inheritdoc cref="KafkaDbContext.ValueSerializationType"/>
    public virtual Type ValueSerializationType => _valueSerializationType;
    /// <inheritdoc cref="KafkaDbContext.ValueContainerType"/>
    public virtual Type ValueContainerType => _valueContainerType;
    /// <inheritdoc cref="KafkaDbContext.UseNameMatching"/>
    public virtual bool UseNameMatching => _useNameMatching;
    /// <inheritdoc cref="KafkaDbContext.DatabaseName"/>
    public virtual string DatabaseName => _databaseName!;
    /// <inheritdoc cref="KafkaDbContext.ApplicationId"/>
    public virtual string ApplicationId => _applicationId!;
    /// <inheritdoc cref="KafkaDbContext.BootstrapServers"/>
    public virtual string BootstrapServers => _bootstrapServers!;
    /// <inheritdoc cref="KafkaDbContext.UseDeletePolicyForTopic"/>
    public virtual bool UseDeletePolicyForTopic => _useDeletePolicyForTopic;
    /// <inheritdoc cref="KafkaDbContext.UseCompactedReplicator"/>
    public virtual bool UseCompactedReplicator => _useCompactedReplicator;
    /// <inheritdoc cref="KafkaDbContext.UsePersistentStorage"/>
    public virtual bool UsePersistentStorage => _usePersistentStorage;
    /// <inheritdoc cref="KafkaDbContext.DefaultNumPartitions"/>
    public virtual int DefaultNumPartitions => _defaultNumPartitions;
    /// <inheritdoc cref="KafkaDbContext.DefaultConsumerInstances"/>
    public virtual int? DefaultConsumerInstances => _defaultConsumerInstances;
    /// <inheritdoc cref="KafkaDbContext.DefaultReplicationFactor"/>
    public virtual short DefaultReplicationFactor => _defaultReplicationFactor;
    /// <inheritdoc cref="KafkaDbContext.ConsumerConfig"/>
    public virtual ConsumerConfigBuilder ConsumerConfig => _consumerConfigBuilder!;
    /// <inheritdoc cref="KafkaDbContext.ProducerConfig"/>
    public virtual ProducerConfigBuilder ProducerConfig => _producerConfigBuilder!;
    /// <inheritdoc cref="KafkaDbContext.StreamsConfig"/>
    public virtual StreamsConfigBuilder StreamsConfig => _streamsConfigBuilder!;
    /// <inheritdoc cref="KafkaDbContext.TopicConfig"/>
    public virtual TopicConfigBuilder TopicConfig => _topicConfigBuilder!;
    /// <inheritdoc cref="KafkaDbContext.OnChangeEvent"/>
    public virtual Action<IEntityType, bool, object> OnChangeEvent => _onChangeEvent!;
    /// <inheritdoc cref="KafkaDbContext.KeySerializationType"/>
    public virtual KafkaOptionsExtension WithKeySerializationType(Type serializationType)
    {
        if (!serializationType.IsGenericTypeDefinition) throw new InvalidOperationException($"{serializationType.Name} shall be a generic type and shall be defined using \"<>\"");

        var clone = Clone();

        clone._keySerializationType = serializationType;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.ValueSerializationType"/>
    public virtual KafkaOptionsExtension WithValueSerializationType(Type serializationType)
    {
        if (!serializationType.IsGenericTypeDefinition) throw new InvalidOperationException($"{serializationType.Name} shall be a generic type and shall be defined using \"<>\"");

        var clone = Clone();

        clone._valueSerializationType = serializationType;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.ValueContainerType"/>
    public virtual KafkaOptionsExtension WithValueContainerType(Type serializationType)
    {
        if (!serializationType.IsGenericTypeDefinition) throw new InvalidOperationException($"{serializationType.Name} shall be a generic type and shall be defined using \"<>\"");

        var clone = Clone();

        clone._valueContainerType = serializationType;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.OnChangeEvent"/>
    public virtual KafkaOptionsExtension WithUseNameMatching(bool useNameMatching = true)
    {
        var clone = Clone();

        clone._useNameMatching = useNameMatching;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.DatabaseName"/>
    public virtual KafkaOptionsExtension WithDatabaseName(string databaseName)
    {
        var clone = Clone();

        clone._databaseName = databaseName;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.ApplicationId"/>
    public virtual KafkaOptionsExtension WithApplicationId(string applicationId)
    {
        var clone = Clone();

        clone._applicationId = applicationId;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.BootstrapServers"/>
    public virtual KafkaOptionsExtension WithBootstrapServers(string bootstrapServers)
    {
        var clone = Clone();

        clone._bootstrapServers = bootstrapServers;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.UseDeletePolicyForTopic"/>
    public virtual KafkaOptionsExtension WithUseDeletePolicyForTopic(bool useDeletePolicyForTopic = false)
    {
        var clone = Clone();

        clone._useDeletePolicyForTopic = useDeletePolicyForTopic;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.UseCompactedReplicator"/>
    public virtual KafkaOptionsExtension WithCompactedReplicator(bool useCompactedReplicator = true)
    {
        var clone = Clone();

        clone._useCompactedReplicator = useCompactedReplicator;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.UsePersistentStorage"/>
    public virtual KafkaOptionsExtension WithUsePersistentStorage(bool usePersistentStorage = false)
    {
        var clone = Clone();

        clone._usePersistentStorage = usePersistentStorage;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.DefaultNumPartitions"/>
    public virtual KafkaOptionsExtension WithDefaultNumPartitions(int defaultNumPartitions = 1)
    {
        var clone = Clone();

        clone._defaultNumPartitions = defaultNumPartitions;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.DefaultConsumerInstances"/>
    public virtual KafkaOptionsExtension WithDefaultConsumerInstances(int? defaultConsumerInstances = null)
    {
        var clone = Clone();

        clone._defaultConsumerInstances = defaultConsumerInstances;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.DefaultReplicationFactor"/>
    public virtual KafkaOptionsExtension WithDefaultReplicationFactor(short defaultReplicationFactor = 1)
    {
        var clone = Clone();

        clone._defaultReplicationFactor = defaultReplicationFactor;

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.ConsumerConfig"/>
    public virtual KafkaOptionsExtension WithConsumerConfig(ConsumerConfigBuilder consumerConfigBuilder)
    {
        var clone = Clone();

        clone._consumerConfigBuilder = ConsumerConfigBuilder.CreateFrom(consumerConfigBuilder);

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.ProducerConfig"/>
    public virtual KafkaOptionsExtension WithProducerConfig(ProducerConfigBuilder producerConfigBuilder)
    {
        var clone = Clone();

        clone._producerConfigBuilder = ProducerConfigBuilder.CreateFrom(producerConfigBuilder);

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.StreamsConfig"/>
    public virtual KafkaOptionsExtension WithStreamsConfig(StreamsConfigBuilder streamsConfigBuilder)
    {
        var clone = Clone();

        clone._streamsConfigBuilder = StreamsConfigBuilder.CreateFrom(streamsConfigBuilder);

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.TopicConfig"/>
    public virtual KafkaOptionsExtension WithTopicConfig(TopicConfigBuilder topicConfigBuilder)
    {
        var clone = Clone();

        clone._topicConfigBuilder = TopicConfigBuilder.CreateFrom(topicConfigBuilder);

        return clone;
    }
    /// <inheritdoc cref="KafkaDbContext.OnChangeEvent"/>
    public virtual KafkaOptionsExtension WithOnChangeEvent(Action<IEntityType, bool, object> onChangeEvent)
    {
        var clone = Clone();

        clone._onChangeEvent = onChangeEvent;

        return clone;
    }
    /// <summary>
    /// Build <see cref="Properties"/> for <see cref="IEntityType"/>
    /// </summary>
    public virtual Properties StreamsOptions(IEntityType entityType)
    {
        return StreamsOptions(entityType.ApplicationIdForTable(this));
    }
    /// <summary>
    /// Build <see cref="Properties"/> for applicationId
    /// </summary>
    public virtual Properties StreamsOptions(string applicationId)
    {
        Properties props = _streamsConfigBuilder ?? new();
        if (props.ContainsKey(Org.Apache.Kafka.Streams.StreamsConfig.APPLICATION_ID_CONFIG))
        {
            props.Remove(Org.Apache.Kafka.Streams.StreamsConfig.APPLICATION_ID_CONFIG);
        }
        props.Put(Org.Apache.Kafka.Streams.StreamsConfig.APPLICATION_ID_CONFIG, applicationId);
        if (props.ContainsKey(Org.Apache.Kafka.Streams.StreamsConfig.BOOTSTRAP_SERVERS_CONFIG))
        {
            props.Remove(Org.Apache.Kafka.Streams.StreamsConfig.BOOTSTRAP_SERVERS_CONFIG);
        }
        props.Put(Org.Apache.Kafka.Streams.StreamsConfig.BOOTSTRAP_SERVERS_CONFIG, BootstrapServers);
        if (props.ContainsKey(Org.Apache.Kafka.Streams.StreamsConfig.DEFAULT_KEY_SERDE_CLASS_CONFIG))
        {
            props.Remove(Org.Apache.Kafka.Streams.StreamsConfig.DEFAULT_KEY_SERDE_CLASS_CONFIG);
        }
        props.Put(Org.Apache.Kafka.Streams.StreamsConfig.DEFAULT_KEY_SERDE_CLASS_CONFIG, Class.ForName("org.apache.kafka.common.serialization.Serdes$ByteArraySerde", true, SystemClassLoader));
        if (props.ContainsKey(Org.Apache.Kafka.Streams.StreamsConfig.DEFAULT_VALUE_SERDE_CLASS_CONFIG))
        {
            props.Remove(Org.Apache.Kafka.Streams.StreamsConfig.DEFAULT_VALUE_SERDE_CLASS_CONFIG);
        }
        props.Put(Org.Apache.Kafka.Streams.StreamsConfig.DEFAULT_VALUE_SERDE_CLASS_CONFIG, Class.ForName("org.apache.kafka.common.serialization.Serdes$ByteArraySerde", true, SystemClassLoader));
        if (props.ContainsKey(Org.Apache.Kafka.Clients.Consumer.ConsumerConfig.AUTO_OFFSET_RESET_CONFIG))
        {
            props.Remove(Org.Apache.Kafka.Clients.Consumer.ConsumerConfig.AUTO_OFFSET_RESET_CONFIG);
        }
        props.Put(Org.Apache.Kafka.Clients.Consumer.ConsumerConfig.AUTO_OFFSET_RESET_CONFIG, "earliest");

        return props;
    }
    /// <summary>
    /// Build <see cref="Properties"/> for producers
    /// </summary>
    public virtual Properties ProducerOptions()
    {
        Properties props = _producerConfigBuilder ?? new();
        if (props.ContainsKey(Org.Apache.Kafka.Clients.Producer.ProducerConfig.BOOTSTRAP_SERVERS_CONFIG))
        {
            props.Remove(Org.Apache.Kafka.Clients.Producer.ProducerConfig.BOOTSTRAP_SERVERS_CONFIG);
        }
        props.Put(Org.Apache.Kafka.Clients.Producer.ProducerConfig.BOOTSTRAP_SERVERS_CONFIG, BootstrapServers);
        if (!props.ContainsKey(Org.Apache.Kafka.Clients.Producer.ProducerConfig.ACKS_CONFIG))
        {
            props.Put(Org.Apache.Kafka.Clients.Producer.ProducerConfig.ACKS_CONFIG, "all");
        }
        if (!props.ContainsKey(Org.Apache.Kafka.Clients.Producer.ProducerConfig.RETRIES_CONFIG))
        {
            props.Put(Org.Apache.Kafka.Clients.Producer.ProducerConfig.RETRIES_CONFIG, 0);
        }
        if (!props.ContainsKey(Org.Apache.Kafka.Clients.Producer.ProducerConfig.LINGER_MS_CONFIG))
        {
            props.Put(Org.Apache.Kafka.Clients.Producer.ProducerConfig.LINGER_MS_CONFIG, 1);
        }
        //if (props.ContainsKey(ProducerConfig.KEY_SERIALIZER_CLASS_CONFIG))
        //{
        //    props.Remove(ProducerConfig.KEY_SERIALIZER_CLASS_CONFIG);
        //}
        //props.Put(ProducerConfig.KEY_SERIALIZER_CLASS_CONFIG, Class.ForName("org.apache.kafka.common.serialization.StringSerializer", true, SystemClassLoader));
        //if (props.ContainsKey(ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG))
        //{
        //    props.Remove(ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG);
        //}
        //props.Put(ProducerConfig.VALUE_SERIALIZER_CLASS_CONFIG, Class.ForName("org.apache.kafka.common.serialization.StringSerializer", true, SystemClassLoader));

        return props;
    }
    /// <inheritdoc/>
    public virtual void ApplyServices(IServiceCollection services) => services.AddEntityFrameworkKafkaDatabase();
    /// <inheritdoc/>
    public virtual void Validate(IDbContextOptions options)
    {
        var kafkaOptions = options.FindExtension<KafkaOptionsExtension>();

        if (kafkaOptions == null) throw new InvalidOperationException("Cannot find an instance of KafkaOptionsExtension");

        if (string.IsNullOrEmpty(kafkaOptions.DatabaseName)) throw new ArgumentException("It is manadatory", "DatabaseName");
        if (string.IsNullOrEmpty(kafkaOptions.ApplicationId)) throw new ArgumentException("It is manadatory", "ApplicationId");
        if (string.IsNullOrEmpty(kafkaOptions.BootstrapServers)) throw new ArgumentException("It is manadatory", "BootstrapServers");
    }

    private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
    {
        private string? _logFragment;

        public ExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension)
        {
        }

        private new KafkaOptionsExtension Extension
            => (KafkaOptionsExtension)base.Extension;

        public override bool IsDatabaseProvider
            => true;

        public override string LogFragment
        {
            get
            {
                if (_logFragment == null)
                {
                    var builder = new System.Text.StringBuilder();

                    builder.Append("DataBaseName=").Append(Extension._databaseName).Append(' ');

                    _logFragment = builder.ToString();
                }

                return _logFragment;
            }
        }

        public override int GetServiceProviderHashCode()
            => Extension._bootstrapServers?.GetHashCode() ?? 0;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            => other is ExtensionInfo otherInfo
                && Extension._bootstrapServers == otherInfo.Extension._bootstrapServers;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            => debugInfo["KafkaDatabase:BootstrapServers"]
                = (Extension._bootstrapServers?.GetHashCode() ?? 0).ToString(CultureInfo.InvariantCulture);
    }
}
