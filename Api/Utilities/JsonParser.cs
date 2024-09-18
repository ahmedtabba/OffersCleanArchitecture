namespace Offers.CleanArchitecture.Api.Utilities;

public static class JsonParser
{
    public static List<string> ParseMessages(string input)
    {
        List<string> messages = new List<string>();
        string[] lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        foreach (var line in lines)
        {
            string trimmedLine = line.Trim();

            // Ignore lines that are headers (e.g., "Create User:" or "Password:")
            if (trimmedLine.EndsWith(":"))
            {
                continue;
            }

            // Add message if it starts with '-'
            if (trimmedLine.StartsWith("- "))
            {
                messages.Add(trimmedLine.Substring(2)); // Remove the "- " part
            }
            else
            {
                messages.Add(trimmedLine);
            }
        }

        return messages;
    }
}
