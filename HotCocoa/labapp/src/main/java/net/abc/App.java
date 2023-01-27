package net.abc;
import java.io.File;
import java.nio.charset.Charset;

import org.notima.bg.BgMaxFile;

import com.google.gson.*;

public class App
{
    public static void main( String[] args )
    {
        Gson gson = new GsonBuilder()
                .setPrettyPrinting()
                .create();

            try {
            	var chs = Charset.forName("ISO-8859-1");
            	
                var bgMaxFile = new BgMaxFile();

                System.out.println(System.getProperty("user.dir"));
            	var inputfile = new File("./files/test1.txt");
                bgMaxFile.readFromFile(inputfile, chs);
                
                System.out.println(gson.toJson(bgMaxFile));
            } catch (Exception e) {
            	
                System.out.println(e.getMessage());
            }

    }
}
