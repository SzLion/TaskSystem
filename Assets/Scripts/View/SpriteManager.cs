using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Task
{
    public class SpriteManager : Singleton<SpriteManager>
    {
        private SpriteManager()
        {
        }

        private Dictionary<string, Sprite> m_sprites = new Dictionary<string, Sprite>();

        public Sprite GetSprite(string name)
        {
            if (m_sprites.ContainsKey(name))
                return m_sprites[name];
            var sprite = Resources.Load<Sprite>("Sprites/" + name);
            m_sprites.Add(name, sprite);
            return sprite;
        }
    }
}