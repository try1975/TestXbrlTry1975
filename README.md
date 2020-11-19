# TestXbrlTry1975
Test Xbrl operate

## Вывод лога работы в файле output.txt, объединенный отчет в файле unionReport.xbrl

Содержание файла отчета (xbrl):
- Контексты (xbrli:context) - описание разреза значений, указанных в отчете (например в разрезе разных периодов). 
	Набор содержащихся в контексте параметров должен быть уникальным. Сценариев в контексте может быть несколько (0..*). 
	Запись объекта (entity) в контексте должна быть только одна (1...1). 
	Период может быть описан следующими вариантами: значение instant, значение forever, или набор значений startDate и endDate.
- Единица измерения (xbrli:unit, юнит) - описание измерения значений, указанных в отчете (на пример штуки или рубли). 
	Набор содержащихся параметров должен быть уникальным. Измерене в записи может быть только одно (1..1).
- Значения (факты, в примере purcb-dic:*) - значения отчета. Код значения в разрезе контектса должен быть уникальным 
	(например "purcb-dic: " в разрезе "A0"), идентификатор параметра (атрибут @id) должен быть уникальным.

Программирование:
- Найти ошибки в файла (повторяющиеся контексты).
- Объединить отчеты, на выходе получить новый объединенный отчет (xbrl) в объединными списками контекстов, единиц измерений и значений (фактов).
- Выявить различия: список отсутствующих и новые факты, факты с различающимися значениями.

Написать запросы XPath для получения:
- контексты с периодом xbrli:period/xbrli:instant, равным "2019-04-30";
- контексты со сценарием, использующим измерение dimension="dim-int:ID_sobstv_CZBTaxis";
- контексты без сценария;


Проверка корректности записей контекстов, единица измерений и фактов в примере НЕ НУЖНА. Предполагается их использование просто, как набор данных.
В приложении предлагаемые модели классов используемых объектов. В описании немного лишей информации (из спецификации таксономии).

ps. Не обязательно выполнять все задания. Важенее код и реализация. Желательно передать результат до 23.11.

