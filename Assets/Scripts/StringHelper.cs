public static class StringHelper {

    public const string EXPERIENCE_BAR_TITLE_GATHERING_FORAGING = "Gathering";
    public const string EXPERIENCE_BAR_TITLE_TREE_FELLING = "Tree Felling";
    public const string EXPERIENCE_BAR_TITLE_STONE_BREAKING = "Stone Breaking";
    public const string EXPERIENCE_BAR_TITLE_MINING = "Mining";
    public const string EXPERIENCE_BAR_TITLE_FISHING = "Fishing";
    public const string EXPERIENCE_BAR_TITLE_CRAFTING = "Crafting";
    public const string EXPERIENCE_BAR_TITLE_FIGHTING = "Combat";

    public static string GetLeadingZeroNumberString(int number) {
        string result = number.ToString();

        if (number < 10) {
            result = "0" + number.ToString();
        }
        return result;
    }
}
