﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Streamiz.Kafka.Net.SerDes
{
    /// <summary>
    /// SerDes for <see cref="Int32"/>.
    /// </summary>
    public class Int32SerDes : AbstractSerDes<int>
    {
        /// <summary>
        /// Deserialize a record value from a byte array into <see cref="Int32"/> value
        /// </summary>
        /// <param name="data">serialized bytes.</param>
        /// <returns>deserialized <see cref="Int32"/> using data; may be null</returns>
        public override int Deserialize(byte[] data) => BitConverter.ToInt32(data);

        /// <summary>
        /// Convert int32 <code>data</code> into a byte array.
        /// </summary>
        /// <param name="data"><see cref="Int32"/> data</param>
        /// <returns>serialized bytes</returns>
        public override byte[] Serialize(int data) => BitConverter.GetBytes(data);
    }
}
