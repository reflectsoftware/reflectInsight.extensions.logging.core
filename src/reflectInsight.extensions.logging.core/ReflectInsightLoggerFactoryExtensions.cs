// ReflectInsight
// Copyright (c) 2020 ReflectSoftware.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using Microsoft.Extensions.Logging;

namespace ReflectInsight.Extensions.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectInsightLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds the reflect insight.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="configFile">The configuration file.</param>
        /// <returns></returns>
        public static ILoggerFactory AddReflectInsight(this ILoggerFactory factory, string configFile = "ReflectInsight.config")
        {
            factory.AddProvider(new ReflectInsightLoggerProvider(configFile));
            return factory;
        }

        /// <summary>
        /// Adds the reflect insight.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configFile">The configuration file.</param>
        /// <returns></returns>
        public static ILoggingBuilder AddReflectInsight(this ILoggingBuilder builder, string configFile = "ReflectInsight.config")
        {
            builder.AddProvider(new ReflectInsightLoggerProvider(configFile));
            return builder;
        }
    }
}
