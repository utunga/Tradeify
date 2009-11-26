using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Offr.Json;
using Offr.Message;
using Offr.Text;

namespace Offr.Repository
{
    public class TagRepository: BaseRepository<ITag>, ITagRepository , IPersistedRepository
    {
        // one day could implement this as a caching lookup, with counts attached etc..
        //but for now just parse the string to get the tag
        //should be a dictionary with tag values
       // private Dictionary<String, ITag> _list;
        public TagRepository()
        {
        }


        public ITag GetTag(string tagString, TagType type)
        {
            string tagStringLowerCase = tagString.ToLowerInvariant();
            ITag tag;
            if (_list.TryGetValue(tagStringLowerCase, out tag))
            {
                return tag;  
            }
            else
            {
                ITag newTag = FromTypeAndText(type, tagStringLowerCase);
                Save(newTag);
                return newTag;
            }
        }

        public List<ITag> GetTagsFromNameValueCollection(NameValueCollection nameVals)
        {
            List<ITag> tags = new List<ITag>();
            if (nameVals==null) return tags;
            foreach (TagType tagType in Enum.GetValues(typeof(TagType)))
            {
                if (nameVals.GetValues(tagType.ToString()) != null)
                {
                    foreach (string tagText in nameVals.GetValues(tagType.ToString()))
                    {
                        ITag tag = this.GetTag(tagText, tagType);
                        tags.Add(tag);
                    }
                }
            }
            return tags;
        }

/*        public void Initialize()
        {
            foreach (MessageType msgType in Enum.GetValues(typeof(MessageType)))
            {
                string tagText = msgType.ToString();
                Save(FromTypeAndText(TagType.msg_type, tagText));
            }
            Save(FromTypeAndText(TagType.group, "ooooby"));
            Save(FromTypeAndText(TagType.group, "freecycle"));
            Save(FromTypeAndText(TagType.type, "cash only"));
            Save(FromTypeAndText(TagType.type, "cash"));
            Save(FromTypeAndText(TagType.type, "nzd"));
            Save(FromTypeAndText(TagType.type, "barter"));
            Save(FromTypeAndText(TagType.type, "free"));
            String serializedList = JSON.Serialize(_list);  
        }*/

        private ITag FromTypeAndText(TagType tagType, string tagText)
        {
            return new Tag(tagType, tagText);
        }

    }
    }



