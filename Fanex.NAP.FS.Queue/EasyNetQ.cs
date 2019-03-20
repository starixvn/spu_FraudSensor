using EasyNetQ;
using EasyNetQ.Topology;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Fanex.NAP.FS.Queue
{
    public class EasyNetQ
    {
        private string Host { get; set; }

        private string VirtualHost { get; set; }

        private int PrefetchCount { get; set; }

        private string Username { get; set; }

        private string Password { get; set; }

        public string QueueName { get; set; }

        public string ExchangeName { get; set; }

        public string RoutingKey { get; set; }

        public IExchange Exchange { get; set; }

        private IBinding Binding { get; set; }

        public IAdvancedBus AdvanceBus { get; set; }

        public IBus Bus { get; set; }

        public IQueue Queue { get; set; }        

        public EasyNetQ(string host, string virtualHost, int? prefetchCount, string username, string password)
        {
            Host = host ?? Host;
            VirtualHost = virtualHost ?? VirtualHost;
            PrefetchCount = prefetchCount ?? PrefetchCount;
            Username = username ?? Username;
            Password = password ?? Password;
        }

        public EasyNetQ CreateBus()
        {
            var connectionString = string.Format(
               "host={0};virtualHost={1};prefetchcount={2};username={3};password={4}",
               this.Host,
               this.VirtualHost,
               this.PrefetchCount,
               this.Username,
               this.Password);

            if(Bus == null)
            {
                Bus = RabbitHutch.CreateBus(connectionString);

                if (Bus != null)
                {
                    AdvanceBus = Bus.Advanced;
                }
            }            

            return this;
        }

        public EasyNetQ DeclareExchange(string exchangeName)
        {
            if (!string.IsNullOrWhiteSpace(exchangeName))
            {
                Exchange = AdvanceBus.ExchangeDeclare(exchangeName, ExchangeType.Direct);

                if(Exchange != null)
                { 
                    this.ExchangeName = Exchange.Name;
                }
            }             
            
            return this;
        }

        public EasyNetQ DeclareRoutingKey(string routingKey)
        {
            RoutingKey = routingKey ?? RoutingKey;

            return this;
        }

        public EasyNetQ DeclareQueue(string queueName)
        {
            if(!string.IsNullOrWhiteSpace(queueName))
            {
                this.Queue = AdvanceBus.QueueDeclare(queueName);

                if(this.Queue != null)
                {
                    this.QueueName = this.Queue.Name;
                }
            }
            
            return this;
        }

        public EasyNetQ Bind()
        {
            Binding = AdvanceBus.Bind(this.Exchange, this.Queue, this.RoutingKey);
            
            return this;
        }               

        public void Dispose()
        {
            this.Bus.Dispose();
            this.AdvanceBus.Dispose();            
        }
    }
}
