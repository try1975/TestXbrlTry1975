using System;
using Diwen.Xbrl;
using System.Linq;
using System.Collections.Generic;

namespace TestXbrlTry1975
{
    class Program
    {
        static void Main(string[] args)
        {
            // для работы с XBRL решил использовать пакет Diwen.Xbrl (исходники https://github.com/dgm9704/Xoxo )
            // посмотрел реализацию, используется XmlReader для потокового чтения больших файлов,
            // реализованы интерфейсы IEquatable. Я сам бы так сделал, если бы был глубоко погружен в вопрос

            var path = "report1.xbrl";
            var instance = Instance.FromFile(path);

            Console.WriteLine("----------------------------------------------------");
            var date = new DateTime(year: 2019, month: 4, day: 30);
            Console.WriteLine($"{path} - контексты с периодом xbrli:period/xbrli:instant, равным \"{date:yyyy-MM-dd}\"");
            Console.WriteLine("эквивалент XPath xbrli:period/xbrli:instant , равным \"2019-04-30\"");
            Console.WriteLine(string.Join(", ", 
                instance.Contexts.Where(z => z.Period.Instant == date)
                .Select(z =>  z.Id )
                ));

            Console.WriteLine("----------------------------------------------------");
            var dimensionName = "dim-int:ID_sobstv_CZBTaxis";
            Console.WriteLine($"{path} - контексты со сценарием, использующим измерение dimension=\"{dimensionName}\"");
            Console.WriteLine(string.Join(", ",
                instance.Contexts.Where(z => z.Scenario != null && z.Scenario.TypedMembers.Any(x => x.Dimension.Name == dimensionName))
                .Select(z => z.Id)
                ));

            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($"{path} - контексты без сценария");
            Console.WriteLine(string.Join(", ",
                instance.Contexts.Where(z => z.Scenario == null)
                .Select(z => z.Id)
                ));
        }
    }
}
