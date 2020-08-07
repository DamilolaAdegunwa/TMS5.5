using Egoal.Domain.Uow;
using Egoal.Extensions;
using Egoal.Runtime.Session;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace Egoal.Auditing
{
    public class AuditingHelper : IAuditingHelper
    {
        private readonly ILogger _logger;
        private readonly ISession _session;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly AuditingOptions _auditingOptions;
        private readonly IAuditInfoProvider _auditInfoProvider;
        private readonly IAuditSerializer _auditSerializer;
        private readonly IAuditingStore _auditingStore;

        public AuditingHelper(
            ILogger<AuditingHelper> logger,
            ISession session,
            IUnitOfWorkManager unitOfWorkManager,
            IOptions<AuditingOptions> options,
            IAuditInfoProvider auditInfoProvider,
            IAuditSerializer auditSerializer,
            IAuditingStore auditingStore)
        {
            _logger = logger;
            _session = session;
            _unitOfWorkManager = unitOfWorkManager;
            _auditingOptions = options.Value;
            _auditInfoProvider = auditInfoProvider;
            _auditSerializer = auditSerializer;
            _auditingStore = auditingStore;
        }

        public bool ShouldSaveAudit(MethodInfo methodInfo, bool defaultValue = false)
        {
            if (!_auditingOptions.IsEnabled)
            {
                return false;
            }

            if (!_auditingOptions.IsEnabledForAnonymousUsers && (_session?.StaffId == null))
            {
                return false;
            }

            if (methodInfo == null)
            {
                return false;
            }

            if (!methodInfo.IsPublic)
            {
                return false;
            }

            if (methodInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            var classType = methodInfo.DeclaringType;
            if (classType != null)
            {
                if (classType.GetTypeInfo().IsDefined(typeof(DisableAuditingAttribute), true))
                {
                    return false;
                }
            }

            return defaultValue;
        }

        public AuditInfo CreateAuditInfo(Type type, MethodInfo method, object[] arguments)
        {
            return CreateAuditInfo(type, method, CreateArgumentsDictionary(method, arguments));
        }

        public AuditInfo CreateAuditInfo(Type type, MethodInfo method, IDictionary<string, object> arguments)
        {
            var auditInfo = new AuditInfo
            {
                UserId = _session?.StaffId,
                ServiceName = type != null ? type.FullName : "",
                MethodName = method.Name,
                Parameters = ConvertArgumentsToJson(arguments),
                ExecutionTime = DateTime.Now
            };

            try
            {
                _auditInfoProvider.Fill(auditInfo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString(), ex);
            }

            return auditInfo;
        }

        public async Task SaveAsync(AuditInfo auditInfo)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                await _auditingStore.SaveAsync(auditInfo);
                await uow.CompleteAsync();
            }
        }

        private string ConvertArgumentsToJson(IDictionary<string, object> arguments)
        {
            try
            {
                if (arguments.IsNullOrEmpty())
                {
                    return "{}";
                }

                var dictionary = new Dictionary<string, object>();

                foreach (var argument in arguments)
                {
                    if (argument.Value != null && _auditingOptions.IgnoredTypes.Any(t => t.IsInstanceOfType(argument.Value)))
                    {
                        dictionary[argument.Key] = null;
                    }
                    else
                    {
                        dictionary[argument.Key] = argument.Value;
                    }
                }

                return _auditSerializer.Serialize(dictionary);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString(), ex);
                return "{}";
            }
        }

        private static Dictionary<string, object> CreateArgumentsDictionary(MethodInfo method, object[] arguments)
        {
            var parameters = method.GetParameters();
            var dictionary = new Dictionary<string, object>();

            for (var i = 0; i < parameters.Length; i++)
            {
                dictionary[parameters[i].Name] = arguments[i];
            }

            return dictionary;
        }
    }
}
