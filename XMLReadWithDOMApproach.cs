using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace IS_Lab1_XML
{
    internal class XMLReadWithDOMApproach
    {
        internal static void Read(string filepath)
        {
            XmlDocument doc = new();
            doc.Load(filepath);

            string postac;
            string sc;
            XmlNode substances;
            int count = 0;
            var drugs = doc.GetElementsByTagName("produktLeczniczy");
            int counterOfDrugsWithSameNameAndDifferentForm = 0;

            Dictionary<string, HashSet<string>> drugsDictionary = new();
            Dictionary<string, int> activeSubst = new();

            foreach (XmlNode d in drugs)
            {
                sc = d.Attributes.GetNamedItem("nazwaPowszechnieStosowana").Value;
                postac = d.Attributes.GetNamedItem("postac").Value;

                if (drugsDictionary.ContainsKey(sc))
                    drugsDictionary[sc].Add(postac);
                else
                {
                    drugsDictionary[sc] = new HashSet<string>
                    {
                        postac
                    };
                }

                substances = d.ChildNodes[0];

                for(int i = 0; i < substances.ChildNodes.Count; i++)
                {
                    string substance = substances.ChildNodes[i].InnerText;

                    if (activeSubst.ContainsKey(substance))
                        activeSubst[substance] += 1;
                    else
                        activeSubst.Add(substance, 1);
                }
            }
            

            foreach(KeyValuePair<string, HashSet<string>> kvp in drugsDictionary)
            {
                if (kvp.Value.Count > 1)
                {
                    counterOfDrugsWithSameNameAndDifferentForm++;
                }
            }

            foreach (XmlNode d in drugs)
            {
                postac = d.Attributes.GetNamedItem("postac").Value;
                sc = d.Attributes.GetNamedItem("nazwaPowszechnieStosowana").Value;
                if (postac == "Krem" && sc == "Mometasoni furoas")
                    count++;
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