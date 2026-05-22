Структура:
	192.168.56.101  сервер времени 1
	192.168.56.102  сервер времени 2
	192.168.56.103  сервер времени 3
	192.168.56.104  посредник Agent
	клиент отправляет запросы на посредника 192.168.56.104


CMD:
	New-NetIPAddress -InterfaceAlias "Ethernet 3" -IPAddress 192.168.56.101 -PrefixLength 24
	New-NetIPAddress -InterfaceAlias "Ethernet 3" -IPAddress 192.168.56.102 -PrefixLength 24
	New-NetIPAddress -InterfaceAlias "Ethernet 3" -IPAddress 192.168.56.103 -PrefixLength 24
	New-NetIPAddress -InterfaceAlias "Ethernet 3" -IPAddress 192.168.56.104 -PrefixLength 24

	ipconfig
	//Открыть UDP порт 5555
	netsh advfirewall firewall add rule name="Lab07 UDP 5555" dir=in action=allow protocol=UDP localport=5555


nodes.txt:
192.168.56.101
192.168.56.102
192.168.56.103

config.txt:
192.168.56.101

Запуск:
.\ServerU.exe 192.168.56.101
.\ServerU.exe 192.168.56.102
.\ServerU.exe 192.168.56.103

.\ServerU_Agent.exe 192.168.56.104
.\ClientU.exe 192.168.56.104


192.168.56.101
192.168.56.102
192.168.56.104















