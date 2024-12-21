# Лабораторная работа №4
## MPI.NET
Проект реализует умножение матриц с использованием библиотеки MPI (Message Passing Interface) на языке C#. 
Программа распределяет вычисления между несколькими процессами, что позволяет эффективно использовать многопоточность и ускорять операции над большими матрицами.
``` csharp
using MPI;
using System.Diagnostics;
class Program
{
    static void Main(string[] args)
    {
        var sw = new Stopwatch();
        MPI.Environment.Run(ref args, communicator =>
        {
            int rank = communicator.Rank;
            int size = communicator.Size;
            int rowsA = 10; 
            int colsA = 10; 
            int colsB = 10; 
            int[] matrixA = new int[rowsA * colsA];
            int[] matrixB = new int[colsA * colsB];
            int[] matrixC = new int[rowsA * colsB];

            if (rank == 0)
            {
                sw.Start();
                int countA = 10;
                int countB = 1;
                for (int i = 0; i < rowsA; i++)
                {
                    for (int j = 0; j < colsA; j++)
                    {
                        matrixA[i * colsA + j] = countA++; 
                    }
                }

                for (int i = 0; i < colsA; i++)
                {
                    for (int j = 0; j < colsB; j++)
                    {
                        matrixB[i * colsB + j] = countB++; 
                    }
                }

                for (int i = 1; i < size; i++)
                {
                    foreach (int item in matrixA)
                    {
                        communicator.Send(item, i, 0);
                    }

                    foreach (int item in matrixB)
                    {
                        communicator.Send(item, i, 1);
                    }
                }
            }
            else
            {
                for (int i = 0; i < rowsA; i++)
                {
                    for (int j = 0; j < colsA; j++)
                    {
                        matrixA[i * colsA + j] = communicator.Receive<int>(0, 0);
                    }
                }
                for (int i = 0; i < colsA; i++)
                {
                    for (int j = 0; j < colsB; j++)
                    {
                        matrixB[i * colsB + j] = communicator.Receive<int>(0, 1);
                    }
                }
            }

            int rowsPerProcess = rowsA / size;
            int startRow = rank * rowsPerProcess;
            int endRow = (rank + 1) * rowsPerProcess;

            for (int i = startRow; i < endRow; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    matrixC[i * colsB + j] = 0;
                    for (int k = 0; k < colsA; k++)
                    {
                        matrixC[i * colsB + j] += matrixA[i * colsA + k] * matrixB[k * colsB + j];
                    }
                }
            }

            if (rank == 0)
            {
                for (int i = 1; i < size; i++)
                {
                    for (int row = i * rowsPerProcess; row < (i + 1) * rowsPerProcess; row++)
                    {
                        for (int col = 0; col < colsB; col++)
                        {
                            matrixC[row * colsB + col] = communicator.Receive<int>(i, 2);
                        }
                    }
                }
            }
            else
            {
                for (int i = startRow; i < endRow; i++)
                {
                    for (int j = 0; j < colsB; j++)
                    {
                        communicator.Send(matrixC[i * colsB + j], 0, 2);
                    }
                }
            }

            if (rank == 0)
            {
                Console.WriteLine("Результирующая матрица:");
                for (int i = 0; i < rowsA; i++)
                {
                    for (int j = 0; j < colsB; j++)
                    {
                        Console.Write(matrixC[i * colsB + j] + "\t");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine($"Elapsed {sw.Elapsed}.");
            }

        });
    }
}
```
### Основные компоненты кода:
* Инициализация MPI: 

``` csharp
MPI.Environment.Run(ref args, communicator =>
{
    int rank = communicator.Rank;
    int size = communicator.Size;
```

Здесь мы запускаем среду MPI и получаем rank (идентификатор процесса) и size (общее количество процессов).

* Определение размеров матриц:

``` csharp
int rowsA = 10; 
int colsA = 10; 
int colsB = 10;
```
Мы определяем размеры матриц A и B. Матрица A имеет размерность rowsA x colsA, а матрица B — colsA x colsB.

* Инициализация матриц:
``` csharp
int[] matrixA = new int[rowsA * colsA];
int[] matrixB = new int[colsA * colsB];
int[] matrixC = new int[rowsA * colsB];
```
Мы создаем одномерные массивы для хранения матриц A, B и C.

* Заполнение матриц:

``` csharp
if (rank == 0)
{
    // Заполнение матрицы A
    for (int i = 0; i < rowsA; i++)
    {
        for (int j = 0; j < colsA; j++)
        {
            matrixA[i * colsA + j] = countA++; 
        }
    }

    // Заполнение матрицы B
    for (int i = 0; i < colsA; i++)
    {
        for (int j = 0; j < colsB; j++)
        {
            matrixB[i * colsB + j] = countB++; 
        }
    }
}
```
Процесс с rank 0 заполняет матрицы A и B значениями.

* Отправка матриц другим процессам:

``` csharp
for (int i = 1; i < size; i++)
{
    foreach (int item in matrixA)
    {
        communicator.Send(item, i, 0);
    }

    foreach (int item in matrixB)
    {
        communicator.Send(item, i, 1);
    }
}
```
Процесс с rank 0 отправляет матрицы A и B всем другим процессам.

* Получение матриц другими процессами:

``` csharp
else
{
    for (int i = 0; i < rowsA; i++)
    {
        for (int j = 0; j < colsA; j++)
        {
            matrixA[i * colsA + j] = communicator.Receive<int>(0, 0);
        }
    }
    for (int i = 0; i < colsA; i++)
    {
        for (int j = 0; j < colsB; j++)
        {
            matrixB[i * colsB + j] = communicator.Receive<int>(0, 1);
        }
    }
}
```
Все процессы, кроме rank 0, получают матрицы A и B.

* Умножение матриц:

```csharp
int rowsPerProcess = rowsA / size;
int startRow = rank * rowsPerProcess;
int endRow = (rank + 1) * rowsPerProcess;

for (int i = startRow; i < endRow; i++)
{
    for (int j = 0; j < colsB; j++)
    {
        matrixC[i * colsB + j] = 0;
        for (int k = 0; k < colsA; k++)
        {
            matrixC[i * colsB + j] += matrixA[i * colsA + k] * matrixB[k * colsB + j];
        }
    }
}
```
Каждый процесс вычисляет свою часть результирующей матрицы C.

* Сбор результатов:

``` csharp
if (rank == 0)
{
    for (int i = 1; i < size; i++)
    {
        for (int row = i * rowsPerProcess; row < (i + 1) * rowsPerProcess; row++)
        {
            for (int col = 0; col < colsB; col++)
            {
                matrixC[row * colsB + col] = communicator.Receive<int>(i, 2);
            }
        }
```
### Запуск программы:
Для запуска программы необходимо установить MS-MPI. Это можно сделать на официальном сайте Microsoft
https://learn.microsoft.com/en-us/message-passing-interface/microsoft-mpi#ms-mpi-source-code
![image](https://github.com/user-attachments/assets/7307e17e-8e13-4cbe-bc58-522854eba550)
### Результат работы программы: 
![image](https://github.com/user-attachments/assets/b20e7eb0-03a9-48a2-b7e1-10c93aea8997)
