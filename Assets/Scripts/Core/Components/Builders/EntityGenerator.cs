using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Guinea.Core.DataHandler;
using Guinea.Core.Inventory;

namespace Guinea.Core.Components
{
    public class EntityGenerator : MonoBehaviour
    {
        [SerializeField] Entity m_entityPrefab;

        SharedInteraction m_sharedInteraction;
        InventoryLoader m_inventoryLoader;
        Inventory.Inventory m_inventory;

        [Inject]
        void Initialize(SharedInteraction sharedInteraction, InventoryLoader inventoryLoader, Inventory.Inventory inventory)
        {
            m_sharedInteraction = sharedInteraction;
            m_inventoryLoader = inventoryLoader;
            m_inventory = inventory;
            Commons.Logger.Log("EntityGenerator::Initialize()");
        }

        void Start()
        {
            DeserializeEntity();
        }

        public void Generate()
        {
            ComponentBase root = m_sharedInteraction.Root?.GetComponent<ComponentBase>();
            Commons.Logger.Assert(m_sharedInteraction.ComponentsContainer.childCount > 0, "ComponentsContainer must have at least one ComponentBase!");
            if (root != null && root.GetEnumerator() != null)
            {
                SerializeEntity(root);
                // Vector3 offset = 0.1f * Vector3.up; // * Fix issue when WheelCollider is too close to the ground
                Entity entity = Instantiate(m_entityPrefab, root.transform.position, root.transform.rotation);
                root.transform.SetParent(entity.transform);
                WalkInComponentBase(root, entity);
                entity.Init();
                RemoveUnattachedComponents();
                Commons.Logger.Log("EntityGenerator::Generate(): Generate Entity DONE!!");
            }
            else
            {
                Commons.Logger.LogWarning("EntityGenerator::Generate(): No root available or no components attached to root!");
            }
        }

        public void SerializeEntity(ComponentBase root)
        {
            Node node = GenerateNode(root);
            string entityJson = JsonHandler.SerializeObject(node);
            Commons.Logger.Log("Entity Json: " + entityJson);
            m_inventory.SaveToEntityJson(entityJson);
        }

        public void DeserializeEntity()
        {
            if (!String.IsNullOrEmpty(m_inventory.EntityJson))
            {
                Node node = JsonHandler.Deserialize<Node>(m_inventory.EntityJson);
                ComponentBase root;
                WalkInNode(node, out root);
                m_sharedInteraction.SetEntityRoot(root, true);
            }
        }

        private void WalkInComponentBase(ComponentBase component, Entity entity)
        {
            if (component.HasChildren) // * Component can have no children
            {
                foreach (ComponentBase child in component)
                {
                    WalkInComponentBase(child, entity);
                    entity.HandleComponent(child);
                }
            }
            component.CleanUp();
        }

        private Node GenerateNode(ComponentBase root)
        {
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
            component = null;
            if (Enum.TryParse(node.name, out ItemType itemType))
            {
                component = Instantiate(m_inventoryLoader.Items[itemType].obj, node.position, node.rotation, m_sharedInteraction.ComponentsContainer).GetComponent<ComponentBase>();
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
                Debug.LogError($"Fail to Parse {node.name} to enum type<{typeof(ItemType)}>");
            }
        }

        private void RemoveUnattachedComponents()
        {
            ComponentBase[] components = m_sharedInteraction.ComponentsContainer.GetComponentsInChildren<ComponentBase>();
            foreach (ComponentBase component in components)
            {
                switch (component.Type)
                {
                    case ComponentType.FRAME:
                        if (component.GetEnumerator() == null)
                        {
                            DestroyComponent(component);
                        }
                        break;
                    default:
                        if (!component.IsAttached)
                        {
                            DestroyComponent(component);
                        }
                        break;
                }
            }
        }

        private void DestroyComponent(ComponentBase component)
        {
            m_inventory.Add(component.ItemType);
            Destroy(component.gameObject);
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