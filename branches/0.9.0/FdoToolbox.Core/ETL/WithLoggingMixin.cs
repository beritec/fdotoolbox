#region LGPL Header
// Copyright (C) 2009, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
//
// See license.txt for more/additional licensing information
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using log4net.Util;
using System.Globalization;
using log4net.Core;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// A base class that expose easily logging events
    /// </summary>
    public class WithLoggingMixin
    {
        private readonly ILog log;
        readonly List<Exception> errors = new List<Exception>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WithLoggingMixin"/> class.
        /// </summary>
        protected WithLoggingMixin()
        {
            log = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Error(Exception exception, string format, params object[] args)
        {
            SystemStringFormat message = new SystemStringFormat(CultureInfo.InvariantCulture, format, args);
            string errorMessage;
            if (exception != null)
                errorMessage = string.Format("{0}: {1}", message, exception.Message);
            else
                errorMessage = message.ToString();
            errors.Add(new FdoETLException(errorMessage, exception));
        }

        /// <summary>
        /// Logs a warn message
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Warn(string format, params object[] args)
        {
            if (log.IsWarnEnabled)
            {
                log.Logger.Log(GetType(), Level.Warn, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Debug(string format, params object[] args)
        {
            if (log.IsDebugEnabled)
            {
                log.Logger.Log(GetType(), Level.Debug, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }


        /// <summary>
        /// Logs a notice message
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Notice(string format, params object[] args)
        {
            if (log.Logger.IsEnabledFor(Level.Notice))
            {
                log.Logger.Log(GetType(), Level.Notice, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }


        /// <summary>
        /// Logs an information message
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        protected void Info(string format, params object[] args)
        {
            if (log.IsInfoEnabled)
            {
                log.Logger.Log(GetType(), Level.Info, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), null);
            }
        }

        /// <summary>
        /// Gets all the errors
        /// </summary>
        /// <value>The errors.</value>
        public Exception[] Errors
        {
            get { return errors.ToArray(); }
        }

        protected void ClearErrors() { errors.Clear(); }
    }
}
