
public class Encoding
{
    public static byte[] Encode(string input)
    {
        return System.Text.Encoding.ASCII.GetBytes(input);
    }

    public static string Decode(byte[] input)
    {
        return System.Text.Encoding.ASCII.GetString(input);
    }
}