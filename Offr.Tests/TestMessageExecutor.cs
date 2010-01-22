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
        private IgnoredUserRepository ignoredUsers;
       
        public TestMessageExecutor() 
        { 
            MockMessageParser mockParser = new MockMessageParser();
            ignoredUsers = new IgnoredUserRepository();
            var ignoredUser = MockData.Users[0];
            ignoredUsers.Save(ignoredUser);
            MessageRepository messageRepository = new MessageRepository(ignoredUsers);
            IncomingMessageProcessor incomingMessageProcessor = new IncomingMessageProcessor(messageRepository, mockParser);
            //_target = new TagDexQueryExecutor();
            MockRawMessageProvider mockProvider = new MockRawMessageProvider(incomingMessageProcessor);
            _target = messageRepository;
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
        public void TestSingleTagBasedQuery()
        {
            foreach (Tag tag in MockData.UsedTags)
            {
                List<IMessage> results = new List<IMessage>(_target.GetMessagesForTags(new ITag[] { tag }, true)); //<-- target execution
                Assert.GreaterOrEqual(results.Count, 1, "Received no results for tag:" + tag);
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
        public void TestDoubleTagBasedQuery()
        {
            foreach (Tag tag in MockData.UsedTags)
            {
                foreach (Tag tag2 in MockData.UsedTags)
                {
                    List<IMessage> results = new List<IMessage>(_target.GetMessagesForTags(new ITag[] { tag, tag2 }, false));
                    Console.Out.WriteLine("For " + tag.ToString() + " && " + tag2.ToString() + ":");
                    foreach (IMessage message in results)
                    {
                        IOfferMessage offer = message as IOfferMessage;
                        Assert.That(offer.Tags.Contains(tag),
                                    "Expected to find results that contain facet:" + tag + " in message:" + message);
                        Assert.That(offer.Tags.Contains(tag2),
                                    "Expected to find results that contain facet:" + tag2 + " in message:" + message);
                        Console.Out.WriteLine("\tfound " + message.ToString());
                    }
                }
            }
        }

        [Test]
        public void TestUserAndTagBasedQuery()
        {
            foreach (IUserPointer user in MockData.Users)
            {
                foreach (Tag tag in MockData.UsedTags)
                {
                    List<IMessage> results = new List<IMessage>(_target.GetMessagesForTagsCreatedByUser(new ITag[] { tag }, user));
                    Console.Out.WriteLine("For " + user.ToString() + " && " + tag.ToString() + ":");
                    foreach (IMessage message in results)
                    {
                        IOfferMessage offer = message as IOfferMessage;
                        Assert.That(offer.Tags.Contains(tag),
                                    "Expected to find results that contain facet:" + tag + " in message:" + message);
                        Assert.That(offer.CreatedBy.Equals(user),
                                    "Expected to find results created by user :" + user + " in message:" + message);
                        Console.Out.WriteLine("\tfound " + message.ToString());
                    }
                }
            }
        }

        [Test]
        public void TestUserBasedQuery()
        {
            foreach (IUserPointer user in MockData.Users)
            {
                List<IMessage> results =
                    new List<IMessage>(_target.GetMessagesCreatedByUser(user));
                Assert.GreaterOrEqual(results.Count, 1, "Received no results created by:" + user);
                Console.Out.WriteLine("Created By " + user.ToString() + " :");
                foreach (IMessage message in results)
                {
                    IOfferMessage offer = message as IOfferMessage;
                    Assert.That(offer.CreatedBy.Equals(user),
                                "Expected to find results created by user :" + user + " in message:" + message);
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

                        List<IMessage> results = new List<IMessage>(_target.GetMessagesForTags(multiTags,false)); //<-- target execution
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
        [Test]
        public void testIgnoreList()
        {
            foreach (Tag tag in MockData.UsedTags)
            {
                List<IMessage> results = new List<IMessage>(_target.GetMessagesForTags(new ITag[] { tag }, false)); //<-- target execution
                //Assert.GreaterOrEqual(results.Count, 1, "Received no results for tag:" + tag);
                Console.Out.WriteLine("For " + tag.ToString() + ":");
                foreach (IMessage message in results)
                {
                    Assert.That(message is IOfferMessage);
                    IOfferMessage offer = message as IOfferMessage;
                    Assert.IsNotNull(offer);
                    Assert.That(offer.Tags.Contains(tag), "Expected to find results that contain facet:" + tag + " in message:" + message);
                    //ensure there are no messages in ignored users that have the ID of the message
                    Assert.That(ignoredUsers.Get(message.CreatedBy.ID)==null);
                    Console.Out.WriteLine("\tfound " + message.ToString());
                }
            }

        }
    }

}
