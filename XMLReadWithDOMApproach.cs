using System;
using System.Collections.Generic;
using System.Xml;

namespace IS_Lab1_XML
{
    internal class XMLReadWithDOMApproach
    {
        internal static void Read(string filepath)
        {
            // odczyt zawartości dokumentu
            XmlDocument doc = new();
            doc.Load(filepath);

            string postac;
            string sc;
            int count = 0;
            var drugs = doc.GetElementsByTagName("produktLeczniczy");
            int counterOfDrugsWithSameNameAndDifferentForm = 0;

            Dictionary<string, HashSet<string>> drugsDictionary = new();

            foreach (XmlNode d in drugs)
            {
                sc = d.Attributes.GetNamedItem("nazwaPowszechnieStosowana").Value;
                postac = d.Attributes.GetNamedItem("postac").Value;

                if(drugsDictionary.ContainsKey(sc))
                {
                    drugsDictionary[sc].Add(postac);
                }
                else
                {
                    drugsDictionary[sc] = new HashSet<string>
                    {
                        postac
                    };
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
            Console.WriteLine("Istnieje {0} preparatów leczniczych o takiej samie nazwie powszechnej, pod różnymi postaciami", counterOfDrugsWithSameNameAndDifferentForm);
            Console.WriteLine("Liczba produktów leczniczych w postaci kremu, których jedyną substancją czynną jest Mometasoni furoas {0}", count);
        }
    }
}