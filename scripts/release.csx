#r "nuget: Newtonsoft.Json, 13.0.1"

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

var rootPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(2));
var tokenPath = $"{rootPath}\\scripts\\github.secret";
var token = File.ReadAllText(tokenPath);
var auth = $"kickass-panda:{token}";
var base64 = Convert.ToBase64String(System.Text.ASCIIEncoding.UTF8.GetBytes(auth));

uint releaseId = 0;

var name = "";
var tag = $"v{name}";

// find version
{
    var metaPath = $"{rootPath}\\Aoe2DEOverlay\\AppInfo\\Metadata.cs";

    string metadataText = File.ReadAllText(metaPath);

    string metadataPattern = "Version Version.*=.*new.*Version\\(\".*\"\\)";


    var metaVersion = Regex.Match(metadataText, metadataPattern).Value;
    var metaRegex = new Regex("Version Version.*=.*new.*Version\\(\"|\"\\)");
    metaVersion = metaRegex.Replace(metaVersion, "");
    name = metaVersion;
}

if(name == "") return;

// create release
{
    var http = new HttpClient();

    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.github.com/repos/kickass-panda/Aoe2DEOverlay/releases");
    request.Headers.Add("Authorization", "Basic " + base64);
    var product = new ProductInfoHeaderValue("ReleaseScript", "1.0");
    request.Headers.UserAgent.Add(product);

    var json = $"{{ \"tag_name\": \"{tag}\", \"target_commitish\": \"main\", \"name\": \"{name}\", \"draft\": true }}";
    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

    var response = http.Send(request);
    var reader = new StreamReader(response.Content.ReadAsStream());
    var body = reader.ReadToEnd();
    JObject jbody = JObject.Parse(body);

    releaseId = jbody["id"].Value<uint>();
}

// upload zip asset
public static void upload(string path, string file, uint id, string base64)
{
    if(id == 0) return;

    var http = new HttpClient();
    http.Timeout = new TimeSpan(1, 0, 0); // 1h
    
    var data = File.ReadAllBytes($"{path}\\{file}");

    var url = $"https://uploads.github.com/repos/kickass-panda/Aoe2DEOverlay/releases/{id}/assets?name={file}";
    var request = new HttpRequestMessage(HttpMethod.Post, url);
    request.Headers.Add("Authorization", "Basic " + base64);
    var product = new ProductInfoHeaderValue("ReleaseScript", "1.0");
    request.Headers.UserAgent.Add(product);
    
    request.Content = new ByteArrayContent(data);
    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-streamp");
    request.Content.Headers.ContentLength = Convert.ToInt64($"{data.Length}");

    var response = http.Send(request);
    var reader = new StreamReader(response.Content.ReadAsStream());
    var body = reader.ReadToEnd();
}

{
    var path = "C:\\Users\\Jan\\RiderProjects\\Aoe2DEOverlay\\publish\\Aoe2DEOverlay";
    upload(path, "x64.zip", releaseId, base64);
    upload(path, "x86.zip", releaseId, base64);
}