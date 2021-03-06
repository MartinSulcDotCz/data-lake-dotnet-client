﻿using MSADLA = Microsoft.Azure.Management.DataLake.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using AdlClient.Models;
using Microsoft.Azure.Management.DataLake.Analytics;
using Microsoft.Azure.Management.DataLake.Analytics.Models;
using MSADL = Microsoft.Azure.Management.DataLake;

namespace AdlClient.Commands
{
    public class JobCommands
    {
        internal readonly AnalyticsAccountRef Account;
        internal readonly AnalyticsRestClients RestClients;

        internal JobCommands(AnalyticsAccountRef a, AnalyticsRestClients clients)
        {
            this.Account = a;
            this.RestClients = clients;
        }

        public void CancelJob(System.Guid jobid)
        {
            this.RestClients.JobsClient.Job.Cancel(this.Account.Name, jobid);
        }

        public bool JobExists(System.Guid jobid)
        {
            return this.RestClients.JobsClient.Job.Exists(this.Account.Name, jobid);
        }

        public JobDetails GetJobDetails(System.Guid jobid, bool extendedInfo)
        {

            var job = this.RestClients.JobsClient.Job.Get(this.Account.Name, jobid);

            var jobinfo = new JobDetails(job, this.Account);

            if (extendedInfo)
            {
                jobinfo.JobDetailsExtended = new JobDetailsExtended();
                jobinfo.JobDetailsExtended.Statistics = this.RestClients.JobsClient.Job.GetStatistics(this.Account.Name, jobid);
                // jobdetails.JobDetailsExtended.DebugDataPath = this.clients._JobRest.GetDebugDataPath(this.account, jobid);
            }

            return jobinfo;
        }

        public IEnumerable<JobInformationBasicEx> ListJobs(JobListParameters parameters)
        {
            var odata_query = new Microsoft.Rest.Azure.OData.ODataQuery<MSADL.Analytics.Models.JobInformation>();

            odata_query.OrderBy = parameters.Sorting.CreateOrderByString();
            odata_query.Filter = parameters.Filter.ToFilterString();

            // enumerate the job objects
            // Other parameters
            string opt_select = null;
            bool? opt_count = null;

            var pageiter = new Rest.PagedIterator<MSADLA.Models.JobInformationBasic>();
            pageiter.GetFirstPage = () => this.RestClients.JobsClient.Job.List(this.Account.Name, odata_query, opt_select, opt_count);
            pageiter.GetNextPage = p => this.RestClients.JobsClient.Job.ListNext(p.NextPageLink);

            var jobs = pageiter.EnumerateItems(parameters.Top);

            // convert them to the JobInfo
            var jobinfos = jobs.Select(j => new JobInformationBasicEx(j, this.Account));

            return jobinfos;
        }

        public IEnumerable<MSADL.Analytics.Models.JobPipelineInformation> ListJobPipelines(JobPipelineListParameters parameters)
        {
            var pageiter = new Rest.PagedIterator<MSADLA.Models.JobPipelineInformation>();
            pageiter.GetFirstPage = () => this.RestClients.JobsClient.Pipeline.List(this.Account.Name, parameters.DateRange.LowerBound, parameters.DateRange.UpperBound);
            pageiter.GetNextPage = p => this.RestClients.JobsClient.Pipeline.ListNext(p.NextPageLink);

            int top = 0;
            var items = pageiter.EnumerateItems(top);

            return items;
        }

        public IEnumerable<MSADL.Analytics.Models.JobRecurrenceInformation> ListJobRecurrences(JobReccurenceListParameters parameters)
        {

            var pageiter = new Rest.PagedIterator<MSADLA.Models.JobRecurrenceInformation>();
            pageiter.GetFirstPage = () => this.RestClients.JobsClient.Recurrence.List(this.Account.Name, parameters.DateRange.LowerBound, parameters.DateRange.UpperBound);
            pageiter.GetNextPage = p => this.RestClients.JobsClient.Recurrence.ListNext(p.NextPageLink);

            int top = 0;
            var recurrences = pageiter.EnumerateItems(top);
            return recurrences;
        }

        public JobInformationBasicEx CreateJob(JobCreateParameters parameters)
        {
            FixupCreateJobParameters(parameters);

            var usql_prop_parameters = new CreateUSqlJobProperties(parameters.ScriptText);
            var cj = new CreateJobParameters(JobType.USql,usql_prop_parameters,parameters.JobName);
            var job_info = this.RestClients.JobsClient.Job.Create(this.Account.Name, parameters.JobId, cj);

            var j = new JobInformationBasicEx(job_info, this.Account);
            return j;
        }

        private static void FixupCreateJobParameters(JobCreateParameters parameters)
        {
            // If caller doesn't provide a guid, then create a new one
            if (parameters.JobId == default(System.Guid))
            {
                parameters.JobId = System.Guid.NewGuid();
            }

            // if caller doesn't provide a name, then create one automativally
            if (parameters.JobName == null)
            {
                // TODO: Handle the date part of the name nicely
                parameters.JobName = "USQL " + System.DateTimeOffset.Now.ToString();
            }
        }

        public JobInformationBasicEx BuildJob(JobCreateParameters parameters)
        {
            FixupCreateJobParameters(parameters);

            var cj = new CreateJobProperties(parameters.ScriptText,null);
            var bj_parameters = new BuildJobParameters(JobType.USql, cj, parameters.JobName);
            var job_info = this.RestClients.JobsClient.Job.Build(this.Account.Name, bj_parameters);
            var j = new JobInformationBasicEx(job_info, this.Account);
            return j;
        }

        public MSADL.Analytics.Models.JobStatistics GetStatistics(System.Guid jobid)
        {
            return this.RestClients.JobsClient.Job.GetStatistics(this.Account.Name, jobid);
        }

        public MSADL.Analytics.Models.JobDataPath GetDebugDataPath(System.Guid jobid)
        {
            var jobdatapath = this.RestClients.JobsClient.Job.GetDebugDataPath(this.Account.Name, jobid);
            return jobdatapath;
        }
    }
}