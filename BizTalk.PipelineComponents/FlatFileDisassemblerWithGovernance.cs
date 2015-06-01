﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlatFileDisassemblerWithGovernance.cs" company="Solidsoft Reply Ltd.">
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
//   limitations under the License. 뱈 D 㚸⯹᐀耀//   Copyright 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Resources;
    using System.Runtime.InteropServices;

    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Component.Utilities;
    using Microsoft.BizTalk.Message.Interop;
    using SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties;
    using SolidsoftReply.Esb.Libraries.Resolution;

    /// <summary>
    /// Implements ESB governance in the context of the Flat File Disassembler pipeline component.
    /// </summary>
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_DisassemblingParser)]
    [ComponentCategory(CategoryTypes.CATID_Streamer)]
    [Guid("5EEEF32A-5601-4D8D-B2DA-D939484F08E7")]
    public class FlatFileDisassemblerWithGovernance : BaseCustomTypeDescriptor, IBaseComponent, IPersistPropertyBag, IComponentUI, IDisassemblerComponent
    {
        /// <summary>
        /// The resource manager.
        /// </summary>
        private static readonly ResourceManager ResourceManager;

        /// <summary>
        /// The Flat File disassember component.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private readonly FFDasmComp flatFileDasmComp;

        /// <summary>
        /// Instance of Flat File Disassembler
        /// </summary>
        private Governance governanceDasm;

        /// <summary>
        /// The current flat file message.
        /// </summary>
        private IBaseMessage currentFlatFileMessage;

        /// <summary>
        /// Initializes static members of the <see cref="FlatFileDisassemblerWithGovernance"/> class.
        /// </summary>
        static FlatFileDisassemblerWithGovernance()
        {
            ResourceManager = new ResourceManager("SolidsoftReply.Esb.Libraries.BizTalk.PipelineComponents.Properties.Resources", typeof(FlatFileDisassemblerWithGovernance).Assembly);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatFileDisassemblerWithGovernance"/> class.
        /// </summary>
        public FlatFileDisassemblerWithGovernance()
            : base(ResourceManager)
        {
            this.governanceDasm = new Governance();
            this.flatFileDasmComp = new FFDasmComp();
        }

        /// <summary>
        /// Gets or sets an identifier for a binding access point.   The identifier should  be
        /// in the form of a URL.   This may be used when the endpoint URL is known,
        /// but other resolution is required, or as a 'virtual' URL.
        /// This is based on UDDI.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescBindingAccessPoint")]
        [BtsPropertyName("PropBindingAccessPoint")]
        public string BindingAccessPoint
        {
            get
            {
                return this.governanceDasm.BindingAccessPoint;
            }

            set
            {
                this.governanceDasm.BindingAccessPoint = value;
            }
        }

        /// <summary>
        /// Gets or sets the type (scheme) of URL for the target service.   This is based on
        /// UDDI.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescBindingUrlType")]
        [BtsPropertyName("PropBindingUrlType")]
        public string BindingUrlType
        {
            get
            {
                return this.governanceDasm.BindingUrlType;
            }

            set
            {
                this.governanceDasm.BindingUrlType = value;
            }
        }

        /// <summary>
        /// Gets or sets an XPath that addresses the element that contains the body of the message.
        /// </summary>
        /// <remarks>
        /// If blank, the conceptual 'Root' node (parent of the Document Element) is used.
        /// </remarks>
        [Browsable(true)]
        [BtsDescription("DescBodyContainerXPath")]
        [BtsPropertyName("PropBodyContainerXPath")]
        public string BodyContainerXPath
        {
            get
            {
                return this.governanceDasm.BodyContainerXPath;
            }

            set
            {
                this.governanceDasm.BodyContainerXPath = value;
            }
        }

        /// <summary>
        /// Gets the description of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescFFDasmDesription")]
        [BtsPropertyName("PropFFDasmDesription")]
        public string Description
        {
            get
            {
                return Resources.FlatFileDasmGovernanceComponentDescription;
            }
        }

        /// <summary>
        /// Gets the component icon to use in BizTalk Editor.
        /// </summary>
        [Browsable(false)]
        public IntPtr Icon
        {
            get
            {
                return Resources.DisassemblerIcon.Handle;
            }
        }

        /// <summary>
        /// Gets or sets the direction of message.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescMessageDirection")]
        [BtsPropertyName("PropMessageDirection")]
        public MessageDirectionTypes MessageDirection
        {
            get
            {
                return this.governanceDasm.MessageDirection;
            }

            set
            {
                this.governanceDasm.MessageDirection = value;
            }
        }

        /// <summary>
        /// Gets or sets a role specifier for the message.   Equivalent to
        /// messageLabel in WSDL 2.0
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [Browsable(true)]
        [BtsDescription("DescMessageRole")]
        [BtsPropertyName("PropMessageRole")]
        public string MessageRole
        {
            get
            {
                return this.governanceDasm.MessageRole;
            }

            set
            {
                this.governanceDasm.MessageRole = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of message.   In a BizTalk context, this should generally be
        /// equivalent to the BTS.MessageType property.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescMessageType")]
        [BtsPropertyName("PropMessageType")]
        public string MessageType
        {
            get
            {
                return this.governanceDasm.MessageType;
            }

            set
            {
                this.governanceDasm.MessageType = value;
            }
        }

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescFFDasmName")]
        [BtsPropertyName("PropFFDasmName")]
        public string Name
        {
            get
            {
                return Resources.FlatFileDasmGovernanceComponentName;
            }
        }

        /// <summary>
        /// Gets or sets the name of the service operation to be invoked.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescOperationName")]
        [BtsPropertyName("PropOperationName")]
        public string OperationName
        {
            get
            {
                return this.governanceDasm.OperationName;
            }

            set
            {
                this.governanceDasm.OperationName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of policy to be executed.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescPolicy")]
        [BtsPropertyName("PropPolicy")]
        public string Policy
        {
            get
            {
                return this.governanceDasm.Policy;
            }

            set
            {
                this.governanceDasm.Policy = value;
            }
        }

        /// <summary>
        /// Gets or sets the version of Policy to be executed.   If empty, the
        /// latest version will be executed.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescPolicyVersion")]
        [BtsPropertyName("PropPolicyVersion")]
        public string PolicyVersion
        {
            get
            {
                return this.governanceDasm.PolicyVersion;
            }

            set
            {
                this.governanceDasm.PolicyVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the human-friendly name of service provider.   This is equivalent to
        /// the business entity name in UDDI.
        /// </summary>
        [Browsable(true), Category("Service"), Description(
            "Human-friendly name of service provider.   This is equivalent to the business entity name in UDDI.")]
        [BtsDescription("DescProviderName")]
        [BtsPropertyName("PropProviderName")]
        public string ProviderName
        {
            get
            {
                return this.governanceDasm.ProviderName;
            }

            set
            {
                this.governanceDasm.ProviderName = value;
            }
        }

        /// <summary>
        /// Gets or sets the human-friendly name for target service.   This is equivalent to
        /// the business service name in UDDI.
        /// </summary>
        [Browsable(true), Category("Service"), Description(
            "Human-friendly name for target service.   This is equivalent to the business service name in UDDI.")]
        [BtsDescription("DescServiceName")]
        [BtsPropertyName("PropServiceName")]
        public string ServiceName
        {
            get
            {
                return this.governanceDasm.ServiceName;
            }

            set
            {
                this.governanceDasm.ServiceName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the BAM event stream should be synchronized with
        /// the pipeline context.
        /// </summary>
        [Browsable(true)]
        [BtsDescription("DescSynchronizeBam")]
        [BtsPropertyName("PropSynchronizeBam")]
        public bool SynchronizeBam
        {
            get
            {
                return this.governanceDasm.SynchronizeBam;
            }

            set
            {
                this.governanceDasm.SynchronizeBam = value;
            }
        }

        /// <summary>
        /// Gets the version of the component.
        /// </summary>
        [Browsable(false)]
        [BtsDescription("DescFFDasmVersion")]
        [BtsPropertyName("PropFFDasmVersion")]
        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        /// <summary>
        /// Gets or sets the schema to be applied to the document.
        /// </summary>
        [BtsDescription("DescDocumentSpecName")]
        [BtsPropertyName("PropDocumentSpecName")]
        public SchemaWithNone DocumentSpecName
        {
            get
            {
                return this.flatFileDasmComp.DocumentSpecName;
            }

            set
            {
                this.flatFileDasmComp.DocumentSpecName = value;
            }
        }

        /// <summary>
        /// Gets or sets the header schema name that the flat file disassembler uses for parsing document headers. 
        /// </summary>
        [BtsDescription("DescHeaderSpecName")]
        [BtsPropertyName("PropHeaderSpecName")]
        public SchemaWithNone HeaderSpecName
        {
            get
            {
                return this.flatFileDasmComp.HeaderSpecName;
            }

            set
            {
                this.flatFileDasmComp.HeaderSpecName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to preserve the headers 
        /// of the documents on the message context.
        /// </summary>
        [BtsDescription("DescPreserveHeader")]
        [BtsPropertyName("PropPreserveHeader")]
        public bool PreserveHeader
        {
            get
            {
                return this.flatFileDasmComp.PreserveHeader;
            }

            set
            {
                this.flatFileDasmComp.PreserveHeader = value;
            }
        }

        /// <summary>
        /// Gets or sets the trailer schema name that the flat file disassembler uses for parsing document trailers.
        /// </summary>
        [BtsDescription("DescTrailerSpecName")]
        [BtsPropertyName("PropTrailerSpecName")]
        public SchemaWithNone TrailerSpecName
        {
            get
            {
                return this.flatFileDasmComp.TrailerSpecName;
            }

            set
            {
                this.flatFileDasmComp.TrailerSpecName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the structure of the parsed document.
        /// </summary>
        [BtsDescription("DescValidateDocumentStructure")]
        [BtsPropertyName("PropValidateDocumentStructure")]
        public bool ValidateDocumentStructure
        {
            get
            {
                return this.flatFileDasmComp.ValidateDocumentStructure;
            }

            set
            {
                this.flatFileDasmComp.ValidateDocumentStructure = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the disassembler will attempt to 
        /// recover from errors during interchange processing.
        /// </summary>
        [BtsDescription("DescRecoverableInterchangeProcessing")]
        [BtsPropertyName("PropRecoverableInterchangeProcessing")]
        public bool RecoverableInterchangeProcessing
        {
            get
            {
                return this.flatFileDasmComp.RecoverableInterchangeProcessing;
            }

            set
            {
                this.flatFileDasmComp.RecoverableInterchangeProcessing = value;
            }
        }

        /// <summary>
        /// Gets class ID of component for usage from unmanaged code.
        /// </summary>
        /// <param name="classid">
        /// Class ID of the component.
        /// </param>
        // ReSharper disable once InconsistentNaming
        public void GetClassID(out Guid classid)
        {
            classid = new Guid(Resources.FlatFileDasmGovernanceComponentClassId);
        }

        /// <summary>
        /// Processes the original message and invokes the resolver
        /// </summary>
        /// <param name="pipelineContext">
        /// The IPipelineContext containing the current pipeline context.
        /// </param>
        /// <param name="inMsg">
        /// The IBaseMessage containing the message to be disassembled.
        /// </param>
        public void Disassemble(IPipelineContext pipelineContext, IBaseMessage inMsg)
        {
            ////this.currentFlatFileMessage = inMsg;
            this.flatFileDasmComp.Disassemble(pipelineContext, inMsg);
        }

        /// <summary>
        /// The get next.
        /// </summary>
        /// <param name="pipelineContext">
        /// The pipeline context.
        /// </param>
        /// <returns>
        /// The <see cref="IBaseMessage"/>.
        /// </returns>
        public IBaseMessage GetNext(IPipelineContext pipelineContext)
        {
            while (true)
            {
                if (this.currentFlatFileMessage == null)
                {
                    this.currentFlatFileMessage = this.flatFileDasmComp.GetNext(pipelineContext);

                    if (this.currentFlatFileMessage == null)
                    {
                        break;
                    }

                    this.InitialiseGovernance();
                    this.governanceDasm.Disassemble(pipelineContext, this.currentFlatFileMessage);
                }

                var nextMsg = this.governanceDasm.GetNext(pipelineContext);

                if (nextMsg != null)
                {
                    return nextMsg;
                }

                this.currentFlatFileMessage = null;
            }

            return null;
        }

        /// <summary>
        /// Loads configuration property for component.
        /// </summary>
        /// <param name="pb">
        /// Configuration property bag.
        /// </param>
        /// <param name="errlog">
        /// Error status (not used in this code).
        /// </param>
        public void Load(IPropertyBag pb, int errlog)
        {
            // Let the Governance disassembler read its properties from the property bag.
            this.governanceDasm.Load(pb, errlog);

            // Let the Flat File disassembler read its properties from the property bag.
            this.flatFileDasmComp.Load(pb, errlog);
        }

        /// <summary>
        /// Saves the current component configuration into the property bag.
        /// </summary>
        /// <param name="pb">
        /// Configuration property bag.
        /// </param>
        /// <param name="clearDirty">
        /// The parameter is not used.
        /// </param>
        /// <param name="saveAllProperties">
        /// The parameter is not used.
        /// </param>
        public void Save(IPropertyBag pb, bool clearDirty, bool saveAllProperties)
        {
            // Let the Governance disassembler write its properties to the property bag.
            this.governanceDasm.Save(pb, clearDirty, saveAllProperties);

            // Let the Flat File disassembler write its properties to the property bag.
            this.flatFileDasmComp.Save(pb, clearDirty, saveAllProperties);
        }

        /// <summary>
        /// Initialize the disassembler component.
        /// </summary>
        public void InitNew()
        {
            // Let the Governance diassembler initialise.
            this.governanceDasm.InitNew();

            // Let the Flat File diassembler initialise.
            this.flatFileDasmComp.InitNew();
        }

        /// <summary>
        /// The Validate method is called by the BizTalk Editor during the build
        ///     of a BizTalk project.
        /// </summary>
        /// <param name="obj">
        /// Project system.
        /// </param>
        /// <returns>
        /// A list of error and/or warning messages encounter during validation
        ///     of this component.
        /// </returns>
        public IEnumerator Validate(object obj)
        {
            // Let the Governance diassembler initialise.
            var listEnumGovernance = this.governanceDasm.Validate(obj);

            // Let the Flat File diassembler initialise.
            var listEnumFlatFile = this.flatFileDasmComp.Validate(obj);

            var listOut = new ArrayList();

            // An enumerator for a concatenation of the validation message lists.
            Func<IEnumerator> enumeratorForConcatenatedLists = () =>
            {
                while (listEnumGovernance.MoveNext())
                {
                    listOut.Add(listEnumGovernance.Current);
                }

                while (listEnumFlatFile.MoveNext())
                {
                    listOut.Add(listEnumFlatFile.Current);
                }

                listEnumGovernance.Reset();
                listEnumFlatFile.Reset();

                return listOut.GetEnumerator();
            };

            return listEnumGovernance == null
                       ? listEnumFlatFile
                       : listEnumFlatFile == null ? listEnumGovernance : enumeratorForConcatenatedLists();
        }

        /// <summary>
        /// Initializes a new instance of the Governance component.
        /// </summary>
        private void InitialiseGovernance()
        {
            this.governanceDasm = new Governance
            {
                BindingAccessPoint = this.BindingAccessPoint,
                BindingUrlType = this.BindingUrlType,
                BodyContainerXPath = this.BodyContainerXPath,
                MessageDirection = this.MessageDirection,
                MessageRole = this.MessageRole,
                MessageType = this.MessageType,
                OperationName = this.OperationName,
                Policy = this.Policy,
                PolicyVersion = this.PolicyVersion,
                ProviderName = this.ProviderName,
                ServiceName = this.ServiceName
            };
        }
    }
}
