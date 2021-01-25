﻿using System.Linq;
using LDtkUnity.UnityAssets;
using UnityEditor;
using UnityEngine;

namespace LDtkUnity.Editor
{
    [CustomEditor(typeof(LDtkLevelFile))]
    public class LDtkLevelFileEditor : LDtkJsonFileEditor<Level>
    {
        private int? _layerCount = null;
        private int? _intGridValueCount = null;
        private int? _autoTileCount = null;
        private int? _gridTileCount = null;
        private int? _entityCount = null;
        
        protected override void DrawInspectorGUI(Level level)
        {
            EditorGUILayout.TextField("Identifier", level.Identifier);
            
            LayerInstance[] layers = level.LayerInstances;
            
            if (layers == null)
            {
                return;
            }
            
            {
                _layerCount ??= layers.Length;
                string layerName = _layerCount == 1 ? "Layer" : "Layers";
                EditorGUILayout.LabelField($"{_layerCount} {layerName}");
            }

            {
                _intGridValueCount ??= layers.Where(p => p.IsIntGridLayer).SelectMany(p => p.IntGrid).Count();
                string intGridValueName = _intGridValueCount == 1 ? "Int Grid Value" : "Int Grid Values";
                EditorGUILayout.LabelField($"{_intGridValueCount} {intGridValueName}");
            }

            {
                _autoTileCount ??= layers.Where(p => p.IsAutoTilesLayer).SelectMany(p => p.AutoLayerTiles)
                    .Count();
                string tileName = _autoTileCount == 1 ? "Auto Tile" : "Auto Tiles";
                EditorGUILayout.LabelField($"{_autoTileCount} {tileName}");
            }
            
            {
                _gridTileCount = layers.Where(p => p.IsGridTilesLayer).SelectMany(p => p.GridTiles)
                    .Count();
                string tileName = _gridTileCount == 1 ? "Grid Tile" : "Grid Tiles";
                EditorGUILayout.LabelField($"{_gridTileCount} {tileName}");
            }
                
            {
                _entityCount = layers.Where(p => p.IsEntityInstancesLayer).SelectMany(p => p.EntityInstances)
                    .Count();
                string entityName = _entityCount == 1 ? "Entity" : "Entities";
                EditorGUILayout.LabelField($"{_entityCount} {entityName}");
            }
        }
    }
}