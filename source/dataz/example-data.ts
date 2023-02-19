

export function getAllData(thing: unknown) {
  return {
    data0: {
      thing,
      aDate: new Date(),
      bignum: 123409324093249n,
      getAllData,
      func1() {
        return 1;
      },
      func2: () => 1,
      func3: function () {
        const u = 0xffff + 1;
        return u / 23;
      },
      emptyArray: [],
      [Symbol("TestSymbol")]: Symbol("TestSymbol"),
      TestSymbol: Symbol("TestSymbol"),
    },
    data1: getData1(),
    data2: getData2(),
    data3: getData3(),

    window,
  };
}

export function getData1() {
  return {
    clients: [
      {
        id: "59761c23b30d971669fb42ff",
        isActive: true,
        age: 36,
        name: "Dunlap Hubbard",
        gender: "male",
        company: "CEDWARD",
        email: "dunlaphubbard@cedward.com",
        phone: "+1 (890) 543-2508",
        address:
          "169 Rutledge Street, Konterra, Northern Mariana Islands, 8551",
      },
      {
        id: "59761c233d8d0f92a6b0570d",
        isActive: true,
        age: 24,
        name: "Kirsten Sellers",
        gender: "female",
        company: "EMERGENT",
        email: "kirstensellers@emergent.com",
        phone: "+1 (831) 564-2190",
        address: "886 Gallatin Place, Fannett, Arkansas, 4656",
      },
      {
        id: "59761c23fcb6254b1a06dad5",
        isActive: true,
        age: 30,
        name: "Acosta Robbins",
        gender: "male",
        company: "ORGANICA",
        email: "acostarobbins@organica.com",
        phone: "+1 (882) 441-3367",
        address: "697 Linden Boulevard, Sattley, Idaho, 1035",
      },
    ],
  };
}

export function getData2() {
  return {
    created_at: "Thu Jun 22 21:00:00 +0000 2017",
    id: 877994604561387500,
    id_str: "877994604561387520",
    text: "Creating a Grocery List Manager Using Angular, Part 1: Add &amp; Display Items https://t.co/xFox78juL1 #Angular",
    truncated: false,
    entities: {
      hashtags: [
        {
          text: "Angular",
          indices: [103, 111],
        },
      ],
      symbols: [],
      user_mentions: [],
      urls: [
        {
          url: "https://t.co/xFox78juL1",
          expanded_url: "http://buff.ly/2sr60pf",
          display_url: "buff.ly/2sr60pf",
          indices: [79, 102],
        },
      ],
    },
    source: '<a href="http://bufferapp.com" rel="nofollow">Buffer</a>',
    user: {
      id: 772682964,
      id_str: "772682964",
      name: "SitePoint JavaScript",
      screen_name: "SitePointJS",
      location: "Melbourne, Australia",
      description:
        "Keep up with JavaScript tutorials, tips, tricks and articles at SitePoint.",
      url: "http://t.co/cCH13gqeUK",
      entities: {
        url: {
          urls: [
            {
              url: "http://t.co/cCH13gqeUK",
              expanded_url: "https://www.sitepoint.com/javascript",
              display_url: "sitepoint.com/javascript",
              indices: [0, 22],
            },
          ],
        },
        description: {
          urls: [
            "https://www.youtube.com/watch?v=rK_YlsEIM_c",
            "file:///C:/CODE/AllKindsOfThings/source/dataz/index.html",
          ],
        },
      },
      protected: false,
      followers_count: 2145,
      friends_count: 18,
      listed_count: 328,
      created_at: "Wed Aug 22 02:06:33 +0000 2012",
      favourites_count: 57,
      utc_offset: 43200,
      time_zone: "Wellington",
    },
  };
}

export function getData3() {
  return {
    "web-app": {
      servlet: [
        {
          "servlet-name": "cofaxCDS",
          "servlet-class": "org.cofax.cds.CDSServlet",
          "init-param": {
            "configGlossary:installationAt": "Philadelphia, PA",
            "configGlossary:adminEmail": "ksm@pobox.com",
            "configGlossary:poweredBy": "Cofax",
            "configGlossary:poweredByIcon": "/images/cofax.gif",
            "configGlossary:staticPath": "/content/static",
            templateProcessorClass: "org.cofax.WysiwygTemplate",
            templateLoaderClass: "org.cofax.FilesTemplateLoader",
            templatePath: "templates",
            templateOverridePath: "",
            defaultListTemplate: "listTemplate.htm",
            defaultFileTemplate: "articleTemplate.htm",
            useJSP: false,
            jspListTemplate: "listTemplate.jsp",
            jspFileTemplate: "articleTemplate.jsp",
            cachePackageTagsTrack: 200,
            cachePackageTagsStore: 200,
            cachePackageTagsRefresh: 60,
            cacheTemplatesTrack: 100,
            cacheTemplatesStore: 50,
            cacheTemplatesRefresh: 15,
            cachePagesTrack: 200,
            cachePagesStore: 100,
            cachePagesRefresh: 10,
            cachePagesDirtyRead: 10,
            searchEngineListTemplate: "forSearchEnginesList.htm",
            searchEngineFileTemplate: "forSearchEngines.htm",
            searchEngineRobotsDb: "WEB-INF/robots.db",
            useDataStore: true,
            dataStoreClass: "org.cofax.SqlDataStore",
            redirectionClass: "org.cofax.SqlRedirection",
            dataStoreName: "cofax",
            dataStoreDriver: "com.microsoft.jdbc.sqlserver.SQLServerDriver",
            dataStoreUrl:
              "jdbc:microsoft:sqlserver://LOCALHOST:1433;DatabaseName=goon",
            dataStoreUser: "sa",
            dataStorePassword: "dataStoreTestQuery",
            dataStoreTestQuery: "SET NOCOUNT ON;select test='test';",
            dataStoreLogFile: "/usr/local/tomcat/logs/datastore.log",
            dataStoreInitConns: 10,
            dataStoreMaxConns: 100,
            dataStoreConnUsageLimit: 100,
            dataStoreLogLevel: "debug",
            maxUrlLength: 500,
          },
        },
        {
          "servlet-name": "cofaxEmail",
          "servlet-class": "org.cofax.cds.EmailServlet",
          "init-param": {
            mailHost: "mail1",
            mailHostOverride: "mail2",
          },
        },
        {
          "servlet-name": "cofaxAdmin",
          "servlet-class": "org.cofax.cds.AdminServlet",
        },

        {
          "servlet-name": "fileServlet",
          "servlet-class": "org.cofax.cds.FileServlet",
        },
        {
          "servlet-name": "cofaxTools",
          "servlet-class": "org.cofax.cms.CofaxToolsServlet",
          "init-param": {
            templatePath: "toolstemplates/",
            log: 1,
            logLocation: "/usr/local/tomcat/logs/CofaxTools.log",
            logMaxSize: "",
            dataLog: 1,
            dataLogLocation: "/usr/local/tomcat/logs/dataLog.log",
            dataLogMaxSize: "",
            removePageCache: "/content/admin/remove?cache=pages&id=",
            removeTemplateCache: "/content/admin/remove?cache=templates&id=",
            fileTransferFolder:
              "/usr/local/tomcat/webapps/content/fileTransferFolder",
            lookInContext: 1,
            adminGroupID: 4,
            betaServer: true,
          },
        },
      ],
      "servlet-mapping": {
        cofaxCDS: "/",
        cofaxEmail: "/cofaxutil/aemail/*",
        cofaxAdmin: "/admin/*",
        fileServlet: "/static/*",
        cofaxTools: "/tools/*",
      },

      taglib: {
        "taglib-uri": "cofax.tld",
        "taglib-location": "/WEB-INF/tlds/cofax.tld",
      },
    },
  };
}