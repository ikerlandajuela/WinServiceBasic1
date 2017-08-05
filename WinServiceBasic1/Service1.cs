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

namespace WinServiceBasic1
{
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
            //bool isError = true;
            string version = ConfigurationManager.AppSettings["version"];

            logger.Trace("OnStart version {0}",version);

            /*eventLog1.WriteEntry("In OnStart", EventLogEntryType.Information, (int)AppLogEvtId.Online);
            if (isError)
            {
                eventLog1.WriteEntry("Error OnStart", EventLogEntryType.Error, (int)AppLogEvtId.OutService);
            }*/
        }

        protected override void OnStop()
        {
            //eventLog1.WriteEntry("In OnStop", EventLogEntryType.Information, (int)AppLogEvtId.Online);
            logger.Trace("OnStop");
        }
    }
}

