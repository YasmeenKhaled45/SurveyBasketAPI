namespace SurveyBasket.Api.Helpers
{
    public static class EmailBodyBuilder
    {
        public static string GenerateEmailBody(string templete , Dictionary<string,string> templeteModel)
        {
            var templetepath = $"{Directory.GetCurrentDirectory()}/Templetes/{templete}.html";
            var streamreader = new StreamReader(templetepath);
            var body = streamreader.ReadToEnd();
            streamreader.Close();

            foreach( var item in templeteModel )
                body = body.Replace(item.Key, item.Value);
            
            return body;
        }
    }
}
