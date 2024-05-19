#include <iostream>
#include <vector>
#include "ctime"

using namespace std;

void Swap(int* a, int* b)
{
    int temp = *a;
    *a = *b;
    *b = temp;
}

void BubbleSort(std::vector<int>& array) {
    for (int i = 0; i < array.size(); i++)
    {
        for (int j = 0; j < array.size() - 1; j++)
        {
            if (array[j] < array[j + 1])
                Swap(&array[j], &array[j + 1]);
        }
    }
}

void PrintArray(std::vector<int> array)
{
    for (int i = 0; i < array.size(); i++)
        cout << array[i] << " ";
        cout << std::endl;
}


void shellSort(vector<int>& A)
{
    int i, j, d;
    d = A.size();

    d = d / 2;
    while (d > 0)
    {
        for (i = 0; i < A.size() - d; i++)
        {
            j = i;
            while (j >= 0 && A[j] < A[j + d])
            {
                Swap(&A[j], &A[j + d]);
                j--;
            }
        }
        d = d / 2;
    }
}


int main()
{
    setlocale(LC_ALL, "rus");
    srand(time(NULL));
    int a_len, b_len;
    cout << "Введите размер массива А\n";
    cin >> a_len;

    cout << "Введите размер массива В\n";
    cin >> b_len;

    vector<int> A;
    vector<int> B;
    vector<int> array1;
    vector<int> array2;

    for (int i = 0; i < a_len; i++)
    {
        A.push_back(rand() % 100 - 50 + 1);
    }

    for (int i = 0; i < b_len; i++)
    {
        B.push_back(rand() % 100 -50 + 1);
    }


    for (int i = 0; i < a_len; i++)
    {
        if (A[i] % 2 != 0) {
            array1.push_back(A[i]);
            array2.push_back(A[i]);
        }
    }

    for (int i = 0; i < b_len; i++)
    {
        if (B[i] % 2 == 0) {
            array1.push_back(B[i]);
            array2.push_back(B[i]);
        }
    }
    cout << "Массив A \n";
    PrintArray(A);
    cout << "Массив B \n";
    PrintArray(B);
    cout << "Массив C \n";
    PrintArray(array1);
 
    clock_t start_time_1 = clock();
    BubbleSort(array2);
    clock_t end_time_1 = clock();

    cout << "Время пузырьковой сортировки: " << end_time_1 - start_time_1 <<" ms \n";
    cout << "Отсортированный массив пузырьком\n";
    PrintArray(array2);

    unsigned int start_time_2 = clock();
    shellSort(array1);
    unsigned int end_time_2 = clock();

    cout << "Время пирамидальной сортировки: " << end_time_2 - start_time_2 << " ms \n";

    cout << "Отсортированный массив пирамидально\n";
    PrintArray(array1);

    return 0;
}