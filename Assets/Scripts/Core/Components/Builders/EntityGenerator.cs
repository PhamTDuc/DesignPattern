using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Guinea.Core.DataHandler;
using Guinea.Core.Inventory;

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
        InventoryLoader m_inventoryLoader;

        [Inject]
        void Initialize(SharedInteraction sharedInteraction, InventoryLoader inventoryLoader)
        {
            m_sharedInteraction = sharedInteraction;
            m_inventoryLoader = inventoryLoader;
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

        public void Serialize()
        {
            m_node = GenerateNode();
            m_json = JsonHandler.SerializeObject(m_node);
        }

        public void Deserialize()
        {
            m_node = JsonHandler.Deserialize<Node>(m_json);
            ComponentBase root;
            WalkInNode(m_node, out root);
            m_sharedInteraction.ResetEntityRoot();
            m_sharedInteraction.SetEntityRoot(root);
        }

        private void WalkInComponentBase(ComponentBase component)
        {
            if (component.GetEnumerator() != null) // * Component can have no children
            {
                foreach (ComponentBase child in component)
                {
                    WalkInComponentBase(child);
                    m_entity.HandleComponent(child);
                }
            }
        }

        private Node GenerateNode()
        {
            ComponentBase root = m_sharedInteraction.Root?.GetComponent<ComponentBase>();
            Node root_node = new Node(root.ItemType, root.transform.position, root.transform.rotation);
            WalkInComponentBaseNode(root, root_node);
            return root_node;
        }

        private void WalkInComponentBaseNode(ComponentBase component, Node node)
        {
            if (component.GetEnumerator() != null) // * Component can have no children
            {
                foreach (ComponentBase child in component)
                {
                    Node child_node = new Node(child.ItemType, child.transform.position, child.transform.rotation);
                    node.AddChild(child_node);
                    WalkInComponentBaseNode(child, child_node);
                }
            }
        }

        private void WalkInNode(Node node, out ComponentBase component)
        {
            if (Enum.TryParse(node.name, out ItemType itemType))
            {
                component = Instantiate(m_inventoryLoader.Items[itemType].obj, node.position, node.rotation).GetComponent<ComponentBase>();
                if (node.children != null)
                {
                    foreach (Node child in node.children)
                    {
                        ComponentBase child_component;
                        WalkInNode(child, out child_component);
                        component.AddComponent(child_component);
                    }
                }
            }
            else
            {
                component = null;
                Debug.LogError($"Fail to Parse {node.name} to enum type<{typeof(ItemType)}>");
            }
        }
    }


    public class Node
    {
        public string name;
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        public List<Node> children;

        public Node(ItemType name, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), List<Node> children = null)
        {
            this.name = name.ToString();
            this.position = position;
            this.rotation = rotation;
            this.children = children;
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

    [System.Serializable]
    public struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;
        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString() => $"SerializableVector3({x}, {y}, {z})";

        public static implicit operator Vector3(SerializableVector3 value)
        {
            return new Vector3(value.x, value.y, value.z);
        }

        public static implicit operator SerializableVector3(Vector3 value)
        {
            return new SerializableVector3(value.x, value.y, value.z);
        }
    }

    [System.Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public override string ToString()
        {
            return $"SerializableQuaternion({x}, {y}, {z}, {w})";
        }

        public static implicit operator Quaternion(SerializableQuaternion value)
        {
            return new Quaternion(value.x, value.y, value.z, value.w);
        }

        public static implicit operator SerializableQuaternion(Quaternion value)
        {
            return new SerializableQuaternion(value.x, value.y, value.z, value.w);
        }
    }
}