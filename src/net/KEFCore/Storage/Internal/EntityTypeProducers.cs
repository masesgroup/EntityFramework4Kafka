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

using System.Collections.Concurrent;

namespace MASES.EntityFrameworkCore.KNet.Storage.Internal;
/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class EntityTypeProducers
{
    static IEntityTypeProducer? _globalProducer = null;
    static readonly ConcurrentDictionary<IEntityType, IEntityTypeProducer> _producers = new ConcurrentDictionary<IEntityType, IEntityTypeProducer>();

    public static IEntityTypeProducer Create<TKey>(IEntityType entityType, IKafkaCluster cluster) where TKey : notnull
    {
        //if (!cluster.Options.ProducerByEntity)
        //{
        //    lock (_producers)
        //    {
        //        if (_globalProducer == null) _globalProducer = CreateProducerLocal<TKey>(entityType, cluster);
        //        return _globalProducer;
        //    }
        //}
        //else
        //{
        return _producers.GetOrAdd(entityType, _ => CreateProducerLocal<TKey>(entityType, cluster));
        //}
    }

    static IEntityTypeProducer CreateProducerLocal<TKey>(IEntityType entityType, IKafkaCluster cluster) where TKey : notnull => new EntityTypeProducer<TKey>(entityType, cluster);
}
