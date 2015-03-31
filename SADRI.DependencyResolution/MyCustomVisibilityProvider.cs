using System.Collections.Generic;
using System.Web;
using MvcSiteMapProvider;

namespace SADRI.Infrastructure.DependencyResolution
{
    public class MyCustomVisibilityProvider : SiteMapNodeVisibilityProviderBase
    {
        public override bool IsVisible(ISiteMapNode node, IDictionary<string, object> sourceMetadata)
        {
            string visibility = string.Empty;
            //Se pasa por parámetro en la llamada al SiteMap. _Layout.
            var value = sourceMetadata["user"];

            if (node.Attributes.ContainsKey("visibility"))
            {
                visibility = node.Attributes["visibility"].GetType().Equals(typeof(string)) ? node.Attributes["visibility"].ToString() : string.Empty;
            }

            if (string.IsNullOrEmpty(visibility))
            {
                return true;
            }
            visibility = visibility.Trim();

            //process visibility
            switch (visibility)
            {
                case "Condition1":
                    //...
                    return true;

                case "Condition2":
                    //...
                    return false;
            }

            return true;
        }
    }
}
