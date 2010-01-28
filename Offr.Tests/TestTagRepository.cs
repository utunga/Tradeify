using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Offr.Repository;
using Offr.Text;

namespace Offr.Tests
{
    [TestFixture]
    public class TestTagRepository
    {
        [Test]
        public void TestTypeAhead()
        {
            TagRepository tagRepository= new TagRepository();
            foreach(ITag tag in MockData.UsedTags)
            {
                tagRepository.Save(tag);

            }
            List<string> tags = tagRepository.GetTagsFromTypeAhead("fr", null);
            Assert.That(tags.Count==1);
            tags = tagRepository.GetTagsFromTypeAhead("fxyz", null);
            Assert.That(tags.Count == 0);
            tags = tagRepository.GetTagsFromTypeAhead("", null);
            Assert.That(tags.Count == MockData.UsedTags.Distinct().Count());
        }
    }
}
