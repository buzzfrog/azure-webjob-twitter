using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitter_webjobs
{
    class TweetEntity : TableEntity
    {
        public TweetEntity(string hashTag, ulong id)
        {
            PartitionKey = hashTag;
            RowKey = id.ToString();
        }

        public TweetEntity()
        {

        }

        public string User { get; set; }
        public string Text { get; set; }
    }
}
