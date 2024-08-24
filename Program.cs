using System.Text;

if (args.Length != 1)
{
    Console.WriteLine("usage ast_tool <out dir>");
}

string outDir = args[0];

DefineAst(outDir, "Expr",
[
    "Binary : Expr left, Token op, Expr right",
    "Grouping : Expr expr",
    "Literal : object? val",
    "Unary : Token op, Expr right"
]);

static void DefineAst(string outDir, string baseName, string[] types)
{
    string path = $"{outDir}/{baseName}.cs";
    using StreamWriter sw = new(File.Create(path), Encoding.UTF8);

    sw.WriteLine("namespace CSharpLox.Src;");
    DefineVisitor(sw, baseName, types);
    sw.WriteLine("public abstract class " + baseName + " \n{");
    sw.WriteLine("  public abstract R Accept<R>(IVisitor<R> visitor);");

    sw.WriteLine("}");
    sw.WriteLine();
    foreach (string type in types)
    {
        string className = type.Split(":")[0].Trim();
        string fields = type.Split(":")[1].Trim();
        DefineType(sw, baseName, className, fields);
    }

    sw.Close();
}

static void DefineVisitor(StreamWriter sw, string baseName, string[] types)
{
    sw.WriteLine("public interface IVisitor<R>");
    sw.WriteLine("{");
    foreach (string type in types)
    {
        string typeName = type.Split(":")[0].Trim();
        sw.WriteLine("  R Visit" + typeName + baseName + "(" + typeName + " " + baseName.ToLower() + ");");
    }
    sw.WriteLine("}");
}

static void DefineType(StreamWriter sw, string baseName, string className, string fieldList)
{
    sw.WriteLine("public class " + className + "(" + fieldList + ")" + " : " + baseName);
    sw.WriteLine("{");
    string[] fields = fieldList.Split(",");
    foreach (string field in fields)
    {
        string[] fieldSplit = field.TrimStart().Split(" ");
        sw.WriteLine("  public readonly " + fieldSplit[0] + $" {fieldSplit[1][0].ToString().ToUpper()}{fieldSplit[1][1..]}" + " = " + fieldSplit[1] + ";");
    }
    sw.WriteLine();
    sw.WriteLine("  public override R Accept<R>(IVisitor<R> Visitor)");
    sw.WriteLine("  {");
    sw.WriteLine("    return Visitor.Visit" + className + baseName + "(this);");
    sw.WriteLine("  }");
    sw.WriteLine("}");
}