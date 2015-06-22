// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Directive.cs" company="Solidsoft Reply Ltd.">
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
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SolidsoftReply.Esb.Libraries.Resolution
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Xml;

    using Microsoft.BizTalk.Bam.EventObservation;

    using SolidsoftReply.Esb.Libraries.Resolution.Dictionaries;
    using SolidsoftReply.Esb.Libraries.Resolution.Properties;
    using SolidsoftReply.Esb.Libraries.Resolution.ResolutionService;

    using Directive = SolidsoftReply.Esb.Libraries.Resolution.ResolutionService.DirectivesDictionaryItemValueDirective;

    /// <summary>
    /// Class representing the item on the output list
    /// </summary>
    [ComVisible(true)]
    [Serializable]
    public class Directive : IDisposable
    {
        /// <summary>
        /// A directive returned from the resolver
        /// </summary>
        private readonly DirectivesDictionaryItemValueDirective directive;

        /// <summary>
        /// A lock for a BAM buffered event stream.
        /// </summary>
        private readonly object eventStreamLock = new object();

        /// <summary>
        /// The type of a map.
        /// </summary>
        private Type mapType;

        /// <summary>
        /// A BAM buffered event stream.
        /// </summary>
        private volatile EventStream eventStream;

        /// <summary>
        /// Indicates whether the event stream has been set manually.
        /// </summary>
        private bool eventStreamSet;

        /// <summary>
        /// The dictionary of BTS property values.
        /// </summary>
        private BtsProperties btsProperties;

        /// <summary>
        /// The dictionary of property values.
        /// </summary>
        private Dictionaries.Properties properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution.Directive"/> class. 
        /// </summary>
        public Directive()
        {
            ////////this.bamInterceptorResolverList = new SerializableDictionary<string, BamStepResolver>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution.Directive"/> class. 
        /// </summary>
        /// <param name="directive">
        /// The directive.
        /// </param>
        public Directive(DirectivesDictionaryItemValueDirective directive)
        {
            //////////this.bamInterceptorResolverList = new SerializableDictionary<string, BamStepResolver>();

            if (directive != null)
            {
                this.directive = directive;
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Resolution.Directive"/> class. 
        /// </summary>
        ~Directive()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the name of directive used as a key in the policy.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.KeyName;
            }
        }

        #region Category: Endpoint

        /// <summary>
        /// Gets or sets the message endpoint.
        /// </summary>
        public virtual string EndPoint
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.EndPoint;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.EndPoint = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the transport type.
        /// </summary>
        public virtual string TransportType
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.TransportType;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.TransportType = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the end point configuration token.
        /// </summary>
        public virtual string EndPointConfiguration
        {
            get
            {
                return
                    (this.directive == null ? string.Empty : this.directive.EndPointConfiguration ?? string.Empty)
                        .DecodeFromBase64();
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.EndPointConfiguration = (value ?? string.Empty).EncodeToBase64();
                }
            }
        }

        /// <summary>
        /// Gets or sets a URI indicating the intent of the SOAP operation.
        /// </summary>
        public virtual string SoapAction
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.SoapAction;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.SoapAction = value;
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the full name of a map.
        /// </summary>
        public virtual string MapFullName
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.MapToApply;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.MapToApply = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the map to apply. 
        /// </summary>
        public virtual Type MapType
        {
            get
            {
                if (this.mapType != null)
                {
                    return this.mapType;
                }

                this.mapType = this.directive == null ? null : Type.GetType(this.directive.MapToApply);
                return this.mapType;
            }

            set
            {
                this.mapType = value;
            }
        }

        /// <summary>
        /// Gets a collection of strong names for the map target schemas. 
        /// </summary>
        /// <remarks>This property is only set after a map has been executed.</remarks>
        public virtual IEnumerable<string> MapTargetSchemaStrongNames { get; private set; }

        /// <summary>
        /// Gets or sets the name of the BAM activity to which this directive applies.
        /// </summary>
        public virtual string BamActivity
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.BamActivity;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.BamActivity = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the step within the BAM activity to which this directive applies.
        /// </summary>
        public virtual string BamStepName
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.BamStepName;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.BamStepName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a list of steps that extend the step specified in the StepName property.
        /// </summary>
        public virtual IList<string> BamStepExtensions
        {
            // NB. The type is List<string> rather than IList<string> in order to be serialisable.
            get
            {
                return this.directive == null ? new List<string>() : this.directive.BamStepExtensions.ToList();
            }

            set
            {
                this.directive.BamStepExtensions = value.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the name of the current step extension within the BAM activity 
        /// to which this directive applies.
        /// </summary>
        public virtual string BamRootStepName { get; set; }

        /// <summary>
        /// Gets or sets the name of the step within the BAM activity to which this directive applies.
        /// </summary>
        public virtual string BamAfterMapStepName
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.BamAfterMapStepName;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.BamAfterMapStepName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that determines under what conditions the buffered 
        /// data will be sent to the tracking database.
        /// </summary>
        /// <remarks>
        /// &lt;= 0 This value is not allowed.   If set to 0, the eventStream 
        ///         would never flush automatically and the application would have to
        ///         call the Flush method explicitly.   There is no obvious way to do
        ///         this in most common resolution scenarios
        /// 1       Each event will be immediately persisted in the BAM database. 
        /// &gt; 1  The eventStream will accumulate the events in memory until the 
        ///         event count equals or exceeds this threshold; at this point, the Flush 
        ///         method will be called internally. 
        /// </remarks>
        public virtual int BamFlushThreshold
        {
            get
            {
                return this.directive == null ? 1 : this.directive.BamFlushThreshold;
            }

            set
            {
                if (this.directive == null)
                {
                    return;
                }

                this.FlushAndReleaseEventBuffers();
                this.directive.BamFlushThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string for BAM.
        /// </summary>
        public virtual string BamConnectionString
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.BamConnectionString;
            }

            set
            {
                if (this.directive == null)
                {
                    return;
                }

                this.FlushAndReleaseEventBuffers();
                this.directive.BamConnectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether BAM will use a buffered event stream.
        /// </summary>
        public virtual bool BamIsBuffered
        {
            get
            {
                return this.directive == null || this.directive.BamIsBuffered;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.BamIsBuffered = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the BAM Trackpoint policy name.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public virtual string BamTrackpointPolicyName
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.BamTrackpointPolicyName;
            }

            set
            {
                if (this.directive == null)
                {
                    return;
                }

                this.FlushAndReleaseEventBuffers();
                this.directive.BamTrackpointPolicyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the BAM Trackpoint policy bamTrackpointVersion.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public virtual string BamTrackpointPolicyVersion
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.BamTrackpointPolicyVersion;
            }

            set
            {
                if (this.directive == null)
                {
                    return;
                }

                this.FlushAndReleaseEventBuffers();
                this.directive.BamTrackpointPolicyVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the validation policy name.
        /// </summary>
        public virtual string ValidationPolicyName
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.ValidationPolicyName;
            }

            set
            {
                if (this.directive == null)
                {
                    return;
                }

                this.directive.ValidationPolicyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the validation policy version.
        /// </summary>
        public virtual string ValidationPolicyVersion
        {
            get
            {
                return this.directive == null ? string.Empty : this.directive.ValidationPolicyVersion;
            }

            set
            {
                if (this.directive == null)
                {
                    return;
                }

                this.directive.ValidationPolicyVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to throw an error if a validation rule policy indicates invalidity.
        /// </summary>
        public virtual bool ErrorOnInvalid
        {
            get
            {
                return this.directive == null || this.directive.ErrorOnInvalid;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.ErrorOnInvalid = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the a level indicator for the retry
        /// </summary>
        /// <remarks>
        /// Retries must sometimes be carried out at different levels.   For example, we may want to
        /// carry out 3 retries at a minute interval at level 0, and then wrap this in an outer
        /// loop that retries 5 times each hour (level 1).
        /// </remarks>
        public virtual int RetryLevel
        {
            get
            {
                return this.directive == null ? 0 : this.directive.RetryLevel;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.RetryLevel = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Retry Level has been specified
        /// </summary>
        public virtual bool RetryLevelSpecified
        {
            get
            {
                return this.directive != null && this.directive.RetryLevelSpecified;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.RetryLevelSpecified = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of retries for the current level.
        /// </summary>
        public virtual int RetryCount
        {
            get
            {
                return this.directive == null ? 0 : this.directive.RetryCount;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.RetryCount = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Retry Count has been specified
        /// </summary>
        public virtual bool RetryCountSpecified
        {
            get
            {
                return this.directive != null && this.directive.RetryCountSpecified;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.RetryCountSpecified = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the interval between retries.
        /// </summary>
        /// <remarks>
        /// Resolver clients are free
        /// to interpret this using any unit of time, but minutes is recommended
        /// (in keeping with BizTalk Send Ports).
        /// </remarks>
        public virtual int RetryInterval
        {
            get
            {
                return this.directive == null ? 0 : this.directive.RetryInterval;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.RetryInterval = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Retry Interval has been set
        /// </summary>
        public virtual bool RetryIntervalSpecified
        {
            get
            {
                return this.directive != null && this.directive.RetryIntervalSpecified;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.RetryIntervalSpecified = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the time at which service window opens.
        /// </summary>
        public virtual DateTime ServiceWindowStartTime
        {
            get
            {
                return this.directive == null ? DateTime.MinValue : this.directive.ServiceWindowStartTime;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.ServiceWindowStartTime = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Service Window start time has been specified
        /// </summary>
        public virtual bool ServiceWindowStartTimeSpecified
        {
            get
            {
                return this.directive != null && this.directive.ServiceWindowStartTimeSpecified;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.ServiceWindowStartTimeSpecified = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the time at which service window closes. 
        /// </summary>
        public virtual DateTime ServiceWindowStopTime
        {
            get
            {
                return this.directive == null ? DateTime.MinValue : this.directive.ServiceWindowStopTime;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.ServiceWindowStopTime = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Service Window stop time has been specified.
        /// </summary>
        public virtual bool ServiceWindowStopTimeSpecified
        {
            get
            {
                return this.directive != null && this.directive.ServiceWindowStopTimeSpecified;
            }

            set
            {
                if (this.directive != null)
                {
                    this.directive.ServiceWindowStopTimeSpecified = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current time is within the specified time service window.
        /// </summary>
        public virtual bool InServiceWindow
        {
            get
            {
                if (this.directive == null || !this.directive.DirectiveCategories.Contains("ServiceWindow"))
                {
                    return true;
                }

                if (!this.directive.ServiceWindowStartTimeSpecified || !this.directive.ServiceWindowStopTimeSpecified)
                {
                    return true;
                }

                // Get times
                var currentTime = DateTime.Now.TimeOfDay;
                var startTime = this.directive.ServiceWindowStartTime.TimeOfDay;
                var stopTime = this.directive.ServiceWindowStopTime.TimeOfDay;

                return (currentTime >= startTime) && (currentTime < stopTime);
            }
        }

        /// <summary>
        /// Gets or sets the BizTalk Server property values.
        /// </summary>
        public virtual BtsProperties BtsProperties
        {
            get
            {
                if (this.btsProperties != null)
                {
                    return this.btsProperties;
                }

                if (this.directive == null || this.directive.BtsProperties == null)
                {
                    return null;
                }

                this.btsProperties = new BtsProperties();

                foreach (var btsProperty in this.directive.BtsProperties)
                {
                    var newBtsProperty = new BtsProperty
                                                  {
                                                      Promoted =
                                                          btsProperty.Value.BtsProperty
                                                          .Promoted,
                                                      Name =
                                                          btsProperty.Value.BtsProperty
                                                          .Name,
                                                      Namespace =
                                                          btsProperty.Value.BtsProperty
                                                          .Namespace,
                                                      Value =
                                                          btsProperty.Value.BtsProperty
                                                          .Value
                                                  };

                    this.btsProperties.Add(btsProperty.Key.@string, newBtsProperty);
                }

                return this.btsProperties;
            }

            set
            {
                var btsPropertiesCurrent = new DirectivesDictionaryItemValueDirectiveItem1[value.Count];
                var index = 0;

                foreach (var btsProperty in value)
                {
                    btsPropertiesCurrent[index++] = new DirectivesDictionaryItemValueDirectiveItem1
                                                     {
                                                         Key =
                                                             new DirectivesDictionaryItemValueDirectiveItemKey1
                                                                 {
                                                                     @string
                                                                         =
                                                                         btsProperty
                                                                         .Key
                                                                 },
                                                         Value =
                                                             new DirectivesDictionaryItemValueDirectiveItemValue1
                                                                 {
                                                                     BtsProperty
                                                                         =
                                                                         new BtsPropertyType
                                                                             {
                                                                                 Promoted
                                                                                     =
                                                                                     btsProperty
                                                                                     .Value
                                                                                     .Promoted,
                                                                                 Name
                                                                                     =
                                                                                     btsProperty
                                                                                     .Value
                                                                                     .Name,
                                                                                 Namespace
                                                                                     =
                                                                                     btsProperty
                                                                                     .Value
                                                                                     .Namespace,
                                                                                 Value
                                                                                     =
                                                                                     btsProperty
                                                                                     .Value
                                                                                     .Value
                                                                             }
                                                                 }
                                                     };
                }
            }
        }

        /// <summary>
        /// Gets or sets the BizTalk Server property values.
        /// </summary>
        public virtual Dictionaries.Properties Properties
        {
            get
            {
                if (this.properties != null)
                {
                    return this.properties;
                }

                if (this.directive == null || this.directive.Properties == null)
                {
                    return null;
                }

                this.properties = new Dictionaries.Properties();

                foreach (var property in this.directive.Properties)
                {
                    var newProperty = new Property
                                                  { 
                                                      Name = property.Value.Property.Name,
                                                      Value = property.Value.Property.Value
                                                  };

                    this.properties.Add(property.Key.@string, newProperty);
                }

                return this.properties;
            }

            set
            {
                var newProperties = new DirectivesDictionaryItemValueDirectiveItem[value.Count];
                var index = 0;

                foreach (var property in value)
                {
                    newProperties[index++] = new DirectivesDictionaryItemValueDirectiveItem
                    {
                        Key =
                            new DirectivesDictionaryItemValueDirectiveItemKey
                            {
                                @string
                                    =
                                    property
                                    .Key
                            },
                        Value =
                            new DirectivesDictionaryItemValueDirectiveItemValue
                            {
                                Property
                                    =
                                    new PropertyType
                                    {
                                        Name
                                            =
                                            property
                                            .Value
                                            .Name,
                                        Value
                                            =
                                            property
                                            .Value
                                            .Value
                                    }
                            }
                    };
                }
            }
        }

        /// <summary>
        /// Gets or sets the BAM event stream.
        /// </summary>
        public virtual EventStream EventStream
        {
            get
            {
                lock (this.eventStreamLock)
                {
                    if (this.eventStreamSet)
                    {
                        return this.eventStream;
                    }

                    if (this.directive == null)
                    {
                        return null;
                    }

                    return this.eventStream ?? new DirectiveEventStream(this);
                }
            }

            set
            {
                lock (this.eventStreamLock)
                {
                    this.eventStream = value;
                    this.eventStreamSet = true;
                }
            }
        }

        /// <summary>
        /// Resets the event stream.  If the event stream has previously been set, it will
        /// revert to an event stream specified by the directive.
        /// </summary>
        public virtual void ResetEventStream()
        {
            lock (this.eventStreamLock)
            {
                this.eventStreamSet = false;

                if (this.directive == null)
                {
                    return;
                }

                this.eventStream = new DirectiveEventStream(this);
            }
        }

        /// <summary>
        /// Transform the message by applying a map.
        /// </summary>
        /// <param name="messageIn">
        /// The inbound message.
        /// </param>
        /// <returns>
        /// A transformed XML document.
        /// </returns>
        public virtual XmlDocument Transform(XmlDocument messageIn)
        {
            XmlDocument transformedMessage;

            if (this.directive == null)
            {
                return messageIn;
            }

            if (this.directive.DirectiveCategories.Contains("Transformation"))
            {
                if (messageIn != null)
                {
                    var transformResults = Transformer.Transform(this.directive.MapToApply, messageIn);

                    if (transformResults.TransformedDocument == null)
                    {
                        transformedMessage = messageIn;
                    }
                    else
                    {
                        this.MapTargetSchemaStrongNames = transformResults.TargetSchemaStrongNames;
                        transformedMessage = transformResults.TransformedDocument;
                    }
                }
                else
                {
                    throw new EsbResolutionException(string.Format(Resources.ExceptionXmlTransformationFailedOnNull, this.directive.KeyName));
                }
            }
            else
            {
                transformedMessage = messageIn;
            }

            return transformedMessage;
        }

        /// <summary>
        /// Transforms the aggregation of two XML messages by applying a map.
        /// </summary>
        /// <param name="messageIn1">
        /// The first inbound message.
        /// </param>
        /// <param name="messageIn2">
        /// The second inbound message.
        /// </param>
        /// <returns>
        /// A transformed XML document.
        /// </returns>
        public virtual XmlDocument Transform(XmlDocument messageIn1, XmlDocument messageIn2)
        {
            XmlDocument transformedMessage;

            if (this.directive == null)
            {
                return messageIn1;
            }

            if (this.directive.DirectiveCategories.Contains("Transformation"))
            {
                if (messageIn1 != null && messageIn2 != null)
                {
                    var transformResults = Transformer.Transform(this.directive.MapToApply, messageIn1, messageIn2);
                    transformedMessage = transformResults.TransformedDocument ?? messageIn1;
                }
                else
                {
                    throw new EsbResolutionException(string.Format(Resources.ExceptionXmlTransformationFailedOnNull, this.directive.KeyName));
                }
            }
            else
            {
                transformedMessage = messageIn1;
            }

            return transformedMessage;
        }

        /// <summary>
        /// Transform the message by applying a map.   Invoke BAM interception as required.
        /// </summary>
        /// <param name="messageIn">
        /// The inbound message.
        /// </param>
        /// <returns>
        /// A transformed XML document.
        /// </returns>
        public virtual XmlDocument TransformWithInterception(XmlDocument messageIn)
        {
            return this.DoTransformWithInterception(messageIn, null);
        }

        /// <summary>
        /// Transform the message by applying a map.   Invoke BAM interception as required.
        /// </summary>
        /// <param name="messageIn">
        /// The inbound message.
        /// </param>
        /// <param name="messageProperties">A dictionary of message properties.</param>
        /// <returns>
        /// A transformed XML document.
        /// </returns>
        public virtual XmlDocument TransformWithInterception(XmlDocument messageIn, IDictionary messageProperties)
        {
            return this.DoTransformWithInterception(messageIn, messageProperties);
        }

        /// <summary>
        /// Retrieves data for a particular step of a BAM activity. Call this method on every 
        /// step in which some data may be needed for BAM - e.g., at the point a service is called,
        /// or at the point of resolution.
        /// </summary>
        /// <param name="data">The BAM step data.</param>
        public virtual void OnStep(BamStepData data)
        {
            this.DoOnStep(data.XmlDocument, data.Properties, data.ValueList, false);
        }

        /// <summary>
        /// Retrieves data for a particular step of a BAM activity. Call this method on every 
        /// step in which some data may be needed for BAM - e.g., at the point a service is called,
        /// or at the point of resolution.
        /// </summary>
        /// <param name="data">The BAM step data.</param>
        /// <param name="afterMap">Indicates if the step is after the application of a map.</param>
        public virtual void OnStep(BamStepData data, bool afterMap)
        {
            this.DoOnStep(data.XmlDocument, data.Properties, data.ValueList, afterMap);
        }

        /// <summary>
        /// Selects a BAM step extension.
        /// </summary>
        /// <param name="directiveEventStream">A track point directive event stream.</param>
        /// <param name="stepExtensionName">The step extension</param>
        /// <remarks>
        /// This is a form of continuation in which the continuation is automatically managed by 
        /// selecting a BAM step extension.
        /// </remarks>
        public virtual void SelectBamStepExtension(TrackpointDirectiveEventStream directiveEventStream, string stepExtensionName)
        {
            if (directiveEventStream == null)
            {
                throw new EsbResolutionException(
                    Resources.ExceptionMissingEventStreamOnSelectBamStepExtension);
            }
 
            if (string.IsNullOrWhiteSpace(stepExtensionName))
            {
                throw new EsbResolutionException(
                    Resources.ExceptionInvalidBamStepExtensionNameOnSelectBamStepExtension);
            }

            if (!this.BamStepExtensions.Contains(stepExtensionName))
            {
                throw new EsbResolutionException(
                    Resources.ExceptionUnregisteredBamStepExtensionNameOnSelectBamStepExtension);
            }

            if (string.IsNullOrWhiteSpace(this.BamStepName))
            {
                throw new EsbResolutionException(
                    Resources.ExceptionInvalidBamStepNameOnSelectBamStepExtension);
            }

            // Manufacture the continuation token for extending the current step. The prefix is 
            // tokenised by replacing each individual whitespace character with an underscore, rather 
            // than grouping whitespace characters.  This handles situations where a developer decides 
            // to treat whitespace as significant. The token follows a pattern that includes the 
            // exteded step name and a counter.  The counter is included to support situations where
            // the developer uses the same directive moe than once in a continuation chain.
            var extendsPrefixToken = "extends_";
            var tokenisedStepName = Regex.Replace(
                this.BamStepName,
                @"\s", 
                "_");
            var counter = 0L;
            var extendsToken = string.Format(
                "{0}_{1}_{2}_{3}",
                extendsPrefixToken,
                tokenisedStepName,
                ++counter,
                directiveEventStream.CurrentBamActivityId);

            // If the current token 
            if (directiveEventStream.CurrentBamActivityId.StartsWith(extendsPrefixToken))
            {
                var tokenWithoutPrefix =
                    directiveEventStream.CurrentBamActivityId.Substring(
                        extendsPrefixToken.Length + tokenisedStepName.Length);
                var match = Regex.Match(tokenWithoutPrefix, @"^_(\d+)_");

                if (match.Success)
                {
                    var activityId = tokenWithoutPrefix.Substring(match.Length);

                    try
                    {
                        counter = Convert.ToInt64(match.Groups[1].Value);
                    }
                    catch
                    {
                        activityId = tokenWithoutPrefix;
                    }

                    extendsToken = string.Format(
                        "{0}_{1}_{2}_{3}",
                        extendsPrefixToken,
                        tokenisedStepName,
                        counter,
                        activityId);
                }
            }

            // Enable continuation on the current BAM step using the manufactured prefix and current activity ID.
            // Then end the activity.
            var activityInstanceKey = string.Format(
                "{0}#{1}#{2}",
                this.BamActivity,
                this.BamStepName,
                directiveEventStream.CurrentBamActivityId);
            directiveEventStream.EnableContinuation(this.BamActivity, directiveEventStream.ActivityInstances[activityInstanceKey], extendsToken);
            directiveEventStream.EndActivity();

            // Record the root step name for reset.
            if (string.IsNullOrWhiteSpace(this.BamRootStepName))
            {
                this.BamRootStepName = this.BamStepName;
            }

            // Use the step extension as the new step name
            this.BamStepName = stepExtensionName;

            // Reset the current activity ID
            directiveEventStream.CurrentBamActivityId = extendsToken;

            // Continue the  activity
            directiveEventStream.ContinueActivity(false, null);
        }

        /// <summary>
        /// Returns a BTS property
        /// </summary>
        /// <param name="name">Name of BTS property (e.g., BTS.MessageType)</param>
        /// <returns>An object that provides a full description of a BTS property</returns>
        public virtual DirectivesDictionaryItemValueDirectiveItemValue1 GetBtsProperty(string name)
        {
            return this.directive == null ? null : (from item in this.directive.BtsProperties where item.Key.@string == name select item.Value).FirstOrDefault();
        }

        /// <summary>
        /// Returns the value of a BTS property
        /// </summary>
        /// <param name="name">Name of BTS property (e.g., BTS.MessageType)</param>
        /// <returns>The value of the BTS property</returns>
        public virtual string GetBtsPropertyValue(string name)
        {
            if (this.directive == null)
            {
                return string.Empty;
            }

            foreach (var item in this.directive.BtsProperties.Where(item => item.Key.@string == name))
            {
                return item.Value.BtsProperty.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the element name for a BTS property
        /// </summary>
        /// <param name="name">Name of BTS property (e.g., BTS.MessageType)</param>
        /// <returns>The element name of the BTS property.</returns>
        public virtual string GetBtsPropertyName(string name)
        {
            if (this.directive == null)
            {
                return string.Empty;
            }

            foreach (var item in this.directive.BtsProperties.Where(item => item.Key.@string == name))
            {
                return item.Value.BtsProperty.Name;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the namespace for a BTS property
        /// </summary>
        /// <param name="name">Name of BTS property (e.g., BTS.MessageType)</param>
        /// <returns>The namespace of the BTS property.</returns>
        public virtual string GetBtsPropertyNamespace(string name)
        {
            if (this.directive == null)
            {
                return string.Empty;
            }

            foreach (var item in this.directive.BtsProperties.Where(item => item.Key.@string == name))
            {
                return item.Value.BtsProperty.Namespace;
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the 'promoted' flag for a BTS property
        /// </summary>
        /// <param name="name">Name of BTS property (e.g., BTS.MessageType)</param>
        /// <returns>True, if marked as promoted</returns>
        public virtual bool IsPromotedBtsProperty(string name)
        {
            return this.directive != null && (from item in this.directive.BtsProperties where item.Key.@string == name select item.Value.BtsProperty.Promoted).FirstOrDefault();
        }

        /// <summary>
        /// Returns a general property.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <returns>An object that provides full details of the property.</returns>
        public virtual DirectivesDictionaryItemValueDirectiveItemValue GetProperty(string name)
        {
            return this.directive == null ? null : (from item1 in this.directive.Properties where item1.Key.@string == name select item1.Value).FirstOrDefault();
        }

        /// <summary>
        /// Returns the value of a general property.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The value of the named property.</returns>
        public virtual string GetPropertyValue(string name)
        {
            if (this.directive == null)
            {
                return string.Empty;
            }

            foreach (var item1 in this.directive.Properties)
            {
                Debug.WriteLine("Processing property " + item1.Key.@string);
                Debug.WriteLine("Property name " + item1.Value.Property.Name);
                Debug.WriteLine("Property value " + item1.Value.Property.Value);

                if (item1.Key.@string == name)
                {
                    return item1.Value.Property.Value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Disposes the current object.
        /// </summary>
        public void Dispose()
        {
            // dispose of resources
            this.Dispose(true);

            // tell the GC that the Finalize process no longer needs
            // to be run for this object.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the current object.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if the current object has been disposed.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Ensure that buffers are always flushed 
            this.FlushAndReleaseEventBuffers();
        }

        /// <summary>
        /// Transform the message by applying a map.   Invoke BAM interception as required.
        /// </summary>
        /// <param name="messageIn">
        /// The inbound message.
        /// </param>
        /// <param name="messageProperties">A dictionary of message properties.</param>
        /// <returns>
        /// A transformed XML document.
        /// </returns>
        private XmlDocument DoTransformWithInterception(XmlDocument messageIn, IDictionary messageProperties)
        {
            if (this.directive == null)
            {
                return messageIn;
            }

            // [var] A value indicating whether interception is required.
            var doInterception = this.directive.DirectiveCategories.Contains("BamInterception");

            // Perform any required pre-transformation interception.
            if (doInterception && !string.IsNullOrEmpty(this.directive.BamStepName))
            {
                var stepDataBefore = new BamStepData { XmlDocument = messageIn, Properties = messageProperties };
                this.OnStep(stepDataBefore);
            }

            // [var] The transformed message
            var transformedMessage = new XmlDocument();

            // If there is no XML content, then we cannot perform any transformation
            if (messageIn != null && messageIn.HasChildNodes)
            {
                // We will only transform and do a post-transformation BAM interception if a 
                // transformation has been defined.
                if (!this.directive.DirectiveCategories.Contains("Transformation"))
                {
                    return messageIn;
                }

                transformedMessage = this.Transform(messageIn);
            }

            // Return the transformed message if no post-transformation interception is required.
            if (!doInterception || string.IsNullOrEmpty(this.directive.BamAfterMapStepName))
            {
                return transformedMessage;
            }

            // Perform post-transformation interception
            var stepDataAfter = new BamStepData
                                    {
                                        XmlDocument =
                                            transformedMessage.HasChildNodes
                                                ? transformedMessage
                                                : messageIn,
                                        Properties = messageProperties
                                    };
            this.OnStep(stepDataAfter, true);

            // Return the transformed message
            return transformedMessage;
        }

        /// <summary>
        /// Retrieves data for a particular step of a BAM activity. Call this method on every 
        /// step in which some data may be needed for BAM - e.g., at the point a service is called,
        /// or at the point of resolution.
        /// </summary>
        /// <param name="xmlMsg">The message containing data.</param>
        /// <param name="messageProperties">A dictionary of message properties.</param>
        /// <param name="values">A positional array of additional values that can be recorded by BAM.</param>
        /// <param name="afterMap">Indicates if the step is after the application of a map.</param>
        private void DoOnStep(XmlNode xmlMsg, IDictionary messageProperties, IList values, bool afterMap)
        {
            if (this.directive == null || !this.directive.DirectiveCategories.Contains("BamInterception"))
            {
                return;
            }

            EventStream currentEventStream;

            lock (this.eventStreamLock)
            {
                if (this.eventStream == null)
                {
                    this.eventStream = new DirectiveEventStream(this);
                }

                currentEventStream = this.eventStream;
            }

            // Get a BAM interceptor for the specified activity
            if (string.IsNullOrEmpty(this.directive.BamActivity))
            {
                throw new EsbResolutionException(
                    string.Format(Resources.ExceptionNoActivity, this.directive.KeyName));
            }

            var bamStepName = afterMap ? this.directive.BamAfterMapStepName : this.directive.BamStepName;

            if (string.IsNullOrEmpty(bamStepName))
            {
                throw new EsbResolutionException(
                    string.Format(Resources.ExceptionNoActivityStep, this.directive.KeyName));
            }

            var bamStepResolver = new BamStepResolver();
            Version bamTrackpointPolicyVersion = null;

            if (!string.IsNullOrWhiteSpace(this.directive.BamTrackpointPolicyVersion))
            {
                if (!Version.TryParse(this.directive.BamTrackpointPolicyVersion, out bamTrackpointPolicyVersion))
                {
                    throw new EsbResolutionException(
                        string.Format(Resources.ExceptionBamTrackpointPolicyVersionInvalid, this.directive.BamTrackpointPolicyVersion));
                }
            }

            var bamInterceptor = bamStepResolver.GetInterceptor(this.directive.BamActivity, bamStepName, this.directive.BamTrackpointPolicyName, bamTrackpointPolicyVersion);

            if (bamInterceptor != null)
            {
                var xpathDataExtractorWithMacros = new TrackpointDirectiveEventStream.XPathDataExtractorWithMacros(messageProperties, values);
                bamInterceptor.OnStep(xpathDataExtractorWithMacros, bamStepName, xmlMsg, currentEventStream);
            }
            else
            {
                throw new EsbResolutionException(
                    string.Format(Resources.ExceptionInterceptorNotObtained, this.directive.BamActivity, this.directive.KeyName));
            }
        }

        /// <summary>
        /// Flush BAM event buffers and release by setting to null.
        /// </summary>
        private void FlushAndReleaseEventBuffers()
        {
            // Double-checking should work OK in .NET on x86
            // architecture.   Some doubt about whether it is
            // reliable on 64-bit architectures, but I believe
            // it is OK.
            if (this.eventStream == null)
            {
                return;
            }

            lock (this.eventStreamLock)
            {
                if (this.eventStream == null)
                {
                    return;
                }

                this.eventStream.Flush();
                this.eventStream = null;
            }
        }

        /// <summary>
        /// Structure representing a BTS property name-value pair with namespace.
        /// </summary>
        [Serializable]
        public struct BtsProperty
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BtsProperty"/> struct. 
            /// </summary>
            /// <param name="name">
            /// BTS name of property.
            /// </param>
            /// <param name="value">
            /// Value of property.
            /// </param>
            /// <param name="namespace">
            /// XML namespace of property.
            /// </param>
            /// <param name="promoted">
            /// Flag indicating if property should be promoted.
            /// </param>
            public BtsProperty(string name, string value, string @namespace, bool promoted)
                : this()
            {
                this.Name = name;
                this.Value = value;
                this.Namespace = @namespace;
                this.Promoted = promoted;
            }

            /// <summary>
            /// Gets or sets the name of BTS property.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the value of BTS property.
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets the XML namespace of BTS property.
            /// </summary>
            public string Namespace { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the property should be marked as promoted on BizTalk messages.
            /// </summary>
            public bool Promoted { get; set; }
        }

        /// <summary>
        /// Structure representing a general purpose property name-value pair.
        /// </summary>
        [Serializable]
        public struct Property
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Property"/> struct. 
            /// </summary>
            /// <param name="name">
            /// Name of property.
            /// </param>
            /// <param name="value">
            /// Value of property.
            /// </param>
            public Property(string name, string value)
                : this()
            {
                this.Name = name;
                this.Value = value;
            }

            /// <summary>
            /// Gets or sets the name of property.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the value of property.
            /// </summary>
            public string Value { get; set; }
        }
    }
}