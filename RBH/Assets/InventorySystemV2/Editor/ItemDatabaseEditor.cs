using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Sirenix.Utilities.Editor;
using InventorySystem;
using UnityEditorInternal;

namespace InventorySystem.Editor
{
    public class ItemDatabaseEditor : OdinEditorWindow
    {
        private const string ItemsFolderKey = "ItemDatabaseEditor.itemsFolder";
        private const string DatabasePathKey = "ItemDatabaseEditor.databasePath";

        [MenuItem("Tools/Item Database Editor")]
        private static void OpenWindow()
        {
            var window = GetWindow<ItemDatabaseEditor>();
            window.Show();
            window.LoadEditorState();
        }

        private new void OnDestroy()
        {
            base.OnDestroy();
            SaveEditorState();
        }

        private void SaveEditorState()
        {
            EditorPrefs.SetString(ItemsFolderKey, itemsFolder);

            if (database != null)
            {
                string databasePath = AssetDatabase.GetAssetPath(database);
                EditorPrefs.SetString(DatabasePathKey, databasePath);
            }
        }

        private void LoadEditorState()
        {
            itemsFolder = EditorPrefs.GetString(ItemsFolderKey, "Assets/Items");

            string databasePath = EditorPrefs.GetString(DatabasePathKey, null);
            if (!string.IsNullOrEmpty(databasePath))
            {
                database = AssetDatabase.LoadAssetAtPath<ItemDatabase>(databasePath);
                if (database != null)
                {
                    LoadItems();
                }
            }
        }

        [SerializeField]
        private ItemDatabase database;

        [FolderPath]
        public string itemsFolder = "Assets/Items";

        [ShowIf("@UnityEngine.Application.isPlaying")]
        public ContainerHandler targetContainerHandler;

        private void OnValidate()
        {
            if (database != null)
            {
                LoadItems();
            }
        }
        private void LoadItems()
        {
            if (database == null)
            {
                Debug.LogWarning("Database not assigned.");
                return;
            }

            items = new List<ItemData>(database.items);
            Debug.Log("Items loaded from the database.");
        }

        private List<ItemData> items;

        [Button("Create New Item")]
        private void AddNewItem()
        {
            if (database == null)
            {
                Debug.LogWarning("Database not assigned.");
                return;
            }

            if (string.IsNullOrEmpty(itemsFolder))
            {
                Debug.LogWarning("Please set a folder path for items in the database.");
                return;
            }

            if (!Directory.Exists(itemsFolder))
            {
                Directory.CreateDirectory(itemsFolder);
            }

            ItemData newItem = CreateInstance<ItemData>();
            newItem.GenerateUniqueId();
            newItem.name = "New Item";
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(itemsFolder, $"{newItem.name}.asset"));
            AssetDatabase.CreateAsset(newItem, assetPath);

            items.Add(newItem);
            database.items.Add(newItem);  // Add to database list
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // Function to remove an item from the list and project assets
        private void RemoveItem(ItemData item)
        {
            if (item == null || !items.Contains(item)) return;

            items.Remove(item);
            database.items.Remove(item);

            string assetPath = AssetDatabase.GetAssetPath(item);
            if (!string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void CloneItem(ItemData item)
        {
               if (item == null || !items.Contains(item)) return;

            ItemData newItem = Instantiate(item);
            newItem.name = item.name + " (Clone)";
            newItem.GenerateUniqueId();
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(itemsFolder, $"{newItem.name}.asset"));
            AssetDatabase.CreateAsset(newItem, assetPath);

            items.Add(newItem);
            database.items.Add(newItem);  // Add to database list
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void AddItemToSelectedContainer(ItemStack itemStack)
        {
            if (targetContainerHandler == null)
            {
                Debug.LogWarning("No container handler selected.");
                return;
            }
            Container focusedContainer = targetContainerHandler.Container;
            if (focusedContainer == null)
            {
                Debug.LogWarning("No container was created in container handler.");
                return;
            }
            focusedContainer.AddItem(itemStack, out int _);
        }

        // Display each item inline with Edit and Remove buttons in a layout
        [ShowInInspector]
        private List<ItemData> ItemsWithButtons => items;

        // Custom item drawer to display edit and remove buttons
        [OnInspectorGUI]
        private void DrawItems()
        {
            ItemData wantsToClone = null;
            if (items == null) return;

            foreach (var item in items)
            {
                if (item == null) continue;

                SirenixEditorGUI.BeginBox(item.name);
                {
                    Event evt = Event.current;
                    Rect boxRect = GUILayoutUtility.GetLastRect();
                    if (evt.type == EventType.ContextClick && boxRect.Contains(evt.mousePosition))
                    {
                        GenericMenu menu = new GenericMenu();
                        //menu.AddItem(new GUIContent("Edit"), false, () => ItemEditorWindow.OpenWindow(item));
                       // menu.AddItem(new GUIContent("Remove"), false, () => RemoveItem(item));
                        menu.AddItem(new GUIContent("Duplicate"), false, () => CloneItem(item));
                        menu.AddItem(new GUIContent("Select"), false, () => Selection.activeObject = item);
                        if (EditorApplication.isPlaying)
                        {
                            menu.AddItem(new GUIContent("Add to Selected Container"), false, () => AddItemToSelectedContainer(new ItemStack(item, false,1)));
                        }
                        menu.ShowAsContext();
                        evt.Use();
                    }
                    EditorGUILayout.BeginHorizontal();

                    // Display inline preview or editor of the item
                    //EditorGUILayout.ObjectField(item, typeof(ItemData), false, GUILayout.Width(150));

                    if (item.ItemSprite != null)
                    {
                        GUILayout.Label(item.ItemSprite.texture, GUILayout.Width(50), GUILayout.Height(50));
                    }
                    else
                    {
                        GUILayout.Label("No Sprite", GUILayout.Width(50), GUILayout.Height(50));
                    }

                    // Add "Edit" button
                    if (GUILayout.Button("Edit", GUILayout.Width(60)))
                    {
                        //Selection.activeObject = item;
                        ItemEditorWindow.OpenWindow(item);
                    }
                    
                    // Add the Remove button to delete this item
                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        bool confirm = EditorUtility.DisplayDialog("Remove Item", $"Are you sure you want to remove {item.name}?", "Yes", "No");
                        if (confirm) RemoveItem(item);
                        break;  
                    }
                    if (Application.isPlaying)
                    {
                        if (GUILayout.Button("Add to target container"))
                        {
                            AddItemToSelectedContainer(new ItemStack(item, true, 1));
                        }
                    }
                    if (string.IsNullOrEmpty(item.Id))
                    {
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("Generate Id",GUILayout.Width(80)))
                        {
                            item.GenerateUniqueId();
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    //if (GUILayout.Button("Duplicate", GUILayout.Width(80)))
                    //{
                    //    wantsToClone = item;
                    //}
                    //if (GUILayout.Button("Select", GUILayout.Width(80)))
                    //{
                    //    Selection.activeObject = item;
                    //}

                    EditorGUILayout.EndHorizontal();
                }
                SirenixEditorGUI.EndBox();
                
            }
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh",GUILayout.Width(80)))
            {
                LoadItems();
            }
            if (wantsToClone != null)
            {
                CloneItem(wantsToClone);
            }
        }

    }
    public class ItemEditorWindow : OdinEditorWindow
    {
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        public ItemData currentItem;

        public static void OpenWindow(ItemData item)
        {
            var window = CreateInstance<ItemEditorWindow>();
            window.currentItem = item;
            window.titleContent = new GUIContent($"Editing: {item.name}");
            window.ShowUtility(); // Opens as a utility window, so it’s lightweight and non-modal
        }

        protected override void OnBeginDrawEditors()
        {
            base.OnBeginDrawEditors();

            if (currentItem == null)
            {
                EditorGUILayout.HelpBox("No item selected.", MessageType.Info);
                return;
            }

            // Use Odin's DrawEditor method to render the properties of currentItem
            // SirenixEditorFields.UnityObjectField(currentItem, typeof(ItemData), true);
            //this.DrawEditor(0);
        }
    }
}
