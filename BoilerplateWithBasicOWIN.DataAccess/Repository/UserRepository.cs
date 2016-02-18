using Microsoft.AspNet.Identity;
using Npgsql;
using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoilerplateWithBasicOWIN.Utility;
using BoilerplateWithBasicOWIN.DataAccess.Models;

namespace BoilerplateWithBasicOWIN.DataAccess.Repository
{
    public class UserRepository : IUserStore<Account>, IUserLoginStore<Account>, IUserPasswordStore<Account>, IUserSecurityStampStore<Account>
    {
        private string _connectionString;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public UserRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException("connectionString");

            this._connectionString = connectionString;
        }

        public void Dispose()
        {

        }

        #region IUserStore

        /// <summary>
        /// Creates an account.
        /// </summary>
        /// <param name="user"></param>
        public async Task CreateAsync(Account user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();                    
                try
                {
                    await con.ExecuteAsync("insert into Account(UserName, PassHash, Email, Created, Updated) values(@userName, @passHash, @email, @created, @updated)", user);
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Create User Failed");
                }

                con.Close();
            }                    
        }

        /// <summary>
        /// Deletes an account (synchronous).
        /// </summary>
        /// <param name="user"></param>
        public async Task DeleteAsync(Account user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                await con.ExecuteAsync("delete from Account where UserId = @userId", new { user.Id });
                con.Close();
            }
                

        }

        /// <summary>
        /// Returns a list of all accounts (async).
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Account>> GetAllAccountsAsync()
        {           
            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                var results = await con.QueryAsync<Account>("select * from Account");
                var accounts = results.ToList();
                con.Close();

                return accounts;
            }                
        }

        ///// <summary>
        ///// Finds Account by ID. This returns a singular account based off of indexed ID.
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //public async Task<Account> GetAccountSubscriptions(string userId)
        //{
        //    if (string.IsNullOrWhiteSpace(userId))
        //        throw new ArgumentNullException("userId");

        //    Int64 parsedUserId;
        //    if (!Int64.TryParse(userId, out parsedUserId))
        //        throw new ArgumentOutOfRangeException("userId", string.Format("'{0}' is not a valid account ID.", new { userId }));

        //    const String sql = "select * from Account A"
        //                        + "inner join Subscription S on S.SubscriberId = A.Id"
        //                        + "where ID = @userId";

        //    using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
        //    {
        //        await con.OpenAsync();
        //        var query = await con.QueryAsync<Account, List<Subscription>, Account>(sql, ();
        //        con.Close();
        //        return query.SingleOrDefault();
        //    }
        //}


        /// <summary>
        /// Finds Account by ID. This returns a singular account based off of indexed ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Account> FindByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException("userId");

            Int64 parsedUserId;
            if (!Int64.TryParse(userId, out parsedUserId))
                throw new ArgumentOutOfRangeException("userId", string.Format("'{0}' is not a valid account ID.", new { userId }));

            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                var query = await con.QueryAsync<Account>("select * from Account where ID = @userId", new { userId = parsedUserId });
                con.Close();
                return query.SingleOrDefault();
            }                
        }

        /// <summary>
        /// Finds Account by UserName. This returns a singular account based off of indexed usernames.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<Account> FindByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException("userName");

            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();
                var query = await con.QueryAsync<Account>("select * from Account where lower(UserName) = lower(@userName)", new { userName });
                con.Close();
                return query.SingleOrDefault();
            }                
        }

        /// <summary>
        /// Updates account row.
        /// </summary>
        /// <param name="user"></param>
        public async Task UpdateAsync(Account user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
            {
                await con.OpenAsync();                
                await con.ExecuteAsync("update Account set UserName = @userName, PasswordHash = @passwordHash, Email = @email where UserId = @userId", user);
                con.Close();
            }
                
        }
        #endregion

        #region IUserLoginStore
        public virtual Task AddLoginAsync(Account user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");

            return Task.Factory.StartNew(() =>
            {
                using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
                    con.Execute("insert into ExternalLogins(ExternalLoginId, UserId, LoginProvider, ProviderKey) values(@externalLoginId, @userId, @loginProvider, @providerKey)",
                        new { externalLoginId = Guid.NewGuid(), userId = user.Id, loginProvider = login.LoginProvider, providerKey = login.ProviderKey });
            });
        }

        public virtual Task<Account> FindAsync(UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");

            return Task.Factory.StartNew(() =>
            {
                using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
                    return con.Query<Account>("select u.* from Users u inner join ExternalLogins l on l.UserId = u.UserId where l.LoginProvider = @loginProvider and l.ProviderKey = @providerKey",
                        login).SingleOrDefault();
            });
        }

        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(Account user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.Factory.StartNew(() =>
            {
                using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
                    return (IList<UserLoginInfo>)con.Query<UserLoginInfo>("select LoginProvider, ProviderKey from ExternalLogins where UserId = @userId", new { user.Id }).ToList();
            });
        }

        public virtual Task RemoveLoginAsync(Account user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");

            return Task.Factory.StartNew(() =>
            {
                using (NpgsqlConnection con = new NpgsqlConnection(_connectionString))
                    con.Execute("delete from ExternalLogins where UserId = @userId and LoginProvider = @loginProvider and ProviderKey = @providerKey",
                        new { user.Id, login.LoginProvider, login.ProviderKey });
            });
        }
        #endregion

        #region IUserPasswordStore
        public virtual Task<string> GetPasswordHashAsync(Account user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PassHash);
        }

        public virtual Task<bool> HasPasswordAsync(Account user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PassHash));
        }

        public virtual Task SetPasswordHashAsync(Account user, string passwordHash)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.PassHash = passwordHash;

            return Task.FromResult(0);
        }

        #endregion

        #region IUserSecurityStampStore
        public virtual Task<string> GetSecurityStampAsync(Account user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return null;
            //return Task.FromResult(user.SecurityStamp);
        }

        public virtual Task SetSecurityStampAsync(Account user, string stamp)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            //user.SecurityStamp = stamp;

            return Task.FromResult(0);
        }

        #endregion


    }
}
