﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rebus.Extensions;
using Rebus.Persistence.SqlServer;
using Rebus.Tests.Contracts.Transports;
using Rebus.Transport;
using Rebus.Transport.SqlServer;

namespace Rebus.Tests.Transport.SqlServer
{
    [TestFixture]
    public class SqlServerTransportBasicSendReceive : BasicSendReceive<SqlTransportFactory> { }

    [TestFixture]
    public class SqlServerTransportMessageExpiration : MessageExpiration<SqlTransportFactory> { }

    public class SqlTransportFactory : ITransportFactory
    {
        readonly HashSet<string> _tablesToDrop = new HashSet<string>();
        readonly List<IDisposable> _disposables = new List<IDisposable>();

        public ITransport Create(string inputQueueAddress)
        {
            var tableName = "RebusMessages" + TestConfig.Suffix;

            _tablesToDrop.Add(tableName);

            var transport = new SqlServerTransport(new DbConnectionProvider(SqlTestHelper.ConnectionString), tableName, inputQueueAddress);
            
            _disposables.Add(transport);
            
            transport.EnsureTableIsCreated();
            transport.Initialize();
            
            return transport;
        }

        public void CleanUp()
        {
            _disposables.ForEach(d => d.Dispose());

            _tablesToDrop.ForEach(SqlTestHelper.DropTable);
        }
    }
}