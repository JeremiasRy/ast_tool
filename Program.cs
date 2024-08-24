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
    "Literal : object val",
    "Unary : Token op, Expr right"
]);

static void DefineAst(string outDir, string baseName, string[] types)
{
    string path = $"{outDir}/{baseName}.cs";
    using StreamWriter sw = new(File.Create(path), Encoding.UTF8);

    sw.WriteLine("namespace CSharpLox.Src;");
    sw.WriteLine();
    sw.WriteLine("public abstract class " + baseName + " \n{");
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

    sw.WriteLine("}");
}