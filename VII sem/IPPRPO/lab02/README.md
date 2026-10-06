# ЛР №2 ИППРПО — Кинотеатр, вариант 2

mvn clean
mvn compile
mvn test
mvn package
mvn verify


Индивидуальное задание: подключить дополнительный Maven-плагин, объяснить его назначение и
продемонстрировать результат его работы через Maven.

Использован: org.apache.maven.plugins:maven-antrun-plugin:3.1.0

Он позволяет выполнять действия Apache Ant во время Maven-сборки. В данной
работе плагин привязан к фазе `verify` и создаёт демонстрационный файл
`target/variant2-plugin-result.txt`.