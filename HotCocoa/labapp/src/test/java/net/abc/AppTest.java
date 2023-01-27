package net.abc;

import org.notima.bg.*;
import com.google.gson.*;
import java.io.*;

import static org.junit.Assert.assertTrue;

import java.nio.charset.Charset;

import org.junit.Test;

public class AppTest
{
    @Test
    public void shouldAnswerWithTrue()
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
            throw new RuntimeException(e.getMessage(), e);
        }
    }
}
