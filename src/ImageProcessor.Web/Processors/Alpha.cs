﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Alpha.cs" company="James Jackson-South">
//   Copyright (c) James Jackson-South.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// <summary>
//   Encapsulates methods to change the alpha component of the image to effect its transparency.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageProcessor.Web.Processors
{
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;
    using System.Web;

    using ImageProcessor.Imaging.Helpers;
    using ImageProcessor.Processors;
    using ImageProcessor.Web.Helpers;

    /// <summary>
    /// Encapsulates methods to change the alpha component of the image to effect its transparency.
    /// </summary>
    public class Alpha : IWebGraphicsProcessor
    {
        /// <summary>
        /// The regular expression to search strings for.
        /// </summary>
        private static readonly Regex QueryRegex = new Regex(@"alpha=\d+", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="Alpha"/> class.
        /// </summary>
        public Alpha() => this.Processor = new ImageProcessor.Processors.Alpha();

        /// <summary>
        /// Gets the regular expression to search strings for.
        /// </summary>
        public Regex RegexPattern => QueryRegex;

        /// <summary>
        /// Gets the order in which this processor is to be used in a chain.
        /// </summary>
        public int SortOrder { get; private set; }

        /// <summary>
        /// Gets the associated graphics processor.
        /// </summary>
        public IGraphicsProcessor Processor { get; }

        /// <summary>
        /// The position in the original string where the first character of the captured substring was found.
        /// </summary>
        /// <param name="queryString">The query string to search.</param>
        /// <returns>
        /// The zero-based starting position in the original string where the captured substring was found.
        /// </returns>
        public int MatchRegexIndex(string queryString)
        {
            this.SortOrder = int.MaxValue;

            Match match = this.RegexPattern.Match(queryString);
            if (match.Success)
            {
                this.SortOrder = match.Index;
                NameValueCollection queryCollection = HttpUtility.ParseQueryString(queryString);
                int percentage = QueryParamParser.Instance.ParseValue<int>(queryCollection["alpha"]);
                percentage = ImageMaths.Clamp(percentage, 0, 100);
                this.Processor.DynamicParameter = percentage;
            }

            return this.SortOrder;
        }
    }
}
