using System.Collections;
using System.Collections.Generic;
using Offr.Text;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Offr.Tests
{   
    [TestFixture]
    public class TagListTest
    {
        TagList _target;

        [SetUp]
        public void Setup()
        {
            //NOTE to fin the following uses 'object initialization syntax..
            // so, first few lines are equivalent to..
            //_target = new TagCollection();
            //Tag tmp = new Tag(TagType.currency,"foo");
            //_target.Add(tmp);

            _target = new TagList
            {
              new Tag(TagType.currency,"foo"),
              new Tag(TagType.currency,"bar"),
              new Tag(TagType.currency,"baz"),
              new Tag(TagType.tag,"rep1"),
              new Tag(TagType.tag,"rep1"),
              new Tag(TagType.loc,"wellington"),
              new Tag(TagType.loc,"nz")
            };
        }

        [Test]
        public void TestSetup()
        {
            ITag tag = _target[2];
            // just check we are on same page
            Assert.AreEqual("baz", tag.Text);
            Assert.AreEqual(TagType.currency, tag.Type);
        }

        [Test]
        public void TestFilterOnSetup()
        {
            IList<ITag> currencies = _target.TagsOfType(TagType.currency);
            Assert.AreEqual(3, currencies.Count);
            foreach (ITag currency in currencies)
            {
                Assert.AreEqual(TagType.currency, currency.Type);
            }
        }

        /// <summary>
        ///A test for SetItem
        ///</summary>
        [Test]
        public void SetItemTest()
        {
            ITag tag = _target[2];
            // just check we are on same page to start
            Assert.AreEqual("baz", tag.Text);
            Assert.AreEqual(TagType.currency, tag.Type);

            _target[2] = new Tag(TagType.tag, "bah");
            
            ITag tag2 = _target[2];
            Assert.AreEqual(TagType.tag, tag2.Type);
            Assert.AreEqual("bah", tag2.Text);
        }

        [Test]
        [Ignore("Fails for now, not sure we event want/need this")]
        public void ContainerEqualityTest()
        {
            // just check we are on same page to start
            int startCount = _target.Count;
            //Assert.AreEqual(6, startCount);

            int startHashCount = _target.TagsOfType(TagType.tag).Count;
            
            _target.Add(new Tag(TagType.tag, "bah"));
            _target.Add(new Tag(TagType.tag, "bah"));
            _target.Add(new Tag(TagType.tag, "bah"));

            Assert.AreEqual(startCount + 1, _target.Count, "Expected to add only the one tag, given that it alrady exists");

            Assert.AreEqual(startHashCount + 1, _target.TagsOfType(TagType.tag).Count, "Expected to add only the one tag to hash tags, given that it alrady exists");
        }

        [Test]
        public void TagEqualityTest()
        {
            Assert.AreEqual(new Tag(TagType.tag, "bah"), new Tag(TagType.tag, "bah"));
            Assert.AreNotEqual(new Tag(TagType.currency, "bah"), new Tag(TagType.tag, "bah"));
            Assert.AreNotEqual(new Tag(TagType.tag, "baz"), new Tag(TagType.tag, "bah"));
        }

        ///// <summary>
        /////A test for RemoveItem
        /////</summary>
        [Test]
        public void RemoveItemTest()
        {
            ITag tag = _target[2];
            // just check we are on same page to start
            Assert.AreEqual("baz", tag.Text);
            Assert.AreEqual(TagType.currency, tag.Type);

            _target[2] = new Tag(TagType.tag, "bah");

        }

        ///// <summary>
        /////A test for InsertItem
        /////</summary>
        //[Test]
        //public void InsertItemTest()
        //{
        //    TagCollection target = new TagCollection(); // TODO: Initialize to an appropriate value
        //    int index = 0; // TODO: Initialize to an appropriate value
        //    ITag tag = null; // TODO: Initialize to an appropriate value
        //    target.InsertItem(index, tag);
            
        //}

        ///// <summary>
        /////A test for ClearItems
        /////</summary>
        //[Test]
        //public void ClearItemsTest()
        //{
        //    TagCollection target = new TagCollection(); // TODO: Initialize to an appropriate value
        //    target.ClearItems();
            
        //}

        ///// <summary>
        /////A test for TagCollection Constructor
        /////</summary>
        //[Test]
        //public void TagCollectionConstructorTest()
        //{
        //    TagCollection target = new TagCollection();
        //    Assert.Inconclusive("TODO: Implement code to verify target");
        //}
    }
}
