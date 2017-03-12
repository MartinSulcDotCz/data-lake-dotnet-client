﻿using System;
using System.Collections.Generic;
using System.Linq;
using ADLA=Microsoft.Azure.Management.DataLake.Analytics;

namespace ADL_Client_Demo
{
    class Program
    {
        private static void Main(string[] args)
        {
            var sub = new AzureDataLakeClient.Rm.Subscription("045c28ea-c686-462f-9081-33c34e871ba3");
            var rg = new AzureDataLakeClient.Rm.ResourceGroup("InsightServices");
            var adla_account = new AzureDataLakeClient.Analytics.AnalyticsAccount("datainsightsadhoc", sub, rg); // change this to an ADL Analytics account you have access to 
            var adls_account = new AzureDataLakeClient.Store.StoreUri("datainsightsadhoc"); // change this to an ADL Store account you have access to 

            var tenant = new AzureDataLakeClient.Authentication.Tenant("microsoft.onmicrosoft.com"); // change this to YOUR tenant
            var auth_session = new AzureDataLakeClient.Authentication.AuthenticatedSession(tenant);
            auth_session.Authenticate();

            var adla_client = new AzureDataLakeClient.Analytics.AnalyticsAccountClient(adla_account, auth_session);
            var mgmt_client = new AzureDataLakeClient.Analytics.AnalyticsRmClient(adla_account.Subscription, auth_session);

            //Demo_GetExactlyOneJob(job_client);
            //Demo_Get10OldestJobs(job_client);
            //Demo_Get10MostRecentJobs(job_client);
            //Demo_Get5FailedJobs(job_client);
            //Demo_GetJobsSubmittedByMe(job_client);
            //Demo_GetJobsSubmittedByUsers(job_client);
            //Demo_GetJobsSubmitedSinceMidnight(job_client);
            //Demo_GetJobs_Submitter_Begins_With(job_client);
            //Demo_GetJobs_Submitter_Contains(job_client);

            //var fs_client = new AzureDataLakeClient.Store.StoreFileSystemClient(adls_account, auth_session);
            //Demo_ListFilesAtRoot(fs_client);
            Demo_ListLinkedDataLakeStoreAccounts(adla_client, adla_account);

            Demo_ListDataLakeAnalyticsAccountsInSubscription(mgmt_client);
        }

        private static void Demo_ListFilesAtRoot(AzureDataLakeClient.Store.StoreFileSystemClient fs_client)
        {
            //var root = AzureDataLakeClient.Store.FsPath.Root; // same as "/"
            var root = new AzureDataLakeClient.Store.FsPath("/Samples");
            var lfo = new AzureDataLakeClient.Store.ListFilesOptions();
            foreach (var page in fs_client.ListFilesPaged(root,lfo))
            {
                foreach (var fileitemn in page.FileItems)
                {
                    Console.WriteLine("path={0} filename={1}",page.Path,fileitemn.PathSuffix);                    
                }
            }

        }

        private static void Demo_GetExactlyOneJob(AzureDataLakeClient.Analytics.AnalyticsAccountClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 1;
            var jobs = job_client.Jobs.GetJobs(opts);

            PrintJobs(jobs);
        }

        private static void Demo_GetJobsSubmittedByMe(AzureDataLakeClient.Analytics.AnalyticsAccountClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 10;
            opts.Filter.SubmitterIsCurrentUser = true;

            var jobs = job_client.Jobs.GetJobs(opts);

            PrintJobs(jobs);
        }


        private static void Demo_GetJobsSubmittedByUsers(AzureDataLakeClient.Analytics.AnalyticsAccountClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 10;
            opts.Filter.Submitter.OneOf("mrys@microsoft.com", "saveenr@microsoft.com");

            var jobs = job_client.Jobs.GetJobs(opts);

            PrintJobs(jobs);
        }

        private static void Demo_GetJobs_Submitter_Begins_With(AzureDataLakeClient.Analytics.AnalyticsAccountClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 10;
            opts.Filter.Submitter.BeginsWith("saa");

            var jobs = job_client.Jobs.GetJobs(opts);

            PrintJobs(jobs);
        }

        private static void Demo_GetJobs_Submitter_Contains(AzureDataLakeClient.Analytics.AnalyticsAccountClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 10;
            opts.Filter.Submitter.Contains("eenr");

            var jobs = job_client.Jobs.GetJobs(opts);

            PrintJobs(jobs);
        }


        private static void Demo_Get10MostRecentJobs(AzureDataLakeClient.Analytics.AnalyticsAccountClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 10;

            var jobfields = new AzureDataLakeClient.Analytics.JobListFields();
            opts.Sorting.Direction = AzureDataLakeClient.Analytics.OrderByDirection.Descending;
            opts.Sorting.Field = jobfields.field_submittime;

            var jobs = job_client.Jobs.GetJobs(opts);

            PrintJobs(jobs);
        }

        private static void Demo_Get10OldestJobs(AzureDataLakeClient.Analytics.AnalyticsAccountClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 10;

            var jobfields = new AzureDataLakeClient.Analytics.JobListFields();
            opts.Sorting.Direction = AzureDataLakeClient.Analytics.OrderByDirection.Ascending;
            opts.Sorting.Field = jobfields.field_submittime;

            var jobs = job_client.Jobs.GetJobs(opts);

            PrintJobs(jobs);
        }

        private static void Demo_Get5FailedJobs(AzureDataLakeClient.Analytics.AnalyticsAccountClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Top = 5;

            opts.Filter.Result.OneOf(ADLA.Models.JobResult.Failed);

            var jobs = job_client.Jobs.GetJobs(opts);

            PrintJobs(jobs);
        }

        private static void Demo_GetJobsSubmitedInLast2hours(AzureDataLakeClient.Analytics.AnalyticsAccountClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Filter.SubmitTime.InRange(AzureDataLakeClient.OData.Utils.RangeDateTime.InTheLastNHours(2));
            var jobs = job_client.Jobs.GetJobs(opts);
            PrintJobs(jobs);
        }

        private static void Demo_GetJobsSubmitedSinceMidnight(AzureDataLakeClient.Analytics.AnalyticsAccountClient job_client)
        {
            var opts = new AzureDataLakeClient.Analytics.GetJobsOptions();
            opts.Filter.SubmitTime.InRange(AzureDataLakeClient.OData.Utils.RangeDateTime.SinceLocalMidnight());
            var jobs = job_client.Jobs.GetJobs(opts);
            PrintJobs(jobs);
        }

        private static void PrintJobs(IEnumerable<ADLA.Models.JobInformation> jobs)
        {
            foreach (var job in jobs)
            {
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("Name = {0}", job.Name);
                Console.WriteLine("DoP = {0}; Priority = {1}", job.DegreeOfParallelism, job.Priority);
                Console.WriteLine("Result = {0}; State = {1}", job.Result, job.State);
                Console.WriteLine("SubmitTime = {0} [ Local = {1} ] ", job.SubmitTime.Value, job.SubmitTime.Value.ToLocalTime());
                Console.WriteLine("Submitter = {0}", job.Submitter);
            }
        }

        private static void Demo_ListLinkedDataLakeStoreAccounts(AzureDataLakeClient.Analytics.AnalyticsAccountClient rm_client, AzureDataLakeClient.Analytics.AnalyticsAccount account)
        {
            var storage_accounts = rm_client.ListLinkedDataLakeStoreAccounts(account).ToList();
            foreach (var i in storage_accounts)
            {
                Console.WriteLine("----------------");
                Console.WriteLine("Name = {0}", i.Name);
                Console.WriteLine("Type = {0}", i.Type);
            }
        }

        private static void Demo_ListDataLakeAnalyticsAccountsInSubscription(AzureDataLakeClient.Analytics.AnalyticsRmClient rm_client)
        {
            var storage_accounts = rm_client.ListAccounts().ToList();
            foreach (var i in storage_accounts)
            {
                Console.WriteLine("----------------");
                Console.WriteLine("Name = {0}", i.Name);
                Console.WriteLine("Location = {0}", i.Location);
                Console.WriteLine("Type = {0}", i.Type);
            }
        }

    }
}