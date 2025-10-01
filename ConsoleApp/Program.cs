using ConsoleApp;

Console.Clear();
Console.WriteLine("\n   --==> Trotting Spider <==--\n");

var app = new App();
await app.RunTrottingNoAsync();

/*
 
static async Task Worker(int index, int delay)
{
    Console.WriteLine($"Starting worker: {index}");
    await Task.Delay(millisecondsDelay: delay);
    Console.WriteLine($"Worker finished:{index}");
}

static async Task Run(int index, int delay, SemaphoreSlim semaphore)
{
    await semaphore.WaitAsync();
    try
    {
        await Worker(index, delay);
    }
    finally
    {
        semaphore.Release();
    }
}

using var semaphore = new SemaphoreSlim(5, 5);
var random = new Random();
var tasks = new List<Task>();

for (var i = 0; i < 50; i++)
{
    int d = random.Next(500, 5000);
    tasks.Add(Run(i,d, semaphore));
}

await Task.WhenAll(tasks);

*/