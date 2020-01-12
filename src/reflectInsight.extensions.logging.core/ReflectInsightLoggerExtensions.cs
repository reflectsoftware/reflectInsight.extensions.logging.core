// ReflectInsight
// Copyright (c) 2020 ReflectSoftware.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Microsoft.Extensions.Logging;
using ReflectSoftware.Insight.Common;
using ReflectSoftware.Insight.Common.Data;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ReflectInsight.Extensions.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectInsightLoggerExtensions
    {
        private static readonly ConcurrentDictionary<int, IReflectInsight[]> _riLoggers;

        /// <summary>
        /// Initializes the <see cref="ReflectInsightLoggerExtensions"/> class.
        /// </summary>
        static ReflectInsightLoggerExtensions()
        {
            _riLoggers = new ConcurrentDictionary<int, IReflectInsight[]>();
        }

        #region Private
        /// <summary>
        /// Gets the reflect insight.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        private static IReflectInsight[] GetReflectInsights(ILogger logger)
        {
            var reflectInsightLoggers = new List<IReflectInsight>();
            var property = logger.GetType().GetProperty("Loggers");
            if (property != null)
            {
                var loggers = (property.GetValue(logger) as Array);
                if(loggers != null)
                {
                    foreach (var logItem in loggers)
                    {
                        var logProperty = logItem.GetType().GetProperty("Logger");
                        if(logProperty.GetValue(logItem) is IReflectInsightLogger)
                        {
                            reflectInsightLoggers.Add((logProperty.GetValue(logItem) as IReflectInsightLogger).GetLogger());
                        }
                    }
                }
            }

            return reflectInsightLoggers.ToArray();
        }
        
        /// <summary>
        /// Gets the reflect insight logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        private static IReflectInsight[] GetReflectInsightInstances(this ILogger logger)
        {
            var loggerType = logger.GetType();

            return _riLoggers.GetOrAdd(loggerType.GetHashCode(), (key) =>
            {
                var riLoggers = (IReflectInsight[])null;                
                var property = loggerType.GetProperty("Loggers"); 
                if(property == null)
                {
                    var bindings = BindingFlags.NonPublic | BindingFlags.Instance;
                    var field = loggerType.GetField("_logger", bindings);
                    var innerLoger = field.GetValue(logger) as ILogger;
                    if(innerLoger != null)
                    {
                        riLoggers = GetReflectInsights(innerLoger);
                    }
                }
                else
                {
                    riLoggers = GetReflectInsights(logger);
                }

                return riLoggers;
            });
        }
        #endregion Private

        #region Standard Messages

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static ILogger LogInformation(this ILogger logger, Exception ex)
        {
            logger.LogInformation(new EventId(0), ex, ex.Message);
            return logger;
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static ILogger LogWarning(this ILogger logger, Exception ex)
        {
            logger.LogWarning(new EventId(0), ex, ex.Message);
            return logger;
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static ILogger LogError(this ILogger logger, Exception ex)
        {
            logger.LogError(new EventId(0), ex, ex.Message);
            return logger;
        }

        /// <summary>
        /// Logs the critical.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        public static ILogger LogCritical(this ILogger logger, Exception ex)
        {
            logger.LogCritical(new EventId(0), ex, ex.Message);
            return logger;
        }

        #endregion Standard Messages

        #region ReflectInsight Specific Methods

        /// <summary>
        /// Clears the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger Clear(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.Clear();                
            }

            return logger;
        }

        /// <summary>
        /// Adds the checkpoint.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger AddCheckpoint(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.AddCheckpoint();
            }

            return logger;
        }

        /// <summary>
        /// Adds the checkpoint.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static ILogger AddCheckpoint(this ILogger logger, string name)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.AddCheckpoint(name);
            }

            return logger;
        }

        /// <summary>
        /// Adds the checkpoint.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="cType">Type of the c.</param>
        /// <returns></returns>
        public static ILogger AddCheckpoint(this ILogger logger, Checkpoint cType)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.AddCheckpoint(cType);
            }

            return logger;
        }

        /// <summary>
        /// Adds the checkpoint.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="name">The name.</param>
        /// <param name="cType">Type of the c.</param>
        /// <returns></returns>
        public static ILogger AddCheckpoint(this ILogger logger, string name, Checkpoint cType)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.AddCheckpoint(name, cType);
            }

            return logger;
        }

        /// <summary>
        /// Adds the separator.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger AddSeparator(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.AddSeparator();
            }

            return logger;
        }

        /// <summary>
        /// Enters the method.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger EnterMethod(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.EnterMethod(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Enters the method.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="currentMethod">The current method.</param>
        /// <param name="fullName">if set to <c>true</c> [full name].</param>
        /// <returns></returns>
        public static ILogger EnterMethod(this ILogger logger, MethodBase currentMethod, bool fullName = true)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.EnterMethod(currentMethod, fullName);
            }

            return logger;
        }

        /// <summary>
        /// Exits the method.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger ExitMethod(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.ExitMethod(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Exits the method.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="currentMethod">The current method.</param>
        /// <param name="fullName">if set to <c>true</c> [full name].</param>
        /// <returns></returns>
        public static ILogger ExitMethod(this ILogger logger, MethodBase currentMethod, bool fullName = true)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.ExitMethod(currentMethod, fullName);
            }

            return logger;
        }

        /// <summary>
        /// Resets all checkpoints.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger ResetAllCheckpoints(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.ResetAllCheckpoints();
            }

            return logger;
        }

        /// <summary>
        /// Resets the checkpoint.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger ResetCheckpoint(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.ResetCheckpoint();
            }

            return logger;
        }

        /// <summary>
        /// Resets the checkpoint.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="cType">Type of the c.</param>
        /// <returns></returns>
        public static ILogger ResetCheckpoint(this ILogger logger, Checkpoint cType)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.ResetCheckpoint(cType);
            }

            return logger;
        }

        /// <summary>
        /// Resets the checkpoint.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static ILogger ResetCheckpoint(this ILogger logger, string name)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.ResetCheckpoint(name);
            }

            return logger;
        }

        /// <summary>
        /// Resets the checkpoint.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="name">The name.</param>
        /// <param name="cType">Type of the c.</param>
        /// <returns></returns>
        public static ILogger ResetCheckpoint(this ILogger logger, string name, Checkpoint cType)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.ResetCheckpoint(name, cType);
            }

            return logger;
        }

        /// <summary>
        /// Logs the specified m type.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mType">Type of the m.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger Log(this ILogger logger, MessageType mType, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.Send(mType, str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the specified m type.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="mType">Type of the m.</param>
        /// <param name="str">The string.</param>
        /// <param name="details">The details.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger Log(this ILogger logger, MessageType mType, string str, string details, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.Send(mType, str, details, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the assert.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogAssert(this ILogger logger, bool condition, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendAssert(condition, str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the assigned.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="obj">The object.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogAssigned(this ILogger logger, string str, object obj, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendAssigned(str, obj, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the attachment.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogAttachment(this ILogger logger, string str, string fileName, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendAttachment(str, fileName, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the audit failure.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogAuditFailure(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendAuditFailure(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the audit success.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogAuditSuccess(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendAuditSuccess(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the checkmark.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogCheckmark(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendCheckmark(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the checkmark.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="cmType">Type of the cm.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogCheckmark(this ILogger logger, string str, Checkmark cmType, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendCheckmark(str, cmType, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the collection.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="enumerator">The enumerator.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogCollection(this ILogger logger, string str, IEnumerable enumerator, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendCollection(str, enumerator, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the collection.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="enumerator">The enumerator.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogCollection(this ILogger logger, string str, IEnumerable enumerator, ObjectScope scope, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendCollection(str, enumerator, scope, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the comment.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogComment(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendComment(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the currency.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="val">The value.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogCurrency(this ILogger logger, string str, decimal? val, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendCurrency(str, val, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the currency.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="val">The value.</param>
        /// <param name="ci">The ci.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogCurrency(this ILogger logger, string str, decimal? val, CultureInfo ci, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendCurrency(str, val, ci, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the custom data.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="cData">The c data.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogCustomData(this ILogger logger, string str, RICustomData cData, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendCustomData(str, cData, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the date time.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogDateTime(this ILogger logger, string str, DateTime? dt, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendDateTime(str, dt, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the date time.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogDateTime(this ILogger logger, string str, DateTime? dt, string format, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendDateTime(str, dt, format, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the date time.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="ci">The ci.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogDateTime(this ILogger logger, string str, DateTime? dt, CultureInfo ci, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendDateTime(str, dt, ci, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the date time.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="format">The format.</param>
        /// <param name="ci">The ci.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogDateTime(this ILogger logger, string str, DateTime? dt, string format, CultureInfo ci, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendDateTime(str, dt, format, ci, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the HTML.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogHTML(this ILogger logger, string str, Stream stream, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendHTML(str, stream, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the HTML.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogHTML(this ILogger logger, string str, TextReader reader, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendHTML(str, reader, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the HTML file.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogHTMLFile(this ILogger logger, string str, string fileName, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendHTMLFile(str, fileName, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the HTML string.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="htmlString">The HTML string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogHTMLString(this ILogger logger, string str, string htmlString, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendHTMLString(str, htmlString, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the json.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogJSON(this ILogger logger, string str, TextReader reader, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendJSON(str, reader, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the json.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="json">The json.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogJSON(this ILogger logger, string str, string json, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendJSON(str, json, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the json.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="json">The json.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogJSON(this ILogger logger, string str, object json, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendJSON(str, json, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the json.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogJSON(this ILogger logger, string str, Stream stream, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendJSON(str, stream, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the json file.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogJSONFile(this ILogger logger, string str, string fileName, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendJSONFile(str, fileName, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="level">The level.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogLevel(this ILogger logger, string str, LevelType level, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendLevel(str, level, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the loaded assemblies.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger LogLoadedAssemblies(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendLoadedAssemblies();
            }

            return logger;
        }

        /// <summary>
        /// Logs the loaded assemblies.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogLoadedAssemblies(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendLoadedAssemblies(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the loaded processes.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger LogLoadedProcesses(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendLoadedProcesses();
            }

            return logger;
        }

        /// <summary>
        /// Logs the loaded processes.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogLoadedProcesses(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendLoadedProcesses(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogMessage(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendMessage(str, args);
            }

            return logger; 
        }

        /// <summary>
        /// Logs the note.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogNote(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendNote(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the object.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="obj">The object.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogObject(this ILogger logger, string str, object obj, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendObject(str, obj, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the object.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="obj">The object.</param>
        /// <param name="bIgnoreStandard">if set to <c>true</c> [b ignore standard].</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogObject(this ILogger logger, string str, object obj, bool bIgnoreStandard, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendObject(str, obj, bIgnoreStandard, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the object.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="obj">The object.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogObject(this ILogger logger, string str, object obj, ObjectScope scope, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendObject(str, obj, scope, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the process information.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger LogProcessInformation(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendProcessInformation();
            }

            return logger;
        }

        /// <summary>
        /// Logs the process information.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="aProcess">a process.</param>
        /// <returns></returns>
        public static ILogger LogProcessInformation(this ILogger logger, Process aProcess)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendProcessInformation(aProcess);
            }

            return logger;
        }

        /// <summary>
        /// Logs the reminder.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogReminder(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendReminder(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the resume.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogResume(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendResume(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the SQL script.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogSQLScript(this ILogger logger, string str, string fileName, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendSQLScript(str, fileName, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the SQL script.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogSQLScript(this ILogger logger, string str, TextReader reader, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendSQLScript(str, reader, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the SQL script.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogSQLScript(this ILogger logger, string str, Stream stream, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendSQLScript(str, stream, args);
            }

            return logger; 
        }

        /// <summary>
        /// Logs the SQL string.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="sql">The SQL.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogSQLString(this ILogger logger, string str, string sql, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendSQLString(str, sql, args);
            }

            return logger; 
        }

        /// <summary>
        /// Logs the stack trace.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger LogStackTrace(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendStackTrace();
            }

            return logger;
        }

        /// <summary>
        /// Logs the stack trace.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogStackTrace(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendStackTrace(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the start.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogStart(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendStart(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the stop.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogStop(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendStop(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the stream.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogStream(this ILogger logger, string str, string fileName, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendStream(str, fileName, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the stream.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogStream(this ILogger logger, string str, Stream stream, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendStream(str, stream, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the stream.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogStream(this ILogger logger, string str, byte[] stream, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendStream(str, stream, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the string.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="theString">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogString(this ILogger logger, string str, string theString, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendString(str, theString, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the string.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="theString">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogString(this ILogger logger, string str, StringBuilder theString, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendString(str, theString, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the suspend.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogSuspend(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendSuspend(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the text file.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogTextFile(this ILogger logger, string str, TextReader reader, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendTextFile(str, reader, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the text file.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogTextFile(this ILogger logger, string str, string fileName, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendTextFile(str, fileName, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the text file.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogTextFile(this ILogger logger, string str, Stream stream, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendTextFile(str, stream, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the timestamp.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger LogTimestamp(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendTimestamp();
            }

            return logger;
        }

        /// <summary>
        /// Logs the timestamp.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="tz">The tz.</param>
        /// <returns></returns>
        public static ILogger LogTimestamp(this ILogger logger, TimeZoneInfo tz)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendTimestamp(tz);
            }

            return logger;
        }

        /// <summary>
        /// Logs the timestamp.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogTimestamp(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendTimestamp(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the timestamp.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="timeZoneId">The time zone identifier.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogTimestamp(this ILogger logger, string str, string timeZoneId, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendTimestamp(str, timeZoneId, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the timestamp.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="tz">The tz.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogTimestamp(this ILogger logger, string str, TimeZoneInfo tz, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendTimestamp(str, tz, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the transfer.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogTransfer(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendTransfer(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the typed collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger">The logger.</param>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns></returns>
        public static ILogger LogTypedCollection<T>(this ILogger logger, IEnumerable<T> enumerable)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendTypedCollection<T>(enumerable);
            }

            return logger;
        }

        /// <summary>
        /// Logs the typed collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="enumerables">The enumerables.</param>
        /// <returns></returns>
        public static ILogger LogTypedCollection<T>(this ILogger logger, string str, params IEnumerable<T>[] enumerables)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendTypedCollection<T>(str, enumerables);
            }

            return logger;
        }

        /// <summary>
        /// Logs the verbose.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogVerbose(this ILogger logger, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendVerbose(str, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the XML.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogXML(this ILogger logger, string str, Stream stream, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendXML(str, stream, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the XML.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogXML(this ILogger logger, string str, TextReader reader, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendXML(str, reader, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the XML.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogXML(this ILogger logger, string str, XmlReader reader, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendXML(str, reader, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the XML.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="node">The node.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogXML(this ILogger logger, string str, XmlNode node, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendXML(str, node, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the XML file.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogXMLFile(this ILogger logger, string str, string fileName, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendXMLFile(str, fileName, args);
            }

            return logger;
        }

        /// <summary>
        /// Logs the XML string.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="str">The string.</param>
        /// <param name="xmlString">The XML string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger LogXMLString(this ILogger logger, string str, string xmlString, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.SendXMLString(str, xmlString, args);
            }

            return logger;
        }

        /// <summary>
        /// Viewers the clear all.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger ViewerClearAll(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.ViewerClearAll();
            }

            return logger;
        }

        /// <summary>
        /// Viewers the clear watches.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static ILogger ViewerClearWatches(this ILogger logger)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.ViewerClearWatches();
            }

            return logger;
        }

        /// <summary>
        /// Viewers the send watch.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="labelID">The label identifier.</param>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static ILogger ViewerSendWatch(this ILogger logger, string labelID, object obj)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.ViewerSendWatch(labelID, obj);
            }

            return logger;
        }

        /// <summary>
        /// Viewers the send watch.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="labelID">The label identifier.</param>
        /// <param name="str">The string.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static ILogger ViewerSendWatch(this ILogger logger, string labelID, string str, params object[] args)
        {
            var riLoggers = GetReflectInsightInstances(logger);
            foreach (var riLogger in riLoggers)
            {
                riLogger.ViewerSendWatch(labelID, str, args);
            }

            return logger;
        }

        #endregion ReflectInsight Specific Methods
    }
}
