using System.Collections.Generic;

namespace Blink.Plugin.GitLog
{
    class CommitDetail
    {
        public string Status { get; set; }
        public string Path { get; set; }
    }
    class CommitInfo
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public List<CommitDetail> Detail { get; set; }

        public CommitInfo()
        {
            Detail = new List<CommitDetail>();
        }
    }
    class CustomGitLog
    {
        public string Path { get; set; }
        public string BranchName { get; set; }
        public List<CommitInfo> Commits { get; set; }

        public CustomGitLog()
        {
            Commits = new List<CommitInfo>();
        }
    }
}
