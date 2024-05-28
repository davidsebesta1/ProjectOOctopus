using Microsoft.UI.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;

namespace ProjectOOctopus.Extensions
{
    public static class ElementExtension
    {
        public static Element GetParent(this Element element, int level = 1)
        {
            if (level == 1) return element.Parent;
            return GetParent(element.Parent, level - 1);
        }
    }
}
