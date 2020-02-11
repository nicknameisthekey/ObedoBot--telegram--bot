using Telegram.Bot.Types.ReplyMarkups;
class BotResponces
{
    public static readonly string ReqFreeTime = "RequestFreeTime";
    public static readonly string ReturnedToWork = "ReturnedToWork";
    public static readonly string ShowFreeTimers = "ShowFreeTimers";
    public static InlineKeyboardMarkup HomeMarkup { get; } = new InlineKeyboardMarkup(
                new[]
                {
                        new [] //row1
                        {
                            InlineKeyboardButton.WithCallbackData("Иду на обед!", ReqFreeTime)
                        },
                        new [] //row2
                        {
                             InlineKeyboardButton.WithCallbackData("Посмотреть кто на обеде", ShowFreeTimers)
                        }
                });
    public static InlineKeyboardMarkup OnFreeTime { get; } = new InlineKeyboardMarkup(
        new[]
                {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("Закончил обедать", ReturnedToWork)
                        },
                        new []
                        {
                             InlineKeyboardButton.WithCallbackData("Посмотреть кто на обеде", ShowFreeTimers)
                        }
                });
}
