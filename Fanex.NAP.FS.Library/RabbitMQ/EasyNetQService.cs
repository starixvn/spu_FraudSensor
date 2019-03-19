using EasyNetQ;
using EasyNetQ.Topology;
using Fanex.NAP.FS.Library.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Library.RabbitMQ
{
    public class EasyNetQService
    {
        private IAdvancedBus _advancedBus;
        private IExchange _receive_exchange;

        public EasyNetQService()
        {
            var connectionString = string.Format(
                "host={0};virtualHost={1};prefetchcount={2};username={3};password={4}",
                ConfigSettings.RabbitMQ_Host(),
                ConfigSettings.RabbitMQ_VirtualHost(),
                ConfigSettings.RabbitMQ_PrefetchCount(),
                ConfigSettings.RabbitMQ_Username(),
                ConfigSettings.RabbitMQ_Password());

            _advancedBus = RabbitHutch.CreateBus(connectionString).Advanced;

            var _receive_queue = _advancedBus.QueueDeclare(ConfigSettings.RabbitMQ_Queue());
            _receive_exchange = _advancedBus.ExchangeDeclare(ConfigSettings.RabbitMQ_Exchange(), ExchangeType.Direct);
            _advancedBus.Bind(_receive_exchange, _receive_queue, ConfigSettings.RabbitMQ_RoutingKey());
        }

        public void Enqueue(int data)
        {
            var message = new Message<int>(data);

            // set persistent for message
            //_advancedBus.PublishAsync<int>(_receive_exchange, ConfigSettings.RabbitMQ_RoutingKey(), true, message);
        }
    }
}
