using AS400IntegrationLayer.Application.Interfaces;
using AS400IntegrationLayer.Domain.Exceptions;
using AS400IntegrationLayer.Domain.Settings;
using Microsoft.Extensions.Options;
using System.Data.Odbc;

namespace AS400IntegrationLayer.Infrastructure.Persistence.DbContexts
{
    public class AS400DbContext(IOptions<AS400Settings> options) : IAS400DbContext
    {
        public OdbcConnection CreateConnection()
        {
            var connectionString = $"Driver={_settings.Driver};" +
                $"System={_settings.Server};" +
                $"Uid={_settings.Username};" +
                $"Pwd={_settings.Password}";

            var connection = new OdbcConnection(connectionString);
            return connection;
        }

        public async Task<bool> Validate(string? username, string? password)
        {
            var connectionString = $"Driver={_settings.Driver};System={_settings.Server};Uid={username};Pwd={password}";
            var connection = new OdbcConnection(connectionString);
            try
            {
                await connection.OpenAsync();
                return true;
            }
            catch (OdbcException oc)
            {
                throw new ApiCustomException(oc.Message);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        private readonly AS400Settings _settings = options.Value;
    }
}
