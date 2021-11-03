using System.Collections.Generic;
using UnityEngine;
using Zenject;
using DataHandler;

namespace Guinea.Core.Components
{
    [RequireComponent(typeof(Entity))]
    public class EntityGenerator : MonoBehaviour
    {
        [TextArea(2, 10)]
        [SerializeField] string m_json;
        [SerializeField] Entity m_entity;


        Node m_node;
        SharedInteraction m_sharedInteraction;

        [Inject]
        void Initialize(SharedInteraction sharedInteraction)
        {
            m_sharedInteraction = sharedInteraction;
        }

        public void Generate()
        {
            ComponentBase root = m_sharedInteraction.Root?.GetComponent<ComponentBase>();
            if (root != null)
            {
                m_entity.transform.position = root.transform.position;
                root.transform.SetParent(m_entity.transform);
                WalkInComponentBase(root);
                m_entity.CleanUp();
                Debug.Log("Generate Entity DONE!!");
            }
            else
            {
                Debug.LogWarning("No Root Frame SPECIFIED!!");
            }
        }


        public Node GenerateNode()
        {
            Node node = new Node("Root");
            Node child_00 = new Node("Child_00");
            Node child_01 = new Node("Child_01");
            Node child_01_00 = new Node("Child 01_00");
            Node child_01_01 = new Node("Child 01_01");
            Node child_02 = new Node("Child_02");
            child_01.AddChild(child_01_00, child_01_01);
            node.AddChild(child_00, child_01, child_02);
            return node;
        }

        public void Serialize()
        {
            m_json = JsonHandler.SerializeObject(m_node);
        }

        public void Deserialize()
        {
            m_node = null;
            m_node = JsonHandler.Deserialize<Node>(m_json);
            Debug.Log(m_node.children[1].children.GetType());
        }
        private void WalkInComponentBase(ComponentBase componentRoot)
        {
            if (componentRoot.GetEnumerator() != null) // * ComponentRoot can have no children
            {
                foreach (ComponentBase child in componentRoot)
                {
                    WalkInComponentBase(child);
                    m_entity.HandleComponent(child);
                }
            }
        }
    }


    public class Node
    {
        public string name;
        public List<Node> children;

        public Node(string name)
        {
            this.name = name;
        }

        public void AddChild(params Node[] nodes)
        {
            if (children == null)
            {
                children = new List<Node>();
            }
            foreach (Node node in nodes)
            {
                children.Add(node);
            }
        }

        public override string ToString()
        {
            return $"Node(name={this.name}, children_count={(this.children != null ? this.children.Count : 0)})";
        }
    }
}