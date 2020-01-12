// ReflectInsight
// Copyright (c) 2020 ReflectSoftware.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Microsoft.Extensions.Logging;
using ReflectSoftware.Insight;
using System;

namespace ReflectInsight.Extensions.Logging
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Logging.ILoggerProvider" />
    public class ReflectInsightLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectInsightLoggerProvider"/> class.
        /// </summary>
        /// <param name="configFile">The configuration file.</param>
        public ReflectInsightLoggerProvider(string configFile = "ReflectInsight.config")
        {
            var config = $"{AppDomain.CurrentDomain.BaseDirectory}{configFile}";
            ReflectInsightConfig.Control.SetExternalConfigurationMode(config);
        }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new ReflectInsightLogger(categoryName);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {      
            // nothing to implement      
        }
    }
}
