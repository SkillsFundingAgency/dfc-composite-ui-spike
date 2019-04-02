﻿using System.Net;
using System.Net.Http;

namespace Ncs.Prototype.Web.WebComposition.Framework
{
    public class DefaultHttpClientHandler : HttpClientHandler
    {
        public DefaultHttpClientHandler() =>
            this.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
    }
}
