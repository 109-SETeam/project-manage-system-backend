﻿using project_manage_system_backend.Dtos;
using project_manage_system_backend.Dtos.Gitlab;
using project_manage_system_backend.Models;
using project_manage_system_backend.Shares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace project_manage_system_backend.RepoInfo
{
    public class GitlabInfo : RepoInfoBase
    {
        private const string token = "access_token=nKswk3SkyZVyMR_q9KJ4";

        public GitlabInfo(string oauthToken, HttpClient httpClient = null) : base(oauthToken, httpClient)
        {
        }

        public override Task<List<ResponseCodebaseDto>> RequestCodebase(Repo repo)
        {
            throw new NotImplementedException();
        }

        public override async Task<RequestCommitInfoDto> RequestCommit(Repo repo)
        {
            RequestCommitInfoDto requestCommitInfo = new RequestCommitInfoDto();
            List<RequestCommitsDto> requestCommits = await GetRequestCommits(repo.RepoId);
            requestCommitInfo.WeekTotalData = GetWeekTotalDatas(requestCommits);

            return requestCommitInfo;
        }

        private List<WeekTotalData> GetWeekTotalDatas(List<RequestCommitsDto> requestCommits)
        {
            List<WeekTotalData> weekTotalDatas = new List<WeekTotalData>();
            List<Week> weeks = BuildWeeks(requestCommits[^1].committed_date);
            
            foreach (var requestCommit in requestCommits)
            {
                String commitWeek = GetDateOfWeek(requestCommit.committed_date).ToShortDateString();
                Week week = weeks.Find(week => week.ws.Equals(commitWeek));
                week.c += 1;
            }

            foreach (Week week in weeks)
            {
                weekTotalDatas.Add(new WeekTotalData { Week = week.ws, Total = week.c });
            }

            return weekTotalDatas;
        }

        private List<DayOfWeekData> GetDayOfWeekDatas(List<RequestCommitsDto> requestCommits)
        {
            List<DayOfWeekData> dayOfWeekData = new List<DayOfWeekData>();
            List<Week> weeks = BuildWeeks(requestCommits[^1].committed_date);

            foreach (Week week in weeks)
            {
                List<DayCommit> dayCommits = new List<DayCommit>();
                dayOfWeekData.Add(new DayOfWeekData { Week = week.ws, DetailDatas = dayCommits });
                for (int i = 0; i < 7; i++)
                {
                    dayCommits.Add(new DayCommit { Day = DateHandler.ConvertToDayOfWeek(i) });
                }
            }

            // 實作 Commit To Date
            return dayOfWeekData;
        }

        public override async Task<List<ContributorsCommitActivityDto>> RequestContributorsActivity(Repo repo)
        {
            string contributorUrl = $"https://sgit.csie.ntut.edu.tw/gitlab/api/v4/projects/{repo.RepoId}/repository/contributors?{token}";
            var contributorResponse = await _httpClient.GetAsync(contributorUrl);
            string contributorContent = await contributorResponse.Content.ReadAsStringAsync();
            var contributorResult = JsonSerializer.Deserialize<List<RequestContributorDto>>(contributorContent);
            var commitsResult = await GetRequestCommits(repo.RepoId);

            List<ContributorsCommitActivityDto> contributors = new List<ContributorsCommitActivityDto>();
            foreach (var item in contributorResult)
            {
                contributors.Add(new ContributorsCommitActivityDto
                {
                    author = new Author { login = item.name, email = item.email },
                    // ^1 = commitsResult.Count - 1
                    weeks = BuildWeeks(commitsResult[^1].committed_date)
                });
            }
            MapCommitsToWeeks(commitsResult, contributors);

            return contributors;
        }

        private async Task<List<RequestCommitsDto>> GetRequestCommits(string repoId)
        {
            string commitsUrl = $"https://sgit.csie.ntut.edu.tw/gitlab/api/v4/projects/{repoId}/repository/commits?{token}&with_stats=true&per_page=100";
            var commitsResponse = await _httpClient.GetAsync(commitsUrl);
            string commitsContent = await commitsResponse.Content.ReadAsStringAsync();
            var commitsResult = JsonSerializer.Deserialize<List<RequestCommitsDto>>(commitsContent);
            var xTotalPages = Enumerable.ToList<string>(commitsResponse.Headers.GetValues("X-Total-Pages"));
            int xTotalPage = int.Parse(xTotalPages[0]);

            for (int i = 2; i <= xTotalPage; i++)
            {
                var response = await _httpClient.GetAsync($"{commitsUrl}&page={i}");
                var content = await response.Content.ReadAsStringAsync();
                commitsResult.AddRange(JsonSerializer.Deserialize<List<RequestCommitsDto>>(content));
            }
            return commitsResult;
        }

        private List<Week> BuildWeeks(DateTime commitDate)
        {
            List<Week> weeks = new List<Week>();
            var dateOfCommitWeek = GetDateOfWeek(commitDate);
            var dateOfCurrentWeek = GetDateOfWeek(DateTime.Today);

            while (dateOfCommitWeek <= dateOfCurrentWeek)
            {
                weeks.Add(new Week { ws = dateOfCommitWeek.ToShortDateString() });
                dateOfCommitWeek = dateOfCommitWeek.AddDays(7);
            }

            return weeks;
        }

        private DateTime GetDateOfWeek(DateTime dateTime)
        {
            return dateTime.AddDays(-((int)dateTime.DayOfWeek));
        }

        private void MapCommitsToWeeks(List<RequestCommitsDto> commitsResult, List<ContributorsCommitActivityDto> contributors)
        {
            foreach (var commit in commitsResult)
            {
                String commitWeek = GetDateOfWeek(commit.committed_date).ToShortDateString();
                foreach (var contributor in contributors)
                {
                    if (contributor.author.login.Equals(commit.committer_name) && contributor.author.email.Equals(commit.committer_email))
                    {
                        Week week = contributor.weeks.Find(week => week.ws.Equals(commitWeek));
                        week.a += commit.stats.additions;
                        week.d += commit.stats.deletions;
                        week.c += 1;
                        contributor.totalAdditions += commit.stats.additions;
                        contributor.totalDeletions += commit.stats.deletions;
                        contributor.total += 1;
                        break;
                    }
                }
            }
        }

        public override Task<RepoIssuesDto> RequestIssue(Repo repo)
        {
            throw new NotImplementedException();
        }
    }
}
