using Dapper;
using Microsoft.Extensions.Configuration;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Egoal.Settings
{
    public class DbConfigurationProvider : ConfigurationProvider
    {
        private readonly string _connectionString;

        public DbConfigurationProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override void Load()
        {
            Data = Policy
                .Handle<Exception>()
                .WaitAndRetry(60, retryCount => TimeSpan.FromSeconds(30))
                .Execute(GetSettings);
        }

        private Dictionary<string, string> GetSettings()
        {
            var data = new Dictionary<string, string>();

            using (var connection = new SqlConnection(_connectionString))
            {
                string sql = @"
SELECT
[Name] AS Id,
[Value]
FROM dbo.SM_SysPara";

                var settings = connection.Query<Setting>(sql);
                foreach (var setting in settings)
                {
                    data.Add(setting.Id, setting.Value);
                }

                string invoiceSql = @"
IF EXISTS(SELECT 1 FROM sysobjects WHERE id=OBJECT_ID('dbo.SM_InvoicePara') AND type='U')
BEGIN
	SELECT
	[Name] AS Id,
	[Value]
	FROM dbo.SM_InvoicePara
END
";
                var invoiceSettings = connection.Query<Setting>(invoiceSql);
                foreach (var setting in invoiceSettings)
                {
                    data.Add(setting.Id, setting.Value);
                }
            }

            return data;
        }
    }
}
