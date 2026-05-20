Структура:
1PC:
	26.83.199.121  сервер времени 1
	26.83.199.122  сервер времени 2
	26.83.199.123  посредник Agent
2PC:
	26.58.222.244  сервер времени 3
	клиент отправляет запросы на посредника 26.83.199.123


CMD:
1PC:
	netsh interface ipv4 add address "Radmin VPN" 26.83.199.122 255.0.0.0
	netsh interface ipv4 add address "Radmin VPN" 26.83.199.123 255.0.0.0

	ipconfig
	//Открыть UDP порт 5555
	netsh advfirewall firewall add rule name="Lab07 UDP 5555" dir=in action=allow protocol=UDP localport=5555
2PC:
	ipconfig
	//Открыть UDP порт 5555
	netsh advfirewall firewall add rule name="Lab07 UDP 5555" dir=in action=allow protocol=UDP localport=5555


nodes.txt:
26.83.199.121
26.83.199.122
26.58.222.244

config.txt:
26.83.199.122

Запуск:
1PC:
	./ServerU.exe 26.83.199.121
	./ServerU.exe 26.83.199.122
	./ServerU_Agent.exe 26.83.199.123
2PC:
	./ServerU.exe 26.58.222.244
	./ClientU.exe 26.83.199.123















удаление ip адресов:
	netsh interface ipv4 delete address "Radmin VPN" 26.83.199.122 255.0.0.0

