using System.Net;

Console.Write("Toplam indirilecek resim sayısını giriniz: ");
int count = GetValidInput();

Console.Write("Aynı anda kaç resim indirilmesine izin verileceğini belirtin: ");
int parallelism = GetValidInput();

Console.WriteLine("İndirme işlemi başlıyor...");

DownloadImages(count, parallelism);

Console.WriteLine("İşlem tamamlandı.");
Console.ReadLine();
static int GetValidInput()
{
    int input;
    while (!int.TryParse(Console.ReadLine(), out input) || input <= 0)
    {
        Console.Write("Geçerli bir sayı giriniz: ");
    }
    return input;
}

static void DownloadImages(int count, int parallelism)
{
    List<Task> tasks = new List<Task>();
    string folderPath = "DownloadedImages";
    Directory.CreateDirectory(folderPath);

    for (int i = 1; i <= count; i++)
    {
        int index = i;

        Task task = Task.Run(async () =>
        {
            string imageUrl = $"https://picsum.photos/200/300?random={Guid.NewGuid()}";
            string fileName = index + ".png";
            string filePath = Path.Combine(folderPath, fileName);

            using (WebClient client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(imageUrl), filePath);
            }

            Console.WriteLine("İndirilen resim: {0}", index);
        });

        tasks.Add(task);

        if (tasks.Count % parallelism == 0)
        {
            Task.WaitAny(tasks.ToArray());
            tasks.RemoveAll(t => t.IsCompleted);
        }
    }

    Task.WaitAll(tasks.ToArray());
}