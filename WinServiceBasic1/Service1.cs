using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using NLog;
using System.Net;
using System.Threading;
using System.IO;

namespace WinServiceBasic1
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            /*if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");
 
            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");
 
            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");
            */
            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);
 
            _responderMethod = method;
            _listener.Start();
        }
 
        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }
 
        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                logger.Trace("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;                            
                            //HttpListenerResponse serverResponse = ctx.Request;
                            // TODO Explorar como obtener parametros query URL GET con Context.Request.QueryString["name"];
                            string name = ctx.Request.QueryString["name"];

                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }
 
        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }    

    public enum AppLogEvtId
    {        
        Online = 1,
        OutService = 2        
    }

    public partial class MyNewService : ServiceBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MyNewService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {            
            string version = ConfigurationManager.AppSettings["version"];
            WebServer ws = new WebServer(SendResponse, "http://localhost:8080/");

            logger.Trace("OnStart version {0}",version);
           
            logger.Trace("Running WebServer!");
            ws.Run();
            Thread.Sleep(20000);
            ws.Stop();
            logger.Trace("WebServer Stopped");
        }

        protected override void OnStop()
        {
            logger.Trace("OnStop");
        }

        public static string SendResponse(HttpListenerRequest request)
        {
            logger.Trace("KeepAlive: {0}", request.KeepAlive);
            logger.Trace("Local end point: {0}", request.LocalEndPoint.ToString());
            logger.Trace("Remote end point: {0}", request.RemoteEndPoint.ToString());
            logger.Trace("Is local? {0}", request.IsLocal);
            logger.Trace("HTTP method: {0}", request.HttpMethod);
            logger.Trace("Protocol version: {0}", request.ProtocolVersion);
            logger.Trace("Is authenticated: {0}", request.IsAuthenticated);
            logger.Trace("Is secure: {0}", request.IsSecureConnection);

            // The HTTP request body is most probably filled up by the client for a HTTP post.
            Stream requestBodyStream = request.InputStream;

            return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
        }
    }
}

