using System;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;

class Program
{
    static string token = "";
    static TelegramBotClient bot;
    static string proxy = "";
    static int maxFreePeople = 2;
    static People[] freeWorkers;
    static void Main(string[] args)
    {
        WebProxy webProxy = new WebProxy(proxy, true);
        bot = new TelegramBotClient(token, webProxy);
        checkBotOnline();
        freeWorkers = new People[maxFreePeople];
        bot.OnMessage += botOnMessageRecived;
        bot.OnCallbackQuery += botOnCallbackQuery;
        bot.StartReceiving();
        Console.WriteLine("Нажми любую кнопку чтобы выключить бота");
        Console.ReadLine();
        bot.StopReceiving();
    }
    static async void botOnMessageRecived(object sender, MessageEventArgs message)
    {
        if (message.Message.Text == "/btns")
        {
            await bot.SendTextMessageAsync(message.Message.Chat.Id,
                "Выбери действие:\n",
                replyMarkup: BotResponces.HomeMarkup);
        }
    }
    static async void botOnCallbackQuery(object sender, CallbackQueryEventArgs args)
    {
        var callback = args.CallbackQuery;
        //запрос обеда
        if (callback.Data == BotResponces.ReqFreeTime)
        {
            People worker = new People(callback.From.FirstName, callback.From.Id);
            //проверка на то, что работник уже на обеде
            if (checkIfWorkerFree(worker.Id))
            {
                await bot.SendTextMessageAsync(callback.From.Id,
              $"{callback.From.FirstName}, ты уже на обеде",
              replyMarkup: BotResponces.OnFreeTime);
                return;
            }
            //попытка отправить на обеда
            if (tryMakeWorkerFree(worker))
            {
                await bot.SendTextMessageAsync(callback.From.Id,
               $"{callback.From.FirstName}, ты ушел на обед",
               replyMarkup: BotResponces.OnFreeTime);
            }
            //мест нет
            else
            {
                await bot.SendTextMessageAsync(callback.From.Id,
              $"{callback.From.FirstName}, пока нет слотов на обед",
              replyMarkup: BotResponces.HomeMarkup);
            }
        }
        //возврат с работы
        else if (callback.Data == BotResponces.ReturnedToWork)
        {
            if (removeWorkerFromFree(callback.From.Id))
            {
                await bot.SendTextMessageAsync(callback.From.Id,
                    $"{callback.From.FirstName}, ты вернулся с обеда",
                    replyMarkup: BotResponces.HomeMarkup);
            }
            else
            {
                await bot.SendTextMessageAsync(callback.From.Id,
                    $"{callback.From.FirstName}, тебя не было на обеде",
                    replyMarkup: BotResponces.HomeMarkup);
            }
        }
        else if (callback.Data == BotResponces.ShowFreeTimers)
        {
            string allppls = "";
            int amount = 0;
            foreach (var worker in freeWorkers)
            {
                if (worker != null)
                {
                    amount++;
                    allppls += $"{worker.Name} \n";
                }
            }
            await bot.SendTextMessageAsync(callback.From.Id,
                    $"сейчас на обеде {amount} человек \n {allppls}");
        }
        else
        {
            await bot.SendTextMessageAsync(callback.From.Id,
                   $"выбери действие",
                   replyMarkup: BotResponces.HomeMarkup);
        }
    }
    static bool checkIfWorkerFree(int id)
    {
        for (int i = 0; i < freeWorkers.Length; i++)
            if (freeWorkers[i] != null && freeWorkers[i].Id == id)
                return true;
        return false;
    }
    static bool tryMakeWorkerFree(People worker)
    {
        for (int i = 0; i < freeWorkers.Length; i++)
        {
            if (freeWorkers[i] == null)
            {
                freeWorkers[i] = worker;
                Console.WriteLine($"id {worker.Id}, {worker.Name} ушел на обед");
                return true;
            }
            else if (freeWorkers[i].Id == worker.Id)
            {
                Console.WriteLine($"ошибка, id {worker.Id}, {worker.Name} уже на обеде");
                break;
            }
        }
        return false;
    }
    static bool removeWorkerFromFree(int id)
    {
        for (int i = 0; i < freeWorkers.Length; i++)
        {
            if (freeWorkers[i] != null && freeWorkers[i].Id == id)
            {
                Console.WriteLine($"id {id}, {freeWorkers[i].Name} вернулся с обеда");
                freeWorkers[i] = null;
                return true;
            }
        }
        Console.WriteLine($"id {id}, не был на обеде");
        return false;
    }
    static async void checkBotOnline()
    {
        var botinfo = await bot.GetMeAsync();
        Console.WriteLine(botinfo.FirstName);
    }
}

