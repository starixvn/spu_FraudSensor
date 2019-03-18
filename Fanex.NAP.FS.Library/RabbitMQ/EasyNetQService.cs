using EasyNetQ;
using EasyNetQ.Topology;
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
        private IQueue _publish_queue;
        private IExchange _receive_exchange;

        public EasyNetQService()
        {
            var connectionString = string.Format(
                "host={0};virtualHost={1};prefetchcount={2};username={3};password={4}",
                AppSettings.RabbitMQ_Host,
                AppSettings.RabbitMQ_VirtualHost,
                AppSettings.RabbitMQ_PrefetchCount,
                AppSettings.RabbitMQ_Username,
                AppSettings.RabbitMQ_Password);

            _advancedBus = RabbitHutch.CreateBus(connectionString).Advanced;

            _publish_queue = _advancedBus.QueueDeclare(AppSettings.RabbitMQ_Queue);
            var _exchange = _advancedBus.ExchangeDeclare(AppSettings.RabbitMQ_Exchange, ExchangeType.Direct);
            _advancedBus.Bind(_exchange, _publish_queue, AppSettings.RabbitMQ_RoutingKey);

            var _receive_queue = _advancedBus.QueueDeclare(AppSettings.RabbitMQ_Queue);
            _receive_exchange = _advancedBus.ExchangeDeclare(AppSettings.RabbitMQ_Exchange, ExchangeType.Direct);
            _advancedBus.Bind(_receive_exchange, _receive_queue, AppSettings.RabbitMQ_RoutingKey);
        }

        public void Enqueue(AddCustomerAndAssociationCriteria data)
        {
            var message = new Message<AddCustomerAndAssociationCriteria>(data);

            // set persistent for message
            message.SetProperties(new MessageProperties() { DeliveryMode = 2 });
            _advancedBus.PublishAsync(_receive_exchange, AppSettings.RabbitMQ_RoutingKey, true, message);
        }
    }
}
