using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace WordPokerCrossV1.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        InitPic(0);
        Init(SwitchLangBt?.Content?.ToString());
        NewGame();


    }

    private Bitmap[] images = new Bitmap[4];
    private readonly HashSet<string> _itemsFull = [];
    private string[]? _str4;
    private int _index;
    private int _life = 15;
    private readonly string[] _langCodes = ["hu", "en", "ru", "pl"];
    private int _currentLangIndex;


    private void InitPic(int picSelect)
    {
        Uri? uri;
        Bitmap? bitmapImg;

        for (int i = 0; i <= 3; i++)
        {
            uri = new Uri($"avares://WordPokerCrossV1/Assets/akaszt{i}.png");
            bitmapImg = new Bitmap(AssetLoader.Open(uri));
            images[i] = bitmapImg;


        }
        Img1.Source = images[picSelect];
    }



    private void Init(string? langCode)
    {

        if (string.IsNullOrEmpty(langCode))
        {
            throw new ArgumentNullException(nameof(langCode), "Language code cannot be null or empty.");
        }

        _itemsFull.Clear();

        var resourceUri = new Uri($"avares://WordPokerCrossV1/Assets/wordpoker-{langCode}.txt");

        try
        {
            using (Stream? stream = AssetLoader.Open(resourceUri))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Resource not found: {resourceUri}");
                }

                using (StreamReader data = new StreamReader(stream, Encoding.UTF8))
                {
                    while (!data.EndOfStream)
                    {
                        string? line = data.ReadLine();
                        if (line is { Length: 4 })
                        {
                            _itemsFull.Add(line.ToUpper());
                        }
                    }
                }
            }
        }
        catch (FileNotFoundException fex)
        {
            Console.WriteLine($"Error: {fex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while loading words: {ex.Message}");
        }

        _str4 = [.. _itemsFull];
        Life1.Content = _life.ToString();
    }

    private static bool IsValidInput(string input)
    {
        if (!string.IsNullOrWhiteSpace(input) && input.Length == 4)
        {
            return true;
        }

        return false;
    }

    private void NewGame()
    {
        Random r = new Random();

        _index = r.Next(_itemsFull.Count);

        _life = 15;
        switch (SwitchLangBt.Content)
        {
            case "hu": Life1.Content = $"Élet:{_life} "; GuessBt.Content = "Tipp"; StartGameBt.Content = "Új játék"; break;
            case "en": Life1.Content = $"Life:{_life}"; GuessBt.Content = "Guess"; StartGameBt.Content = "New Game"; break;
            case "ru": Life1.Content = $"житие:{_life}"; GuessBt.Content = "Предполагать"; StartGameBt.Content = "Новая игра"; break;
            case "pl": Life1.Content = $"życie:{_life}";GuessBt.Content = "Zgadnąć";StartGameBt.Content = "Nowa Gra"; break;

        }

        LbWord0.Content = "_";
        LbWord1.Content = "_";
        LbWord2.Content = "_";
        LbWord3.Content = "_";
        TbFullWord.Text = "";
        LbWin.Content = "";

        Img1.Source = images[0];


        Console.WriteLine(_str4?[_index]);
    }


    private void StartGameBT_OnClick(object? sender, RoutedEventArgs e)
    {
        NewGame();
    }


    private void GuessBt_OnClick(object? sender, RoutedEventArgs e)
    {
        string? plainText = TbFullWord.Text?.ToUpper();

        if (plainText == null || !IsValidInput(plainText))
        {
            return;
        }

        Label[] letterLabels = [LbWord0, LbWord1, LbWord2, LbWord3];


        for (int i = 0; i < letterLabels.Length; i++)
        {
            if (_str4 == null || _index < 0 || _index >= _str4.Length || _str4?[_index] == null)
            {
                LbWin.Content = "Game error: Word not loaded.";
                return;
            }

            char guessedChar = plainText[i];
            char correctChar = _str4[_index][i];

            if (char.ToUpper(guessedChar) == char.ToUpper(correctChar))
            {
                letterLabels[i].Content = char.ToUpper(guessedChar).ToString();
            }
        }




        _life--;

        if (_life < 0)
        {

            _life = 0;


        }


        if (_life == 10)
        {

            InitPic(1);

        }
        if (_life == 5)
        {
            InitPic(2);


        }




        switch (SwitchLangBt.Content)
        {
            case "hu": Life1.Content = $"Élet:{_life} "; break;
            case "en": Life1.Content = $"Life:{_life}"; break;
            case "ru": Life1.Content = $"житие:{_life}"; break;
            case "pl": Life1.Content = $"życie:{_life}"; break;
        }


        CheckGameStatus(plainText);
    }

    private void CheckGameStatus(string plainText)
    {
        string currentGuessedWord = $"{LbWord0.Content}{LbWord1.Content}{LbWord2.Content}{LbWord3.Content}";

        bool isValidIndex = _str4 != null && _index >= 0 && _index < _str4.Length;

        if (isValidIndex &&
            (plainText == _str4[_index].ToUpper() || currentGuessedWord == _str4[_index].ToUpper()))
        {
            string language = SwitchLangBt.Content?.ToString() ?? "en";
            LbWin.Content = language switch
            {
                "ru" => "ты выиграл!",
                "hu" => "Nyertél!",
                "pl" => "Wygrywasz!",
                _ => "You win!"
            };
        }


        if (_life == 0)
        {
            InitPic(3);

        
            string? word = _str4 != null && _index >= 0 && _index < _str4.Length
                ? _str4[_index] : null;

            if (word != null)
            {
            
                Label[] wordLabels = new[] { LbWord0, LbWord1, LbWord2, LbWord3 };

                for (int i = 0; i < wordLabels.Length; i++)
                {
                    wordLabels[i].Content = word.Length > i ? word[i] : null;
                    wordLabels[i].FontStyle = FontStyle.Italic;
                }

                string language = SwitchLangBt.Content?.ToString() ?? "en";
                string loseMessage = language switch
                {
                    "ru" => "ты не выиграл!",
                    "hu" => "Vesztettél! ",
                    "pl" => "Przegrywasz!",
                    _ => "You lose!"
                };

                LbWin.Content = $"{loseMessage}";
            }

        }

    }

    private void SwitchLangBT_OnClick(object? sender, RoutedEventArgs e)
    {
        _currentLangIndex = (_currentLangIndex + 1) % _langCodes.Length;
        string newLang = _langCodes[_currentLangIndex];

        SwitchLangBt.Content = newLang;
        Init(newLang);
        NewGame();
    }

    private void TbFullWord_OnKeyUp(object? sender, KeyEventArgs e)
    {
        TextBox? currentContainer = sender as TextBox;
        if (currentContainer != null)
        {
            int caretPosition = currentContainer.SelectionStart;

            if (currentContainer.Text != null) currentContainer.Text = currentContainer.Text.ToUpper();
            currentContainer.SelectionStart = caretPosition++;
        }
    }
}