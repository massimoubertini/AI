import java.net.URI;
import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.utils.URIBuilder;
import org.apache.http.impl.client.HttpClients;
import org.apache.http.util.EntityUtils;
public class JavaSample {
    public static void main(String[] args) 
    {   HttpClient httpclient = HttpClients.createDefault();
        try {
            URIBuilder builder = new URIBuilder("https://api.projectoxford.ai/vision/v1.0/analyze");
            builder.setParameter("visualFeatures", "Categories");
            builder.setParameter("details", "{string}");
            URI uri = builder.build();
            HttpPost request = new HttpPost(uri);
            request.setHeader("Content-Type", "application/json");
            request.setHeader("Ocp-Apim-Subscription-Key", "{subscription key}");
             // richiesta body
            StringEntity reqEntity = new StringEntity("{\"url\": \"https://projectoxfordportal.azureedge.net/vision/Analysis/1-1.jpg\"}");
            request.setEntity(reqEntity);
            HttpResponse response = httpclient.execute(request);
            HttpEntity entity = response.getEntity();
            if (entity != null) 
                System.out.println(EntityUtils.toString(entity));
        }
        catch (Exception e)
        {  System.out.println(e.getMessage());  }
    }
}