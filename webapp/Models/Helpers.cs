using System;

namespace ChartsMix.Models
{
    public class Helpers
    {
        public static string FormTreeView(Meter node, string name)
        {
            var result = "";
            if (node.Children.Count > 0)
            {
                if (node.Name.Contains("Server"))
                    result += "<li>";
                else
                    result += "<li style='display:none'>";
                result += "<span><i class='fa fa-lg fa-plus-circle'></i> ";
                result += node.Name;
                result += "</span><ul>";
                foreach (var child in node.Children)
                {
                    result += FormTreeView(child,name);
                }
                result += "</ul></li>";
            }
            else
            {
                result += "<li style='display:none'>";
                result += "<span><label class='checkbox inline-block'><input type='checkbox' value=" + node.EntityId + " name=" + name + "Ids><i></i> ";
                result += node.Name;
                result += "</label></span>";
                result += "</li>";
            }
            return String.Format(result);
        }
    }
}