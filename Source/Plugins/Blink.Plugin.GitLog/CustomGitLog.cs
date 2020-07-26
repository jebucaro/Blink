/*
 * MIT License
 * 
 * Copyright (c) 2020 Jonathan Búcaro
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 */

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
