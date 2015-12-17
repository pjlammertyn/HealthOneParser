using System;
using HealthOneParser;
using Newtonsoft.Json;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = @"A1\4848289\URO_M+R\
A2\4848289\DELAMILLIEURE\RONNY\M\18091966\
A3\4848289\Gentsesteenweg 208 \8650\HOUTHULST\
A4\4848289\1.35680.23.004\15122014\\C\
L5\4848289\URO_M+R\\\\\Geachte Collega,\
L5\4848289\URO_M+R\\\\\\
L5\4848289\URO_M+R\\\\\Betreft uw patiënt :	Delamillieure Ronny \
L5\4848289\URO_M+R\\\\\Franky (° 18/09/1966)	\
L5\4848289\URO_M+R\\\\\	Gentsesteenweg 208, 8650 Houthulst\
L5\4848289\URO_M+R\\\\\\
L5\4848289\URO_M+R\\\\\Onze ref: LG/DS\
L5\4848289\URO_M+R\\\\\\
L5\4848289\URO_M+R\\\\\Ik zag uw patiënt op de raadpleging Urologie op 27/11/2014. \
L5\4848289\URO_M+R\\\\\ACTUELE PROBLEMATIEK:\
L5\4848289\URO_M+R\\\\\Patiënt door u verwezen omwille van intermittente lumbalgie links \
L5\4848289\URO_M+R\\\\\en microscopische hematurie. \
L5\4848289\URO_M+R\\\\\KLINISCH ONDERZOEK:\
L5\4848289\URO_M+R\\\\\CT kon geen hydronefrose noch nefro of ureterolithiasis weerhouden. \
L5\4848289\URO_M+R\\\\\Alleen de gekende rugproblematiek in het bijzonder op niveau L5 S1. \
L5\4848289\URO_M+R\\\\\PSA via uw goede zorgen toont een waarde van 0.31 ng/ ml. \
L5\4848289\URO_M+R\\\\\De flow is normaal, geen residu na mictie. \
L5\4848289\URO_M+R\\\\\Echografie toont prostaat van 35 ml met mogelijk beginnende \
L5\4848289\URO_M+R\\\\\inflammatie aan de rechter zijde. \
L5\4848289\URO_M+R\\\\\Klinisch onderzoek toont voornamelijk last ter hoogte van de psoas \
L5\4848289\URO_M+R\\\\\op niveau L3 L2 en ook eventueel drukpijn ter hoogte van C6/C7. \
L5\4848289\URO_M+R\\\\\BESLUIT:\
L5\4848289\URO_M+R\\\\\We vermoeden inderdaad dat de lumbalgie eerder van musculoskeletale \
L5\4848289\URO_M+R\\\\\oorsprong is. Hiervoor kan eventueel het advies ingewonnen worden \
L5\4848289\URO_M+R\\\\\op uw verwijzing bij de dienst fysische geneeskunde. \
L5\4848289\URO_M+R\\\\\Gezien zijn microscopische hematurie, gezien de bevindingen van de \
L5\4848289\URO_M+R\\\\\prostaat kan eventueel gedacht worden aan prostatitis. Hiervoor \
L5\4848289\URO_M+R\\\\\werd Levofloxacine en Urofyt voorgeschreven met klinische \
L5\4848289\URO_M+R\\\\\herevaluatie zo nodig.\
L5\4848289\URO_M+R\\\\\  \
L5\4848289\URO_M+R\\\\\     \
L5\4848289\URO_M+R\\\\\             \
L5\4848289\URO_M+R\\\\\\
L5\4848289\URO_M+R\\\\\\
";

            var letters = Parser.ParseReport(text);
            foreach (var letter in letters)
                foreach (var parserError in letter.ParserErrors)
                Console.WriteLine("error on line {0}: {1}", parserError.Key, string.Join(Environment.NewLine, parserError.Value));
            Console.WriteLine("Press enter to view result");
            Console.ReadLine();
            var json = JsonConvert.SerializeObject(letters, Formatting.Indented);
            Console.WriteLine(json);
            Console.ReadLine();
        }
    }
}
