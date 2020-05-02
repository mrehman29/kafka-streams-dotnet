﻿using NUnit.Framework;
using Streamiz.Kafka.Net.Mock;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Streamiz.Kafka.Net.Tests.Processors
{
    public class KStreamPassThoughTests
    {
        [Test]
        public void ShouldNotAllowNullTopicDest()
        {
            var builder = new StreamBuilder();
            var stream = builder.Stream<string, string>("topic");
            string topicDes = null;
            Assert.Throws<ArgumentNullException>(() => stream.To(topicDes));
        }

        [Test]
        public void ShouldNotAllowEmptyTopicDest()
        {
            var builder = new StreamBuilder();
            var stream = builder.Stream<string, string>("topic");
            string topicDes = "";
            Assert.Throws<ArgumentException>(() => stream.To(topicDes));
        }

        [Test]
        public void PassThoughElements()
        {
            var builder = new StreamBuilder();

            builder.Stream<string, string>("topic").To("topic2");

            var config = new StreamConfig<StringSerDes, StringSerDes>();
            config.ApplicationId = "test-passthrough";

            Topology t = builder.Build();

            using (var driver = new TopologyTestDriver(t, config))
            {
                var inputTopic = driver.CreateInputTopic<string, string>("topic");
                var outputTopic = driver.CreateOuputTopic<string, string>("topic2");
                var expected = new List<KeyValuePair<string, string>>();
                for (int i = 0; i < 42; i++)
                {
                    string key = i.ToString();
                    string value = $"V{i}";
                    inputTopic.PipeInput(key, value);
                    expected.Add(KeyValuePair.Create(key, value));
                }

                var list = outputTopic.ReadKeyValueList().Select(r => KeyValuePair.Create(r.Message.Key, r.Message.Value)).ToList();

                Assert.AreEqual(expected, list);
            }
        }
    }
}