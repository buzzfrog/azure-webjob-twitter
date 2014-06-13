using LinqToTwitter;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace twitter_webjobs
{

    class Job
    {
        TwitterContext twitterCtx;
        CloudStorageAccount storageAccount;
        CloudTableClient tableClient;
        CloudTable tweetTable;
        CloudTable settingsTable;


        public async void Run()
        {
            CreateTwitterContext();
            CreateTableContext();

            // get latest id that are stored in the table
            var latestTweetId = GetLatestTweetId();

            var searchResponse = 
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                    search.Query == "#windowsazure" &&
                    search.SinceID == latestTweetId
                 select search).SingleOrDefault();

            if (searchResponse != null && searchResponse.Statuses != null)
            {
                searchResponse.Statuses.ForEach(tweet =>
                {
                    Console.WriteLine("User: {0}, Tweet: {1}\n{2}", tweet.User.ScreenNameResponse,
                        tweet.Text,
                        tweet.StatusID);
                    StoreTweetInTable(tweet);

                    latestTweetId = latestTweetId < tweet.StatusID ? tweet.StatusID : latestTweetId;
                });

                SaveLatestTweetId(latestTweetId);
            }
        }

        private void StoreTweetInTable(Status tweet)
        {
            TweetEntity te = new TweetEntity("windowsazure", tweet.StatusID);
            te.Text = tweet.Text;
            te.User = tweet.User.ScreenNameResponse;
            TableOperation insert = TableOperation.Insert(te);
            tweetTable.Execute(insert);
        }

        private void CreateTableContext()
        {
            storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            tableClient = storageAccount.CreateCloudTableClient();

            tweetTable = tableClient.GetTableReference("tweets");
            tweetTable.CreateIfNotExists();

            settingsTable = tableClient.GetTableReference("tweetssettings");
            settingsTable.CreateIfNotExists();
                    
        }
        private void CreateTwitterContext()
        {
            var auth = new SingleUserAuthorizer
             {
                 CredentialStore = new SingleUserInMemoryCredentialStore
                 {
                     ConsumerKey = ConfigurationManager.AppSettings["TwitterConsumerKey"],
                     ConsumerSecret = ConfigurationManager.AppSettings["TwitterConsumerSecret"],
                     AccessToken = ConfigurationManager.AppSettings["TwitterAccessToken"],
                     AccessTokenSecret = ConfigurationManager.AppSettings["TwitterAccessTokenSecret"]
                 }
             };

            twitterCtx = new TwitterContext(auth);
        }

        private ulong GetLatestTweetId()
        {
            TableOperation getTweetIdOperation = TableOperation.Retrieve<SettingsEntity>("settings", "latesttweetid");
            var result = settingsTable.Execute(getTweetIdOperation);
            
            if(result.Result != null)
            {
                return Convert.ToUInt64(((SettingsEntity)result.Result).Value);
            }
            else
            {
                return 0;
            }
        }

        private void SaveLatestTweetId(ulong tweetId)
        {
            SettingsEntity se = new SettingsEntity("latesttweetid");
            se.Value = tweetId.ToString();
            TableOperation insertOrReplaceLatestTweetId = TableOperation.InsertOrReplace(se);

            settingsTable.Execute(insertOrReplaceLatestTweetId);
        }
    }
}


