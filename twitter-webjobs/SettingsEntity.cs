using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitter_webjobs
{
    class SettingsEntity : TableEntity
    {
        public SettingsEntity(string settingsId)
        {
            PartitionKey = "settings";
            RowKey = settingsId;
        }

        public SettingsEntity()
        {

        }

        public string Value { get; set; }
    }
}
