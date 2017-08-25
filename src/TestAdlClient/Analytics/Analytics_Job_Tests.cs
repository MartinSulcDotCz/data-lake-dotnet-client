﻿using System.Linq;
using AdlClient.Models;
using Microsoft.Azure.Management.DataLake.Analytics.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestAdlClient.Analytics
{
    [TestClass]
    public class Analytics_Job_Tests : Base_Tests
    {

        [TestMethod]
        public void Verify_Paging_1()
        {
            this.Initialize();
            var listing_parameters = new JobListParameters();
            listing_parameters.Top = 100;

            var jobs = this.AnalyticsClient.Jobs.ListJobs(listing_parameters).ToList();
            Assert.AreEqual(100,jobs.Count);
        }

        [TestMethod]
        public void Verify_Paging_300()
        {
            this.Initialize();
            var listing_parameters = new JobListParameters();
            listing_parameters.Top = 300;

            var jobs = this.AnalyticsClient.Jobs.ListJobs(listing_parameters).ToList();
            Assert.AreEqual(300, jobs.Count);
        }

        [TestMethod]
        public void Verify_Paging_450()
        {
            this.Initialize();
            var listing_parameters = new JobListParameters();
            listing_parameters.Top = 450;

            var jobs = this.AnalyticsClient.Jobs.ListJobs(listing_parameters).ToList();
            Assert.AreEqual(450, jobs.Count);
        }


        [TestMethod]
        public void List_Jobs()
        {
            this.Initialize();

            var listing_parameters = new JobListParameters();
            listing_parameters.Top = 30;
            listing_parameters.Sorting.BySubmitTime(AdlClient.OData.Models.OrderByDirection.Descending);

            var jobs = this.AnalyticsClient.Jobs.ListJobs(listing_parameters).ToList();
            foreach (var job in jobs)
            {
                System.Console.WriteLine("submitter{0} dop {1}", job.Submitter, job.DegreeOfParallelism);
            }
        }

        [TestMethod]
        public void Create_Job_with_Syntax_Error()
        {
            this.Initialize();
            var create_job__parameters = new JobCreateParameters();
            create_job__parameters.ScriptText = "FOOBAR";
            create_job__parameters.JobName = "Test Job";
            var ji = this.AnalyticsClient.Jobs.CreateJob(create_job__parameters);

            System.Console.WriteLine("{0} {1} {2}", ji.Name, ji.JobId, ji.SubmitTime);

            var ji2 = this.AnalyticsClient.Jobs.GetJobDetails(ji.JobId,false);

            Assert.AreEqual(ji.Name, ji2.Name);
        }

        [TestMethod]
        public void List_Jobs_Ended()
        {
            this.Initialize();
            var listing_parameters = new JobListParameters();
            listing_parameters.Top = 30;
            listing_parameters.Filter.State.IsOneOf(JobState.Ended);

            var jobs = this.AnalyticsClient.Jobs.ListJobs(listing_parameters).ToList();
            if (jobs.Count > 0)
            {
                foreach (var job in jobs)
                {
                    Assert.AreEqual(JobState.Ended,job.State);
                }
            }
        }

        [TestMethod]
        public void List_Jobs_Running()
        {
            this.Initialize();
            var listing_parameters = new JobListParameters();
            listing_parameters.Top = 30;
            listing_parameters.Filter.State.IsOneOf(JobState.Running);

            var jobs = this.AnalyticsClient.Jobs.ListJobs(listing_parameters).ToList();
            if (jobs.Count > 0)
            {
                foreach (var job in jobs)
                {
                    Assert.AreEqual(JobState.Running, job.State);
                }
            }
        }

        [TestMethod]
        public void List_Jobs_Ended_Failed()
        {
            this.Initialize();
            var listing_parameters = new JobListParameters();
            listing_parameters.Top = 30;
            listing_parameters.Filter.State.IsOneOf( JobState.Ended);
            listing_parameters.Filter.Result.IsOneOf( JobResult.Failed);

            var jobs = this.AnalyticsClient.Jobs.ListJobs(listing_parameters).ToList();
            if (jobs.Count > 0)
            {
                foreach (var job in jobs)
                {
                    Assert.AreEqual(JobState.Ended, job.State);
                    Assert.AreEqual(JobResult.Failed, job.Result);
                }
            }
        }

        [TestMethod]
        public void Create_500_Jobs()
        {
            this.Initialize();

            int num_jobs=500;
            bool doit = false;
            if (doit)
            {
                for (int i = 0; i < num_jobs; i++)
                {
                    var create_job_parameters = new JobCreateParameters();
                    create_job_parameters.ScriptText = "FOOBAR";
                    create_job_parameters.JobName = "Test Job " + i.ToString();
                    var ji = this.AnalyticsClient.Jobs.CreateJob(create_job_parameters);

                }
            }
        }
    }
}
