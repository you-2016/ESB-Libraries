// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BamActivityStep.cs" company="Solidsoft Reply Ltd.">
//   Copyright 2015 Solidsoft Reply Limited.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.     ????//   Copyright 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.Facts
{
    using System;
    using System.Collections;
    using System.Xml.Serialization;
    using Microsoft.BizTalk.Bam.EventObservation; 
    
    /// <summary>
    /// Represents an activity as a fact.  The policy for the named activity
    /// will register BAM tracking points to pass back to the resolver.
    /// </summary>
    /// <remarks>
    /// This class subclasses the ActivityInterceptorConfiguration and surfaces
    /// the activity name in order to support simple rules that test for the activity name.
    /// </remarks>
    [Serializable]
    [XmlInclude(typeof(TrackPoint))]
    public class BamActivityStep : ActivityInterceptorConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BamActivityStep"/> class. 
        /// </summary>
        public BamActivityStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BamActivityStep"/> class. 
        /// Constructor.
        /// </summary>
        /// <param name="activityName">
        /// Name of activity.
        /// </param>
        /// <param name="stepName">
        /// Name of activity step.
        /// </param>
        public BamActivityStep(string activityName, string stepName)
            : base(activityName)
        {
            this.ActivityName = activityName;
            this.StepName = stepName;
        }
        
        /// <summary>
        /// Gets or sets the activity name.
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// Gets or sets the BAM step name.
        /// </summary>
        public string StepName { get; set; }

        /// <summary>
        /// Gets the TrackPoints.
        /// </summary>
        [XmlElement("TrackPoint", typeof(TrackPoint))]
        public new ArrayList TrackPoints
        {
            get
            {
                return base.TrackPoints;
            }
        }

        /// <summary>
        /// Registers the start of a new activity.
        /// </summary>
        /// <param name="location">The named location.</param>
        /// <param name="activityIdExtractionInfo">The extraction information of the activity ID.</param>
        public void RegisterStartNewEx(object location, object activityIdExtractionInfo)
        {
            this.RegisterStartNew(location, activityIdExtractionInfo);
            this.RegisterEnd(location);
        }

        /// <summary>
        /// Registers the continuation of an existing activity.
        /// </summary>
        /// <param name="location">The named location.</param>
        /// <param name="continuationTokenExtractionInfo">The extraction information of the continuation ID.</param>
        public void RegisterContinueEx(object location, object continuationTokenExtractionInfo)
        {
            this.RegisterContinueEx(location, continuationTokenExtractionInfo, null);
        }

        /// <summary>
        /// Registers the continuation of an existing activity.
        /// </summary>
        /// <param name="location">The named location.</param>
        /// <param name="continuationTokenExtractionInfo">The extraction information of the activity ID.</param>
        /// <param name="prefix">A unique prefix used to convert the activity ID to a continuation ID.</param>
        public void RegisterContinueEx(object location, object continuationTokenExtractionInfo, string prefix)
        {
            var trimmedPrefix = prefix;

            if (trimmedPrefix != null)
            {
                trimmedPrefix = trimmedPrefix.Trim();
            }

            this.RegisterContinue(location, continuationTokenExtractionInfo, trimmedPrefix);
            this.RegisterEnd(location);
        }
    }
}
