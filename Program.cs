using System;
using Diwen.Xbrl;
using System.Linq;
using System.IO;

namespace TestXbrlTry1975
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Fields
            var taskColor = ConsoleColor.Green;
            var errorColor = ConsoleColor.Red;

            var report1Path = "report1.xbrl";
            var report2Path = "report2.xbrl";
            Instance report1;
            Instance report2; 
            #endregion

            // для работы с XBRL решил использовать пакет Diwen.Xbrl (исходники https://github.com/dgm9704/Xoxo )
            // посмотрел реализацию, используется XmlReader для потокового чтения больших файлов,
            // реализованы интерфейсы IEquatable. Я сам бы так сделал, если бы был глубоко погружен в вопрос

            #region read files
            try
            {
                using var stream = new FileStream(report1Path, FileMode.Open, FileAccess.Read);
                report1 = Instance.FromStream(stream: stream, removeUnusedObjects: false, collapseDuplicateContexts: false);
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = errorColor;
                Console.WriteLine($"Ошибка файла {report1Path}");
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.InnerException.Message);
                return;
            }

            try
            {
                using var stream = new FileStream(report2Path, FileMode.Open, FileAccess.Read);
                report2 = Instance.FromStream(stream: stream, removeUnusedObjects: false, collapseDuplicateContexts: false);
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = errorColor;
                Console.WriteLine($"Ошибка файла {report2Path}");
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.InnerException.Message);
                return;
            }
            #endregion

            #region check contexts
            Console.ForegroundColor = taskColor;
            Console.WriteLine("Найти ошибки в файле (повторяющиеся контексты).");
            Console.ResetColor();
            FindDuplicates(report1Path, report1);
            FindDuplicates(report2Path, report2);
            #endregion

            #region compare facts
            Console.ForegroundColor = taskColor;
            ConsoleWriteDevider();
            Console.WriteLine("Выявить различия: список отсутствующих и новые факты, факты с различающимися значениями.");
            var comparisonReport = InstanceComparer.Report(report1, report2);
            Console.ResetColor();
            foreach (var message in comparisonReport.Messages)
            {
                Console.WriteLine($"{message}");
            }
            #endregion

            #region Like XPath
            ////Написать запросы XPath для получения:
            LinqAsXPath(report1Path, report1, taskColor); 
            #endregion

            #region union files
            Console.ForegroundColor = taskColor;
            ConsoleWriteDevider();
            Console.WriteLine("Объединить отчеты, на выходе получить новый объединенный отчет (xbrl) с объединенными списками контекстов, единиц измерений и значений (фактов).");
            Console.ResetColor();
            var unionPath = "unionReport.xbrl";
            Console.WriteLine($"имя файла объединенного отчета: {unionPath}");
            var unionReport = report1;// Instance.FromFile(report1Path);
            unionReport.Contexts.AddRange(report2.Contexts.Except(unionReport.Contexts));
            unionReport.Units.AddRange(report2.Units.Except(unionReport.Units));
            unionReport.Facts.AddRange(report2.Facts.Except(unionReport.Facts));
            unionReport.ToFile(unionPath);
            #endregion

        }

        /// <summary>
        /// Find and remove dublicate Context
        /// </summary>
        /// <param name="path"></param>
        /// <param name="instance"></param>
        private static void FindDuplicates(string path, Instance instance)
        {
            var duplicates = instance.Contexts
                .GroupBy(c => c)
                .Where(g => g.Skip(1).Any());

            if (duplicates.Any())
            {
                foreach (var group in duplicates)
                {
                    var id = group.FirstOrDefault().Id;
                    var duplicateIds = string.Join(", ", group.Skip(1).Select(z => z.Id));
                    Console.WriteLine($"{path} - {id} дублируется в {duplicateIds}");

                    foreach (var duplicate in group.Skip(1))
                    {
                        instance.Facts.
                             Where(f => f.Context?.Id == duplicate.Id).
                             ToList().
                             ForEach(f => f.Context = group.First());
                    }
                    instance.RemoveUnusedContexts();
                }
                Console.WriteLine($"{path} - дубли удалены");
            }
        }

        /// <summary>
        /// Use Linq as eqvivalent of XPath
        /// </summary>
        /// <param name="path"></param>
        private static void LinqAsXPath(string path, Instance instance, ConsoleColor taskColor)
        {
            Console.ForegroundColor = taskColor;
            ConsoleWriteDevider();
            var date = new DateTime(year: 2019, month: 4, day: 30);
            Console.WriteLine($"{path} - контексты с периодом xbrli:period/xbrli:instant, равным \"{date:yyyy-MM-dd}\"");
            Console.ResetColor();
            //Console.WriteLine("эквивалент XPath xbrli:period/xbrli:instant , равным \"2019-04-30\"");
            Console.WriteLine(string.Join(", ",
                instance.Contexts.Where(z => z.Period.Instant == date)
                .Select(z => z.Id)
                ));

            Console.ForegroundColor = taskColor;
            ConsoleWriteDevider();
            var dimensionName = "dim-int:ID_sobstv_CZBTaxis";
            Console.WriteLine($"{path} - контексты со сценарием, использующим измерение dimension=\"{dimensionName}\"");
            Console.ResetColor();
            Console.WriteLine(string.Join(", ",
                instance.Contexts.Where(z => z.Scenario != null && z.Scenario.TypedMembers.Any(x => x.Dimension.Name == dimensionName))
                .Select(z => z.Id)
                ));

            Console.ForegroundColor = taskColor;
            ConsoleWriteDevider();
            Console.WriteLine($"{path} - контексты без сценария");
            Console.ResetColor();
            Console.WriteLine(string.Join(", ",
                instance.Contexts.Where(z => z.Scenario == null)
                .Select(z => z.Id)
                ));
        }

        private static void ConsoleWriteDevider() => Console.WriteLine("----------------------------------------------------");
    }
}
