Graph graph1 = new Graph("D:/1.txt");
graph1.UI();
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
        this.AdjacencyList = graph.AdjacencyList;
        this.orn = graph.orn;
        this.wh = graph.wh;
    }
    public Graph()
    {
        AdjacencyList = new Dictionary<string, List<(string, double)>>();
        wh = false;
        orn = false;
    }
    public Graph(string path)
    {
        double[][] AdjacencyMatrix = File.ReadAllLines(path).Select(str => str.Split(",").Select(c => Convert.ToDouble(c)).ToArray()).ToArray();
        for (int i = 0; i < AdjacencyMatrix.Length; i++)
            AddVertex((($"{i + 1}")));
        for (int i = 0; i < AdjacencyMatrix.Length; i++)
        {
            for (int j = 0; j < AdjacencyMatrix.Length; j++)
            {
                if (AdjacencyMatrix[i][j] > 0) AdjacencyList[((($"{i + 1}")))].Add(((($"{j + 1}")), (AdjacencyMatrix[i][j]))); ;
                if (!wh && (AdjacencyMatrix[i][j] != 1 || AdjacencyMatrix[i][j] != -1)) wh = true;
                if (!orn && AdjacencyMatrix[i][j] < 0) orn = true;
            }
        }
    }
    private void AddVertex(string vertex)
    {
        AdjacencyList.Add(vertex, new List<(string, double)>());
    }
    private void RemoveVertex(string vertex)
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
    private void AddEdge(string vertex1, string vertex2, double weight = 1)
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
    private void RemoveEdge(string vertex1, string vertex2)
    {
        AdjacencyList[vertex1].Remove(AdjacencyList[vertex1].First(x => x.Item1 == vertex2));
        AdjacencyList[vertex2].Remove(AdjacencyList[vertex2].First(x => x.Item1 == vertex1));
    }
    private void CreateAdjacencyList() { }
    private double[][] CreateAdjacencyMatrix()
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
        string res = " ";
        for (int i = 0; i < AdjacencyList.Count; i++)
        {
            res += ($"{AdjacencyList.ElementAt(i).Key} : ");
            res += (String.Join("  ", (AdjacencyList[AdjacencyList.ElementAt(i).Key].Select(x => x.Item1).ToList())) + "\n");
        }
        File.WriteAllText(path, res);
    }
    private bool IsVertexExists(string id)
    {
        bool check = false; ;
        foreach (var v in AdjacencyList)
        {
            if (v.Key == id) check = true; ;

        }
        return check;

    }
    public void UI()
    {
        Console.WriteLine(
            "1. Добавить вершину\n" +
            "2. Добавить ребро\n" +
            "3. Удалить вершину\n" +
            "4. Удалить ребро\n" +
            "5. Вывести список смежности\n" +
            "6. Сохранить список смежности в файл\n" +
            "7. Выход");
        string id;
        List<string> idList = new();

        while (true)
        {
            Console.WriteLine("выберете номер операции:");
            switch (Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("введите название новой вершины");
                    id = Console.ReadLine();
                    if (!IsVertexExists(id)) AddVertex((id));
                    else Console.WriteLine("вершина уже существует");
                    break;
                case "2":
                    if (wh)
                    {
                        Console.WriteLine("введите вершины, между которыми создается ребро и вес ребра(v1 v2 wheight)");
                        idList = Console.ReadLine().Split(" ").ToList();
                        AddEdge((idList[0]), (idList[1]), Convert.ToDouble(idList[2]));
                    }
                    else
                    {
                        Console.WriteLine("введите вершины, между которыми создается ребро(v1 v2)");// вес ребра
                        idList = Console.ReadLine().Split(" ").ToList();
                        AddEdge((idList[0]), (idList[1]));
                    }
                    break;
                case "3":
                    Console.WriteLine("введите название удаляемой вершины");
                    id = Console.ReadLine();
                    if (IsVertexExists(id)) RemoveVertex((id));
                    else Console.WriteLine("вершины не существует");
                    break;
                case "4":
                    Console.WriteLine("введите вершины, между которыми удаляется ребро(v1 v2)");
                    idList = Console.ReadLine().Split(" ").ToList();
                    RemoveEdge((idList[0]), (idList[1]));
                    break;
                case "5":
                    PrintAdjacencyList();
                    break;
                case "6"://граф в файл
                    Console.WriteLine("введите путь до файла:");
                    WriteAdjacencyListToFile(Console.ReadLine());               
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