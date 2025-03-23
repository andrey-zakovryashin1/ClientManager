### Проект для тестового задания на позицию backend разработчика

Разработано приложение на C# для получения и изменения таблицы с данными. С точки зрения конечного пользователя структура хранения данных:  
<pre>
clients:  
+------+----------+----------+-------+-------+---------+-------------+  
| id   | firstName| lastName | email | phone | address | description |  
+------+----------+----------+-------+-------+---------+-------------+  
|input | input    | input    | input | input | subform | textarea    |  
+------+----------+----------+-------+-------+---------+-------------+  
  
address:  
+---------------+------+-------+-----+  
| streetAddress | city | state | zip |  
+---------------+------+-------+-----+  
| input         |input | input |input|  
+---------------+------+-------+-----+  
</pre>
  
1.	Разработана структура данных для хранения указанных сущностей clients, address.  
2.	Используется API для получения и изменения указанных данных:  
  a.	Получение сведений о клиентах и адресах.  
  b.	Изменение сведений о клиенте и адресе.  
  c.	Удаление клиента и связанного с ним адреса.  
  d.	Изменение адреса для клиентов, удовлетворяющих условиям фильтрации (например, для клиентов с определенной фамилией).  
3.	Запросы на SQL для получения:  
  a.	Количества клиентов  
  b.	Перечня городов и количества клиентов, проживающих в указанном городе  
  c.	Перечня городов, в которых проживает более одного клиента  
добавлены в файл [SQL Queries](https://github.com/andrey-zakovryashin1/ClientManager/blob/main/SQL%20Queries.txt).  
  
Реализована дополнительная функциональность  
● Сортировка по каждому из столбцов при запросе данных (в порядке возрастания или убывания).  
● Пагинация: данные представлены для отображения постранично на фронте, максимум 10 элементов на страницу, с возможностью перехода по страницам.  
● Фильтрация: API имеет возможность задать фильтр: строки таблицы, данные которых не содержат подстроку, введённую пользователем, не передаются в результатах запроса.  
● Для каждого поля добавлена валидация при создании и изменении данных:  
	○ id — обязательное, число;  
	○ firstName, lastName — обязательное, буквы;  
	○ email — обязательное, формат email;  
	○ phone — обязательное, формат телефона;  
  	○ streetAddress - обязательное;  
	○ city - обязательное;  
	○ state - обязательное;  
	○ zip - обязательное, формат почтового индекса (6 цифр, может быть ведущий ноль);  
	○ description - не обязательное поле.  

Формат данных.  
API возвращает JSON-массив данных. Пример данных:  
<pre>
[  
    {  
        "id": 101,  
        "firstName": "Sue",  
        "lastName": "Corson",  
        "email": "DWhalley@in.gov",  
        "phone": "(612)211-6296",  
        "address": {  
            "streetAddress": "9792 Mattis Ct",  
            "city": "Waukesha",  
            "state": "WI",  
            "zip": "221781"  
        },  
        "description": 'et lacus magna dolor...',  
    }  
}  
</pre>
  
Данные могут быть запрошены в нужном формате (HTML-представление или JSON-массив данных) через заголовок Accept:  
● Если клиент (например, браузер) запрашивает HTML, данные возвращаются в виде HTML-представления.  
● Если клиент (например, API-клиент) запрашивает JSON (заголовок Accept: application/json), данные возвращаются в виде JSON-массива.  
  
В приложении используется упрощенный движок базы данных LocalDB, который представляет легковесную версию SQL Server Express, предназначенную для разработки приложений.  
Для удобства демонстрации и упрощения процесса база данных заполняется сгенерированными значениями внутри приложения. Это сделано для наглядного примера работы приложения и его функциональности.  
