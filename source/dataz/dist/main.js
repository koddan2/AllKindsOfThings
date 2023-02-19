(() => {
  // tablify.ts
  var defaultTablifyStrings = {
    Property: "Property",
    Index: "Index",
    Value: "Value"
  };
  function tablify(element, data, configuration) {
    cleanElement(element);
    const context = makeContext(element.ownerDocument, configuration);
    renderThing(context, element, data);
  }
  function cleanElement(element) {
    const children = element.childNodes;
    for (let child of children) {
      element.removeChild(child);
    }
  }
  function maybeTransform(ctx, t, value) {
    if (ctx.configuration.transforms[t]) {
      return ctx.configuration.transforms[t].call(null, value);
    }
    return value.toString();
  }
  function renderThing(ctx, element, data) {
    ctx.level++;
    if (typeof data === "object") {
      if (data === null) {
        renderKeyword(ctx, element, ctx.getClass("keyword"), "null");
      } else if (Array.isArray(data)) {
        renderArray(ctx, element, data);
      } else if (data instanceof Date) {
        ctx.ele("span", element, ctx.getClass("date"), data.toString());
      } else {
        if (ctx.referenceLoopCheck.has(data)) {
          ctx.ele("span", element, "reference-loop", "\u{1F4A5}");
        } else {
          ctx.referenceLoopCheck.set(data, true);
          renderAssoc(ctx, element, data);
        }
      }
    } else if (typeof data === "bigint") {
      ctx.ele("span", element, ctx.getClass("number"), data.toString());
    } else if (typeof data === "number") {
      ctx.ele("span", element, ctx.getClass("number"), data.toString());
    } else if (typeof data === "boolean") {
      ctx.ele(
        "span",
        element,
        ctx.getClass("keyword boolean"),
        maybeTransform(ctx, "boolean", data)
      );
    } else if (typeof data === "string") {
      ctx.ele("span", element, ctx.getClass("string"), data);
    } else if (typeof data === "symbol") {
      ctx.ele("span", element, ctx.getClass("symbol"), data.toString());
    } else if (data === void 0) {
      renderKeyword(ctx, element, ctx.getClass("keyword"), "undefined");
    } else {
      ctx.errors.push(new Error(`Cannot render a ${typeof data}`));
      const fallback = data?.toString();
      renderKeyword(ctx, element, ctx.getClass("unknown"), fallback);
    }
  }
  function renderKeyword(ctx, element, cls, contents) {
    const containingEl = ctx.ele("code", element, cls);
    ctx.ele("pre", containingEl, null, contents);
  }
  function renderArray(ctx, element, data) {
    if (data.length < 1) {
      renderKeyword(ctx, element, null, "[]");
      return;
    }
    const assocData = {};
    for (let i = 0; i < data.length; i++) {
      const element2 = data[i];
      assocData[i.toString()] = element2;
    }
    renderAssoc(ctx, element, assocData, ["Index", "Value"]);
  }
  function renderAssoc(ctx, element, data, headingNames) {
    const keys = Object.keys(data);
    const symbols = Object.getOwnPropertySymbols(data);
    if (keys.length < 1 && symbols.length < 1) {
      renderKeyword(ctx, element, null, "{}");
      return;
    }
    if (headingNames == null) {
      headingNames = ["Property", "Value"];
    }
    const table = ctx.ele("table", element);
    table.className = ctx.getClass();
    const thead = ctx.ele("thead", table);
    const theadtr = ctx.ele("tr", thead);
    ctx.ele("th", theadtr, null, ctx.str(headingNames[0]));
    ctx.ele("th", theadtr, null, ctx.str(headingNames[1]));
    const tbody = ctx.ele("tbody", table);
    for (let k of keys) {
      const tr = ctx.ele("tr", tbody);
      ctx.ele("td", tr, null, ctx.strProp(k));
      const valueCell = ctx.ele("td", tr);
      renderThing(ctx, valueCell, data[k]);
    }
    for (let sym of symbols) {
      const tr = ctx.ele("tr", tbody);
      ctx.ele("td", tr, null, ctx.strProp(sym.toString()));
      const valueCell = ctx.ele("td", tr);
      renderThing(ctx, valueCell, data[sym]);
    }
  }
  function makeContext(doc, configuration) {
    return {
      referenceLoopCheck: /* @__PURE__ */ new WeakMap(),
      doc,
      configuration,
      level: -1,
      getClass(name) {
        if (name) {
          return `${name} level-${this.level}`;
        }
        return `level-${this.level}`;
      },
      ele(name, parent, cls, text) {
        const result = doc.createElement(name);
        if (parent) {
          parent.appendChild(result);
        }
        if (cls) {
          result.className = cls;
        }
        if (text) {
          result.innerText = text;
        }
        return result;
      },
      str(key, category) {
        return tryGet(key, configuration.strings.tablify) || defaultTablifyStrings[key];
      },
      strProp(key, category) {
        return tryGet(key, configuration.strings.properties) || key;
      },
      strValue(value, category) {
        return tryGet(value, configuration.strings.values) || value;
      },
      errors: []
    };
  }
  function tryGet(key, obj) {
    if (obj) {
      return obj[key];
    }
  }

  // main.ts
  document.addEventListener("DOMContentLoaded", function init() {
    tablify(document.getElementById("root"), getAllData(), {
      transforms: {
        boolean(v) {
          return v ? "Yes" : "No";
        }
      },
      strings: {
        properties: {
          id: "Identity",
          url: "Address"
        }
      }
    });
  });
  function getAllData() {
    return {
      data0: {
        getAllData,
        func1() {
          return 1;
        },
        func2: () => 1,
        func3: function() {
          const u = 65535 + 1;
          return u / 23;
        },
        emptyArray: [],
        [Symbol("TestSymbol")]: Symbol("TestSymbol"),
        TestSymbol: Symbol("TestSymbol")
      },
      data1: getData1(),
      data2: getData2(),
      data3: getData3(),
      window
    };
  }
  function getData1() {
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
          address: "169 Rutledge Street, Konterra, Northern Mariana Islands, 8551"
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
          address: "886 Gallatin Place, Fannett, Arkansas, 4656"
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
          address: "697 Linden Boulevard, Sattley, Idaho, 1035"
        }
      ]
    };
  }
  function getData2() {
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
            indices: [103, 111]
          }
        ],
        symbols: [],
        user_mentions: [],
        urls: [
          {
            url: "https://t.co/xFox78juL1",
            expanded_url: "http://buff.ly/2sr60pf",
            display_url: "buff.ly/2sr60pf",
            indices: [79, 102]
          }
        ]
      },
      source: '<a href="http://bufferapp.com" rel="nofollow">Buffer</a>',
      user: {
        id: 772682964,
        id_str: "772682964",
        name: "SitePoint JavaScript",
        screen_name: "SitePointJS",
        location: "Melbourne, Australia",
        description: "Keep up with JavaScript tutorials, tips, tricks and articles at SitePoint.",
        url: "http://t.co/cCH13gqeUK",
        entities: {
          url: {
            urls: [
              {
                url: "http://t.co/cCH13gqeUK",
                expanded_url: "https://www.sitepoint.com/javascript",
                display_url: "sitepoint.com/javascript",
                indices: [0, 22]
              }
            ]
          },
          description: {
            urls: [
              "https://www.youtube.com/watch?v=rK_YlsEIM_c",
              "file:///C:/CODE/AllKindsOfThings/source/dataz/index.html"
            ]
          }
        },
        protected: false,
        followers_count: 2145,
        friends_count: 18,
        listed_count: 328,
        created_at: "Wed Aug 22 02:06:33 +0000 2012",
        favourites_count: 57,
        utc_offset: 43200,
        time_zone: "Wellington"
      }
    };
  }
  function getData3() {
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
              dataStoreUrl: "jdbc:microsoft:sqlserver://LOCALHOST:1433;DatabaseName=goon",
              dataStoreUser: "sa",
              dataStorePassword: "dataStoreTestQuery",
              dataStoreTestQuery: "SET NOCOUNT ON;select test='test';",
              dataStoreLogFile: "/usr/local/tomcat/logs/datastore.log",
              dataStoreInitConns: 10,
              dataStoreMaxConns: 100,
              dataStoreConnUsageLimit: 100,
              dataStoreLogLevel: "debug",
              maxUrlLength: 500
            }
          },
          {
            "servlet-name": "cofaxEmail",
            "servlet-class": "org.cofax.cds.EmailServlet",
            "init-param": {
              mailHost: "mail1",
              mailHostOverride: "mail2"
            }
          },
          {
            "servlet-name": "cofaxAdmin",
            "servlet-class": "org.cofax.cds.AdminServlet"
          },
          {
            "servlet-name": "fileServlet",
            "servlet-class": "org.cofax.cds.FileServlet"
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
              fileTransferFolder: "/usr/local/tomcat/webapps/content/fileTransferFolder",
              lookInContext: 1,
              adminGroupID: 4,
              betaServer: true
            }
          }
        ],
        "servlet-mapping": {
          cofaxCDS: "/",
          cofaxEmail: "/cofaxutil/aemail/*",
          cofaxAdmin: "/admin/*",
          fileServlet: "/static/*",
          cofaxTools: "/tools/*"
        },
        taglib: {
          "taglib-uri": "cofax.tld",
          "taglib-location": "/WEB-INF/tlds/cofax.tld"
        }
      }
    };
  }
})();
