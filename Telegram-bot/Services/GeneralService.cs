using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram_bot.Services
{
    public class GeneralService
    {
        public class ListId
        {
            public int IdList { get; set; }
            public int IdDB { get; set; }
        }
        public static string MainMenu = "<b>Выберите действие:</b>\n   Достопримечательности\n   Мероприятия\n   Гостиницы\n   Места общепита\n   Сувениры\n   Обратная связь\n   Индивидуальные маршруты";
        public static async Task MainMenuShow(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken, KeyboardService _keyboardService)
        {
            await botClient.SendTextMessageAsync(
                        chatId,
                        MainMenu,
                        parseMode: ParseMode.Html,
                        replyMarkup: _keyboardService.GetMainMenuKeyboard(),
                        cancellationToken: cancellationToken);
        }
        public string mediaPath { get; set; }
        public GeneralService()
        {
            mediaPath = GetMediaPathFromConfig();
            Console.WriteLine(mediaPath);
        }
        public string GetMediaPathFromConfig()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (!System.IO.File.Exists(configPath))
                {
                    Console.WriteLine($"Файл конфигурации не найден: {configPath}");
                    return FindMediaFolder();
                }

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                string mediaPathFromConfig = configuration["MediaSettings:MediaFolderPath"];

                if (string.IsNullOrEmpty(mediaPathFromConfig))
                {
                    Console.WriteLine("Путь к медиафайлам не найден в конфигурации");
                    return FindMediaFolder();
                }
                Console.WriteLine($"Путь из конфигурации: {mediaPathFromConfig}");

                if (Directory.Exists(mediaPathFromConfig))
                {
                    return mediaPathFromConfig;
                }
                else
                {
                    return FindMediaFolder();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении конфигурации: {ex.Message}");
                return FindMediaFolder();
            }
        }
        public string FindMediaFolder()
        {
            string curDir = Directory.GetCurrentDirectory();
            for (int i = 0; i < 5; i++)
            {
                string mediaPath = Path.Combine(curDir, "data_media_visit_center");
                if (Directory.Exists(mediaPath))
                    return mediaPath;
                DirectoryInfo parent = Directory.GetParent(curDir);
                if (parent == null) break;
                curDir = parent.FullName;
            }
            string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data_media_visit_center");
            if (!Directory.Exists(defaultPath))
                Directory.CreateDirectory(defaultPath);
            return defaultPath;
        }
        public async Task SendPhoto(List<string> photoLinks, ITelegramBotClient botClient, ChatId chatId, string caption = "", CancellationToken cancellationToken = default)
        {
            if (photoLinks == null || !photoLinks.Any())
            {
                Console.WriteLine("Нет фото");
                return;
            }
            int sentCount = 0;
            for (int i = 0; i < photoLinks.Count; i++)
            {
                string photo = photoLinks[i];
                string path = Path.Combine(mediaPath, photo);
                if (!System.IO.File.Exists(path))
                {
                    continue;
                }
                var fileInfo = new FileInfo(path);
                bool sent = false;
                int maxRetries = 3;

                for (int attempt = 1; attempt <= maxRetries && !sent; attempt++)
                {
                    FileStream fileStream = null;
                    try
                    {
                        Console.WriteLine($"Отправка {i + 1}/{photoLinks.Count}: {photo} ({fileInfo.Length} байт) - попытка {attempt}/{maxRetries}");
                        fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true);
                        int timeoutSeconds = 60 * attempt;
                        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
                        var inputFile = new InputFileStream(fileStream, Path.GetFileName(path));
                        await botClient.SendPhotoAsync(
                            chatId,
                            photo: inputFile,
                            cancellationToken: cancellationToken);
                        sent = true;
                        sentCount++;
                        Console.WriteLine($"Фото {i + 1} отправлено (попытка {attempt})");
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"Таймаут при отправке фото {i + 1} (попытка {attempt})");
                        if (attempt == maxRetries)
                        {
                            Console.WriteLine($"Фото {i + 1} не отправлено");
                        }
                        else
                        {
                            int delaySeconds = 2 * attempt;
                            await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при отправке фото {i + 1} (попытка {attempt}) : {ex.Message}");
                        if (attempt == maxRetries)
                            Console.WriteLine($"Фото {i + 1} не отправлено");
                        else await Task.Delay(1000, cancellationToken);
                    }
                    finally
                    {
                        if (fileStream != null)
                            await fileStream.DisposeAsync();
                    }
                }
                //задержка между фото
                if (i < photoLinks.Count - 1) await Task.Delay(1000, cancellationToken);
            }

        }
    }
}
