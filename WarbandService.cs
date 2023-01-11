using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace SwordcraftBrisbane.Data
{
    public static class WarbandService
    {
        private const int ChaosId = 12;

        private static readonly string LoadDirectory = "xml";
        private static readonly string SaveDirectory = "wwwroot";
        private static readonly string SaveSubDirectory = "Warbands";

        private static readonly Regex ClearTable = new Regex(@"(?<=<tbody>)(?:(?>[\r\n\s]+)<?(?!\/tbody>)[^<].+)+");
        //new Regex(@"(?<=<div class="""">)(?>[\r\n\s]+)");

        public static void WriteHtmlPages()
        {
            string templatePath, indexPath;
            try
            {
                var currDir = Directory.GetCurrentDirectory();
                templatePath = Path.Combine(currDir, Directory.GetFiles(LoadDirectory, "_template.html", SearchOption.TopDirectoryOnly).First());
                indexPath = Path.Combine(currDir, Directory.GetFiles(LoadDirectory, "_indexTemplate.html", SearchOption.TopDirectoryOnly).First());
            }
            catch
            {
                throw new ArgumentException("Could not find html templates");
            }

            string originalTemplate = File.ReadAllText(templatePath);
            string indexTemplate = File.ReadAllText(indexPath);

            var files = Directory.GetFiles(LoadDirectory, "*.xml", SearchOption.TopDirectoryOnly);
            XmlSerializer reader = new XmlSerializer(typeof(Warband));

            var allWarbands = new List<Warband>();

            foreach (var f in files.Where(x => !x.EndsWith("template.xml")))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), f);
                var file = new StreamReader(filePath);
                var input = reader.Deserialize(file);
                Warband? band = input as Warband;
                if (band != null)
                {
                    allWarbands.Add(band);
                    var template = ProcessTemplate(originalTemplate, band);
                    OutputFile(template, $"{band.Id:D2}.html");
                }
            }

            indexTemplate = ClearTable.Replace(indexTemplate,
                allWarbands.Aggregate(new StringBuilder(),
                (sb, warband) =>
                    sb.AppendLine(warband.ToShortHtml()),
                sb => sb.ToString()));

            OutputFile(indexTemplate, "index.html", false);
        }

        private static void OutputFile(string contents, string filename, bool useSubDirectory = true)
        {
            var outPath = useSubDirectory
                ? Path.Combine(Directory.GetCurrentDirectory(), SaveDirectory, SaveSubDirectory, filename)
                : Path.Combine(Directory.GetCurrentDirectory(), SaveDirectory, filename);
            File.Delete(outPath);
            var outFile = new StreamWriter(outPath);
            outFile.Write(contents);
            outFile.Flush();
            outFile.Close();
        }

        private record Map
        {
            public Regex Search;
            public string? Property;
            public bool Condition = true;
        }

        private static string ProcessTemplate(string originalTemplate, Warband warband)
        {
            var template = new string(originalTemplate);

            List<Map> replacements = new()
            {
                new Map { Search = SearchFor("Name"), Property = warband.Name },
                new Map { Search = SearchFor("ShortName"), Property = warband.ShortName },
                new Map { Search = SearchFor("Hook"), Property = warband.Hook },
                new Map { Search = SearchFor("ActivePeriod"), Property = warband.Active ?? "Default, approximately every week" },
                new Map { Search = SearchFor("ApproxSize"), Property = warband.Size },

                new Map { Search = SearchFor("Location"), Property = warband.Location ?? "<i>-- No data --</i>" },
                new Map { Search = SearchFor("Description"), Property = string.IsNullOrEmpty(warband.Description)
                    ? "<i>-- No data --</i>" : warband.Description },
                new Map { Search = SearchFor("Values"), Property = warband.Traits.Values ?? "???" },
                new Map { Search = SearchFor("Feelings"), Property = warband.Traits.Feelings ?? "???" },
                new Map { Search = SearchFor("Combat"), Property = warband.Traits.Combat ?? "???" },
                new Map { Search = SearchFor("Equip"), Property = warband.Traits.Equip ?? "???" },
                new Map { Search = RemoveBlockForTrait("Equip"), Property = string.Empty,
                    Condition = warband.Traits.Equip == null || string.IsNullOrEmpty(warband.Traits.Equip) },
                new Map { Search = SearchFor("Noise"), Property = warband.Traits.Noise ?? "???"},
                new Map { Search = RemoveBlockForTrait("Noise"), Property = string.Empty,
                    Condition = warband.Traits.Noise == null || string.IsNullOrEmpty(warband.Traits.Noise) },

                new Map { Search = SearchFor("Style"), Property = warband.Style ?? "<i>-- No data --</i>" },
                new Map { Search = SearchFor("Accent"), Property = string.IsNullOrEmpty(warband.Accent)
                    ? "<i>-- No data --</i>" : warband.Accent },
                new Map { Search = SearchForLogo, Property = $"{warband.Id:D2}" },
                new Map { Search = SearchFor("FirstColourCode"),
                    Property = $"{warband.Colours.Primary.Name} ({warband.Colours.Primary.code})" },
                new Map { Search = SearchForColour(1), Property = warband.Colours.Primary.code },
                new Map { Search = SearchFor("SecondColourCode"),
                    Property = $"{warband.Colours.Secondary.Name} ({warband.Colours.Secondary.code})" },
                new Map { Search = SearchForColour(2), Property = warband.Colours.Secondary.code },
                new Map { Search = SearchFor("ThirdColourCode"),
                    Property = $"{warband.Colours.Tertiary?.Name} ({warband.Colours.Tertiary?.code})" },
                new Map { Search = SearchForColour(3), Property = TertiaryColourCalculation(warband) },
                new Map { Search = RemoveBlockForColour(3), Property = string.Empty,
                    Condition = warband.Colours.Tertiary == null || string.IsNullOrEmpty(warband.Colours.Tertiary.Name) },
                new Map { Search = SearchFor("FourthColourCode"),
                    Property = $"{warband.Colours.Quarternary?.Name} ({warband.Colours.Quarternary?.code})" },
                new Map { Search = SearchForColour(4), Property = warband.Colours.Quarternary?.code ?? "AAAAAA" },
                new Map { Search = RemoveBlockForColour(4), Property = string.Empty,
                    Condition = warband.Colours.Quarternary == null || string.IsNullOrEmpty(warband.Colours.Quarternary.Name) },

                new Map { Search = ReplaceFullRoleBlock,
                    Property = warband.Roles.Aggregate(new StringBuilder(), 
                        (sb, group) => sb.Append(group.ToHtml()), sb => sb.ToString()) },

                new Map { Search = ReplaceFullWeaponBlock,
                    Property = warband.Weapons.Aggregate(new StringBuilder(),
                        (sb, weap) => sb.Append(weap.ToHtml()), sb => sb.ToString()) },
            };

            template = replacements.Aggregate(template,
                (input, map) => map.Condition
                    ? map.Search.Replace(input, map.Property)
                    : input,
                x => x);

            return template;
        }

        private static Regex SearchFor(string searchFor)
        {
            return new Regex($@"(?<=for=""Warband_{searchFor}"">)XXXXXX(?: \(#XXXXXX\))?");
        }

        private static Regex SearchForColour(int colourId)
        {
            if (colourId < 1 || colourId > 4) throw new ArgumentOutOfRangeException();

            var searchTerm = $@"<div id=""colour{colourId}""";
            return new Regex($@"(?<={searchTerm} class=""swatch"" style=""background-color: )#XXXXXX");
        }

        static readonly Regex SearchForLogo
            = new Regex(@"(?<=<img class=""autoSpace block p200"" src=""\.\.\/Images\/)XXXXXX");
        static readonly Regex ReplaceFullRoleBlock
            = new Regex(@"(?<=for=""Warband_Roles"">Roles</label>\r\n {12}<div class=""card-body"">)(?:[\r\n\s]+[^\n]+){12}");
        static readonly Regex ReplaceFullWeaponBlock
            = new Regex(@"(?<=style=""display: flex"">)(?:[\r\n\s]+[^\n]+){18}");

        private static Regex RemoveBlockForColour(int colourId)
        {
            if (colourId != 3 && colourId != 4) throw new ArgumentOutOfRangeException();

            var searchTerm = (colourId == 3) ? "tertiary" : "quarternary";
            return new Regex($@"[\r\n\s]+<div id=""{searchTerm}""[^\r\n]+>(?:(?>[\r\n\s]+)(?>[^\r\n]+)){{3}}[\r\n\s]+</div>");
        }

        private static Regex RemoveBlockForTrait(string searchFor)
        {
            return new Regex($@"[\r\n\s]+<div[^\r\n]+>[\r\n\s]+[^\r\n]+[\r\n\s]+<label.+?for=""Warband_{searchFor}"">.+?</label>[\r\n\s]+</div>");
        }

        private static string TertiaryColourCalculation(Warband warband)
        {
            if (warband.Id != ChaosId)
                return warband.Colours.Tertiary?.code ?? "XXXXXX";

            var result = new StringBuilder(@"""><div style='display:flex'>");
            foreach (var col in new[] { "#650100", "#056503", "#011667", "#650048" })
            {
                result.Append(DisplayColourBlock(col, "span", "width: 25%"));
            }
            result.Append("</div");

            return result.ToString();
        }

        private static string DisplayColourBlock(string colour, string elementName = "div", string styles = "")
        {
            return string.Format(@"<{0} class='swatch' style='background-color: {1}; {2}'>&nbsp;</{0}>",
                elementName, colour, styles);
        }
    }
}