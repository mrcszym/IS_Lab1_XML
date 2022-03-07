using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace IS_Labs
{
    internal class XMLReadWithXLSTDOM
    {
        internal static void Read(string filepath)
        {
            XPathDocument document = new(filepath);
            XPathNavigator navigator = document.CreateNavigator();
            XmlNamespaceManager manager = new(navigator.NameTable);

            manager.AddNamespace("x", "http://rejestrymedyczne.ezdrowie.gov.pl/rpl/eksport-danych-v1.0");
            XPathExpression query = navigator.Compile("/x:produktyLecznicze/x:produktLeczniczy/x:substancjeCzynne/x:substancjaCzynna");

            query.SetContext(manager);
            XPathNodeIterator nodeIter = navigator.Select(query);

            Dictionary<string, int> activeSubst = new();

            while (nodeIter.MoveNext())
            {
                XPathNavigator n = nodeIter.Current;
                string substance = n.Value;

                if (activeSubst.ContainsKey(substance))
                    activeSubst[substance] = activeSubst[substance] + 1;
                else
                    activeSubst.Add(substance, 1);
            }
            activeSubst = activeSubst.OrderBy(key => key.Value).ToDictionary(x => x.Key, x => x.Value);
            string[] keys = activeSubst.Keys.ToArray();
            Console.WriteLine("Najczęściej wykorzystywana substancja czynna: {0}", keys[keys.Length - 1]);
        }

        internal static void CountMometasoni(string filepath)
        {
            XPathDocument document = new(filepath);
            XPathNavigator navigator = document.CreateNavigator();
            XmlNamespaceManager manager = new(navigator.NameTable);
            manager.AddNamespace("x", "http://rejestrymedyczne.ezdrowie.gov.pl/rpl/eksport-danych-v1.0");
            XPathExpression query = navigator.Compile("/x:produktyLecznicze/x:produktLeczniczy[@postac='Krem' and @nazwaPowszechnieStosowana='Mometasoni furoas']");
            query.SetContext(manager);
            Console.WriteLine("Liczba produktów leczniczych w postaci kremu, których jedyną substancją czynną jest Mometasoni furoas {0}", navigator.Select(query).Count);
        }

        internal static void MakeListOfThreeDrugsWithMostPackages(string filepath)
        {
            XPathDocument document = new(filepath);
            XPathNavigator navigator = document.CreateNavigator();
            XmlNamespaceManager manager = new(navigator.NameTable);

            manager.AddNamespace("x", "http://rejestrymedyczne.ezdrowie.gov.pl/rpl/eksport-danych-v1.0");
            XPathExpression query = navigator.Compile("/x:produktyLecznicze/x:produktLeczniczy");
            query.SetContext(manager);

            XPathNodeIterator nodeIter = navigator.Select(query);
            Dictionary<string, int> packagesDict = new();

            while (nodeIter.MoveNext())
            {
                XPathNavigator n = nodeIter.Current;
                string name = n.GetAttribute("nazwaProduktu", "");
                XPathNodeIterator children = n.SelectChildren(XPathNodeType.Element);
                children.MoveNext();
                children.MoveNext();

                int counterPackages = children.Current.SelectChildren(XPathNodeType.Element).Count;

                if (packagesDict.ContainsKey(name))
                {
                    if (packagesDict[name] < counterPackages)
                        packagesDict[name] = counterPackages;
                }
                else
                    packagesDict[name] = counterPackages;
            }
            packagesDict = packagesDict.OrderByDescending(key => key.Value).ToDictionary(x => x.Key, x => x.Value);
            int maxValue = packagesDict.Values.Max();
            string mostCommonSubstance = packagesDict.FirstOrDefault(substance => substance.Value == maxValue).Key;

            packagesDict = packagesDict.OrderBy(key => key.Value).ToDictionary(x => x.Key, x => x.Value);
            string[] keys = packagesDict.Keys.ToArray();
            packagesDict = packagesDict.OrderBy(value => value.Value).ToDictionary(x => x.Key, x => x.Value);
            int[] values = packagesDict.Values.ToArray();

            Console.WriteLine("Trzy produkty lecznicze, w kórych jest najwięcej różnych opakowań:");
            Console.WriteLine("1." + keys[keys.Length - 1] + ", liczba opakowań: " + values[values.Length - 1]);
            Console.WriteLine("2." + keys[keys.Length - 2] + ", liczba opakowań: " + values[values.Length - 2]);
            Console.WriteLine("3." + keys[keys.Length - 3] + ", liczba opakowań: " + values[values.Length - 3]);
        }
    }
}
