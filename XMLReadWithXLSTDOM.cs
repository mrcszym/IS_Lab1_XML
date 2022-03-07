using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace IS_Labs
{
    internal class XMLReadWithXLSTDOM
    {
        internal static void Read(string filepath)
        {
            XPathDocument document = new XPathDocument(filepath);
            XPathNavigator navigator = document.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            int count = 0;
            manager.AddNamespace("x", "http://rejestrymedyczne.ezdrowie.gov.pl/rpl/eksport-danych-v1.0");
            XPathExpression query = navigator.Compile("/x:produktyLecznicze/x:produktLeczniczy");
            //XPathExpression query = navigator.Compile("/x:produktyLecznicze/x:produktLeczniczy[@postac='Krem' and @nazwaPowszechnieStosowana='Mometasoni furoas']");
            query.SetContext(manager);
            XPathNodeIterator nodeIter = navigator.Select(query);
            Dictionary<string, HashSet<string>> nazwyPowszechne = new Dictionary<string, HashSet<string>>();
            while (nodeIter.MoveNext())
            {
                XPathNavigator n = nodeIter.Current;
                string nazwa = n.GetAttribute("nazwaPowszechnieStosowana", "");
                string postac = n.GetAttribute("postac", "");
                if (!nazwyPowszechne.ContainsKey(nazwa))
                {
                    nazwyPowszechne.Add(nazwa, new HashSet<string>());
                    nazwyPowszechne[nazwa].Add(postac);
                }
                else
                {
                    if (!nazwyPowszechne[nazwa].Contains(postac))
                    {
                        nazwyPowszechne[nazwa].Add(postac);
                    }
                }
            }
        }
        internal static void ListThreeProductsWithMostPackageOptionsFromXML(string filepath)
        {
            XPathDocument document = new XPathDocument(filepath);
            XPathNavigator navigator = document.CreateNavigator();
            XmlNamespaceManager manager = new XmlNamespaceManager(navigator.NameTable);
            int count = 0;
            manager.AddNamespace("x", "http://rejestrymedyczne.ezdrowie.gov.pl/rpl/eksport-danych-v1.0");
            XPathExpression query = navigator.Compile("/x:produktyLecznicze/x:produktLeczniczy");
            query.SetContext(manager);
            XPathNodeIterator nodeIter = navigator.Select(query);
            Dictionary<string, int> ilosciOpakowan = new Dictionary<string, int>();
            while (nodeIter.MoveNext())
            {
                XPathNavigator n = nodeIter.Current;
                string nazwa = n.GetAttribute("nazwaProduktu", "");
                XPathNodeIterator children = n.SelectChildren(XPathNodeType.Element);
                children.MoveNext();
                children.MoveNext();
                int opakowaniaCntr = children.Current.SelectChildren(XPathNodeType.Element).Count;
                ilosciOpakowan[nazwa] = opakowaniaCntr;
            }
            ilosciOpakowan = ilosciOpakowan.OrderBy(key => key.Value).ToDictionary(x => x.Key, x => x.Value);
            string[] keys = ilosciOpakowan.Keys.ToArray();
            int[] values = ilosciOpakowan.Values.ToArray();
            Console.WriteLine("Produkty lecznicze z najwieksza iloscia dostepnych opakowan to: {0}:{3},{1}:{4},{2}:{5}", keys[keys.Length - 1], keys[keys.Length - 2], keys[keys.Length - 3], values[values.Length - 1], values[values.Length - 2], values[values.Length - 3]);
        }
    }
}
