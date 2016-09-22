using SinplestLogger.Properties;
using System.Text;

namespace SinplestLogger.Mailer
{
    public class ContentDescription
    {
        private static string _htmlContentDescription = Resource.ContentDescription;
        StringBuilder _contentBuilder = new StringBuilder();

        public ContentDescription(string contentDescription, string contentException="")
        {
            _contentBuilder.Append(_htmlContentDescription.Replace("ContentName", ContentDescriptionType.Description.ToString()).Replace("ContentText", contentDescription));
            if(!string.IsNullOrEmpty(contentException))
            {
                _contentBuilder.Append(_htmlContentDescription.Replace("ContentName", ContentDescriptionType.Exception.ToString()).Replace("ContentText", contentException));
            }
        }

        public override string ToString()
        {

            return _contentBuilder.ToString();
        }
    }

    public enum ContentDescriptionType
    {
        Description,Exception
    }
}
