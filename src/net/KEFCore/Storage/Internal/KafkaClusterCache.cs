/*
*  Copyright 2022 - 2025 MASES s.r.l.
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

using System.Collections.Concurrent;
using MASES.EntityFrameworkCore.KNet.Infrastructure.Internal;

namespace MASES.EntityFrameworkCore.KNet.Storage.Internal;
/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class KafkaClusterCache : IKafkaClusterCache
{
    private readonly IKafkaTableFactory _tableFactory;
    private readonly ConcurrentDictionary<string, IKafkaCluster> _namedClusters;
    /// <summary>
    /// Default initializer
    /// </summary>
    public KafkaClusterCache(
        IKafkaTableFactory tableFactory,
        IKafkaSingletonOptions? options)
    {
        _tableFactory = tableFactory;
        _namedClusters = new ConcurrentDictionary<string, IKafkaCluster>();
    }
    /// <inheritdoc/>
    public virtual IKafkaCluster GetCluster(KafkaOptionsExtension options)
        => _namedClusters.GetOrAdd(options.ClusterId, _ => new KafkaCluster(options, _tableFactory));
    /// <inheritdoc/>
    public virtual void Dispose(IKafkaCluster cluster)
    {
        if (cluster != null)
        {
            cluster.Dispose();
            _namedClusters.TryRemove(cluster.ClusterId, out _);
        }
    }
}
