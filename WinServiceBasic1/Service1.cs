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
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
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
            WebServer ws = new WebServer(SendResponse, "http://localhost:8080/test/");

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
            return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
        }
    }
}

