using System;

Console.WriteLine("1.Создать пустой граф\n" +
                          "2.Создать граф из файла");
switch (Console.ReadLine())
{
    case "1":
        Graph graph = new Graph(); UI ui = new UI(graph); break;
    case "2":
        Console.WriteLine("Укажите путь к файлу:");
        graph = new Graph(Console.ReadLine()); UI ui1 = new UI(graph); break;
    default:
        break;
}
Graph graph2 = new Graph(Console.ReadLine());
Graph graph3 = new Graph(Console.ReadLine());
Graph graph4 = Graph.IntersectionGraph(graph2, graph3);
graph4.PrintAdjacencyList();
class Graph
{
    internal Dictionary<string, List<(string, double)>> AdjacencyList = new();
    internal bool wh;
    internal bool orn;
    public Graph(Dictionary<string, List<(string, double)>> vertices, bool wh, bool orn)
    {
        this.AdjacencyList = vertices;
        this.wh = wh;
        this.orn = orn;
    }
    public Graph(Graph graph)
    {
        AdjacencyList = new Dictionary<string, List<(string, double)>>(graph.AdjacencyList);
        foreach (var item in AdjacencyList.Keys) AdjacencyList[item] = new List<(string, double)>(graph.AdjacencyList[item]);
        wh = graph.wh;
        orn = graph.orn;
    }
    public Graph()
    {
        AdjacencyList = new Dictionary<string, List<(string, double)>>();
        Console.WriteLine("Граф взвешенный? y/n");
        if (Console.ReadLine() == "y") wh = true;
        else wh = false;
        Console.WriteLine("Граф ориентированный y/n");
        if (Console.ReadLine() == "y") orn = true;
        else orn = false;
    }
    public Graph(string path)
    {
        string[] adjlist = { "" };
        try
        {
            adjlist = File.ReadAllLines(path);
        }
        catch (Exception)
        {
            Console.WriteLine("Выбран не правильный путь или неправильная запись графа в файле");

        }
        wh = Convert.ToBoolean(adjlist[0].Split(":")[1]);
        orn = Convert.ToBoolean(adjlist[1].Split(":")[1]);
        for (int i = 2; i < adjlist.Length; i++)
        {
            List<(string, double)> buf1 = new List<(string, double)>();
            foreach (var item in adjlist[i].Split(" : ")[1].Split(" "))
            {
                (string, double) buf2;
                if (wh) buf2 = (item.Split("-")[0], Convert.ToDouble(item.Split("-")[1]));
                else buf2 = (item, 1);
                buf1.Add(buf2);
            }
            AdjacencyList.Add(adjlist[i].Split(" : ")[0], buf1);
        }
     
    }
    public static Graph CompleteGraph(Graph graph)
    {
        Dictionary<string, List<(string, double)>> AdjList = new();
        foreach (var vertexK in graph.AdjacencyList.Keys)
        {
            AdjList.Add(vertexK, new List<(string, double)>());
            foreach (var VertexV in graph.AdjacencyList.Keys)
            {
                if (vertexK != VertexV) AdjList[vertexK].Add((VertexV, 1));
            }
        }
        return new Graph(AdjList, graph.wh, graph.orn);
    }
    public static Graph ComplementGraph(Graph graph)
    {
        Dictionary<string, List<(string, double)>> AdjList = new();
        foreach (var vertexK in graph.AdjacencyList.Keys)
        {
            AdjList.Add(vertexK, new List<(string, double)>());
            foreach (var VertexV in graph.AdjacencyList.Keys)
            {
                if (!graph.AdjacencyList[vertexK].Any(x => x.Item1 == VertexV) && VertexV != vertexK) AdjList[vertexK].Add((VertexV, 1));
            }
        }
        return new Graph(AdjList, graph.wh, graph.orn);
    }
    public static Graph Union_graph(Graph gr1,Graph gr2)
    {
        Dictionary<string, List<(string, double)>> AdjacencyList = new();
        if (gr1.wh == gr2.wh && gr1.orn == gr2.orn)
        {
           
            if (!gr1.AdjacencyList.Keys.Intersect(gr2.AdjacencyList.Keys).Any())
            {
                foreach (var item in gr1.AdjacencyList.Keys)
                {
                    AdjacencyList.Add(item, gr1.AdjacencyList[item]);
                }
                foreach (var item in gr2.AdjacencyList.Keys)
                {
                    AdjacencyList.Add(item, gr2.AdjacencyList[item]);
                }
                return new Graph(AdjacencyList, gr1.wh, gr1.orn);
            }
        }
        return null;
    }
    public static Graph IntersectionGraph(Graph graph1, Graph graph2)
    {
        Dictionary<string, List<(string, double)>> AdjList = new();
        if (graph1.AdjacencyList.Keys.Intersect(graph2.AdjacencyList.Keys).Any())
            return null;
        foreach (var item in graph1.AdjacencyList)
        {
            AdjList.Add(item.Key, item.Value);
        }
        foreach (var item in graph2.AdjacencyList)
        {
            AdjList.Add(item.Key, item.Value);
        }
        Graph gr = new Graph(AdjList, false, false);
        foreach (var item1 in graph1.AdjacencyList)
        {
            foreach (var item2 in graph2.AdjacencyList)
            {
                gr.AddEdge(item1.Key, item2.Key);
            }
        }
        return gr;
    }
    public void AddVertex(string vertex)
    {
        AdjacencyList.Add(vertex, new List<(string, double)>());
    }
    public void RemoveVertex(string vertex)
    {
        if (wh)
        {
            foreach (var vertices in AdjacencyList.Values)
            {
                try
                {
                    vertices.Remove(vertices.First(x => (x.Item1 == vertex)));
                }
                catch { }
            }
        }
        else
            foreach (var vertices in AdjacencyList.Values) vertices.Remove((vertex, 1));
        AdjacencyList.Remove(vertex);
    }
    public void AddEdge(string vertex1, string vertex2, double weight = 1)
    {
        if (!(AdjacencyList[vertex1].Contains((vertex2, weight)) && (AdjacencyList[vertex2].Contains((vertex1, weight)))))
        {
            if (vertex1 == vertex2) AdjacencyList[vertex1].Add((vertex1, weight));
            else
            {
                if (orn) AdjacencyList[vertex1].Add((vertex2, weight));
                else
                {
                    AdjacencyList[vertex1].Add((vertex2, weight));
                    AdjacencyList[vertex2].Add((vertex1, weight));
                }
            }
        }
    }
    public void RemoveEdge(string vertex1, string vertex2)
    {
        AdjacencyList[vertex1].Remove(AdjacencyList[vertex1].First(x => x.Item1 == vertex2));
        AdjacencyList[vertex2].Remove(AdjacencyList[vertex2].First(x => x.Item1 == vertex1));
    }
    private void CreateAdjacencyList() { }
    public double[][] CreateAdjacencyMatrix()
    {
        List<string> vertexList = AdjacencyList.Keys.ToList();
        double[][] matrix = new double[AdjacencyList.Keys.Count][];
        foreach (string v in vertexList) matrix[vertexList.IndexOf(v)] = new double[AdjacencyList.Keys.Count];

        foreach (string v in vertexList)
        {
            matrix[vertexList.IndexOf(v)] = new double[AdjacencyList.Keys.Count];
            foreach (var adjVert in AdjacencyList[v])
            {
                matrix[vertexList.IndexOf(v)][vertexList.IndexOf(adjVert.Item1)] = adjVert.Item2;
                if (orn) matrix[vertexList.IndexOf(adjVert.Item1)][vertexList.IndexOf(v)] = -adjVert.Item2;
            }
        }

        return matrix;
    }
    public void PrintAdjacencyMatrix(double[][] matrix)
    {
        for (int i = 0; i < matrix.Length; i++)
            Console.WriteLine(String.Join("  ", matrix[i]));
    }
    public void PrintAdjacencyList()
    {
        for (int i = 0; i < AdjacencyList.Count; i++)
        {
            Console.Write($"{AdjacencyList.ElementAt(i).Key} : ");
            Console.WriteLine(String.Join("  ", (AdjacencyList[AdjacencyList.ElementAt(i).Key].Select(x => x.Item1).ToList())));
        }
    }
    public void WriteAdjacencyMatrixToFile(string path)
    {
        double[][] adjMatrix = CreateAdjacencyMatrix();
        string res = " ";
        for (int i = 0; i < adjMatrix.Length; i++)
            res += (String.Join(" ", adjMatrix[i]) + "\n");

    }
    public void WriteAdjacencyListToFile(string path)
    {
        string res = "";
        res += "Wh:" + wh.ToString() + "\n";
        res += "Orn:" + orn.ToString() + "\n";
        for (int i = 0; i < AdjacencyList.Count; i++)
        {
            res += ($"{AdjacencyList.ElementAt(i).Key} :");
            foreach (var item in AdjacencyList[AdjacencyList.ElementAt(i).Key])
            {
                if (wh) res += " " + item.Item1 + "-" + item.Item2;
                else res += " " + item.Item1;
            }
            res += "\n";
        }
        File.WriteAllText(path, res);
    }
    public bool IsVertexExists(string id)
    {
        bool check = false; ;
        foreach (var v in AdjacencyList)
        {
            if (v.Key == id) check = true; ;

        }
        return check;

    }
}
class UI
{
    public UI(Graph graph)
    {

        while (true)
        {
            Console.WriteLine(
            "\n1. Добавить вершину\n" +
            "2. Добавить ребро\n" +
            "3. Удалить вершину\n" +
            "4. Удалить ребро\n" +
            "5. Вывести список смежности\n" +
            "6. Сохранить список смежности в файл\n" +
            "7. Выход");
            string id;
            List<string> idList = new();

            Console.WriteLine("выберете номер операции:");
            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("введите название новой вершины");
                    id = Console.ReadLine();
                    if (!graph.IsVertexExists(id)) graph.AddVertex((id));
                    else Console.WriteLine("вершина уже существует");
                    break;
                case "2":
                    if (graph.wh)
                    {
                        Console.WriteLine("введите вершины, между которыми создается ребро и вес ребра(v1 v2 wheight)");
                        idList = Console.ReadLine().Split(" ").ToList();
                        graph.AddEdge((idList[0]), (idList[1]), Convert.ToDouble(idList[2]));
                    }
                    else
                    {
                        Console.WriteLine("введите вершины, между которыми создается ребро(v1 v2)");// вес ребра
                        idList = Console.ReadLine().Split(" ").ToList();
                        graph.AddEdge((idList[0]), (idList[1]));
                    }
                    break;
                case "3":
                    Console.WriteLine("введите название удаляемой вершины");
                    id = Console.ReadLine();
                    if (graph.IsVertexExists(id)) graph.RemoveVertex((id));
                    else Console.WriteLine("вершины не существует");
                    break;
                case "4":
                    Console.WriteLine("введите вершины, между которыми удаляется ребро(v1 v2)");
                    idList = Console.ReadLine().Split(" ").ToList();
                    graph.RemoveEdge((idList[0]), (idList[1]));
                    break;
                case "5":
                    graph.PrintAdjacencyList();
                    break;
                case "6"://граф в файл
                    Console.WriteLine("введите путь до файла:");
                    graph.WriteAdjacencyListToFile(Console.ReadLine());
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("Выбрана несущесвующая операция");
                    break;
            }
        }
    }
}
