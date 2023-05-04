using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace PopUP
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopUp_Page : ContentPage
    {
        private int questionIndex;
        private int score;
        private List<int> numbers;
        private Random random;

        public PopUp_Page()
        {
            InitializeComponent();
            random = new Random();
        }

        private async void OnStartTestClicked(object sender, EventArgs e)
        {
            // Get the name from the entry
            var name = NameEntry.Text;

            if (string.IsNullOrWhiteSpace(name))
            {
                await DisplayAlert("Viga", "Palun sisesta oma nimi.", "OK");
                return;
            }

            // Initialize variables
            questionIndex = 0;
            score = 0;
            numbers = new List<int>();

            // Generate a list of random numbers for the test
            for (int i = 0; i < 10; i++)
            {
                numbers.Add(random.Next(1, 11));
            }

            // Show the first question
            await DisplayQuestion();
        }

        private async System.Threading.Tasks.Task DisplayQuestion()
        {
            // Get the first number for the question
            var number1 = numbers[questionIndex];

            // Generate a random second number for the question
            var number2 = random.Next(1, 11);

            // Calculate the correct answer
            var answer = number1 * number2;

            // Display the question
            var response = await DisplayPromptAsync($"Küsimus {questionIndex + 1}", $"Kui palju on {number1} x {number2}?", placeholder: "Vastus");

            await SaveDataToFileAsync("test.txt", $"{number1} x {number2} = {answer}\n");

            // Check the answer
            if (int.TryParse(response, out int userAnswer))
            {
                if (userAnswer == answer)
                {
                    score++;
                }
            }

            // Move to the next question or finish the test
            if (questionIndex < 9)
            {
                questionIndex++;
                await DisplayQuestion();
            }
            else
            {
                var percentageScore = (double)score /10 * 100;

                var letterGrade = GetLetterGrade(percentageScore);

                await DisplayAlert("Test lõpetatud", $"{NameEntry.Text}, sinu punktid {score} / 10. ({percentageScore}%)\nHinne: {letterGrade}", "OK");
            }
        }

        private string GetLetterGrade(double percentageScore)
        {
            if (percentageScore >= 90)
            {
                return "5";
            }
            else if(percentageScore >= 75)
            {
                return "4";
            }
            else if (percentageScore >= 50)
            {
                return "3";
            }
            else
            {
                return "2";
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async Task SaveDataToFileAsync(string fileName, string data)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string fullPath = Path.Combine(path, fileName);

            using (StreamWriter sw = new StreamWriter(fullPath, true))
            {
                await sw.WriteAsync(data).ConfigureAwait(false);
            }
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullPath = Path.Combine(path, "test.txt");

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                    await DisplayAlert("Korras", $"Faili kustutamine õnnestus", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Viga", $"Viga faili kustutamisel: {ex.Message}", "OK");
                }
            }
            else
            {
                await DisplayAlert("Viga", "Faili ei leitud", "OK");
            }
        }
    }
}