﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QZ.Service.Enterprise
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "AnalysesResult", Namespace = "http://schemas.datacontract.org/2004/07/QZ.N2E.ClassifierService")]
    [System.SerializableAttribute()]
    public partial class AnalysesResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> exhibitonTagListField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> forwardTradeListField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> productListField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> tradeListField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> exhibitonTagList
        {
            get
            {
                return this.exhibitonTagListField;
            }
            set
            {
                if ((object.ReferenceEquals(this.exhibitonTagListField, value) != true))
                {
                    this.exhibitonTagListField = value;
                    this.RaisePropertyChanged("exhibitonTagList");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> forwardTradeList
        {
            get
            {
                return this.forwardTradeListField;
            }
            set
            {
                if ((object.ReferenceEquals(this.forwardTradeListField, value) != true))
                {
                    this.forwardTradeListField = value;
                    this.RaisePropertyChanged("forwardTradeList");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> productList
        {
            get
            {
                return this.productListField;
            }
            set
            {
                if ((object.ReferenceEquals(this.productListField, value) != true))
                {
                    this.productListField = value;
                    this.RaisePropertyChanged("productList");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> tradeList
        {
            get
            {
                return this.tradeListField;
            }
            set
            {
                if ((object.ReferenceEquals(this.tradeListField, value) != true))
                {
                    this.tradeListField = value;
                    this.RaisePropertyChanged("tradeList");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://entprise.qianzhan.com/TradeAnalysisService", ConfigurationName = "OrgCompanyTrade.TradeAnalysisService")]
    public interface ICompanyTradeService
    {
        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisFo" +
            "rwardTrade", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisFo" +
            "rwardTradeResponse")]
        System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> AnalysisForwardTrade(string doc, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisFo" +
            "rwardTrade", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisFo" +
            "rwardTradeResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>>> AnalysisForwardTradeAsync(string doc, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisFo" +
            "rwardTradeList", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisFo" +
            "rwardTradeListResponse")]
        System.Collections.Generic.List<AnalysesResult> AnalysisForwardTradeList(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisFo" +
            "rwardTradeList", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisFo" +
            "rwardTradeListResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<AnalysesResult>> AnalysisForwardTradeListAsync(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisEx" +
            "hibitionTrade", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisEx" +
            "hibitionTradeResponse")]
        System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> AnalysisExhibitionTrade(string doc, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisEx" +
            "hibitionTrade", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisEx" +
            "hibitionTradeResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>>> AnalysisExhibitionTradeAsync(string doc, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisEx" +
            "hibitionTradeList", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisEx" +
            "hibitionTradeListResponse")]
        System.Collections.Generic.List<AnalysesResult> AnalysisExhibitionTradeList(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisEx" +
            "hibitionTradeList", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisEx" +
            "hibitionTradeListResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<AnalysesResult>> AnalysisExhibitionTradeListAsync(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisPr" +
            "oductAndTrade", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisPr" +
            "oductAndTradeResponse")]
        AnalysesResult AnalysisProductAndTrade(string doc, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisPr" +
            "oductAndTrade", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisPr" +
            "oductAndTradeResponse")]
        System.Threading.Tasks.Task<AnalysesResult> AnalysisProductAndTradeAsync(string doc, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisPr" +
            "oductAndTradeList", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisPr" +
            "oductAndTradeListResponse")]
        System.Collections.Generic.List<AnalysesResult> AnalysisProductAndTradeList(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisPr" +
            "oductAndTradeList", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisPr" +
            "oductAndTradeListResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<AnalysesResult>> AnalysisProductAndTradeListAsync(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisAl" +
            "lTrade", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisAl" +
            "lTradeResponse")]
        AnalysesResult AnalysisAllTrade(string doc, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisAl" +
            "lTrade", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisAl" +
            "lTradeResponse")]
        System.Threading.Tasks.Task<AnalysesResult> AnalysisAllTradeAsync(string doc, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisAl" +
            "lTradeList", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisAl" +
            "lTradeListResponse")]
        System.Collections.Generic.List<AnalysesResult> AnalysisAllTradeList(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput);

        [System.ServiceModel.OperationContractAttribute(Action = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisAl" +
            "lTradeList", ReplyAction = "http://entprise.qianzhan.com/TradeAnalysisService/TradeAnalysisService/AnalysisAl" +
            "lTradeListResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<AnalysesResult>> AnalysisAllTradeListAsync(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput);
    }

    public partial class TradeAnalysisServiceClient : System.ServiceModel.ClientBase<ICompanyTradeService>, ICompanyTradeService
    {

        public TradeAnalysisServiceClient()
        {
        }

        public TradeAnalysisServiceClient(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public TradeAnalysisServiceClient(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public TradeAnalysisServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public TradeAnalysisServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> AnalysisForwardTrade(string doc, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisForwardTrade(doc, top, allowFormattingInput);
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>>> AnalysisForwardTradeAsync(string doc, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisForwardTradeAsync(doc, top, allowFormattingInput);
        }

        public System.Collections.Generic.List<AnalysesResult> AnalysisForwardTradeList(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisForwardTradeList(docList, top, allowFormattingInput);
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<AnalysesResult>> AnalysisForwardTradeListAsync(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisForwardTradeListAsync(docList, top, allowFormattingInput);
        }

        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>> AnalysisExhibitionTrade(string doc, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisExhibitionTrade(doc, top, allowFormattingInput);
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, double>>> AnalysisExhibitionTradeAsync(string doc, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisExhibitionTradeAsync(doc, top, allowFormattingInput);
        }

        public System.Collections.Generic.List<AnalysesResult> AnalysisExhibitionTradeList(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisExhibitionTradeList(docList, top, allowFormattingInput);
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<AnalysesResult>> AnalysisExhibitionTradeListAsync(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisExhibitionTradeListAsync(docList, top, allowFormattingInput);
        }

        public AnalysesResult AnalysisProductAndTrade(string doc, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisProductAndTrade(doc, top, allowFormattingInput);
        }

        public System.Threading.Tasks.Task<AnalysesResult> AnalysisProductAndTradeAsync(string doc, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisProductAndTradeAsync(doc, top, allowFormattingInput);
        }

        public System.Collections.Generic.List<AnalysesResult> AnalysisProductAndTradeList(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisProductAndTradeList(docList, top, allowFormattingInput);
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<AnalysesResult>> AnalysisProductAndTradeListAsync(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisProductAndTradeListAsync(docList, top, allowFormattingInput);
        }

        public AnalysesResult AnalysisAllTrade(string doc, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisAllTrade(doc, top, allowFormattingInput);
        }

        public System.Threading.Tasks.Task<AnalysesResult> AnalysisAllTradeAsync(string doc, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisAllTradeAsync(doc, top, allowFormattingInput);
        }

        public System.Collections.Generic.List<AnalysesResult> AnalysisAllTradeList(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisAllTradeList(docList, top, allowFormattingInput);
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<AnalysesResult>> AnalysisAllTradeListAsync(System.Collections.Generic.List<string> docList, int top, bool allowFormattingInput)
        {
            return base.Channel.AnalysisAllTradeListAsync(docList, top, allowFormattingInput);
        }
    }
}
