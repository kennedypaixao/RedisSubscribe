using System;
using StackExchange.Redis;

namespace RedisExemplo
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "##";
            var redis = ConnectionMultiplexer.Connect(connectionString);
            IDatabase db = redis.GetDatabase();

            Console.WriteLine("Subscribe Iniciado:");

            _executeSubscribeAnswers(redis, db);

            while (true)
            {
                System.Console.ReadKey();
            };//This wont stop app

            Console.ReadLine();
        }

        static void _executeCommands(IDatabase _db)
        {
            var stringKey = "A";
            _db.StringSet(stringKey, "1");
            var getA = _db.StringGet(stringKey);

            _db.StringIncrement(stringKey);
            getA = _db.StringGet(stringKey);

            var setKey = "tech";
            _db.SetAdd(setKey, "SQL");
            var getSet = _db.SetMembers(setKey);

            var hashKey = "tamanho";
            _db.HashSet(hashKey, "P", "1");
            var hashGet = _db.HashGetAll(hashKey);

            var listKey = "L1";
            _db.ListLeftPush(listKey, "A");
            _db.ListLeftPush(listKey, "B");
            var listGet0 = _db.ListGetByIndex(listKey, 0);
            var listGet1 = _db.ListGetByIndex(listKey, 1);
        }

        static void _executeSubscribeSample(ConnectionMultiplexer redis)
        {
            var sub = redis.GetSubscriber();
            sub.Subscribe("net20").OnMessage((e) =>
            {
                Console.WriteLine(e.Message);
            });
        }

        static void _executeSubscribeAnswers(ConnectionMultiplexer redis, IDatabase db)
        {
            var sub = redis.GetSubscriber();
            sub.Subscribe("perguntas").OnMessage((e) =>
            {
                Console.WriteLine($"{e.Message}:");
                var position = e.Message.ToString().Split(":");
                if (position.Length == 2)
                {
                    var questionID = position[0];
                    var questionDescription = position[1];

                    var answer = "";

                    if (questionDescription.Contains("Quanto eh"))
                    {
                        var countValues = questionDescription.Replace("Quanto eh", "").Replace("?", "").Trim().Split("+");
                        if(countValues.Length == 2)
                        {
                            var valA = int.Parse(countValues[0]);
                            var valB = int.Parse(countValues[1]);

                            answer = (valA + valB).ToString();
                        }
                    }
                    else
                    {
                        answer = Console.ReadLine();
                    }

                    if (!string.IsNullOrEmpty(answer))
                    {
                        db.HashSet(questionID, "GRUPO4", answer);
                        
                        // Valida se foi salvo a resposta.
                        //var hashGet = db.HashGetAll(questionID);
                    }
                }
            });
        }
    }
}
