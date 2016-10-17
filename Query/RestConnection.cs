using System;
using System.Collections.Generic;
using RestSharp;

namespace IW4MAdmin.Discord.Query
{
    class RestConnection
    {
        private static RestClient rc;
        private static RestConnection conn;

        public static RestConnection Initialize(Uri loc)
        {
            if (conn == null)
                conn = new RestConnection(loc);
            return conn;
        }

        public static RestConnection getConn()
        {
            return conn;
        }

        private RestConnection(Uri loc)
        {
            rc = new RestClient(loc);
        }

        public void makeRequest(string location, Dictionary<string, object> parameters,  Action<IRestResponse> callback)
        {
            var request = new RestRequest(location, Method.GET);

            foreach (string key in parameters.Keys)
                request.AddParameter(key, parameters[key]);

            rc.ExecuteAsync(request, callback);
        }
    }
}
