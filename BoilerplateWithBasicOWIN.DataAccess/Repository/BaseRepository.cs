using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoilerplateWithBasicOWIN.Utility;

namespace BoilerplateWithBasicOWIN.DataAccess.Repository
{
    public class BaseRepository
    {
        protected String _connectionString;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        protected NpgsqlConnection GetConnection()
        {

            return new NpgsqlConnection(_connectionString);
        }
    }
}
