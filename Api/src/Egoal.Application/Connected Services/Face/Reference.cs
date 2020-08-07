//------------------------------------------------------------------------------
// <自动生成>
//     此代码由工具生成。
//     //
//     对此文件的更改可能导致不正确的行为，并在以下条件下丢失:
//     代码重新生成。
// </自动生成>
//------------------------------------------------------------------------------

namespace Face
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ZKLiveFaceServiceRequest", Namespace="http://schemas.datacontract.org/2004/07/Common")]
    public partial class ZKLiveFaceServiceRequest : object
    {
        
        private string FaceIDField;
        
        private byte[] ImageDataField;
        
        private string MethodField;
        
        private string TicketTypeTypeIDField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FaceID
        {
            get
            {
                return this.FaceIDField;
            }
            set
            {
                this.FaceIDField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] ImageData
        {
            get
            {
                return this.ImageDataField;
            }
            set
            {
                this.ImageDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Method
        {
            get
            {
                return this.MethodField;
            }
            set
            {
                this.MethodField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TicketTypeTypeID
        {
            get
            {
                return this.TicketTypeTypeIDField;
            }
            set
            {
                this.TicketTypeTypeIDField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ZKLiveFaceServiceResponse", Namespace="http://schemas.datacontract.org/2004/07/Common")]
    public partial class ZKLiveFaceServiceResponse : object
    {
        
        private string ErrorDescField;
        
        private int ICaoFeatureAngleField;
        
        private int ICaoFeatureGlassesField;
        
        private int ICaoFeatureQualityField;
        
        private byte[] PhotoTemplateField;
        
        private System.Drawing.Rectangle RectField;
        
        private string ResultField;
        
        private string ScoreField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorDesc
        {
            get
            {
                return this.ErrorDescField;
            }
            set
            {
                this.ErrorDescField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ICaoFeatureAngle
        {
            get
            {
                return this.ICaoFeatureAngleField;
            }
            set
            {
                this.ICaoFeatureAngleField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ICaoFeatureGlasses
        {
            get
            {
                return this.ICaoFeatureGlassesField;
            }
            set
            {
                this.ICaoFeatureGlassesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ICaoFeatureQuality
        {
            get
            {
                return this.ICaoFeatureQualityField;
            }
            set
            {
                this.ICaoFeatureQualityField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] PhotoTemplate
        {
            get
            {
                return this.PhotoTemplateField;
            }
            set
            {
                this.PhotoTemplateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Drawing.Rectangle Rect
        {
            get
            {
                return this.RectField;
            }
            set
            {
                this.RectField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Result
        {
            get
            {
                return this.ResultField;
            }
            set
            {
                this.ResultField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Score
        {
            get
            {
                return this.ScoreField;
            }
            set
            {
                this.ScoreField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Face.IZkLiveFaceService")]
    public interface IZkLiveFaceService
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IZkLiveFaceService/HandlerZKLiveFaceRequest", ReplyAction="http://tempuri.org/IZkLiveFaceService/HandlerZKLiveFaceRequestResponse")]
        System.Threading.Tasks.Task<Face.ZKLiveFaceServiceResponse> HandlerZKLiveFaceRequestAsync(Face.ZKLiveFaceServiceRequest req);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public interface IZkLiveFaceServiceChannel : Face.IZkLiveFaceService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class ZkLiveFaceServiceClient : System.ServiceModel.ClientBase<Face.IZkLiveFaceService>, Face.IZkLiveFaceService
    {
        
    /// <summary>
    /// 实现此分部方法，配置服务终结点。
    /// </summary>
    /// <param name="serviceEndpoint">要配置的终结点</param>
    /// <param name="clientCredentials">客户端凭据</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public ZkLiveFaceServiceClient() : 
                base(ZkLiveFaceServiceClient.GetDefaultBinding(), ZkLiveFaceServiceClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.ZkLiveFaceHttpBinding_IZkLiveFaceService.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ZkLiveFaceServiceClient(EndpointConfiguration endpointConfiguration) : 
                base(ZkLiveFaceServiceClient.GetBindingForEndpoint(endpointConfiguration), ZkLiveFaceServiceClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ZkLiveFaceServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(ZkLiveFaceServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ZkLiveFaceServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(ZkLiveFaceServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ZkLiveFaceServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<Face.ZKLiveFaceServiceResponse> HandlerZKLiveFaceRequestAsync(Face.ZKLiveFaceServiceRequest req)
        {
            return base.Channel.HandlerZKLiveFaceRequestAsync(req);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.ZkLiveFaceHttpBinding_IZkLiveFaceService))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("找不到名称为“{0}”的终结点。", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.ZkLiveFaceHttpBinding_IZkLiveFaceService))
            {
                return new System.ServiceModel.EndpointAddress("http://192.168.1.60:21000/");
            }
            throw new System.InvalidOperationException(string.Format("找不到名称为“{0}”的终结点。", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return ZkLiveFaceServiceClient.GetBindingForEndpoint(EndpointConfiguration.ZkLiveFaceHttpBinding_IZkLiveFaceService);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return ZkLiveFaceServiceClient.GetEndpointAddress(EndpointConfiguration.ZkLiveFaceHttpBinding_IZkLiveFaceService);
        }
        
        public enum EndpointConfiguration
        {
            
            ZkLiveFaceHttpBinding_IZkLiveFaceService,
        }
    }
}
