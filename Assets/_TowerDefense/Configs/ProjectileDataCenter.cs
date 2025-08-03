using System;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace TowerDefense
{
    [Serializable]
    public class ProjectileDataCenter
    {
        [SerializeField] [Required]
        private List<ProjectileConfig> _projectileConfigs = new();

        public ProjectileConfig this[string name]
        {
            get
            {
                return _projectileConfigs.Find(config => config.Name == name);
            }
        }
    }
}