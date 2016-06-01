using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bib2tex
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = "";
            string bibFile = "importantList.bib";
            string texFile = "hoge.txt";
            /*
            if(args.Length==2)
            {
                bibFile = args[0];
                texFile = args[1];
            }
            else
            {
                Console.Write("input bibFile: ");
                bibFile = Console.ReadLine();

                Console.Write("input texFile: ");
                texFile = Console.ReadLine();
            }
            */
            using (StreamWriter w = new StreamWriter(texFile))
            {
                using (StreamReader r = new StreamReader(bibFile))
                {
                    var re = new System.Text.RegularExpressions.Regex(@"\w+ = \{+(.*)\},?");
                    Article arti = new Article();
                    string s;
                    for (;;)
                    {
                        s = r.ReadLine();
                        if (s == null) break;
                        var match = re.Match(s).Groups[1].ToString().Trim('}');
                        var hoge = re.Match(s).Groups;
                        foreach (var c in WhiteSpaceDelimiters)
                        {
                            match = match.Replace(c, ' ');
                        }

                        switch (s.Split(' ')[0])
                        {
                            case "abstract":
                                arti.abst = match;
                                break;
                            case "author":
                                arti.author = match;
                                arti.FixAuthor();
                                break;
                            case "journal":
                                arti.journal = match;
                                break;
                            case "number":
                                arti.number = match;
                                break;
                            case "pages":
                                arti.pages = match;
                                break;
                            case "title":
                                arti.title = match;
                                break;
                            case "volume":
                                arti.volume = match;
                                break;
                            case "year":
                                arti.year = match;
                                break;
                            case "}":
                                text += arti.ToTex();
                                w.Write(arti.ToTex());
                                arti = new Article();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        static char[] WhiteSpaceDelimiters = new char[] {
            '\u0009',  // CHARACTER TABULATION
            '\u000A',  // LINE FEED
            '\u000B',  // LINE TABULATION
            '\u000C',  // FORM FEED
            '\u000D',  // CARRIAGE RETURN
//            '\u0020',  // SPACE
            '\u00A0',  // NO-BREAK SPACE
            '\u2000',  // EN QUAD
            '\u2001',  // EM QUAD
            '\u2002',  // EN SPACE
            '\u2003',  // EM SPACE
            '\u2004',  // THREE-PER-EM SPACE
            '\u2005',  // FOUR-PER-EM SPACE
            '\u2006',  // SIX-PER-EM SPACE
            '\u2007',  // FIGURE SPACE
            '\u2008',  // PUNCTUATION SPACE
            '\u2009',  // THIN SPACE
            '\u200A',  // HAIR SPACE
            '\u200B',  // ZERO WIDTH SPACE
            '\u3000',  // IDEOGRAPHIC SPACE
            '\uFEFF' // ZERO WIDTH NO-BREAK SPACE
        };
    }

    class Article
    {
        public string abst;
        public string author;
        public string journal;
        public string number;
        public string pages;
        public string title;
        public string volume;
        public string year;

        public string firstAuthor;
        public string fixedAuthor;

        public string ToTex()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"\newpage");
            sb.AppendLine(@"\section");
            sb.AppendLine(@"[" + title + @"\\");
            sb.AppendLine(firstAuthor + " " + FixVolume() + @"\\");
            sb.AppendLine(@"]");
            sb.AppendLine(@"{" + title + @"}");
            sb.AppendLine(fixedAuthor);
            sb.AppendLine("");
            sb.AppendLine(FixJournal());
            sb.AppendLine("");
            sb.AppendLine(@"\subsection*{abstract}");
            sb.AppendLine(abst);
            sb.AppendLine("");
            sb.AppendLine(@"\subsection*{要約}");
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("");

            return sb.ToString();
        }

        public string FixAuthor()
        {
            string[] s = author.Replace(" and", ",").Split(',');
            string[] names = new string[s.Length / 2];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = s[2 * i + 1].Trim() + " " + s[2 * i].Trim();
            }
            fixedAuthor = String.Join(", ", names);

            if (names.Length < 3) firstAuthor = fixedAuthor;
            else firstAuthor = names[0] + @" $et$ $al$.";

            return fixedAuthor;
        }

        string FixJournal()
        {
            return journal + @" {\bf " + volume + @"} (" + year + @") " + pages.Replace("--", "-");
        }
        string FixVolume()
        {
            return @" {\bf " + volume + @"} (" + year + @") " + pages.Replace("--", "-");
        }
    }
}
