﻿/***************************************************************************
 *   MapTile.cs
 *   Based on code from ClintXNA's renderer.
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using UltimaXNA.Entity;
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaWorld.View;
#endregion

namespace UltimaXNA.UltimaWorld.Model
{
    public class MapTile
    {
        private bool m_NeedsSorting = false;

        private Ground m_Ground;
        public Ground Ground
        {
            get
            {
                return m_Ground;
            }
        }

        public int X
        {
            get { return m_Ground.Position.X; }
        }

        public int Y
        {
            get { return m_Ground.Position.Y; }
        }

        private List<BaseEntity> m_Entities;

        public void OnEnter(BaseEntity entity)
        {
            if (entity is Ground)
            {
                if (m_Ground != null)
                    m_Ground.Dispose();
                m_Ground = (Ground)entity;
            }
            m_Entities.Add(entity);
            m_NeedsSorting = true;
        }

        public void OnExit(BaseEntity entity)
        {
            m_Entities.Remove(entity);
        }

        public MapTile()
        {
            m_Entities = new List<BaseEntity>();
        }

        /// <summary>
        /// Checks if the specified z-height is under an item or a ground.
        /// </summary>
        /// <param name="originZ"></param>
        /// <param name="underItem"></param>
        /// <param name="underTerrain"></param>
        public void IsPointUnderAnEntity(int originZ, out BaseEntity underItem, out BaseEntity underTerrain)
        {
            underItem = null;
            underTerrain = null;

            List<BaseEntity> iObjects = this.Items;
            for (int i = iObjects.Count - 1; i >= 0; i--)
            {
                if (iObjects[i].Z <= originZ)
                    continue;

                if (iObjects[i] is StaticItem)
                {
                    UltimaData.ItemData iData = ((StaticItem)iObjects[i]).ItemData;
                    if (iData.IsRoof || iData.IsSurface || iData.IsWall)
                    {
                        if (underItem == null || iObjects[i].Z < underItem.Z)
                            underItem = iObjects[i];
                    }
                }
                else if (iObjects[i] is Ground && iObjects[i].Z >= originZ + 20)
                {
                    underTerrain = iObjects[i];
                }
            }
        }

        public List<StaticItem> GetStatics()
        {
            List<StaticItem> items = new List<StaticItem>();

            for (int i = 0; i < m_Entities.Count; i++)
            {
                if (m_Entities[i] is StaticItem)
                    items.Add((StaticItem)m_Entities[i]);
            }

            return items;
        }

        private bool matchNames(ItemData m1, ItemData m2)
        {
            return (m1.Name == m2.Name);
        }

        private void removeDuplicateObjects()
        {
            int[] itemsToRemove = new int[0x100];
            int removeIndex = 0;

            for (int i = 0; i < m_Entities.Count; i++)
            {
                for (int j = 0; j < removeIndex; j++)
                {
                    if (itemsToRemove[j] == i)
                        continue;
                }

                if (m_Entities[i] is StaticItem)
                {
                    // Make sure we don't double-add a static or replace an item with a static (like doors on multis)
                    for (int j = i + 1; j < m_Entities.Count; j++)
                    {
                        if (m_Entities[i].Z == m_Entities[j].Z)
                        {
                            if (m_Entities[j] is StaticItem && (
                                ((StaticItem)m_Entities[i]).ItemID == ((StaticItem)m_Entities[j]).ItemID ||
                                matchNames(((StaticItem)m_Entities[i]).ItemData, ((StaticItem)m_Entities[j]).ItemData)))
                            {
                                itemsToRemove[removeIndex++] = i;
                                break;
                            }
                        }
                    }
                }
                else if (m_Entities[i] is Item)
                {
                    // if we are adding an item, replace existing statics with the same *name*
                    // We could use same *id*, but this is more robust for items that can open ...
                    // an open door will have a different id from a closed door, but the same name.
                    // Also, don't double add an item.
                    for (int j = i + 1; j < m_Entities.Count; j++)
                    {
                        if (m_Entities[i].Z == m_Entities[j].Z)
                        {
                            if ((m_Entities[j] is StaticItem && matchNames(((Item)m_Entities[i]).ItemData, ((StaticItem)m_Entities[j]).ItemData)) ||
                                (m_Entities[j] is Item && m_Entities[i].Serial == m_Entities[j].Serial))
                            {
                                itemsToRemove[removeIndex++] = j;
                                continue;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < removeIndex; i++)
            {
                m_Entities.RemoveAt(itemsToRemove[i] - i);
            }
        }

        public List<BaseEntity> Items
        {
            get
            {
                if (m_NeedsSorting)
                {
                    removeDuplicateObjects();
                    Entity.EntityViews.KrriosSort.Sort(m_Entities);
                    m_NeedsSorting = false;
                }
                return m_Entities;
            }
        }

        public void Resort()
        {
            m_NeedsSorting = true;
        }
    }
}
