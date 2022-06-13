using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace WantedPersonsBot
{
    public static class TelegramBotMethods
    {
        public static List<WantedPerson> wPersons { get; set; }
        public static List<Photo> photos { get; set; }

        private static List<string> onAddMessagesList = new List<string>() {
            "Введіть ПІБ особи",
            "Введіть дату народження особи",
            "Введіть стать особи",
            "Введіть дату з якої розшукують особу",
            "Введіть назву місця зникнення особи",
            "Введіть назву ОВС, що розшукує особу",
            "Введіть назву категорії розшукуваної особи",
            "Введіть назву статті, за яку розшукують особу",
            "Введіть міру покарання для особи",
            "Введіть контакти для повідомлення"
        };

        private static List<string> onAddSavedMessagesList = new List<string>(onAddMessagesList.Count);

        private static int currOnAddItem = 0;

        private static State currState = State.Init;

        private enum State {
            Init,
            FindByName,
            FindByDate,
            Add
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            UpdateType updType = update.Type;

            switch (updType) 
            {
                case UpdateType.Message:
                    {
                        if (update.Message.Type != MessageType.Text) return;
                        var chatId = update.Message.Chat.Id;
                        string msgText = update.Message.Text;

                        Console.WriteLine($"Received a '{msgText}' message in chat {chatId}.");

                        string botTextResponse = String.Empty;

                        switch (msgText)
                        {
                            case "/start":
                                {
                                    var replyKeyboardMarkup = FormKeybord(new string[][] { new string[] { "Пошук", "Додати особу"} });
                                    botTextResponse = "Цей бот використовується для пошуку осіб, що переховуються від влади.\n" +
                                        "Напишіть /help щоб отримати інформацію про команди бота.";
                                    await botClient.SendTextMessageAsync(chatId: chatId, text: botTextResponse,
                                        parseMode: ParseMode.Html, replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);

                                    currState = State.Init;
                                    break;
                                }
                            case "/help":
                                {
                                    BotCommand[] cmd = await botClient.GetMyCommandsAsync();
                                    foreach (BotCommand item in cmd)
                                        botTextResponse += "/" + item.Command + " - " + item.Description + "\n";
                                    await botClient.SendTextMessageAsync(chatId: chatId, text: botTextResponse,
                                        parseMode: ParseMode.Html, cancellationToken: cancellationToken);
                                    currState = State.Init;
                                    break;
                                }
                            case "Пошук":
                                {
                                    var replyKeyboardMarkup = FormKeybord(new string[][] { new string[] { "За ПІБ", "За датою народження" } });

                                    botTextResponse = "Будь ласка, оберіть варіант пошуку.";
                                    await botClient.SendTextMessageAsync(chatId: chatId, text: botTextResponse,
                                        parseMode: ParseMode.Html, replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
                                    currState = State.FindByName;
                                    break;
                                }
                            case "Додати особу":
                                {
                                    currOnAddItem = 0;
                                    onAddSavedMessagesList.Clear();
                                    onAddSavedMessagesList.Capacity = onAddMessagesList.Count;
                                    botTextResponse = onAddMessagesList[currOnAddItem];
                                    currOnAddItem++;
                                    await botClient.SendTextMessageAsync(chatId: chatId, text: botTextResponse + currOnAddItem.ToString(),
                                        parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
                                    currState = State.Add;
                                    break;
                                }
                            case "За ПІБ":
                                {
                                    botTextResponse = "Будь ласка, введіть ПІБ особи: наприклад, Степашина Ольга Миколаївна.";
                                    SendTextMassageOnFind(botClient, update, cancellationToken, botTextResponse);
                                    currState = State.FindByName;
                                    break;
                                }
                            case "За датою народження":
                                {
                                    botTextResponse = "Будь ласка, введіть дату народження особи: наприклад, 22.06.1988.";
                                    SendTextMassageOnFind(botClient, update, cancellationToken, botTextResponse);
                                    currState = State.FindByDate;
                                    break;
                                }
                            default:
                                {
                                    if (currState == State.FindByName || currState == State.FindByDate)
                                    {
                                        List<WantedPerson> findedPersons = new List<WantedPerson>();
                                        if (currState == State.FindByName)
                                            findedPersons = FindWantedPerson(name: msgText);
                                        else if (currState == State.FindByDate)
                                            findedPersons = FindWantedPerson(birthdate: msgText);
                                        else break;

                                        if (findedPersons.Count > 0)
                                        {
                                            foreach (WantedPerson item in findedPersons)
                                            {
                                                botTextResponse = item.PrintHtml();
                                                if (!String.Equals(item.PHOTOID, String.Empty))
                                                {
                                                    Photo ph = FindPhoto(item.PHOTOID);
                                                    if (ph.ID != null)
                                                    {
                                                        await botClient.SendPhotoAsync(chatId: chatId,
                                                            photo: new InputOnlineFile(new MemoryStream(Convert.FromBase64String(ph.PHOTOBASE64ENCODE))),
                                                            cancellationToken: cancellationToken);
                                                    }
                                                }
                                            }
                                        }
                                        else botTextResponse = "Не можу знайти особу.";

                                        var replyKeyboardMarkup = FormKeybord(new string[][] { new string[] { "Пошук", "Додати особу" } });
                                        await botClient.SendTextMessageAsync(chatId: chatId, text: botTextResponse, parseMode: ParseMode.Html, 
                                                replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);
                                        currState = State.Init;
                                    }
                                    else if (currState == State.Add)
                                    {
                                        onAddSavedMessagesList.Add(msgText);
                                        if(currOnAddItem == onAddMessagesList.Count)
                                        {
                                            currOnAddItem = 0;
                                            AddWantedPerson(onAddSavedMessagesList);

                                            var options = new JsonSerializerOptions
                                            {
                                                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                                                WriteIndented = true,
                                            };
                                            string jsonStr = JsonSerializer.Serialize(wPersons, options);
                                            System.IO.File.WriteAllText("../../../data/mvswantedperson_1.json", jsonStr);


                                            var replyKeyboardMarkup = FormKeybord(new string[][] { new string[] { "Пошук", "Додати особу" } });
                                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Особу додано!", parseMode: ParseMode.Html,
                                                    replyMarkup: replyKeyboardMarkup, cancellationToken: cancellationToken);

                                            currState = State.Init;
                                            break;
                                        }
                                        await botClient.SendTextMessageAsync(chatId: chatId, text: onAddMessagesList[currOnAddItem], 
                                            parseMode: ParseMode.Html, cancellationToken: cancellationToken);
                                        currOnAddItem++;
                                    }
                                    break;
                                }
                        }
                        break;                    
                    }
                default: 
                    {
                        break;
                    }

            }
            
        }

        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

        public static void AddWantedPerson(List<string> onAddSavedMessagesList)
        {
            string pID = String.Empty;
            do
            {
                pID = GenerateUniquePersonID();

            } while (!IsUniquePersonID(pID));

            string[] str = onAddSavedMessagesList[0].Split(" ");
            WantedPerson wp = new WantedPerson(pID, str[0], str[1], str[2],
                onAddSavedMessagesList[1], onAddSavedMessagesList[2], onAddSavedMessagesList[5],
                onAddSavedMessagesList[6], onAddSavedMessagesList[3], onAddSavedMessagesList[4],
                onAddSavedMessagesList[7], onAddSavedMessagesList[8], onAddSavedMessagesList[9]);
            wPersons.Add(wp);
        }

        private static bool IsUniquePersonID(string ID)
        {
            bool res = false;
            foreach (WantedPerson item in wPersons)
            {
                if (!item.IsEqualID(ID))
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        private static string GenerateUniquePersonID()
        {
            Random r = new Random();
            return r.Next(10000000, 100000000).ToString();

        }

        public static List<WantedPerson> FindWantedPerson(string name = null, string birthdate = null)
        {
            List<WantedPerson> res = new List<WantedPerson>();
            if (name != null)
            {
                Regex rgx = new Regex(@"^[a-zA-Z|а-яА-яёЁіІїЇґҐ]{2,}(\'?\-?[a-zA-Z|а-яА-яёЁіІїЇґҐ]{2,})?$");
                string[] str = name.Split(" ");
                if (str.Length != 3) return res;
                foreach (string item in str)
                    if (!rgx.IsMatch(item)) return res;

                foreach (WantedPerson item in wPersons)
                {
                    string nameU = item.FIRST_NAME_U + item.LAST_NAME_U + item.MIDDLE_NAME_U;
                    string nameR = item.FIRST_NAME_R + item.LAST_NAME_R + item.MIDDLE_NAME_R;
                    string nameE = item.FIRST_NAME_E + item.LAST_NAME_E + item.MIDDLE_NAME_E;
                    string searchedName = str[0] + str[1] + str[2];
                    if (String.Equals(searchedName, nameU, StringComparison.CurrentCultureIgnoreCase)
                        || String.Equals(searchedName, nameR, StringComparison.CurrentCultureIgnoreCase)
                        || String.Equals(searchedName, nameE, StringComparison.CurrentCultureIgnoreCase)) res.Add(item);
                }
            }
            else if (birthdate != null)
            {
                foreach (WantedPerson item in wPersons)
                {
                    if (DateTime.Equals(DateTime.Parse(birthdate).ToString("dd.MM.yyyy"), 
                        DateTime.Parse(item.BIRTH_DATE).ToString("dd.MM.yyyy")))
                        res.Add(item);
                }
            }
            return res;
        }

        public static Photo FindPhoto(string photoID)
        {
            foreach (Photo item in photos)
            {
                if (String.Equals(photoID, item.ID)) return item;
            }
            return new Photo();
        }

        private static async void SendTextMassageOnFind(ITelegramBotClient botClient, Update update, 
            CancellationToken cancellationToken, string botTextResponse)
        {
            await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, text: botTextResponse,
                parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
        }

        private static ReplyKeyboardMarkup FormKeybord(string[][] keyboardValues)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(FormKeybordButtons(keyboardValues));
            replyKeyboardMarkup.ResizeKeyboard = true;
            return replyKeyboardMarkup;
        }

        private static List<List<KeyboardButton>> FormKeybordButtons(string[][] keyboardValues)
        {
            List<List<KeyboardButton>> res = new List<List<KeyboardButton>>();
            List<KeyboardButton> kBtns = new List<KeyboardButton>();
            foreach (string[] row in keyboardValues)
            {
                foreach (string val in row)
                {
                    kBtns.Add(val);
                }
                res.Add(new List<KeyboardButton>(kBtns));
                kBtns.Clear();
            }
            return res;
        }

    }
}
