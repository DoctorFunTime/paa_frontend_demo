using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static paa_frontend.Client.Services.Demo.DemoStore;

namespace paa_frontend.Client.Services.Demo;

/// <summary>Terminal handler: deliberately has no network transport or fallback.</summary>
public sealed class DemoHttpHandler(DemoStore store) : HttpMessageHandler
{
    private static HttpResponseMessage Reply(object? value, HttpStatusCode status = HttpStatusCode.OK) => new(status)
    { Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json") };
    private static HttpResponseMessage Unsupported() => Reply(new { error = "This operation is unavailable in the frontend demo. No school service is connected." }, HttpStatusCode.UnprocessableEntity);
    private static JsonNode? Copy(JsonNode? value) => value?.DeepClone();
    private static string Id(JsonNode? row) => Text(row, "id");
    private static JsonObject? Find(IEnumerable<JsonObject> rows, string id) => rows.FirstOrDefault(x => Id(x) == id);
    private JsonObject Save(string table, JsonNode? body, string? id = null, bool guid = false)
    {
        var rows = store[table];
        id ??= Text(body, "id");
        var existing = Find(rows, id);
        var row = existing ?? new JsonObject();
        if (body is JsonObject obj)
            foreach (var pair in obj) row[pair.Key] = Copy(pair.Value);
        if (string.IsNullOrEmpty(id) || id == "0" || id == Guid.Empty.ToString())
            row["id"] = guid ? JsonValue.Create(Guid.NewGuid().ToString()) : JsonValue.Create(rows.Select(x => Number(x,"id")).DefaultIfEmpty().Max()+1);
        else row["id"] = long.TryParse(id, out var n) ? JsonValue.Create(n) : JsonValue.Create(id);
        if (existing is null) rows.Add(row);
        return row;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var uri = request.RequestUri!;
        var path = Uri.UnescapeDataString(uri.AbsolutePath);
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var last = parts.LastOrDefault() ?? "";
        var query = uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split('=',2)).ToDictionary(x => Uri.UnescapeDataString(x[0]), x => x.Length>1 ? Uri.UnescapeDataString(x[1]) : "");
        string Q(string key, string fallback = "") => query.GetValueOrDefault(key, fallback);
        var get = request.Method == HttpMethod.Get;
        var delete = request.Method == HttpMethod.Delete;
        JsonNode? body = request.Content is null ? null : JsonNode.Parse(await request.Content.ReadAsStringAsync(cancellationToken));

        if (path.EndsWith("/auth/login"))
        {
            var user = Text(body,"username").Trim().ToLowerInvariant();
            if (!new[] { "admin", "student", "teacher", "burser", "super" }.Contains(user) || Text(body,"password") != "demo")
                return Reply(new { error = "Use a demo role and the password demo." }, HttpStatusCode.Unauthorized);
            var payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { sub = user, roles = new[] { user.ToUpperInvariant() }, userId = 1, exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds() })));
            return Reply(new { accessToken = "eyJhbGciOiJub25lIn0." + payload + ".demo", username = user, userId = 1, expiresIn = 86400 });
        }
        if (path.Contains("/auth/")) return Unsupported();

        if (parts.Length >= 4 && new[] { "student", "teacher", "admin", "burser" }.Contains(parts[2]))
        {
            var kind = parts[2]; var rows = store[kind];
            if (get && last == "all")
            {
                int.TryParse(Q("page","0"), out var page); int.TryParse(Q("size","2000"), out var size); size=Math.Max(size,1);
                return Reply(new { content = rows.Skip(page*size).Take(size), last = (page+1)*size>=rows.Count, totalElements = rows.Count, totalPages = (int)Math.Ceiling(rows.Count/(double)size), size, number = page });
            }
            if (get) return Reply(Find(rows,last), Find(rows,last) is null ? HttpStatusCode.NotFound : HttpStatusCode.OK);
            if (delete) { rows.RemoveAll(x => Id(x)==last); return Reply(true); }
            if (last == "bulk-create") return Unsupported();
            var row = Save(kind, body, parts.Contains("update") ? last : null);
            if (last == "create") return Reply(new { id = Number(row,"id"), userId = Number(row,"id"), username = "demo-created", defaultPassword = "demo", message = "Demo record created locally; no real login account was created." });
            return Reply(row);
        }
        if (path.Contains("/enrollment/"))
        {
            if (path.Contains("/class/amounts"))
                return Reply(store["student"].Where(s => path.Contains("/alevel/") ? Number(s,"form")>=5 : Number(s,"form")<=4).GroupBy(s => Number(s,"form")).Select(g => new { id = g.Key, form = g.Key, className = "A", amount = g.Count() }));
            if (path.Contains("/class/names/")) return Collection(path.Contains("/alevel/") ? "alevel-classes" : "olevel-classes", get, delete, last, body);
            if (path.Contains("/fields/")) return Collection("fields",get,delete,last,body);
            if (path.Contains("/field_choices/subjects")) return Reply(new { field = "Sciences", subjects = Subjects });
            if (path.Contains("/field_choices/")) return get ? Reply(new { id = 1, studentId = int.TryParse(last,out var sid)?sid:1, field = "Sciences" }) : Unsupported();
            if (path.Contains("/class-subjects"))
            {
                if (!get) return Reply(Save("class-subjects",body));
                if (last=="all") return Reply(store["class-subjects"]);
                return Reply(store["class-subjects"].FirstOrDefault(x=>Text(x,"form")==Q("form") && Text(x,"className")==Q("className")));
            }
            if (path.Contains("/student-subjects/") || path.Contains("/subject_choices/"))
            {
                var rows = store["student"].Where(s=> path.Contains("/alevel/") ? Number(s,"form")>=5 : Number(s,"form")<=4)
                    .Select(s=>Row(new { id=Number(s,"id"), studentId=Number(s,"id"), form=Number(s,"form"), className=Text(s,"classroom"), classCode=$"{Number(s,"form")}{Text(s,"classroom")}", subjects=Subjects })).ToList();
                if (!get) return Unsupported();
                return last=="all" ? Reply(rows) : Reply(Find(rows,last));
            }
            if (path.Contains("/students/")) return last=="all" ? Reply(store["student"].Where(s=>Number(s,"form")>=5)) : Reply(Find(store["student"],last));
            if (path.Contains("/subjects/")) return Collection("subjects",get,delete,last,body);
        }
        if (path.Contains("/student-grades/"))
        {
            if (path.Contains("/grading-scale/"))
            {
                if (get) return Reply(last=="all" ? store["grading"] : store["grading"].Where(x=>Text(x,"levelType")==last));
                if (body is JsonArray scales) { store["grading"].RemoveAll(x=>Text(x,"levelType")==last); foreach(var scale in scales) { var row=Save("grading",scale);row["levelType"]=last; } return Reply(store["grading"].Where(x=>Text(x,"levelType")==last)); }
            }
            if (path.Contains("/report-card/"))
            {
                var rows=store["reports"];
                if (get) return Reply(path.Contains("/pending/") ? rows.Where(x=>Text(x,"adminApproved")=="false") : rows.Where(x=>Text(x,"studentId")==last));
                if (path.Contains("/approve/")) { var r=Find(rows,parts[^3]); if(r is null)return Reply(null,HttpStatusCode.NotFound);r["adminApproved"]=true;r["releasedToStudent"]=true;return Reply(true); }
                if (delete) { if(path.Contains("/student/delete/")) rows.RemoveAll(x=>Text(x,"studentId")==parts[6]); else rows.RemoveAll(x=>Id(x)==last);return Reply(true); }
                if (path.Contains("/generate/"))
                {
                    var sid=Text(body,"studentId");var sample=rows.FirstOrDefault(x=>Text(x,"studentId")==sid) ?? rows.FirstOrDefault();
                    var report=sample?.DeepClone().AsObject() ?? new JsonObject();
                    if(body is JsonObject fields)foreach(var item in fields)report[item.Key]=Copy(item.Value);
                    report.Remove("id");report["generatedDate"]=DateTime.Today.ToString("yyyy-MM-dd");report["adminApproved"]=false;report["releasedToStudent"]=false;
                    return Reply(Save("reports",report));
                }
            }
            if (path.Contains("/assessment/"))
            {
                var type=parts.FirstOrDefault(x=>new[]{"assignment","test","exam"}.Contains(x));
                if(type is not null)
                {
                    if(get)return Reply(store[type].Where(x=>Q("subjectName")=="" || Text(x,"subjectName")==Q("subjectName")));
                    if(delete){store[type].RemoveAll(x=>Id(x)==last);return Reply(true);}
                    var row=Save(type,body,parts.Contains("edit") ? parts[^2] : null);if(Q("subjectName")!="")row["subjectName"]=Q("subjectName");return Reply(true);
                }
            }
            if (path.Contains("/marks/") && path.EndsWith("/student/get"))
            {
                var student=Find(store["student"],Text(body,"studentId")) ?? store["student"][0];
                var marks = Subjects.ToDictionary(s=>s,s=>new[]{new { id=1, studentId=Number(student,"id"), score=78, actualMark=78, percentageScore=78, maxScore=100, feedback="Sample result", gradedDate=DateTime.Today.ToString("yyyy-MM-dd"), term=$"Term {Number(body,"term")}", termYear=Number(body,"termYear") }});
                return Reply(new { firstName=Text(student,"firstName"), lastName=Text(student,"lastName"), form=Number(student,"form"), classroom=Text(student,"classroom"), Term=Number(body,"term"), Year=Number(body,"termYear"), CourseMarks=marks, marksBySubject=marks });
            }
            if (path.Contains("/marks/")) return get ? Reply(Array.Empty<object>()) : Unsupported();
            if (path.Contains("/comments/")) return get ? Reply(store["comments"]) : Unsupported();
        }
        if(path.Contains("/attendance/"))
        {
            if(path.Contains("/records/"))return Reply(new { records=store["records"].Where(x=>Text(x,path.Contains("/person/")?"personId":"sessionId")== (parts.Contains("range")?parts[^2]:last)) });
            if(path.Contains("/record/") && !get) {var row=Find(store["records"],last);if(row is null)return Reply(null,HttpStatusCode.NotFound);row["status"]=Q("status","Present");row["remarks"]=Q("remarks");return Reply(new {record=row});}
            if(get && (path.Contains("/date/") || last=="range"))return Reply(new {sessions=store["sessions"].Where(x=>last=="range" ? string.Compare(Text(x,"date"),Q("startDate"),StringComparison.Ordinal)>=0 && string.Compare(Text(x,"date"),Q("endDate"),StringComparison.Ordinal)<=0 : Text(x,"date")==last)});
            if(get)return Reply(new {session=Find(store["sessions"],last)});
            if(store["sessions"].Any(x=>Text(x,"date")==Text(body,"date") && Text(x,"type")==Text(body,"type")))return Reply(new {error="A demo attendance session already exists for this date and type."},HttpStatusCode.Conflict);
            var session=Save("sessions",body);
            var people=store[Text(body,"type").Equals("Teacher",StringComparison.OrdinalIgnoreCase)?"teacher":"student"];
            foreach(var person in people.Where(x=>Q("form")=="" || Text(x,"form")==Q("form") && Text(x,"classroom")==Q("classroom")))Save("records",Row(new {sessionId=Number(session,"id"),personId=Number(person,"id"),status="Present",remarks="Demo attendance"}));
            return Reply(new {session});
        }
        if(path.Contains("/timetables/") || path.Contains("/periods/"))
        {
            if(path.Contains("/config/"))
            {
                var key=path.Contains("/teachers/subjects")?"teacher-subjects":parts[4];
                return Collection(key,get,delete,last,body,true);
            }
            if(last=="active")return Reply(store["versions"].FirstOrDefault(x=>Text(x,"lifecycle")=="PUBLISHED"));
            if(last=="versions")return Reply(store["versions"].Where(x=>Text(x,"term")==Q("term")));
            if(last=="validate")return Reply(Array.Empty<object>());
            if(last=="generate" || path.Contains("/jobs/"))return Unsupported();
            if(get) {var kind=parts[^2];return Reply(store["periods"].Where(x=>Text(x,"versionId")==parts[3] && Text(x,kind+"Id")==last));}
            if(last=="publish" || last=="draft") {var version=Find(store["versions"],parts[^2]);if(version is null)return Reply(null,HttpStatusCode.NotFound);version["lifecycle"]=last=="publish"?"PUBLISHED":"DRAFT";return Reply(version);}
            if(delete){store["versions"].RemoveAll(x=>Id(x)==last);store["periods"].RemoveAll(x=>Text(x,"versionId")==last);return Reply(true);}
            if(last=="periods")return Reply(Save("periods",body,null,true));
            if(last=="move")return Reply(Save("periods",body,parts[^2],true));
        }
        if(path.Contains("/finance/") || path.Contains("/invoicing/") || path.Contains("/fees-configuration/"))
            return Finance(path,parts,last,get,delete,body,query);
        return Unsupported();
    }
    private HttpResponseMessage Collection(string table,bool get,bool delete,string last,JsonNode? body,bool guid=false)
    {
        var isId=long.TryParse(last,out _) || Guid.TryParse(last,out _);
        if(get)return isId ? Reply(Find(store[table],last)) : Reply(store[table]);
        if(delete){store[table].RemoveAll(x=>Id(x)==last);return Reply(true);}
        return Reply(Save(table,body,isId?last:null,guid));
    }
    private HttpResponseMessage Finance(string path,string[] parts,string last,bool get,bool delete,JsonNode? body,Dictionary<string,string> query)
    {
        static decimal Money(JsonNode? row,string key)=>decimal.TryParse(Text(row,key),out var n)?n:0;
        var date=DateTime.Today.ToString("yyyy-MM-dd");
        if(path.Contains("/accounts/"))
        {
            if(last=="all")return Reply(store["accounts"]);
            var account=store["accounts"].FirstOrDefault(x=>Text(x,"studentId")==parts[^2]);
            if(account is null)return Reply(null,HttpStatusCode.NotFound);
            if(!get && last=="charge") {var amount=Money(body,"amount");if(amount<=0)return Reply(new {error="Enter a positive demo amount."},HttpStatusCode.BadRequest);account["totalCharged"]=Money(account,"totalCharged")+amount;account["balance"]=Money(account,"totalCharged")-Money(account,"totalPaid");}
            return Reply(account);
        }
        if(path.Contains("/payments/history/"))return Reply(store["payments"].Where(x=>Text(x,"studentId")==last));
        if(path.Contains("/payments/status/"))return Reply(new {status="PAID",message="Simulated demo payment"});
        if(path.Contains("/payments/") && !get)
        {
            if(last=="refund")return Unsupported();
            var account=store["accounts"].FirstOrDefault(x=>Text(x,"studentId")==Text(body,"studentId"));
            var amount=Money(body,"amount");
            if(account is null || amount<=0)return Reply(new {error="Choose a demo student and a positive amount."},HttpStatusCode.BadRequest);
            if(Text(body,"currency")!="USD")return Reply(new {error="The demo payment flow uses USD sample amounts."},HttpStatusCode.BadRequest);
            account["totalPaid"]=Money(account,"totalPaid")+amount;account["balance"]=Money(account,"totalCharged")-Money(account,"totalPaid");account["lastPaymentDate"]=date;
            var payment=Save("payments",body);payment["paymentReference"]=$"DEMO-PAY-{Number(payment,"id"):000}";payment["status"]="PAID";payment["amountPaid"]=amount;payment["amountUsd"]=amount;payment["paidCurrency"]="USD";payment["paymentDate"]=date;payment["createdAt"]=date;payment["instructions"]="Simulated payment complete. No money was transferred.";payment["message"]="Demo payment saved locally.";return Reply(payment);
        }
        if(path.Contains("/statements/"))return Reply(new {studentDetails=Find(store["student"],last),accountSummary=store["accounts"].FirstOrDefault(x=>Text(x,"studentId")==last),transactions=store["payments"].Where(x=>Text(x,"studentId")==last),generatedAt=date,currency="USD"});
        if(path.Contains("/cashflow/summary"))
        {
            var fees=store["accounts"].Sum(x=>Money(x,"totalPaid"));
            var income=store["cashflow"].Where(x=>Text(x,"type")=="INCOME").Sum(x=>Money(x,"amountUsd"));
            var expense=store["cashflow"].Where(x=>Text(x,"type")=="EXPENSE").Sum(x=>Money(x,"amountUsd"));
            return Reply(new {cashFeeIncome=fees,nonCashFeeIncome=0,otherIncome=income,totalMoneyIn=fees+income,totalMoneyOut=expense,netTotal=fees+income-expense,currency="USD",todayIncome=fees+income,todayExpenses=expense,todayNet=fees+income-expense,generatedAt=date,incomeByCategory=new Dictionary<string,decimal>{{"Demo fees",fees},{"Other income",income}},dailySummaries=new[]{new {date,income=fees+income,expenses=expense,net=fees+income-expense}}});
        }
        if(path.Contains("/cashflow/movements"))
        {
            if(get)return Reply(store["cashflow"].Where(x=>!query.ContainsKey("type") || Text(x,"type")==query["type"]));
            var row=Save("cashflow",body);row["amountUsd"]=Money(body,"amount");row["originalAmount"]=Money(body,"amount");row["transactionReference"]=$"DEMO-CF-{Number(row,"id"):000}";row["createdAt"]=date;return Reply(row);
        }
        if(path.Contains("/exchange-rates"))
        {
            if(last=="convert")return Unsupported();
            if(get)return Reply(store["rates"]);
            var existing=store["rates"].FirstOrDefault(x=>Text(x,"currencyCode")==Text(body,"currencyCode"));return Reply(Save("rates",body,existing is null?null:Id(existing)));
        }
        if(path.Contains("/scholarships/")) {if(get)return Reply(store["scholarships"]);if(delete){store["scholarships"].RemoveAll(x=>Id(x)==last);return Reply(true);}return Reply(new {data=Save("scholarships",body,long.TryParse(last,out _)?last:null)});}
        if(path.Contains("/invoicing/monthly/") && get)
        {
            var sid=parts[^3];var student=Find(store["student"],sid);var account=store["accounts"].FirstOrDefault(x=>Text(x,"studentId")==sid);
            return Reply(new {id=1,invoiceNumber=$"DEMO-INV-{sid}",studentId=sid,studentName=$"{Text(student,"firstName")} {Text(student,"lastName")}",issueDate=date,dueDate=DateTime.Today.AddDays(30).ToString("yyyy-MM-dd"),status="UNPAID",totalAmount=300,balanceDue=Money(account,"balance"),currency="USD",academicYear=parts[^2],billingMonth=int.Parse(last),items=new[]{new {id=1,description="Demo tuition",amount=300,finalAmount=300}},financials=new {totalAmount=300,amountPaid=0,balanceDue=300}});
        }
        if(path.Contains("/fees-heads"))return Collection("fees-heads",get,delete,last,body);
        if(path.Contains("/structures"))
        {
            if(last=="status"){var row=Find(store["fee-structures"],parts[^2]);if(row is null)return Reply(null,HttpStatusCode.NotFound);row["isActive"]=query.GetValueOrDefault("isActive")=="true";return Reply(row);}
            return Collection("fee-structures",get,delete,last,body);
        }
        if(path.Contains("/fees-configuration/items"))
        {
            if(get)return Reply(store["fee-items"].Where(x=>Text(x,"feeStructureId")==last));
            return Collection("fee-items",get,delete,last,body);
        }
        if(path.Contains("/student-pass/"))
        {
            var account=store["accounts"].FirstOrDefault(x=>Text(x,"nfcId")==last);
            if(get && path.Contains("/validate/"))return Reply(new {studentId=account is null?0:Number(account,"studentId"),allowed=account is not null && Money(account,"balance")<=0,reason="Demo pass validation only"});
            if(get)return Reply(account,account is null?HttpStatusCode.NotFound:HttpStatusCode.OK);
            if(path.Contains("/nfc/set/")){account=store["accounts"].FirstOrDefault(x=>Text(x,"studentId")==last);if(account is null)return Reply(null,HttpStatusCode.NotFound);account["nfcId"]=Text(body,"nfcId");return Reply(true);}
        }
        return Unsupported();
    }
}
