using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using NUnit.Framework;
using Offr.Message;
using Offr.Query;
using Offr.Repository;
using Offr.Text;

namespace Offr.Tests
{
    [TestFixture]
    public class TestMessageExecutor
    {
        readonly IMessageQueryExecutor _target;
      
        public TestMessageExecutor() {
         
            MockRawMessageProvider mockProvider = new MockRawMessageProvider();
            MockMessageParser mockParser = new MockMessageParser();
            MessageRepository messageRepository = new MessageRepository();
            MessageProvider realMessageProvider = new MessageProvider(messageRepository, mockProvider, mockParser);
            _target = new TagDexQueryExecutor(realMessageProvider);
            mockProvider.Update();

        }

        //[Test]
        //[Ignore("Not supported anymore")]
        //public void TestTextualQuery()
        //{
        //    foreach (string offerKeyword in MockData.Offers)
        //    {
        //        List<IMessage> results = new List<IMessage>(_target.GetMessagesForKeywordAndTags(offerKeyword, null));//<-- target execution
        //        Assert.GreaterOrEqual(results.Count, 1, "Received no results for query:" + offerKeyword);
        //        Console.Out.WriteLine("For " + offerKeyword.ToString() + ":");
        //        foreach (IMessage message in results)
        //        {
        //            Assert.That(message is IOfferMessage);
        //            IOfferMessage offer = message as IOfferMessage;
        //            Assert.IsNotNull(offer);
        //            Assert.That(offer.ToString().Contains(offerKeyword));
        //            Console.Out.WriteLine("\tfound " + message.ToString());
        //        }
        //    }
        //}

        [Test]
        public void TestTagBasedQuery()
        {
            foreach (Tag tag in MockData.UsedTags)
            {
                List<IMessage> results = new List<IMessage>(_target.GetMessagesForTags(new ITag[] { tag })); //<-- target execution
                Assert.GreaterOrEqual(results.Count, 1, "Received no results for query:" + tag);
                Console.Out.WriteLine("For " + tag.ToString() + ":");
                foreach (IMessage message in results)
                {
                    Assert.That(message is IOfferMessage);
                    IOfferMessage offer = message as IOfferMessage;
                    Assert.IsNotNull(offer);
                    Assert.That(offer.Tags.Contains(tag), "Expected to find results that contain facet:" + tag + " in message:" + message);
                    Console.Out.WriteLine("\tfound " + message.ToString());
                }
            }
        }
        
        [Test]
        public void TestAllTagCounts()
        {
            TagCounts allResults = _target.GetTagCounts();
            Assert.AreEqual(MockData.MSG_COUNT, allResults.Total, "Expected total count to equal message count for blank query");
        }

        [Test]
        public void TestTagCounts()
        {
            
            foreach (Tag tag in MockData.UsedTags)
            {
                TagCounts results = _target.GetTagCountsForTags(new[] { tag });
                foreach(TagWithCount foundTag in results.Tags)
                {
                    if (foundTag.Equals(tag))
                    {
                        Assert.AreEqual(results.Total, foundTag.count, "Expected anything that is in the search query to have max count");
                    }
                }

                int lastCount = int.MaxValue;
                foreach(TagWithCount foundTag in results.Tags)
                {
                    Assert.GreaterOrEqual(lastCount, foundTag.count, "Expected results to be in descending order");
                }

                Console.Out.WriteLine("For " + tag.ToString() + ":");
                foreach (TagWithCount foundTag in results.Tags)
                {
                    Console.Out.WriteLine("\tfound " + foundTag.ToString());
                }
            }
        }

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
                        List<ITag> multiTags = new List<ITag>();
                        multiTags.Add(facet1);
                        multiTags.Add(facet2);
                        multiTags.Add(facet3);

                        List<IMessage> results = new List<IMessage>(_target.GetMessagesForTags(multiTags)); //<-- target execution
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
        [Test]
        public void testBlankQuery()
        {

            List<ITag> multiTags = new List<ITag>();
            TagCounts results =  _target.GetTagCountsForTags(multiTags);
            Assert.That(results.Tags.Count == 0);
        }
    }

}
