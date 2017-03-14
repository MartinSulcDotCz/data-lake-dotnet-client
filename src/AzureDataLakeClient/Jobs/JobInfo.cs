﻿using System;
using System.Collections.Generic;
using MSADLA=Microsoft.Azure.Management.DataLake.Analytics;

namespace AzureDataLakeClient.Jobs
{
    public class JobInfo
    {
        public readonly string Name;
        public readonly string LogFolder;
        public readonly int? AUs;
        public readonly DateTimeOffset? EndTime;
        public readonly IList<MSADLA.Models.JobErrorDetails> ErrorMessage;
        public readonly Guid? Id;
        public readonly IList<string> LogFilePatterns;
        public readonly int? Priority;
        public readonly MSADLA.Models.JobProperties Properties;
        public readonly MSADLA.Models.JobResult? Result;
        public readonly DateTimeOffset? StartTime;
        public readonly MSADLA.Models.JobState? State ;
        public readonly IList<MSADLA.Models.JobStateAuditRecord> StateAuditRecords;
        public readonly DateTimeOffset? SubmitTime;
        public readonly MSADLA.Models.JobType Type;
        public readonly string Submitter;

        public readonly AnalyticsAccount Account;

        public TimeSpan? QueueDuration
        {
            get
            {
                if (this.SubmitTime.HasValue && this.StartTime.HasValue)
                {
                    return this.StartTime.Value- this.SubmitTime.Value;
                }
                return null;
            }
        }

        public TimeSpan? ExecutionDuration
        {
            get
            {
                if (this.StartTime.HasValue && this.EndTime.HasValue)
                {
                    return this.EndTime.Value - this.StartTime.Value;
                }
                return null;
            }
        }

        public double? AUSeconds
        {
            get
            {
                var dur = this.ExecutionDuration;
                if (this.AUs.HasValue && dur.HasValue)
                {
                    return (this.AUs.Value * dur.Value.TotalSeconds);
                }
                return null;
            }
        }


        public string GetUri()
        {
            string joburi_string = $"https://{this.Account.Name}.azuredatalakeanalytics.net/jobs/{this.Id}?api-version=2015-10-01-preview";
            return joburi_string;
        }

        public string GetAzurePortalLink()
        {
            string job_portal_link_string = $"https://portal.azure.com/#blade/Microsoft_Azure_DataLakeAnalytics/SqlIpJobDetailsBlade/accountId/%2Fsubscriptions%2F{this.Account.Subscription.Id}%2FresourceGroups%2F{this.Account.ResourceGroup.Name}%2Fproviders%2FMicrosoft.DataLakeAnalytics%2Faccounts%2F{this.Account.Name}/jobId/{this.Id}";
            return job_portal_link_string;
        }

        internal JobInfo(MSADLA.Models.JobInformation job, AnalyticsAccount acct)
        {
            this.Account = acct;
            this.Name = job.Name;
            this.LogFolder = job.LogFolder;
            this.LogFolder = job.Submitter;
            this.AUs = job.DegreeOfParallelism;
            this.EndTime = job.EndTime;
            this.ErrorMessage = job.ErrorMessage;
            this.Id = job.JobId;
            this.LogFilePatterns = job.LogFilePatterns;
            this.Priority = job.Priority;
            this.Properties = job.Properties;
            this.Result = job.Result;
            this.StartTime = job.StartTime;
            this.State = job.State;
            this.StateAuditRecords = job.StateAuditRecords;
            this.SubmitTime = job.SubmitTime;
            this.Type = job.Type;
            this.Submitter = job.Submitter;
        }
    }
}