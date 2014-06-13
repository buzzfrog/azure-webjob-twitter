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
    class Program
    {
        static void Main(string[] args)
        {
            var job = new Job();
            job.Run();
        }
    }
}
