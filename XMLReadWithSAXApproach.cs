using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace IS_Labs
{
    internal class XMLReadWithSAXApproach
    {
        internal static void Read(string filepath)
        {
            XmlReaderSettings settings = new();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            
            XmlReader reader = XmlReader.Create(filepath, settings);
            
            int count = 0;
            string postac = "";
            string sc = "";
            int counterOfDrugsWithSameNameAndDifferentForm = 0;

            reader.MoveToContent();
            string substance = "";

            Dictionary<string, int> activeSubst = new();
            Dictionary<string, HashSet<string>> drugsDictionary = new();

            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name.Equals("produktLeczniczy")))
                {
                    postac = reader.GetAttribute("postac");
                    sc = reader.GetAttribute("nazwaPowszechnieStosowana");

                    if (drugsDictionary.ContainsKey(sc))
                        drugsDictionary[sc].Add(postac);
                    else
                    {
                        drugsDictionary[sc] = new HashSet<string>
                    {
                        postac
                    };
                    }
                    if (postac == "Krem" && sc == "Mometasoni furoas")
                        count++;
                }

                if ((reader.NodeType == XmlNodeType.Element) && (reader.Name.Equals("substancjaCzynna")))
                {
                    reader.Read();
                    substance = reader.Value;

                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        if (activeSubst.ContainsKey(substance))
                            activeSubst[substance] += 1;
                        else
                            activeSubst.Add(substance, 1);
                    }
                }
            }

            foreach (KeyValuePair<string, HashSet<string>> kvp in drugsDictionary)
            {
                if (kvp.Value.Count > 1)
                {
                    counterOfDrugsWithSameNameAndDifferentForm++;
                }
            }

            int maxValue = activeSubst.Values.Max();
            string mostCommonSubstance = activeSubst.FirstOrDefault(substance => substance.Value == maxValue).Key;

            activeSubst = activeSubst.OrderBy(key => key.Value).ToDictionary(x => x.Key, x => x.Value);
            string[] keys = activeSubst.Keys.ToArray();

            Console.WriteLine("Istnieje {0} preparatów leczniczych o takiej samie nazwie powszechnej, pod różnymi postaciami", counterOfDrugsWithSameNameAndDifferentForm);
            Console.WriteLine("Najczęściej wykorzystywana substancja czynna: {0}", keys[keys.Length - 1]);
            Console.WriteLine("Liczba produktów leczniczych w postaci kremu, których jedyną substancją czynną jest Mometasoni furoas {0}", count);
            Console.WriteLine("\n");
        }
    }
}