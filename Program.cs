using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private static SemaphoreSlim semaforo = new SemaphoreSlim(3);
    private static Queue<int> numeros = new Queue<int>();

    static async Task Main()
    {
        // Llenar la cola con los números
        for (int i = 1; i <= 20; i++)
        {
            numeros.Enqueue(i);
        }

        // Crear 5 tareas concurrentes
        Task[] tareas = new Task[5];
        for (int i = 0; i < 5; i++)
        {
            tareas[i] = Task.Run(async () =>
            {
                await ProcesarNumeros();
            });
        }

        // Esperar a que todas las tareas terminen
        await Task.WhenAll(tareas);

        Console.WriteLine("Todos los procesos han terminado.");
    }

    private static async Task ProcesarNumeros()
    {
        int numero = ObtenerSiguienteNumero();
        while (numero != -1)
        {
            await semaforo.WaitAsync(); // Espera para adquirir el semáforo
            try
            {
                Console.WriteLine($"Proceso {Thread.CurrentThread.ManagedThreadId}: Tomó el número {numero}");
                await Task.Delay(TimeSpan.FromSeconds(numero));
                Console.WriteLine($"Proceso {Thread.CurrentThread.ManagedThreadId}: Esperó {numero} segundos");
            }
            finally
            {
                semaforo.Release(); // Libera el semáforo cuando termine
            }
            numero = ObtenerSiguienteNumero();
        }
    }

    private static int ObtenerSiguienteNumero()
    {
        if (numeros.Count > 0)
        {
            return numeros.Dequeue();
        }
        else
        {
            return -1; // Se han procesado todos los números
        }
    }
}
