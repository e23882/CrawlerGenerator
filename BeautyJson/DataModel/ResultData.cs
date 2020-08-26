using System;
using System.Collections.Generic;

namespace BeautyJson.DataModel
{
    public class ResultData
    {
        public string Result { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Creator    {
        public string name { get; set; } 
        public string version { get; set; } 
    }

    public class PageTimings    {
        public double onContentLoad { get; set; } 
        public double onLoad { get; set; } 
    }

    public class Page    {
        public DateTime startedDateTime { get; set; } 
        public string id { get; set; } 
        public string title { get; set; } 
        public PageTimings pageTimings { get; set; } 
    }

    public class Initiator    {
        public string type { get; set; } 
    }

    public class Cache    {
    }

    public class Header    {
        public string name { get; set; } 
        public string value { get; set; } 
    }

    public class Cooky    {
        public string name { get; set; } 
        public string value { get; set; } 
        public object expires { get; set; } 
        public bool httpOnly { get; set; } 
        public bool secure { get; set; } 
    }

    public class Param    {
        public string name { get; set; } 
        public string value { get; set; } 
    }

    public class PostData    {
        public string mimeType { get; set; } 
        public string text { get; set; } 
        public List<Param> _params { get; set; } 
    }

    public class Request    {
        public string method { get; set; } 
        public string url { get; set; } 
        public string httpVersion { get; set; } 
        public List<Header> headers { get; set; } 
        public List<object> queryString { get; set; } 
        public List<Cooky> cookies { get; set; } 
        public int headersSize { get; set; } 
        public int bodySize { get; set; } 
        public PostData postData { get; set; } 
    }

    public class Header2    {
        public string name { get; set; } 
        public string value { get; set; } 
    }

    public class Cooky2    {
        public string name { get; set; } 
        public string value { get; set; } 
        public string path { get; set; } 
        public string domain { get; set; } 
        public object expires { get; set; } 
        public bool httpOnly { get; set; } 
        public bool secure { get; set; } 
    }

    public class Content    {
        public int size { get; set; } 
        public string mimeType { get; set; } 
        public int compression { get; set; } 
        public string text { get; set; }
    }

    public class Response    {
        public int status { get; set; } 
        public string statusText { get; set; } 
        public string httpVersion { get; set; } 
        public List<Header2> headers { get; set; } 
        public List<Cooky2> cookies { get; set; } 
        public Content content { get; set; } 
        public string redirectURL { get; set; } 
        public int headersSize { get; set; } 
        public int bodySize { get; set; } 
        public int _transferSize { get; set; } 
        public object _error { get; set; } 
    }

    public class Timings    {
        public double blocked { get; set; } 
        public double dns { get; set; } 
        public double ssl { get; set; } 
        public double connect { get; set; } 
        public double send { get; set; } 
        public double wait { get; set; } 
        public double receive { get; set; } 
        public double _blocked_queueing { get; set; } 
    }

    public class Entry    {
        public Initiator _initiator { get; set; } 
        public string _priority { get; set; } 
        public string _resourceType { get; set; } 
        public Cache cache { get; set; } 
        public string connection { get; set; } 
        public string pageref { get; set; } 
        public Request request { get; set; } 
        public Response response { get; set; } 
        public string serverIPAddress { get; set; } 
        public DateTime startedDateTime { get; set; } 
        public double time { get; set; } 
        public Timings timings { get; set; } 
    }

    public class Log    {
        public string version { get; set; } 
        public Creator creator { get; set; } 
        public List<Page> pages { get; set; } 
        public List<Entry> entries { get; set; } 
    }

    public class Root    {
        public Log log { get; set; } 
    }


        

}