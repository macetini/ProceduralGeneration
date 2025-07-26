using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.Elements;
using Assets.Scripts.DungeonGenerator.Utils;
using UnityEngine;

namespace Assets.Meta.Sets
{
    [CreateAssetMenu(fileName = "DungeonSet", menuName = "DungeonSet", order = 2)]
    public class DungeonSet : ScriptableObject
    {
        public string setName = "";

        public List<Element> spawnTemplates = new List<Element>();
        public List<Element> roomTemplates = new List<Element>();
        public List<Element> hallwayTemplates = new List<Element>();
        public List<Element> closingTemplates = new List<Element>();

        public List<Door> doorTemplates = new List<Door>();

        private List<Element> openElements;
        private List<Element> openTwoWayElements;
        private List<Element> hallwayElements;
        private List<Element> closingElements;

        public void InitTemplateElements()
        {
            openElements = GetTemplateElements(roomTemplates);
            openTwoWayElements = GetTemplateElements(roomTemplates);
            hallwayElements = GetTemplateElements(hallwayTemplates);
            closingElements = GetTemplateElements(closingTemplates);
        }

        protected static List<Element> GetTemplateElements(List<Element> templates)
        {
            int templatesCount = templates.Count;

            List<Element> elements = new List<Element>(templatesCount);

            for (int i = 0; i < templatesCount; i++)
            {
                Element element = templates[i];
                elements.Add(element);
            }

            return elements;
        }

        public Dictionary<string, List<GameObject>> GetElementPools(Dictionary<string, List<GameObject>> pools)
        {
            InitTemplatePool(roomTemplates, pools);
            InitTemplatePool(hallwayTemplates, pools);
            InitTemplatePool(closingTemplates, pools);

            return pools;
        }

        protected static void InitTemplatePool(List<Element> templates, Dictionary<string, List<GameObject>> pools)
        {
            int templatesCount = templates.Count;
            for (int i = 0; i < templatesCount; i++)
            {
                Element element = templates[i];
                pools[element.ID] = new List<GameObject>();
            }
        }

        // MOAR REFACTOR!! GENERALIZE! GENERALIZE!    

        public Element[] GetAllHallwayElementsShuffled(DRandom random)
        {
            hallwayElements.Shuffle(random.random);
            return hallwayElements.ToArray();
        }

        public Element[] GetAllOpenElementsShuffled(DRandom random)
        {
            openElements.Shuffle(random.random);
            return openElements.ToArray();
        }

        public Element[] GetAllTwoWayOpenElementsShuffled(DRandom random)
        {
            openTwoWayElements.Shuffle(random.random);
            return openTwoWayElements.ToArray();
        }

        public Element[] GetAllClosingElementsShuffled(DRandom random)
        {
            closingElements.Shuffle(random.random);
            return closingElements.ToArray();
        }
        //
    }
}