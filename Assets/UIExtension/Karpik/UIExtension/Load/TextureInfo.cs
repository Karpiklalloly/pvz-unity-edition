using System;
using UnityEngine;

namespace Karpik.UIExtension.Load
{
    [Serializable]
    public class TextureInfo
    {
        public Texture Texture;
        public string LoadPath;

        public TextureInfo(Texture texture, string loadPath)
        {
            Texture = texture;
            LoadPath = loadPath;
        }

        private TextureInfo(string loadPath)
        {
            LoadPath = loadPath;
            Texture = TextureLoader.Instance[loadPath].Texture;
        }
    }
}