
/* Unmerged change from project 'ProjectOOctopus (net8.0-windows10.0.19041.0)'
Before:
using Microsoft.UI.Composition;
After:
using Microsoft.Maui;
using Microsoft.UI.Composition;
*/

/* Unmerged change from project 'ProjectOOctopus (net8.0-maccatalyst)'
Before:
using Microsoft.UI.Composition;
After:
using Microsoft.Maui;
using Microsoft.UI.Composition;
*/

/* Unmerged change from project 'ProjectOOctopus (net8.0-ios)'
Before:
using Microsoft.UI.Composition;
After:
using Microsoft.Maui;
using Microsoft.UI.Composition;
*/

/* Unmerged change from project 'ProjectOOctopus (net8.0-windows10.0.19041.0)'
Before:
using System.Threading.Tasks;
using Microsoft.Maui;
After:
using System.Threading.Tasks;
*/

/* Unmerged change from project 'ProjectOOctopus (net8.0-maccatalyst)'
Before:
using System.Threading.Tasks;
using Microsoft.Maui;
After:
using System.Threading.Tasks;
*/

/* Unmerged change from project 'ProjectOOctopus (net8.0-ios)'
Before:
using System.Threading.Tasks;
using Microsoft.Maui;
After:
using System.Threading.Tasks;
*/
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
