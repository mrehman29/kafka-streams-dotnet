﻿using Streamiz.Kafka.Net.Processors;
using Streamiz.Kafka.Net.State.Internal;
using Streamiz.Kafka.Net.Stream;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Streamiz.Kafka.Net.State
{
    /// <summary>
    /// Provides access to the <see cref="IQueryableStoreType{T, K, V}"/>s provided with <see cref="KafkaStream"/>.
    /// These can be used with <see cref="KafkaStream.Store{T, K, V}(StoreQueryParameters{T, K, V})"/>
    /// to access and query the <see cref="IStateStore"/>s that are part of a <see cref="Topology"/>.
    /// </summary>
    public class QueryableStoreTypes
    {
        /// <summary>
        /// A <see cref="IQueryableStoreType{T, K, V}"/> that accepts <see cref="ReadOnlyKeyValueStore{K, V}"/> as T.
        /// </summary>
        /// <typeparam name="K">key type of the store</typeparam>
        /// <typeparam name="V">value type of the store</typeparam>
        /// <returns><see cref="KeyValueStore{K, V}"/></returns>
        public static IQueryableStoreType<ReadOnlyKeyValueStore<K, V>, K, V> KeyValueStore<K, V>() => new KeyValueStoreType<K, V>();

        /// <summary>
        /// A <see cref="IQueryableStoreType{T, K, V}"/> that accepts <see cref="ReadOnlyKeyValueStore{K, U}"/> as T where
        /// U is <see cref="ValueAndTimestamp{V}"/>.
        /// </summary>
        /// <typeparam name="K">key type of the store</typeparam>
        /// <typeparam name="V">value type of the store</typeparam>
        /// <returns><see cref="QueryableStoreTypes.TimestampedKeyValueStore{K, V}"/></returns>
        public static IQueryableStoreType<ReadOnlyKeyValueStore<K, ValueAndTimestamp<V>>, K, ValueAndTimestamp<V>> TimestampedKeyValueStore<K, V>() => new TimestampedKeyValueStoreType<K, V>();
    }

    internal class KeyValueStoreType<K, V> : QueryableStoreTypeMatcher<ReadOnlyKeyValueStore<K, V>, K, V>
    {
        public KeyValueStoreType() 
            : base(new[] { typeof(ReadOnlyKeyValueStore<K, V>), typeof(ReadOnlyKeyValueStore<K, ValueAndTimestamp<V>>) })
        {
        }

        public override ReadOnlyKeyValueStore<K, V> Create(IStateStoreProvider<ReadOnlyKeyValueStore<K, V>, K, V> storeProvider, string storeName)
        {
            return new CompositeReadOnlyKeyValueStore<K, V>(storeProvider, this, storeName);
        }
    }

    internal class TimestampedKeyValueStoreType<K, V>: QueryableStoreTypeMatcher<ReadOnlyKeyValueStore<K, ValueAndTimestamp<V>>, K, ValueAndTimestamp<V>>
    {
        public TimestampedKeyValueStoreType(): base(new[] { typeof(ReadOnlyKeyValueStore<K, ValueAndTimestamp<V>>), typeof(TimestampedKeyValueStore<K, V>) })
        {
        }

        public override ReadOnlyKeyValueStore<K, ValueAndTimestamp<V>> Create(IStateStoreProvider<ReadOnlyKeyValueStore<K, ValueAndTimestamp<V>>, K, ValueAndTimestamp<V>> storeProvider, string storeName)
        {
            return new CompositeReadOnlyKeyValueStore<K, ValueAndTimestamp<V>>(storeProvider, this, storeName);
        }
    }

    internal abstract class QueryableStoreTypeMatcher<T, K, V> : IQueryableStoreType<T, K, V> where T : class
    {

        private readonly IEnumerable<Type> matchTo;

        protected QueryableStoreTypeMatcher(IEnumerable<Type> matchTo)
        {
            this.matchTo = matchTo;
        }

        public abstract T Create(IStateStoreProvider<T, K, V> storeProvider, string storeName);

        public bool Accepts(IStateStore stateStore)
        {
            return this.matchTo.Any(type => type.IsAssignableFrom(stateStore.GetType()));
        }
    }
}