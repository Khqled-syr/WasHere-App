using System.Threading.Tasks;
using System.Windows.Controls;

namespace WasHere.Utils
{
    public static class OutputManager
    {
        private static int currentIndex;
        private static string? outputText;

        public static void ClearOutput(TextBlock outputTextBlock)
        {
            outputTextBlock.Text = "";
            currentIndex = 0;
            outputText = "";
        }

        public static async Task SetOutputAsync(TextBlock outputTextBlock, string text)
        {
            ClearOutput(outputTextBlock);
            outputText = text;
            await TypeTextAsync(outputTextBlock);
        }

        public static async Task AppendOutputAsync(TextBlock outputTextBlock, string text)
        {
            outputText = text;
            await TypeTextAsync(outputTextBlock);
        }

        private static async Task TypeTextAsync(TextBlock outputTextBlock)
        {
            while (currentIndex < outputText?.Length)
            {
                outputTextBlock.Text += outputText[currentIndex];
                currentIndex++;
                await Task.Delay(50); // Adjust typing speed here
            }
        }
    }
}
