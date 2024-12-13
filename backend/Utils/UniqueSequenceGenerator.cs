namespace backend.Utils
{
    public abstract class UniqueSequenceGenerator
    {
        public static string GenerateUniqueString(int randomStringLength)
        {
            string datePart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            Random random = new();
            string stringPart = "";

            char[] characters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                              'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                              '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            for (int i = 0; i < randomStringLength; i++)
            {
                int randomNumber = random.Next(0, characters.Length);
                stringPart += characters[randomNumber];
            }

            string uniqueString = datePart + stringPart;
            return uniqueString;
        }

        public static long GenerateLongIdUsingTicks()
        {
            long ticks = DateTime.UtcNow.Ticks;
            return ticks;
        }

    }
}
