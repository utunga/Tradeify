using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Ninject.Core;
using NUnit.Framework;
using Offr.Message;
using Offr.Query;
using Offr.Text;

namespace Offr.Tests
{
    [TestFixture]
    public class TestMessageExecutor
    {
        readonly IMessageQueryExecutor _target;
        readonly ITagProvider _tagProvider;

        public TestMessageExecutor()
        {
            Global.Initialize(new TestModule());
            _target = Global.Kernel.Get<IMessageQueryExecutor>(); 
            _tagProvider = Global.Kernel.Get<ITagProvider>();
        }

        [Test]
        public void TestKeywordQuery()
        {
            foreach (string offerKeyword in MockData.Offers)
            {
                IMessageQuery query = new MessageQuery() {Keywords = offerKeyword};
                List<IMessage> results = new List<IMessage>(_target.GetMessagesForQuery(query));//<-- target execution
                Assert.GreaterOrEqual( results.Count, 1, "Received no results for query:" + query);
                Console.Out.WriteLine("For " + query.ToString() + ":");
                foreach (IMessage message in results)
                {
                    Assert.That(message is IOfferMessage);
                    IOfferMessage offer = message as IOfferMessage;
                    Assert.IsNotNull(offer);
                    Assert.That(offer.ToString().Contains(offerKeyword));
                    Console.Out.WriteLine("\tfound " + message.ToString());
                }
            }
        }

        [Test]
        public void TestSingleFacetQuery()
        {
            foreach (Tag facet in MockData.UsedTags)
            {
                IMessageQuery query = new MessageQuery();
                query.Facets.Add(facet);

                List<IMessage> results = new List<IMessage>(_target.GetMessagesForQuery(query));//<-- target execution
                Assert.GreaterOrEqual(results.Count, 1, "Received no results for query:" + query);
                Console.Out.WriteLine("For " + query.ToString() + ":");
                foreach (IMessage message in results)
                {
                    Assert.That(message is IOfferMessage);
                    IOfferMessage offer = message as IOfferMessage;
                    Assert.IsNotNull(offer);
                    Assert.That(offer.Tags.Contains(facet), "Expected to find results that contain facet:" + facet + " in message:" + message);
                    Console.Out.WriteLine("\tfound " + message.ToString());
                }
            }
        }

        //[Test]
        //public void TestTagCounts()
        //{
        //    foreach (Tag facet in MockData.UsedTags)
        //    {
        //        IMessageQuery query = new MessageQuery();
        //        query.Facets.Add(facet);
                
        //        TagCounts results = _target.GetTagCountsForQuery(query);
        //        foreach(ITag foundTag in results.Tags.Values)
        //        {
        //            if (foundTag.Equals(facet))
        //            {
        //                Assert.AreEqual(results.Total,);
        //            }
        //        }
        //        Assert.GreaterOrEqual(results., 1, "Received no results for query:" + query);
        //        Console.Out.WriteLine("For " + query.ToString() + ":");
        //        foreach (IMessage message in results)
        //        {
        //            Assert.That(message is IOfferMessage);
        //            IOfferMessage offer = message as IOfferMessage;
        //            Assert.IsNotNull(offer);
        //            Assert.That(offer.Tags.Contains(facet), "Expected to find results that contain facet:" + facet + " in message:" + message);
        //            Console.Out.WriteLine("\tfound " + message.ToString());
        //        }
        //    }
        //}

        [Test]
        public void TestMultiFacetQuery()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int queryCount =0;
            foreach (Tag facet1 in MockData.UsedTags)
            {
                foreach (Tag facet2 in MockData.UsedTags)
                {
                    foreach (Tag facet3 in MockData.UsedTags)
                    {
                        IMessageQuery query = new MessageQuery();
                        query.Facets.Add(facet1);
                        query.Facets.Add(facet2);
                        query.Facets.Add(facet3);

                        List<IMessage> results = new List<IMessage>(_target.GetMessagesForQuery(query)); //<-- target execution
                        queryCount++;
                        foreach (IMessage message in results)
                        {
                            Assert.That(message is IOfferMessage);
                            IOfferMessage offer = message as IOfferMessage;
                            Assert.IsNotNull(offer);
                            Assert.That(offer.Tags.Contains(facet1), "Expected to find results that specified facet1:" + facet1 + " in message:" + message);
                            Assert.That(offer.Tags.Contains(facet2), "Expected to find results that specified facet2:" + facet2 + " in message:" + message);
                            Assert.That(offer.Tags.Contains(facet3), "Expected to find results that specified facet3:" + facet3 + " in message:" + message);
                            //Console.Out.WriteLine("For " + query.ToString() + ":"); 
                            //Console.Out.WriteLine("\tfound " + message.ToString());
                        }
                    }
                }
            }
            stopwatch.Stop();
            Console.Out.WriteLine("ran " + queryCount + " queries in " + stopwatch.ElapsedMilliseconds + "ms - average:" + stopwatch.ElapsedMilliseconds / queryCount + "ms or " + stopwatch.ElapsedTicks / queryCount + "ticks");
        }

    }

}
