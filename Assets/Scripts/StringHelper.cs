public static class StringHelper {

    public static string GetLeadingZeroNumberString(int number) {
        string result = number.ToString();

        if (number < 10) {
            result = "0" + number.ToString();
        }
        return result;
    }
}
