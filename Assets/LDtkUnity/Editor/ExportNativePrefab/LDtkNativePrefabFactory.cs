﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace LDtkUnity.Editor
{
    public class LDtkNativePrefabFactory
    {
        private readonly LDtkNativePrefabAssets _assets;
        
        private readonly List<Tilemap> _tilemaps = new List<Tilemap>();
        private readonly List<SpriteRenderer> _renderers = new List<SpriteRenderer>();

        private readonly Dictionary<TileBase, TileBase> _oldToNewTiles = new Dictionary<TileBase, TileBase>();
        private readonly Dictionary<Sprite, Sprite> _oldToNewBackgrounds = new Dictionary<Sprite, Sprite>();

        public LDtkNativePrefabFactory(LDtkNativePrefabAssets assets)
        {
            _assets = assets;
        }
        
        public GameObject CreateNativePrefabInstance(GameObject importRoot)
        {
            if (importRoot == null)
            {
                Debug.LogError("Null input");
                return null;
            }

            GameObject newRoot = (GameObject)PrefabUtility.InstantiatePrefab(importRoot);
            if (newRoot == null)
            {
                Debug.LogError("Null instantiation");
                return null;
            }

            PrefabUtility.UnpackPrefabInstance(newRoot, PrefabUnpackMode.OutermostRoot, InteractionMode.UserAction);
            TryRemove<LDtkComponentProject>(newRoot);
            TryRemove<LDtkDetachChildren>(newRoot);

            foreach (Transform level in newRoot.transform)
            {
                StripLevel(level.gameObject);
            }
            
            PopulateOldToNewTiles();
            PopulateOldToNewBackgrounds();
            
            SwapOldToNewTiles();
            SwapOldToNewBackgrounds();

            return newRoot;
        }

        private void StripLevel(GameObject level)
        {
            TryRemove<LDtkDetachChildren>(level);
            TryRemove<LDtkFields>(level);
            TryRemove<LDtkComponentLevel>(level);

            foreach (Transform layer in level.transform)
            {
                StripLayer(layer.gameObject);
            }
        }

        private void StripLayer(GameObject layer)
        {
            //for entities root obj
            TryRemove<LDtkDetachChildren>(layer);

            TryCollectComponent(layer, _renderers, renderer => renderer.sprite != null);

            foreach (Transform layerElement in layer.transform)
            {
                StripLayerElements(layerElement.gameObject);
            }
        }
        
        private void StripLayerElements(GameObject layerElement)
        {
            //for entity
            TryRemove<LDtkFields>(layerElement);
            TryRemove<LDtkEntityDrawerComponent>(layerElement);
            
            TryCollectComponent(layerElement, _tilemaps);
        }

        private static void TryRemove<T>(GameObject obj) where T : Component
        {
            T[] components = obj.GetComponents<T>();
            if (components.IsNullOrEmpty())
            {
                return;
            }

            foreach (T component in components)
            {
                Object.DestroyImmediate(component);
            }
        }

        private delegate bool ComponentCheck<in T>(T input);
        private void TryCollectComponent<T>(GameObject obj, List<T> list, ComponentCheck<T> onlyIf = null)
        {
            if (!obj.TryGetComponent(out T component))
            {
                return;
            }

            if (onlyIf == null || onlyIf.Invoke(component))
            {
                list.Add(component);
            }
        }

        private void PopulateOldToNewTiles()
        {
            List<Tile> newTiles = _assets.ArtTiles.Concat(_assets.IntGridTiles).ToList();
            foreach (Tilemap tilemap in _tilemaps)
            {
                TileBase[] oldTiles = tilemap.GetTilesBlock(tilemap.cellBounds);
                foreach (TileBase oldTile in oldTiles)
                {
                    if (oldTile == null || _oldToNewTiles.ContainsKey(oldTile))
                    {
                        continue;
                    }

                    TileBase newTile = newTiles.FirstOrDefault(newPotentialTile => newPotentialTile.name == oldTile.name);
                    if (newTile == null)
                    {
                        Debug.LogError("Problem getting a new tile, they should always exist");
                        continue;
                    }

                    _oldToNewTiles.Add(oldTile, newTile);
                }
            }
        }

        private void PopulateOldToNewBackgrounds()
        {
            foreach (SpriteRenderer renderer in _renderers)
            {
                if (renderer == null)
                {
                    Debug.LogError("null renderer");
                    continue;
                }

                Sprite oldBg = renderer.sprite;
                if (oldBg == null || _oldToNewBackgrounds.ContainsKey(oldBg))
                {
                    continue;
                }

                Debug.Log($"{_assets.BackgroundArtifacts.Count}");
                Sprite newBg = _assets.BackgroundArtifacts.FirstOrDefault(newPotentialBg => B(newPotentialBg, oldBg));
                
                
                
                if (newBg == null)
                {
                    
                    
                    Debug.LogError("Problem getting a new background, they should always exist.");
                    continue;
                }

                _oldToNewBackgrounds.Add(oldBg, newBg);
            }
        }

        private static bool B(Sprite newPotentialBg, Sprite oldBg)
        {
            Debug.Log($"new grabbed bg{newPotentialBg}", newPotentialBg);
            Debug.Log($"on import{oldBg}", oldBg);
            
            return newPotentialBg.name == oldBg.name;
        }

        private void SwapOldToNewTiles()
        {
            foreach (Tilemap tilemap in _tilemaps)
            {
                foreach (TileBase oldTile in _oldToNewTiles.Keys)
                {
                    if (!tilemap.ContainsTile(oldTile))
                    {
                        continue;
                    }

                    TileBase newTile = _oldToNewTiles[oldTile];
                    tilemap.SwapTile(oldTile, newTile);
                }
            }
        }

        private void SwapOldToNewBackgrounds()
        {
            foreach (SpriteRenderer renderer in _renderers)
            {
                if (renderer.sprite == null)
                {
                    continue;
                }

                foreach (Sprite oldBg in _oldToNewBackgrounds.Keys)
                {
                    if (renderer.sprite != oldBg)
                    {
                        continue;
                    }

                    Sprite newBg = _oldToNewBackgrounds[oldBg];
                    renderer.sprite = newBg;
                }
            }
        }
    }
}