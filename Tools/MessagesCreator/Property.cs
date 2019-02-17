using MessagesCreator.Extensions;

namespace MessagesCreator
{
    public class Property
    {

        #region Properties

        public string Name { get; set; }

        public string Type { get; set; }

        public string ToPropertyLine => $"\t\tpublic {Type} {Name} {{ get; set; }}";

        public string ToWriteLine => $"\t\t\twriter.Write({Name});";

        public string ToReadLine => $"\t\t\t{Name} = reader.Read{Type.Capitalize()}();";

        #endregion

    }
}
