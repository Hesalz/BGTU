#pragma once
#define LISTNIL (Element*)-1
namespace listx
{
	struct Element
	{
		Element* prev;                                             //Указатель на предыдущий элемент списка
		Element* next;                                             //Указатель на следующий элемент списка
		void* data;                                                //Поле, содержащее данные
		Element(Element* prevO, void* dataO, Element* nextO)       //Конструктор создания списка
		{
			prev = prevO;
			data = dataO;
			next = nextO;
		}
		Element* getNext()                                         //Метод получения указателя на следующий элемент списка
		{
			return next;
		};
		Element* getPrev()                                         //Метод получения указателя на предыдущий элемент списка
		{
			return prev;
		};
	};
	static Element* NIL = 0;                                       //Создание пустого списка как элемент таблицы                    
	struct Object
	{
		Element* head;
		Object()
		{
			head = NIL;                                           //Создание указателей на списки-члены таблицы
		};
		Element* getFirst()                                       //Метод получения указателя на первый список-элемент таблицы
		{
			return head;
		};
		Element* getLast();                                       //Метод получения указателя на последний список-элемент таблицы
		Element* search(void* data);                              //Метод поиска списка-элемента таблицы
		bool insert(void* data);                                  //Метод вставки нового списка-элемента в таблицу
		bool deleteByElement(Element* e);						  //Метод удаления списка-элемента таблицы
		bool deleteByData(void* data);							  //Метод удаления элемента списка-элемента таблицы по значению информации 
		void scan();											  //Метод обхода всех списков-элементов и их самостоятельных элементов
	};
	Object create();
}
#undef LISTNIL 