using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;
using System;
using static System.Net.WebRequestMethods;

int num = Convert.ToInt32(System.IO.File.ReadAllText("data/number/number1.txt"));
bool flag = false;
bool search=false;

var botClient = new TelegramBotClient("7160449716:AAHxUj3qrTo3SuhynedGIke49Pfcw4E2Sxc");

using CancellationTokenSource cts = new();

//набор стикеров
string[] stickers = { "CAACAgIAAxkBAAErAAGBZibZod0GCakuy8I-kgXEj5QGnFIAAu02AAK7OsFLGmxjty5F5es0BA",
        "CAACAgIAAxkBAAErAAGDZibZuAP-Nej3Rqwa-tbjogvnH-oAAns2AAIm9LlLEO5Phm0xN3s0BA",
        "CAACAgIAAxkBAAErAAGFZibZ2utcQo6PJO5Fg2KHN7OKJA8AAow6AALSyrlLOGkqjroRcWE0BA",
        "CAACAgIAAxkBAAErAAGHZibaG0nBX8JVeqzL-naJ64Wq1okAAkc2AAKiVMFLvvOTgHL5Bqk0BA",
    "CAACAgIAAxkBAAErAAGJZibaRo_yjrpvzWYpqhn0fbZQjvUAAkQ3AAJV5blL9Xm52_DKRvo0BA",
    "CAACAgIAAxkBAAErAAGLZibaVQ5gTZKS4ZdTPgAB8hgrfs4WAAL7NQACDOu5SygrV9J2YC4YNAQ",
    "CAACAgIAAxkBAAErAAGNZibaZslFnI1U0ugsKE1_ytnLMlQAAlo3AAIxg8FLPSaRHpiua_c0BA"};


// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();



// Send cancellation request to stop bot
cts.Cancel();
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{

    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Regex reg = new Regex(@"^/[Aa][Dd][Dd]\s\w+");

    //добавить
     if (flag == true && !string.IsNullOrWhiteSpace(update.Message.Text))
    {
        flag = false;
        num += 1;
        System.IO.File.WriteAllText("data/number/number1.txt", $"{num}");

        System.IO.File.WriteAllText("data/joke" + (num) + ".txt", $"№{num}\n{messageText}\n#{message.Chat.Id}");

        await botClient.SendTextMessageAsync(message.Chat.Id, $"{num}");

        Random rnd = new Random();
        var ind = rnd.Next(0, 7);
        Message message2 = await botClient.SendStickerAsync(
    chatId: chatId,
    sticker: InputFile.FromFileId(stickers[ind]),
    cancellationToken: cancellationToken);

    }

     /*
    else if (flag == true && string.IsNullOrWhiteSpace(update.Message.Text))
    {
        flag = false;
        await botClient.SendTextMessageAsync(message.Chat.Id, "А текст где?");
        Message message2 = await botClient.SendStickerAsync(
    chatId: chatId,
    sticker: InputFile.FromFileId("CAACAgIAAxkBAAErAAFpZibSkr-AFZTezdQyVSkHcsjyHToAAuMAA1eEbA_LD3f9g8IMvDQE"),
    cancellationToken: cancellationToken);

    }
     */

    else if (message.Text.ToLower() == "/add")
    {
        flag = true;
        await botClient.SendTextMessageAsync(message.Chat.Id, "Введите анекдот:"); 
    }

     //Поиск
     if(search==true && !string.IsNullOrWhiteSpace(update.Message.Text))
    {
        search = false;
        string path = "data/joke" + message.Text + ".txt";
        if (System.IO.File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string text = await reader.ReadToEndAsync();
                if (text.Contains("#"))
                {
                    text = text.Remove(text.LastIndexOf("#"));

                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{text}");
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{text}");
                   
                }
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Такого анекдота нет");
            Message message2 = await botClient.SendStickerAsync(
   chatId: chatId,
   sticker: InputFile.FromFileId("CAACAgIAAxkBAAErNRlmNV_GA43eQ8k145qzzTT7RVARIAAC5AADV4RsD_xAm28YM60zNAQ"),
   cancellationToken: cancellationToken);
        }
        
    }
    //Поиск
    else if (message.Text.ToLower() == "/поиск")
    {
        search = true;
        botClient.SendTextMessageAsync(message.Chat.Id, "Введите номер анекдота:");
    }
    else
    {
        search = false;
    }

    

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
    if(message.Text == "/start")
    {
        Message message2 = await botClient.SendStickerAsync(
        chatId: chatId,
        sticker: InputFile.FromFileId("CAACAgIAAxkBAAErAAF9ZibWzp3f92PJTdfreSyV5mp4I7gAAhM3AAKPyblLgJqiExo9bg00BA"),
        cancellationToken: cancellationToken);
       /* string path1 = "data/instruction.txt";
        using (StreamReader reader = new StreamReader(path1))
        {
            string text = await reader.ReadToEndAsync();
            botClient.SendTextMessageAsync(message.Chat.Id, $"{text}");
        }*/


        //Клавиатура
        var replyKeyboard = new ReplyKeyboardMarkup(
              new List<KeyboardButton[]>()
                   {
                       new KeyboardButton[]
                                   {
                                      new KeyboardButton("/Анекдот"),
                                      new KeyboardButton("/Инструкция"),
                                    },
                                     new KeyboardButton[]
                                   {
                                     new KeyboardButton("/Add")
                                     },
                              new KeyboardButton[]
                              {
                              new KeyboardButton("/Поиск")
                               }
                                })
        {
            ResizeKeyboard = true,
        };
        await botClient.SendTextMessageAsync(
                                       message.Chat.Id,
                                       "/Aнекдот - выводит анекдот\r\n/Add - добавить анекдот\r\n/Инструкция - выводит инструкцию\r\n/Поиск - поиск анекдота по номеру", //инструкция
                                       replyMarkup: replyKeyboard);

        return;
    }

    //анекдот
    else if (message.Text.ToLower() == "/анекдот")
    {
        Random rnd = new Random();
        int n = rnd.Next(1, num+1);
        string path = "data/joke"+ n +".txt";

        using (StreamReader reader = new StreamReader(path))
        {
            string text = await reader.ReadToEndAsync();
            if(text.Contains("#"))
            {
                text = text.Remove(text.LastIndexOf("#"));

                await botClient.SendTextMessageAsync(message.Chat.Id, $"{text}");
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, $"{text}");
            }
        }
    }
    //add
    else if (reg.IsMatch(message.Text))
    {
        var text= message.Text.Trim();
        text = text.Substring(4);
        text = text.Trim();
        //пустой анекдот
        if (text == "")
        {
            flag = true;

            
            botClient.SendTextMessageAsync(message.Chat.Id, "А текст где?");
            Message message2 = await botClient.SendStickerAsync(
        chatId: chatId,
        sticker: InputFile.FromFileId("CAACAgIAAxkBAAErAAFpZibSkr-AFZTezdQyVSkHcsjyHToAAuMAA1eEbA_LD3f9g8IMvDQE"),
        cancellationToken: cancellationToken);
            
       }

        else
        {
            num += 1;
            System.IO.File.WriteAllText("data/number/number1.txt", $"{num}");

            System.IO.File.WriteAllText("data/joke" + (num) + ".txt", $"№{num}\n{text}\n#{message.Chat.Id}");

            botClient.SendTextMessageAsync(message.Chat.Id, $"{num}");

            Random rnd = new Random();
            var ind= rnd.Next(0, 7);
            Message message2 = await botClient.SendStickerAsync(
        chatId: chatId,
        sticker: InputFile.FromFileId(stickers[ind]),
        cancellationToken: cancellationToken);
           
        }
    }

    //Вызов инструкции
    else if (message.Text.ToLower() == "/инструкция")
    {
        string path1 = "data/instruction.txt";
        using (StreamReader reader = new StreamReader(path1))
        {
            string text = await reader.ReadToEndAsync();
            botClient.SendTextMessageAsync(message.Chat.Id, $"{text}");
        }
    }

    
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
//Цензура


//мемы в картинках

//темы анекдотов


//ограничение по id

//рейтинг анекдотов

//рейтинг пользователей


//id пользователя а не чата

//что-то типо фильтрации???