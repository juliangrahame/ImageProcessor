﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GaussianSharpen.cs" company="James Jackson-South">
//   Copyright (c) James Jackson-South.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// <summary>
//   Applies a Gaussian sharpen to the image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageProcessor.Web.Processors
{
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;

    using ImageProcessor.Imaging;
    using ImageProcessor.Imaging.Helpers;
    using ImageProcessor.Processors;
    using ImageProcessor.Web.Helpers;

    /// <summary>
    /// Applies a Gaussian sharpen to the image.
    /// </summary>
    public class GaussianSharpen : IWebGraphicsProcessor
    {
        /// <summary>
        /// The regular expression to search strings for.
        /// </summary>
        private static readonly Regex QueryRegex = new Regex(@"\b(?!\W+)sharpen\b[=]", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianSharpen"/> class.
        /// </summary>
        public GaussianSharpen() => this.Processor = new ImageProcessor.Processors.GaussianSharpen();

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
        /// <param name="queryString">
        /// The query string to search.
        /// </param>
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

                int.TryParse(this.Processor.Settings["MaxSize"], NumberStyles.Any, CultureInfo.InvariantCulture, out int maxSize);
                double.TryParse(this.Processor.Settings["MaxSigma"], NumberStyles.Any, CultureInfo.InvariantCulture, out double maxSigma);
                int.TryParse(this.Processor.Settings["MaxThreshold"], NumberStyles.Any, CultureInfo.InvariantCulture, out int maxThreshold);

                NameValueCollection queryCollection = HttpUtility.ParseQueryString(queryString);
                int size = QueryParamParser.Instance.ParseValue<int>(queryCollection["sharpen"]);

                // Fall back to default sigma.
                double sigma = queryCollection["sigma"] != null ? QueryParamParser.Instance.ParseValue<double>(queryCollection["sigma"]) : 1.4d;
                int threshold = QueryParamParser.Instance.ParseValue<int>(queryCollection["threshold"]);

                // Normalize and set the variables.
                size = ImageMaths.Clamp(size, 0, maxSize);
                sigma = ImageMaths.Clamp(sigma, 0, maxSigma);
                threshold = ImageMaths.Clamp(threshold, 0, maxThreshold);

                this.Processor.DynamicParameter = new GaussianLayer(size, sigma, threshold);
            }

            return this.SortOrder;
        }
    }
}