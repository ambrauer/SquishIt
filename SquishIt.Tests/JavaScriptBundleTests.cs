using NUnit.Framework;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.JavaScript.Minifiers;
using SquishIt.Framework.Tests.Mocks;
using SquishIt.Tests.Stubs;

namespace SquishIt.Tests
{
    [TestFixture]
    public class JavaScriptBundleTests
    {
        private string javaScript = @"
																				function product(a, b)
																				{
																						return a * b;
																				}

																				function sum(a, b){
																						return a + b;
																				}";

        private string javaScript2 = @"function sum(a, b){
																						return a + b;
																			 }";

        private IJavaScriptBundle javaScriptBundle;
        private IJavaScriptBundle javaScriptBundle2;
        private IJavaScriptBundle debugJavaScriptBundle;
        private IJavaScriptBundle debugJavaScriptBundle2;
        private StubFileWriterFactory fileWriterFactory;
        private StubFileReaderFactory fileReaderFactory;
        private StubCurrentDirectoryWrapper currentDirectoryWrapper;

        [SetUp]
        public void Setup()
        {
            var nonDebugStatusReader = new StubDebugStatusReader(false);
            var debugStatusReader = new StubDebugStatusReader(true);
            fileWriterFactory = new StubFileWriterFactory();
            fileReaderFactory = new StubFileReaderFactory();
            currentDirectoryWrapper = new StubCurrentDirectoryWrapper();

            fileReaderFactory.SetContents(javaScript);

            javaScriptBundle = new JavaScriptBundle(nonDebugStatusReader,
                                                                                            fileWriterFactory,
                                                                                            fileReaderFactory,
                                                                                            currentDirectoryWrapper);

            javaScriptBundle2 = new JavaScriptBundle(nonDebugStatusReader,
                                                                                            fileWriterFactory,
                                                                                            fileReaderFactory,
                                                                                            currentDirectoryWrapper);

            debugJavaScriptBundle = new JavaScriptBundle(debugStatusReader,
                                                                                                    fileWriterFactory,
                                                                                                    fileReaderFactory,
                                                                                                    currentDirectoryWrapper);

            debugJavaScriptBundle2 = new JavaScriptBundle(debugStatusReader,
                                                                                                    fileWriterFactory,
                                                                                                    fileReaderFactory,
                                                                                                    currentDirectoryWrapper);
        }

        [Test]
        public void CanBundleJavaScript()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .RenderFile("~/js/output_1.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_1.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\output_1.js"]);
        }

        [Test]
        public void CanBundleJavaScriptWithQuerystringParameter()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .RenderFile("~/js/output_querystring.js?v=2");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_querystring.js?v=2&r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
        }

        [Test]
        public void CanCreateNamedBundle()
        {
            javaScriptBundle
                    .Add("~/js/test.js")
                    .AsNamedFile("TestNamed", "~/js/output_namedbundle.js");

            var tag = javaScriptBundle.RenderNamed("TestNamed");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_namedbundle.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\output_namedbundle.js"]);
        }

        [Test]
        public void CanBundleJavaScriptWithRemote()
        {
            var tag = javaScriptBundle
                    .AddRemote("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                    .Add("~/js/test.js")
                    .RenderFile("~/js/output_1_2.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script><script type=\"text/javascript\" src=\"js/output_1_2.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\output_1_2.js"]);
        }

        [Test]
        public void CanBundleJavaScriptWithRemoteAndQuerystringParameter()
        {
            var tag = javaScriptBundle
                    .AddRemote("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                    .Add("~/js/test.js")
                    .RenderFile("~/js/output_querystring.js?v=2_2");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script><script type=\"text/javascript\" src=\"js/output_querystring.js?v=2_2&r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
        }

        [Test]
        public void CanCreateNamedBundleWithRemote()
        {
            javaScriptBundle
                    .AddRemote("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                    .Add("~/js/test.js")
                    .AsNamedFile("TestCdn", "~/js/output_3_2.js");

            var tag = javaScriptBundle.RenderNamed("TestCdn");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script><script type=\"text/javascript\" src=\"js/output_3_2.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\output_3_2.js"]);
        }

        [Test]
        public void CanBundleJavaScriptWithEmbeddedResource()
        {
            var tag = javaScriptBundle
                    .AddEmbeddedResource("~/js/test.js", "SquishIt.Tests://EmbeddedResource.Embedded.js")
                    .RenderFile("~/js/output_Embedded.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_Embedded.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\output_Embedded.js"]);
        }

        [Test]
        public void CanDebugBundleJavaScriptWithEmbeddedResource()
        {
            var tag = debugJavaScriptBundle
                    .AddEmbeddedResource("~/js/test.js", "SquishIt.Tests://EmbeddedResource.Embedded.js")
                    .RenderFile("~/js/output_Embedded.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test.js\"></script>", tag);
        }

        //-------------------------------------------------------------------------
        [Test]
        public void CanRenderDebugTags()
        {
            debugJavaScriptBundle
                    .Add("~/js/test1.js")
                    .Add("~/js/test2.js")
                    .AsNamedFile("TestWithDebug", "~/js/output_3.js");

            var tag = debugJavaScriptBundle.RenderNamed("TestWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script><script type=\"text/javascript\" src=\"js/test2.js\"></script>", tag);
        }

        [Test]
        public void CanRenderDebugTagsTwice()
        {
            debugJavaScriptBundle
                    .Add("~/js/test1.js")
                    .Add("~/js/test2.js")
                    .AsNamedFile("TestWithDebug", "~/js/output_4.js");

            debugJavaScriptBundle2
                    .Add("~/js/test1.js")
                    .Add("~/js/test2.js")
                    .AsNamedFile("TestWithDebug", "~/js/output_4.js");

            var tag1 = debugJavaScriptBundle.RenderNamed("TestWithDebug");
            var tag2 = debugJavaScriptBundle2.RenderNamed("TestWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script><script type=\"text/javascript\" src=\"js/test2.js\"></script>", tag1);
            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script><script type=\"text/javascript\" src=\"js/test2.js\"></script>", tag2);
        }

        [Test]
        public void CanCreateNamedBundleWithDebug()
        {
            debugJavaScriptBundle
                    .Add("~/js/test1.js")
                    .Add("~/js/test2.js")
                    .AsNamedFile("NamedWithDebug", "~/js/output_5.js");

            var tag = debugJavaScriptBundle.RenderNamed("NamedWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script><script type=\"text/javascript\" src=\"js/test2.js\"></script>", tag);
        }

        [Test]
        public void CanCreateBundleWithNullMinifer()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .WithMinifier(JavaScriptMinifiers.NullMinifier)
                    .RenderFile("~/js/output_6.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_6.js?r=361CBAF229793498C1325D939DE3B25F\"></script>", tag);
            Assert.AreEqual(javaScript, fileWriterFactory.Files[@"C:\js\output_6.js"]);
        }

        [Test]
        public void CanCreateBundleWithJsMinMinifer()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .WithMinifier(JavaScriptMinifiers.JsMin)
                    .RenderFile("~/js/output_7.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_7.js?r=8AA0EB763B23F6041902F56782ADB346\"></script>", tag);
            Assert.AreEqual("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;}", fileWriterFactory.Files[@"C:\js\output_7.js"]);
        }

        [Test]
        public void CanCreateBundleWithJsMinMiniferByPassingInstance()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .WithMinifier(new JsMinMinifier())
                    .RenderFile("~/js/output_jsmininstance.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_jsmininstance.js?r=8AA0EB763B23F6041902F56782ADB346\"></script>", tag);
            Assert.AreEqual("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;}", fileWriterFactory.Files[@"C:\js\output_jsmininstance.js"]);
        }

        [Test]
        public void CanCreateEmbeddedBundleWithJsMinMinifer()
        {
            var tag = javaScriptBundle
                    .AddEmbeddedResource("~/js/test.js", "SquishIt.Tests://EmbeddedResource.Embedded.js")
                    .WithMinifier(JavaScriptMinifiers.JsMin)
                    .RenderFile("~/js/output_embedded7.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_embedded7.js?r=8AA0EB763B23F6041902F56782ADB346\"></script>", tag);
            Assert.AreEqual("\nfunction product(a,b)\n{return a*b;}\nfunction sum(a,b){return a+b;}", fileWriterFactory.Files[@"C:\js\output_embedded7.js"]);
        }

        /*[Test]
        public void CanCreateBundleWithClosureMinifer()
        {
                var tag = javaScriptBundle
                        .Add("~/js/test.js")
                        .WithMinifier(JavaScriptMinifiers.Closure)
                        .RenderFile("~/js/output_8.js");

                Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_8.js?r=00DFDFFC4078EFF6DFCC6244EAB77420\"></script>", tag);
                Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b};\r\n", fileWriterFactory.Files[@"C:\js\output_8.js"]);
        }*/

        [Test]
        public void CanRenderOnlyIfFileMissing()
        {
            fileReaderFactory.SetFileExists(false);

            javaScriptBundle
                    .Add("~/js/test.js")
                    .RenderOnlyIfOutputFileMissing()
                    .RenderFile("~/js/output_9.js");

            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\output_9.js"]);

            fileReaderFactory.SetContents(javaScript2);
            fileReaderFactory.SetFileExists(true);
            javaScriptBundle.ClearTestingCache();

            javaScriptBundle
                    .Add("~/js/test.js")
                    .RenderOnlyIfOutputFileMissing()
                    .RenderFile("~/js/output_9.js");

            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\output_9.js"]);
        }

        [Test]
        public void CanRerenderFiles()
        {
            fileReaderFactory.SetFileExists(false);

            javaScriptBundle
                    .Add("~/js/test.js")
                    .RenderFile("~/js/output_10.js");

            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\output_10.js"]);

            fileReaderFactory.SetContents(javaScript2);
            fileReaderFactory.SetFileExists(true);
            fileWriterFactory.Files.Clear();
            javaScriptBundle.ClearTestingCache();

            javaScriptBundle2
                    .Add("~/js/test.js")
                    .RenderFile("~/js/output_10.js");

            Assert.AreEqual("function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\output_10.js"]);
        }

        [Test]
        public void CanBundleJavaScriptWithHashInFilename()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .RenderFile("~/js/output_#.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_E36D384488ABCF73BCCE650C627FB74F.js\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\output_E36D384488ABCF73BCCE650C627FB74F.js"]);
        }

        [Test]
        public void CanBundleJavaScriptWithUnderscoresInName()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test_file.js")
                    .RenderFile("~/js/outputunder_#.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/outputunder_E36D384488ABCF73BCCE650C627FB74F.js\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\outputunder_E36D384488ABCF73BCCE650C627FB74F.js"]);
        }

        [Test]
        public void CanCreateNamedBundleWithForcedRelease()
        {
            debugJavaScriptBundle
                    .Add("~/js/test.js")
                    .ForceRelease()
                    .AsNamedFile("ForceRelease", "~/js/output_forcerelease.js");

            var tag = javaScriptBundle.RenderNamed("ForceRelease");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/output_forcerelease.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", fileWriterFactory.Files[@"C:\js\output_forcerelease.js"]);
        }

        [Test]
        public void CanBundleJavaScriptWithSingleAttribute()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .WithAttribute("charset", "utf-8")
                    .RenderFile("~/js/output_att.js");

            Assert.AreEqual("<script type=\"text/javascript\" charset=\"utf-8\" src=\"js/output_att.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
        }

        [Test]
        public void CanBundleJavaScriptWithSingleMultipleAttributes()
        {
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .WithAttribute("charset", "utf-8")
                    .WithAttribute("other", "value")
                    .RenderFile("~/js/output_att2.js");

            Assert.AreEqual("<script type=\"text/javascript\" charset=\"utf-8\" other=\"value\" src=\"js/output_att2.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
        }

        [Test]
        public void CanDebugBundleWithAttribute()
        {
            string tag = debugJavaScriptBundle
                    .Add("~/js/test1.js")
                    .Add("~/js/test2.js")
                    .WithAttribute("charset", "utf-8")
                    .RenderFile("~/js/output_debugattr.js");
            Assert.AreEqual("<script type=\"text/javascript\" charset=\"utf-8\" src=\"js/test1.js\"></script><script type=\"text/javascript\" charset=\"utf-8\" src=\"js/test2.js\"></script>", tag);
        }

        [Test]
        public void CanRenderCachedBundle()
        {
            javaScriptBundle.ClearTestingCache();
            var tag = javaScriptBundle
                    .Add("~/js/test.js")
                    .RenderCache("output_2.js");

            var content = javaScriptBundle.GetCachedContent("output_2.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"bundle/script/output_2.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", content);
        }

        [Test]
        public void CanRenderCachedBundleWithDebug()
        {
            var tag = debugJavaScriptBundle
                    .Add("~/js/test.js")
                    .RenderCache("output_2.js");
            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test.js\"></script>", tag);
        }

        [Test]
        public void CanOverrideCacheRoute()
        {
            IJavaScriptBundle bundle = new JavaScriptBundle(new StubDebugStatusReader(false),
                                                                        fileWriterFactory,
                                                                        fileReaderFactory,
                                                                        currentDirectoryWrapper);
            bundle.SetCacheRoute("foo/bar/");
            var tag = bundle
                    .Add("~/js/test.js")
                    .RenderCache("output_2.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"foo/bar/output_2.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
        }

        [Test]
        public void CanCreateNamedCacheBundle()
        {
            javaScriptBundle.ClearTestingCache();
            javaScriptBundle
                    .Add("~/js/test.js")
                    .AsNamedCache("TestNamed", "output_namedbundle.js");

            var tag = javaScriptBundle.RenderNamed("TestNamed");
            var content = javaScriptBundle.GetCachedContent("output_namedbundle.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"bundle/script/output_namedbundle.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", content);
        }

        [Test]
        public void CanCreateNamedCacheBundleWithHash()
        {
            javaScriptBundle.ClearTestingCache();
            javaScriptBundle
                    .Add("~/js/test.js")
                    .AsNamedCache("TestNamed", "output_namedbundle_#.js");

            var tag = javaScriptBundle.RenderNamed("TestNamed");
            var content = javaScriptBundle.GetCachedContent("output_namedbundle_E36D384488ABCF73BCCE650C627FB74F.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"bundle/script/output_namedbundle_E36D384488ABCF73BCCE650C627FB74F.js\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", content);
        }

        [Test]
        public void CanCreateNamedCacheBundleWithRemote()
        {
            javaScriptBundle
                    .AddRemote("~/js/test.js", "http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js")
                    .Add("~/js/test.js")
                    .AsNamedCache("TestCdn", "output_3_2.js");

            var tag = javaScriptBundle.RenderNamed("TestCdn");
            var content = javaScriptBundle.GetCachedContent("output_3_2.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script><script type=\"text/javascript\" src=\"bundle/script/output_3_2.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", content);
        }

        [Test]
        public void CanCreateNamedCacheBundleWithDebug()
        {
            debugJavaScriptBundle
                    .Add("~/js/test1.js")
                    .Add("~/js/test2.js")
                    .AsNamedCache("NamedWithDebug", "output_5.js");

            var tag = debugJavaScriptBundle.RenderNamed("NamedWithDebug");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"js/test1.js\"></script><script type=\"text/javascript\" src=\"js/test2.js\"></script>", tag);
        }

        [Test]
        public void CanCreateNamedCacheBundleWithForcedRelease()
        {
            debugJavaScriptBundle
                    .Add("~/js/test.js")
                    .ForceRelease()
                    .AsNamedCache("ForceRelease", "output_forcerelease.js");

            var tag = debugJavaScriptBundle.RenderNamed("ForceRelease");
            var content = debugJavaScriptBundle.GetCachedContent("output_forcerelease.js");

            Assert.AreEqual("<script type=\"text/javascript\" src=\"bundle/script/output_forcerelease.js?r=E36D384488ABCF73BCCE650C627FB74F\"></script>", tag);
            Assert.AreEqual("function product(a,b){return a*b}function sum(a,b){return a+b}", content);
        }
    }
}