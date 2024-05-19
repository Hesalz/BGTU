#include "List.h"
#include <iostream>
struct AAA
{
	int key;
	char* mas;
	char* name;
};
namespace listx
{
	bool Object::insert(void* data)
	{
		bool rc = NULL;
		if (head == NULL)
			head = new Element(NULL, data, head);
		else
			head = (head->prev = new Element(NULL, data, head));
		return rc;
	}

	Element* Object::search(void* data)
	{
		Element* rc = head;
		while ((rc != NULL) && ((((AAA*)rc->data)->key) != ((AAA*)data)->key))
			rc = rc->next;
		return rc;
	}

	bool Object::deleteByElement(Element* e)
	{
		bool rc = NULL;
		if (rc = (e != NULL))
		{
			if (e->next != NULL)
				e->next->prev = e->prev;
			if (e->prev != NULL)
				e->prev->next = e->next;
			else
				head = e->next;
			delete e;
		}
		std::cout << "Элемент удалён" << std::endl;
		return rc;
	}

	bool Object::deleteByData(void* data)
	{
		return deleteByElement(search(data));
	}

	Element* Object::getLast()
	{
		listx::Element* e = this->getFirst(), * rc = this->getFirst();
		while (e != NULL)
		{
			rc = e;
			e = e->getNext();
		};
		return rc;
	}
	Object create()
	{
		return *(new Object());
	};

	void Object::scan()
	{
		listx::Element* e = this->getFirst();
		bool k = 0;
		while (e != NULL)
		{
			std::cout << "Год создания университета: " << ((AAA*)e->data)->key << "; " << "ФИО : " << ((AAA*)e->data)->mas << "; " << "Название университета : " << ((AAA*)e->data)->name << "; ";
			e = e->getNext();
			k = 1;
		};
		if (!k)
		{
			std::cout << " - ";
		}
	}
}