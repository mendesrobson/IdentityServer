using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Domain.Interfaces
{
    public class EventManager
    {
        private static ConcurrentDictionary<String, IXDomainEventParser> _parsers = new ConcurrentDictionary<String, IXDomainEventParser>();

        public static void RegisterParser(String context, IXDomainEventParser parser)
        {
            if (_parsers.ContainsKey(context))
            {
                Console.WriteLine("EventManager - ThreadId Override");
                Remove(context);
            }

            if (!_parsers.TryAdd(context, parser))
            {
                throw new Exception(String.Format("Falha na inserção do context {0} no parser", context));
            }
        }

        public static void Raise<T>(T e) where T : IXDomainEvent
        {
            String contextKey = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();

            _parsers[contextKey].Parse<T>(e);
        }

        public static void Remove(String context)
        {
            IXDomainEventParser removedElement = null;
            Int32 maxTries = 100;
            for (var i = 0; i < maxTries; i++)
            {
                if (_parsers.TryRemove(context, out removedElement))
                    break;
                else
                    Console.WriteLine(String.Format("Falha na remoção do context {0} no parser", context));
            }

        }
    }
}
