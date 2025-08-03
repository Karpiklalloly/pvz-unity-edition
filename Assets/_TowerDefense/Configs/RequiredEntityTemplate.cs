using System.Collections.Generic;
using System.Linq;
using DCFApixels.DragonECS;
using TriInspector;
using UnityEngine;

namespace TowerDefense
{
    public abstract class RequiredEntityTemplate : ScriptableEntityTemplate
    {
        [SerializeReference]
        [ReferenceButton(true, typeof(IComponentTemplate))]
        [Required]
        private IComponentTemplate[] _additionalComponents;
        public T Get<T>() where T : IEcsComponent
        {
            foreach (var template in GetComponentTemplates())
            {
                if (template.Type == typeof(T))
                {
                    return (T)template.GetRaw();
                }
            }

            return default;
        }
        
        protected abstract IEnumerable<IComponentTemplate> GetRequiredComponents();

        private void OnEnable()
        {
            var requiredComponents = GetRequiredComponents();
            var t = GetComponentTemplates();
            List<IComponentTemplate> tt = new();
            foreach (var tem in t)
            {
                if (_additionalComponents.Any(x => x.Type == tem.Type))
                {
                    continue;
                }
                tt.Add(tem);
            }

            var templates = tt.ToArray();
            
            var componentTemplates = requiredComponents as IComponentTemplate[] ?? requiredComponents.ToArray();
            if (templates.Length != componentTemplates.Length)
            {
                Set(componentTemplates);
                return;
            }
            
            for (int i = 0; i < componentTemplates.Length && i < templates.Length; i++)
            {
                if (templates[i].GetType() == componentTemplates.ElementAt(i).GetType()) continue;
                
                Set(componentTemplates);
                return;
            }
            
            Set(componentTemplates);
        }
        
        private void Set(IEnumerable<IComponentTemplate> templates)
        {
            List<IComponentTemplate> t = new();
            var tt = templates.ToList();
            foreach (var template in GetComponentTemplates())
            {
                var index = tt.FindIndex(x => x.Type == template.Type);
                if (index != -1)
                {
                    t.Add(template);
                    tt.RemoveAt(index);
                }
            }
            t.AddRange(tt);
            t.AddRange(_additionalComponents);
            SetComponentTemplates(t);
        }
    }
}