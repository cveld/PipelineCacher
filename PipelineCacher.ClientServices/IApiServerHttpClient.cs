using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PipelineCacher.Client.Services
{
    public interface IApiServerHttpClient
    {
        public HttpClient AnonymousHttpClient { get; }
        public HttpClient AuthorizedHttpClient { get; }
    }
}
