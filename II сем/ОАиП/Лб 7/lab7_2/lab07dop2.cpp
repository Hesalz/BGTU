#include <iostream>
#include <cmath>
#include <string>
#include <fstream>
#include <vector>

using namespace std;
struct stack
{
	int ind;
	stack* next;
};
void length(stack** el, int n)
{
	stack* i = new stack;
	i->ind = n;
	i->next = *el;
	*el = i;
}

void out2(vector<int> len) {
	for (int i = 0; i < len.size(); i++)
	{
		cout << len[i] << '\n';
	}
}

void out(stack* el)
{
	stack* i = el;
	if (el == NULL)
	{
		cout << "Список пуст";
		return;
	}
	while (i != NULL)
	{
		cout << i->ind << endl;
		i = i->next;
	}
}
int main2()
{
	int min = 256, j = 0;
	setlocale(LC_CTYPE, "Russian");
	stack* el = NULL;
	ifstream file1("1.txt");
	char str[255];
	int n;
	vector<int> len;
	while (!file1.eof())
	{
		file1.getline(str, 255);
		n = strlen(str);

		

		length(&el, n);
		len.push_back(n);

		if (n < min)
		{
			min = n;
			j++;
		}
	}
	file1.close();
	out(el);
	cout << endl;
	//out2(len);
	cout << endl;
	cout << "Длина самой маленькой строки:" << min << endl;
	cout << "Её номер:" << j + 1 << endl;
	return 0;
}