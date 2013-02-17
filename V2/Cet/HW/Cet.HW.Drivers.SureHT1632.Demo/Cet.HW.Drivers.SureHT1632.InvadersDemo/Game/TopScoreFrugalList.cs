using System;
using Microsoft.SPOT;

/*
 * Copyright 2012, 2013 by Mario Vernari, Cet Electronics
 * Part of "Cet Open Toolbox" (http://cetdevelop.codeplex.com/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace Cet.HW.Drivers.Invaders
{
    public class TopScoreFrugalList
    {
        private const int ItemsCount = 4;


        public TopScoreFrugalList()
        {
            for (int i = 0; i < ItemsCount; i++)
            {
                TopScoreEntry item = this._items[i] = new TopScoreEntry();
                item.Name = "....";
            }
        }


        private readonly TopScoreEntry[] _items = new TopScoreEntry[4];


        public int Count
        {
            get { return ItemsCount; }
        }


        public TopScoreEntry GetItem(int index)
        {
            return this._items[index];
        }


        public int Add(TopScoreEntry item)
        {
            for (int i = 0; i < ItemsCount; i++)
            {
                if (this._items[i].Score < item.Score)
                {
                    for (int k = ItemsCount - 2; k >= i; k--)
                    {
                        this._items[k + 1] = this._items[k];
                    }

                    this._items[i] = item;
                    return i;
                }
            }

            return -1;
        }

    }


    public class TopScoreEntry
    {
        public string Name = string.Empty;
        public int Score;
    }
}
